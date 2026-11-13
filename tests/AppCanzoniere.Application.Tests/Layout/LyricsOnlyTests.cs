using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class LyricsOnlyTests
{
    private static LayoutOptions LyricsOnly(double width = 400) =>
        Options(width) with { DisplayMode = LayoutDisplayMode.LyricsOnly };

    [Fact]
    public void Case14_ChordsAreIgnored_AndTheLineIsOneLyricRun()
    {
        var row = OnlyRow(OnlySection(Flow("[Do]Signore [Sol]pietà", LyricsOnly())).Lines[0]);

        Assert.Equal(24, row.Height);
        var run = Assert.Single(row.Runs);
        Assert.Equal(RunKind.Lyric, run.Kind);
        Assert.Equal("Signore pietà", run.Text);
        AssertBox(run, 0, 130, 0);
        Assert.False(run.HyphenAfter);
    }

    [Fact]
    public void Case15_InstrumentalLines_AreDroppedEntirely()
    {
        var section = OnlySection(Flow(Lines("[Do] [Sol]", "Signore"), LyricsOnly()));

        var line = Assert.Single(section.Lines);
        Assert.Equal(1, line.SourceLineIndex);
        Assert.Equal("Signore", Single(OnlyRow(line), RunKind.Lyric).Text);
        Assert.Equal(0, line.Y);
        Assert.Equal(24, section.Height);
    }

    [Fact]
    public void AnnotationsAreIgnoredToo()
    {
        var row = OnlyRow(OnlySection(Flow("Amen[*x2]", LyricsOnly())).Lines[0]);

        Assert.Equal("Amen", Assert.Single(row.Runs).Text);
    }

    [Fact]
    public void ASectionOfOnlyInstrumentalLines_DisappearsWithItsLabel()
    {
        var flow = Flow(Lines("Signore", "", "{c: Intro}", "[Do] [Sol]", "", "Pietà"), LyricsOnly());

        Assert.Equal(2, flow.Sections.Count);
        Assert.All(flow.Sections, s => Assert.Null(s.Label));
        Assert.Equal(40, flow.Sections[1].Y);
    }

    [Fact]
    public void TheChordsAndLyricsModeKeepsTheInstrumentalLine()
    {
        var section = OnlySection(Flow(Lines("[Do] [Sol]", "Signore")));

        Assert.Equal(2, section.Lines.Count);
    }
}
