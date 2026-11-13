using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class WrappingTests
{
    private const string TwoChordLine = "[Do]Signore [Sol]pietà";

    private static LayoutLine LineOf(string chordPro, LayoutOptions options) =>
        OnlySection(Flow(chordPro, options)).Lines[0];

    [Fact]
    public void Case30_WrapsAtAWord_AndTheChordTravelsWithIt()
    {
        var line = LineOf(TwoChordLine, Options(100));

        Assert.Equal(2, line.Rows.Count);
        var row0 = line.Rows[0];
        var row1 = line.Rows[1];
        AssertBox(Single(row0, RunKind.Lyric), 0, 70);
        Assert.Equal("Signore", Single(row0, RunKind.Lyric).Text);
        AssertBox(Single(row0, RunKind.Chord), 0, 16);
        Assert.Equal(48, row1.Y);
        Assert.Equal(1, row1.Index);
        AssertBox(Single(row1, RunKind.Lyric), 20, 50);
        Assert.Equal("pietà", Single(row1, RunKind.Lyric).Text);
        AssertBox(Single(row1, RunKind.Chord), 20, 24);
        Assert.Equal(92, line.Height);
        Assert.False(row0.HardBreakAfter);
        Assert.False(row1.HardBreakAfter);
    }

    [Fact]
    public void Case31_ChordStaysWithItsWord()
    {
        var line = LineOf("Ti [Do]lodiamo [Sol]Signore", Options(120));

        Assert.Equal(2, line.Rows.Count);
        AssertBox(Run(line.Rows[0], RunKind.Lyric, "Ti "), 0, 30);
        AssertBox(Run(line.Rows[0], RunKind.Lyric, "lodiamo"), 30, 70);
        AssertBox(Single(line.Rows[0], RunKind.Chord), 30, 16);
        AssertBox(Single(line.Rows[1], RunKind.Lyric), 20, 70);
        Assert.Equal("Signore", Single(line.Rows[1], RunKind.Lyric).Text);
        AssertBox(Single(line.Rows[1], RunKind.Chord), 20, 24);
    }

    [Fact]
    public void Case32_ChordInTheWhitespaceBeforeAWord_MovesWithTheNextWord()
    {
        var line = LineOf("Signore [Sol] pietà", Options(100));

        Assert.Equal(2, line.Rows.Count);
        var row0 = line.Rows[0];
        AssertBox(Single(row0, RunKind.Lyric), 0, 70);
        Assert.Equal("Signore", Single(row0, RunKind.Lyric).Text);
        Assert.Empty(Of(row0, RunKind.Chord));
        var row1 = line.Rows[1];
        AssertBox(Single(row1, RunKind.Chord), 20, 24);
        var lyric = Single(row1, RunKind.Lyric);
        AssertBox(lyric, 20, 50);
        Assert.Equal("pietà", lyric.Text);
        Assert.Equal(9, lyric.SourceOffset);
        Assert.Equal(8, Single(row1, RunKind.Chord).SourceOffset);
    }

    [Fact]
    public void Case33_ExactFit_IsInclusive()
    {
        var line = LineOf(TwoChordLine, Options(130));

        Assert.Single(line.Rows);
    }

    [Fact]
    public void JustBelowTheExactFit_Wraps()
    {
        var line = LineOf(TwoChordLine, Options(129));

        Assert.Equal(2, line.Rows.Count);
    }

    [Fact]
    public void Case34_AWordWiderThanTheRow_IsBrokenHard()
    {
        var line = LineOf("Alleluia", Options(50));

        Assert.Equal(2, line.Rows.Count);
        var first = Single(line.Rows[0], RunKind.Lyric);
        Assert.Equal("Allel", first.Text);
        AssertBox(first, 0, 50);
        Assert.True(line.Rows[0].HardBreakAfter);
        var second = Single(line.Rows[1], RunKind.Lyric);
        Assert.Equal("uia", second.Text);
        AssertBox(second, 20, 30);
        Assert.False(line.Rows[1].HardBreakAfter);
        Assert.Equal(5, second.SourceOffset);
    }

    [Fact]
    public void Case35_HardBreakWithAChord_KeepsTheChordOnTheFirstRow()
    {
        var line = LineOf("[Re]Alleluia", Options(50));

        Assert.Equal(2, line.Rows.Count);
        AssertBox(Single(line.Rows[0], RunKind.Chord), 0, 16);
        Assert.Empty(Of(line.Rows[1], RunKind.Chord));
    }

    [Fact]
    public void HardBreakAcrossAChord_StartsTheNextRowWithTheRestOfTheWord()
    {
        var line = LineOf("[Do]All[Re]eluia", Options(50));

        var texts = line.Rows.Select(r => string.Concat(Of(r, RunKind.Lyric).Select(l => l.Text))).ToArray();
        Assert.Equal("Alleluia", string.Concat(texts));
        Assert.All(line.Rows, r => Assert.True(r.Runs.Where(x => x.Kind == RunKind.Lyric).Sum(x => x.Width) <= 50 + 1e-9));
        Assert.True(line.Rows.Count >= 2);
        Assert.Equal(new[] { "Do", "Re" }, line.Rows.SelectMany(r => Of(r, RunKind.Chord)).Select(c => c.Text));
    }

    [Fact]
    public void ChordWiderThanTheRow_IsPlacedAnyway()
    {
        var line = LineOf("[Sol]a", Options(10));

        AssertBox(Single(line.Rows[0], RunKind.Chord), 0, 24);
        Assert.Single(line.Rows);
    }

    [Fact]
    public void WrappingAppliesInLyricsOnlyMode()
    {
        var line = LineOf(TwoChordLine, Options(100) with { DisplayMode = LayoutDisplayMode.LyricsOnly });

        Assert.Equal(2, line.Rows.Count);
        Assert.Equal("Signore", Single(line.Rows[0], RunKind.Lyric).Text);
        Assert.Equal("pietà", Single(line.Rows[1], RunKind.Lyric).Text);
        Assert.Equal(28, line.Rows[1].Y);
        Assert.Equal(52, line.Height);
    }

    [Fact]
    public void ManyWords_FillRowsGreedily()
    {
        var line = LineOf("uno due tre quattro cinque", Options(100));

        var rows = line.Rows.Select(r => Single(r, RunKind.Lyric).Text).ToArray();
        Assert.Equal(new[] { "uno due", "tre", "quattro", "cinque" }, rows);
    }

    [Fact]
    public void TrailingWhitespaceDoesNotForceAWrap()
    {
        var line = LineOf("Signore ", Options(70));

        var row = OnlyRow(line);
        Assert.Equal("Signore", Single(row, RunKind.Lyric).Text);
    }

    [Fact]
    public void InstrumentalLines_WrapBetweenChords()
    {
        var line = LineOf("[Do] [Sol] [Do] [Sol] [Do]", Options(50));

        Assert.Equal(new[] { 2, 1, 1, 1 }, line.Rows.Select(r => Of(r, RunKind.Chord).Count));
    }

    [Fact]
    public void ALeadingSpace_IsKeptOnTheFirstRow()
    {
        var line = LineOf(" Signore pietà", Options(100));

        Assert.Equal(new[] { " Signore", "pietà" }, line.Rows.Select(r => Single(r, RunKind.Lyric).Text));
    }

    [Fact]
    public void AChordThatFollowsATextOnlyHardBreak_IsNeverLost()
    {
        var line = LineOf("Amen[Sol]", Options(30));

        Assert.Equal("Amen", string.Concat(line.Rows.SelectMany(r => Of(r, RunKind.Lyric)).Select(l => l.Text)));
        Assert.Equal("Sol", Single(line.Rows[^1], RunKind.Chord).Text);
        Assert.True(line.Rows.Count >= 3);
    }

    [Fact]
    public void AHardBreakBeforeAChord_CutsAtTheChordBoundary()
    {
        var line = LineOf("Al[Sol]leluia", Options(40));

        Assert.Equal("Al", Single(line.Rows[0], RunKind.Lyric).Text);
        Assert.Empty(Of(line.Rows[0], RunKind.Chord));
        Assert.True(line.Rows[0].HardBreakAfter);
        Assert.Equal("Sol", Single(line.Rows[1], RunKind.Chord).Text);
    }

    [Fact]
    public void AChordInTrailingWhitespace_StillAppears()
    {
        var line = LineOf("Signore [Sol] ", Options(70));

        Assert.Contains(line.Rows.SelectMany(r => r.Runs), r => r.Kind == RunKind.Chord && r.Text == "Sol");
        Assert.Equal("Signore", string.Concat(line.Rows.SelectMany(r => Of(r, RunKind.Lyric)).Select(l => l.Text)).TrimEnd());
    }
}
