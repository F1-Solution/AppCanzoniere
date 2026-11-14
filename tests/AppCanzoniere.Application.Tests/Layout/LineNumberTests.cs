using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class LineNumberTests
{
    private static LayoutOptions Numbered(double width = 400) => Options(width) with { LineNumbers = true };

    [Fact]
    public void Case28_LyricLinesAreNumbered_ButInstrumentalLinesAreNot()
    {
        var flow = Flow(Lines("Uno", "Due", "[Do] [Sol]", "Tre"), Numbered());

        var lines = OnlySection(flow).Lines;
        Assert.Equal(new int?[] { 1, 2, null, 3 }, lines.Select(l => l.Number));
        Assert.Equal(14, flow.GutterWidth);
        Assert.Equal(3, flow.NumberedLineCount);
        Assert.Equal(new[] { 0d, 28, 56, 80 }, lines.Select(l => l.Y));
        Assert.Equal(20, lines[2].Height);
        Assert.All(new[] { lines[0], lines[1], lines[3] }, l => Assert.Equal(14, Single(OnlyRow(l), RunKind.Lyric).X));
        var number = Single(OnlyRow(lines[0]), RunKind.LineNumber);
        Assert.Equal("1", number.Text);
        Assert.Equal(TextRole.LineNumber, number.Role);
        AssertBox(number, 0, 6, 0, 24, 19);
    }

    [Fact]
    public void Case29_TwoDigitNumbers_WidenTheGutter_AndContinueAcrossSections()
    {
        var song = Lines(
            "L1", "L2", "L3", "L4", "L5", "",
            "{c: Nota}", "L6", "L7", "{sot}", "e|--0--|", "{eot}", "",
            "L8", "L9", "L10", "L11", "L12");
        var flow = Flow(song, Numbered());

        Assert.Equal(20, flow.GutterWidth);
        Assert.Equal(12, flow.NumberedLineCount);
        var numbers = flow.Sections.SelectMany(s => s.Lines).Select(l => l.Number).ToArray();
        Assert.Equal(new int?[] { 1, 2, 3, 4, 5, 6, 7, null, 8, 9, 10, 11, 12 }, numbers);
        var runs = flow.Sections.SelectMany(s => s.Lines).SelectMany(l => l.Rows).SelectMany(r => r.Runs).Where(r => r.Kind == RunKind.LineNumber).ToArray();
        AssertBox(runs.Single(r => r.Text == "3"), 6, 6);
        AssertBox(runs.Single(r => r.Text == "12"), 0, 12);
    }

    [Fact]
    public void ContentOfEveryKind_IsShiftedByTheGutter()
    {
        var flow = Flow(Lines("{c: Nota}", "Uno", "", "{c: Standalone}", "", "{sot: T}", "e|--0--|", "{eot}"), Numbered());

        Assert.Equal(14, flow.GutterWidth);
        Assert.Equal(14, flow.Sections[0].LabelRun!.X);
        Assert.Equal(14, Single(OnlyRow(flow.Sections[1].Lines[0]), RunKind.Comment).X);
        Assert.Equal(14, flow.Sections[2].LabelRun!.X);
        Assert.Equal(14, Single(OnlyRow(flow.Sections[2].Lines[0]), RunKind.Tab).X);
    }

    [Fact]
    public void Case36_TheNumberIsOnTheFirstRowOnly_AndTheGutterNarrowsTheWrap()
    {
        var flow = Flow("[Do]Signore [Sol]pietà", Numbered(100));

        Assert.Equal(14, flow.GutterWidth);
        var line = flow.Sections[0].Lines[0];
        Assert.Equal(2, line.Rows.Count);
        Assert.Single(Of(line.Rows[0], RunKind.LineNumber));
        Assert.Empty(Of(line.Rows[1], RunKind.LineNumber));
        AssertBox(Single(line.Rows[1], RunKind.Lyric), 34, 50);
        Assert.Equal(14, Single(line.Rows[0], RunKind.Lyric).X);
    }

    [Fact]
    public void WithoutLineNumbers_ThereIsNoGutterAndNoNumberRun_ButLinesStillCarryTheirNumber()
    {
        var flow = Flow(Lines("Uno", "Due"));

        Assert.Equal(0, flow.GutterWidth);
        Assert.Equal(2, flow.NumberedLineCount);
        Assert.Equal(new int?[] { 1, 2 }, OnlySection(flow).Lines.Select(l => l.Number));
        Assert.All(OnlySection(flow).Lines, l => Assert.Empty(Of(OnlyRow(l), RunKind.LineNumber)));
    }

    [Fact]
    public void NumberedLines_AreCountedInLyricsOnlyMode_WithoutTheInstrumentalOnes()
    {
        var flow = Flow(Lines("[Do] [Sol]", "Uno", "Due"), Numbered() with { DisplayMode = LayoutDisplayMode.LyricsOnly });

        Assert.Equal(2, flow.NumberedLineCount);
        Assert.Equal(new int?[] { 1, 2 }, OnlySection(flow).Lines.Select(l => l.Number));
    }

    private sealed class TallNumberMeasurer : ITextMeasurer
    {
        private readonly FixedAdvanceMeasurer _inner = new();

        public TextMetrics Measure(string text, TextRole role) =>
            role == TextRole.LineNumber ? new TextMetrics(text.Length * 6, 40, 30) : _inner.Measure(text, role);
    }

    [Fact]
    public void ANumberFontTallerThanTheRow_StillHasItsBoxInsideTheRow()
    {
        var flow = Flow("Signore", Numbered(), new TallNumberMeasurer());

        var row = OnlyRow(flow.Sections[0].Lines[0]);
        var number = Single(row, RunKind.LineNumber);
        Assert.True(number.Y >= row.Y);
        Assert.True(number.Y + number.Height <= row.Y + row.Height);
    }

    [Fact]
    public void NoLyricLines_MeansNoGutter()
    {
        var flow = Flow(Lines("{c: Solo commento}"), Numbered());

        Assert.Equal(0, flow.GutterWidth);
        Assert.Equal(0, flow.NumberedLineCount);
    }

    [Fact]
    public void TheNumberBaseline_FollowsTheLyricBaselineBelowTheChords()
    {
        var flow = Flow("[Do]Signore", Numbered());

        var row = OnlyRow(flow.Sections[0].Lines[0]);
        var number = Single(row, RunKind.LineNumber);
        Assert.Equal(Single(row, RunKind.Lyric).BaselineY, number.BaselineY);
        Assert.Equal(20, number.Y);
    }
}
