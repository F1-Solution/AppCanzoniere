using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class LayoutRobustnessTests
{
    /// <summary>The fixed-advance measurer with one metric of one role replaced.</summary>
    private sealed class AdjustedMeasurer : ITextMeasurer
    {
        private readonly FixedAdvanceMeasurer _inner = new();
        private readonly Func<TextRole, TextMetrics, TextMetrics> _adjust;

        public AdjustedMeasurer(Func<TextRole, TextMetrics, TextMetrics> adjust) => _adjust = adjust;

        public TextMetrics Measure(string text, TextRole role) => _adjust(role, _inner.Measure(text, role));
    }

    private static LayoutOptions Numbered() => Options() with { LineNumbers = true };

    [Theory]
    [InlineData("Width", double.NaN)]
    [InlineData("Width", double.PositiveInfinity)]
    [InlineData("Width", -1.0)]
    [InlineData("LineHeight", double.NaN)]
    [InlineData("LineHeight", double.NegativeInfinity)]
    [InlineData("LineHeight", -5.0)]
    [InlineData("Ascent", double.NaN)]
    [InlineData("Ascent", double.PositiveInfinity)]
    public void ADegenerateMeasurement_RaisesAnInvalidOperationNamingTheRole(string field, double value)
    {
        var measurer = new AdjustedMeasurer((role, m) => role != TextRole.Chord ? m : field switch
        {
            "Width" => m with { Width = value },
            "LineHeight" => m with { LineHeight = value },
            _ => m with { Ascent = value },
        });

        var error = Assert.Throws<InvalidOperationException>(() => SongLayoutEngine.Flow(Source("[Do]Signore"), Options(), measurer));

        Assert.Contains("Chord", error.Message);
        Assert.Contains(field, error.Message);
    }

    [Fact]
    public void ADegenerateMeasurementOfTheText_IsCaughtToo()
    {
        var measurer = new AdjustedMeasurer((role, m) => m.Width > 0 && role == TextRole.Lyric ? m with { Width = double.NaN } : m);

        var error = Assert.Throws<InvalidOperationException>(() => SongLayoutEngine.Flow(Source("Signore"), Options(), measurer));

        Assert.Contains("Lyric", error.Message);
    }

    [Fact]
    public void AContinuationIndent_BeyondTheAvailableWidth_IsClampedToLeaveAOneUnitColumn()
    {
        var flow = Flow("abcdefghijkl", Options(100) with { ContinuationIndent = 150 });

        var rows = OnlySection(flow).Lines[0].Rows;
        Assert.Equal(new[] { "abcdefghij", "k", "l" }, rows.Select(r => Single(r, RunKind.Lyric).Text));
        Assert.Equal(new[] { 0d, 99, 99 }, rows.Select(r => Single(r, RunKind.Lyric).X));
        Assert.Equal(new[] { false, true, true }, rows.Select(r => r.Overflow));
    }

    [Fact]
    public void AWidthSmallerThanTheNumberGutter_StillLaysOut_WithEveryRowFlagged()
    {
        var flow = Flow("Signore", Numbered() with { Width = 10 });

        Assert.Equal(14, flow.GutterWidth);
        var rows = OnlySection(flow).Lines[0].Rows;
        Assert.All(rows, r => Assert.True(r.Overflow));
        Assert.All(rows.SelectMany(r => r.Runs), run => Assert.True(double.IsFinite(run.X) && double.IsFinite(run.Width)));
        Assert.Equal("Signore", string.Concat(rows.Select(r => Single(r, RunKind.Lyric).Text)));
    }

    [Fact]
    public void TheLineNumber_StaysInsideItsRowBox_WhenItsAscentExceedsTheLyrics()
    {
        var measurer = new AdjustedMeasurer((role, m) => role == TextRole.LineNumber ? m with { Ascent = 22 } : m);

        var row = OnlyRow(OnlySection(SongLayoutEngine.Flow(Source("Signore"), Numbered(), measurer)).Lines[0]);

        var number = Single(row, RunKind.LineNumber);
        Assert.Equal((0d, 22d), (number.Y, number.BaselineY));
    }

    [Fact]
    public void TheLineNumber_KeepsTheLyricBaseline_WhenItAlreadyFits()
    {
        var measurer = new AdjustedMeasurer((role, m) => role == TextRole.LineNumber ? m with { Ascent = 22 } : m);

        var row = OnlyRow(OnlySection(SongLayoutEngine.Flow(Source("[Do]Signore"), Numbered(), measurer)).Lines[0]);

        var number = Single(row, RunKind.LineNumber);
        Assert.Equal((17d, 39d), (number.Y, number.BaselineY));
    }

    [Fact]
    public void ALineSourceIndex_IsItsIndexInTheParsedDocument_EvenWhenTitleLinesAreDropped()
    {
        var flow = Flow(Lines("{title: Santo}", "Signore", "", "{c: Nota}", "{title: Altro}", "Pietà"));

        var indexes = flow.Sections.SelectMany(s => s.Lines).Select(l => l.SourceLineIndex);
        Assert.Equal(new[] { 1, 3, 5 }, indexes);
    }

    [Fact]
    public void TabLinesToo_KeepTheirDocumentIndex()
    {
        var flow = Flow(Lines("{title: T}", "{sot}", "e|--0--|", "{eot}"));

        Assert.Equal(1, OnlySection(flow).Lines[0].SourceLineIndex);
    }

    [Fact]
    public void StackedChordsAtTheSameOffset_CarryTheirOrdinalInTheLine()
    {
        var row = OnlyRow(OnlySection(Flow("[G][D/F#]Voce")).Lines[0]);

        var chords = Of(row, RunKind.Chord);
        Assert.Equal(new[] { "G", "D/F#" }, chords.Select(c => c.Text));
        Assert.Equal(new[] { 0, 0 }, chords.Select(c => c.SourceOffset));
        Assert.Equal(new[] { 0, 1 }, chords.Select(c => c.ChordIndex));
    }

    [Fact]
    public void TheChordOrdinal_IsLineWide_AcrossWrappedRows_AndMinusOneForOtherRuns()
    {
        var rows = OnlySection(Flow("[Do]aaaa [Sol]bbbb [Re]cccc", Options(60))).Lines[0].Rows;

        Assert.Equal(new[] { 0, 1, 2 }, rows.SelectMany(r => Of(r, RunKind.Chord)).Select(c => c.ChordIndex));
        Assert.All(rows.SelectMany(r => Of(r, RunKind.Lyric)), l => Assert.Equal(-1, l.ChordIndex));
    }

    [Fact]
    public void EveryTextRole_IsMeasuredAtTheStartOfAFlow()
    {
        var roles = new HashSet<TextRole>();
        var measurer = new AdjustedMeasurer((role, m) =>
        {
            roles.Add(role);
            return m;
        });

        SongLayoutEngine.Flow(Source("Signore"), Options(), measurer);

        Assert.Equal(Enum.GetValues<TextRole>().ToHashSet(), roles);
    }
}
