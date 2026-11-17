using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Layout;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests.Display;

public class SongDrawingTests
{
    private sealed class ProportionalMeasurer : ITextMeasurer
    {
        public TextMetrics Measure(string text, TextRole role) => role switch
        {
            TextRole.Chord or TextRole.Annotation => new(text.Length * 6.5, 18, 14),
            TextRole.SectionLabel => new(text.Length * 7, 18, 14),
            _ => new(text.Length * 9.25, 22, 17)
        };
    }

    private const string Song = "[G]Santo, [D]Santo, [Em]Santo il Signore [C]Dio dell'universo\n[G]Pieni sono i cieli e la [D]terra della tua gloria";

    private static readonly SongDisplayService Service = new(new ChordProParser());

    private static PreparedSong Prepare(string song = Song, int steps = 0) =>
        Service.Prepare(new SongDisplayRequest(song, steps, ChordDisplaySettings.Default, "en"));

    private static readonly ITextMeasurer Measurer = new ProportionalMeasurer();

    private static double Right(DrawnRow row) => row.Runs.Max(r => r.X + r.Width);

    [Fact]
    public void ANarrowerWidth_GivesMoreRows()
    {
        var song = Prepare();

        var wide = song.LayoutAt(1200, Measurer);
        var narrow = song.LayoutAt(260, Measurer);

        Assert.True(narrow.Rows.Count > wide.Rows.Count);
        Assert.True(narrow.Height > wide.Height);
    }

    [Theory]
    [InlineData(260)]
    [InlineData(340)]
    [InlineData(500)]
    public void NoRowIsWiderThanTheWidth(double width)
    {
        var drawing = Prepare().LayoutAt(width, Measurer);

        Assert.All(drawing.Rows, row => Assert.True(row.Overflow || Right(row) <= width + 0.001, $"row at {row.Y} ends at {Right(row)}"));
        Assert.Equal(width, drawing.Width);
    }

    [Fact]
    public void AWordLongerThanTheWidth_IsBrokenIntoPiecesThatFit()
    {
        const string word = "Supercalifragilistichespiralidoso";
        var drawing = Prepare($"[G]{word}").LayoutAt(100, Measurer);

        Assert.All(drawing.Rows, row => Assert.True(!row.Overflow && Right(row) <= 100.001));
        Assert.Equal(word, string.Concat(drawing.Runs.Where(r => r.Kind == RunKind.Lyric).Select(r => r.Text.TrimEnd('-'))));
    }

    [Fact]
    public void AWidthSmallerThanOneCharacter_FlagsTheRowAsOverflow()
    {
        var drawing = Prepare("[G]Santo").LayoutAt(4, Measurer);

        Assert.Contains(drawing.Rows, r => r.Overflow);
    }

    [Fact]
    public void AfterWrapping_EachChordStaysOverTheStartOfItsSyllable()
    {
        var drawing = Prepare().LayoutAt(300, Measurer);

        foreach (var row in drawing.Rows)
        {
            foreach (var chord in row.Runs.Where(r => r.Kind == RunKind.Chord))
            {
                var lyric = row.Runs.Where(r => r.Kind == RunKind.Lyric).LastOrDefault(l => l.X <= chord.X + 0.001);
                Assert.NotNull(lyric);
                Assert.True(Math.Abs(lyric!.X - chord.X) < 0.001, $"{chord.Text} at {chord.X} is not over a syllable start ({lyric.X})");
            }
        }
    }

    [Fact]
    public void EveryChordIsDrawnAboveItsLyrics()
    {
        var drawing = Prepare().LayoutAt(300, Measurer);

        Assert.All(drawing.Rows, row =>
        {
            var lyrics = row.Runs.Where(r => r.Kind == RunKind.Lyric).ToList();
            Assert.All(row.Runs.Where(r => r.Kind == RunKind.Chord), chord => Assert.True(chord.BaselineY < lyrics.Min(l => l.BaselineY)));
        });
    }

    [Fact]
    public void WideningTheWidthAgain_RestoresTheFewerRows()
    {
        var song = Prepare();
        var before = song.LayoutAt(900, Measurer);

        _ = song.LayoutAt(250, Measurer);
        var after = song.LayoutAt(900, Measurer);

        Assert.Equal(before.Rows.Count, after.Rows.Count);
        Assert.Equal(before.Height, after.Height);
    }

    [Fact]
    public void RowsAreStackedInsideTheHeight()
    {
        var drawing = Prepare().LayoutAt(300, Measurer);

        Assert.True(drawing.Rows.Zip(drawing.Rows.Skip(1)).All(p => p.First.Y < p.Second.Y));
        Assert.All(drawing.Runs, r => Assert.InRange(r.BaselineY, 0, drawing.Height));
    }

    [Fact]
    public void ChordsFollowTheTransposition()
    {
        var drawing = Prepare("[G]Santo", steps: 2).LayoutAt(300, Measurer);

        Assert.Contains(drawing.Runs, r => r.Kind == RunKind.Chord && r.Text == "A");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void AWidthThatIsNotPositiveAndFinite_IsRejected(double width)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Prepare().LayoutAt(width, Measurer));
    }

    [Fact]
    public void ANullMeasurer_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => Prepare().LayoutAt(300, null!));
    }

    [Fact]
    public void TheSoundingKeyAndCapo_TravelWithThePreparedSong()
    {
        var song = Prepare("{key:G}\n{capo:2}\n[G]x", steps: 2);

        Assert.Equal("B", song.Key);
        Assert.Equal(2, song.Capo);
    }
}
