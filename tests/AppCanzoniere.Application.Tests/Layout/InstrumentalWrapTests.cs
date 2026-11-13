using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class InstrumentalWrapTests
{
    private static string Chords(int count, string chord = "[Em7]") => string.Concat(Enumerable.Repeat(chord, count));

    private static IReadOnlyList<LayoutRow> RowsOf(string song, double width) =>
        OnlySection(Flow(song, Options(width))).Lines[0].Rows;

    [Fact]
    public void ThirtyChordsAtWidth200_WrapIntoSixRows_SixThenFiveFiveFiveFiveFour()
    {
        var rows = RowsOf(Chords(30), 200);

        Assert.Equal(new[] { 6, 5, 5, 5, 5, 4 }, rows.Select(r => Of(r, RunKind.Chord).Count));
    }

    [Fact]
    public void WrappedInstrumentalRows_StartAtTheContinuationIndent_AndKeepTheMinimumGap()
    {
        var rows = RowsOf(Chords(30), 200);

        Assert.Equal(new[] { 0d, 32, 64, 96, 128, 160 }, Of(rows[0], RunKind.Chord).Select(r => r.X));
        Assert.Equal(new[] { 20d, 52, 84, 116, 148 }, Of(rows[1], RunKind.Chord).Select(r => r.X));
        Assert.Equal(new[] { 0d, 24, 48, 72, 96, 120 }, rows.Select(r => r.Y));
        Assert.All(rows, r => Assert.Equal(20, r.Height));
    }

    [Fact]
    public void NoInstrumentalRow_ExceedsTheWidth_AndNoneIsFlagged()
    {
        var rows = RowsOf(Chords(30), 200);

        Assert.All(rows, r => Assert.False(r.Overflow));
        Assert.All(rows, r => Assert.True(r.Runs.Max(run => run.X + run.Width) <= 200));
    }

    [Fact]
    public void TheLineHeightCoversAllWrappedRows()
    {
        var line = OnlySection(Flow(Chords(30), Options(200))).Lines[0];

        Assert.Equal(140, line.Height);
    }

    [Fact]
    public void AChordWiderThanTheWidth_IsAloneOnItsRow_AndTheRowIsFlagged()
    {
        var rows = RowsOf("[Em7][Am7]", 20);

        Assert.Equal(2, rows.Count);
        Assert.Equal(new[] { true, true }, rows.Select(r => r.Overflow));
        Assert.Equal("Em7", Single(rows[0], RunKind.Chord).Text);
        Assert.Equal(20, Single(rows[1], RunKind.Chord).X);
    }

    [Fact]
    public void AnAnnotation_IsAnUnbreakableAtomLikeAChord()
    {
        var fits = RowsOf("[Em7][*xxxx]", 60);
        var wraps = RowsOf("[Em7][*xxxx]", 59);

        Assert.Single(fits);
        Assert.Equal(2, wraps.Count);
        var annotation = Single(wraps[1], RunKind.Annotation);
        Assert.Equal((20d, 28d), (annotation.X, annotation.Width));
    }

    [Fact]
    public void AnInstrumentalLineThatFits_IsStillOneRow()
    {
        var rows = RowsOf(Chords(6), 200);

        Assert.Single(rows);
        Assert.False(rows[0].Overflow);
    }

    [Fact]
    public void Pagination_SplitsTheRowsOfAnInstrumentalLineLikeAnyOtherLine()
    {
        var layout = LayoutKit.Layout(Chords(30), Options(200, 100));

        Assert.Equal(2, layout.Pages.Count);
        var first = layout.Pages[0].Sections[0].Lines[0];
        var second = layout.Pages[1].Sections[0].Lines[0];
        Assert.Equal(new[] { 0, 1, 2, 3 }, first.Rows.Select(r => r.Index));
        Assert.Equal(new[] { 4, 5 }, second.Rows.Select(r => r.Index));
        Assert.True(layout.Pages[1].Sections[0].ContinuedFromPrevious);
        Assert.Equal(0, second.Rows[0].Y);
        Assert.False(layout.Pages[0].Overflow);
    }
}
