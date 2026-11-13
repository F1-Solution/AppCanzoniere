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
    public void ASectionOfOnlyInstrumentalLines_DisappearsButItsLabelIsKeptAsAComment()
    {
        var flow = Flow(Lines("Signore", "", "{c: Intro}", "[Do] [Sol]", "", "Pietà"), LyricsOnly());

        Assert.Equal(new[] { SectionKind.Verse, SectionKind.Comment, SectionKind.Verse }, flow.Sections.Select(s => s.Kind));
        Assert.All(flow.Sections, s => Assert.Null(s.Label));
        Assert.Equal(new[] { 0d, 40, 78 }, flow.Sections.Select(s => s.Y));
    }

    [Fact]
    public void TheChordsAndLyricsModeKeepsTheInstrumentalLine()
    {
        var section = OnlySection(Flow(Lines("[Do] [Sol]", "Signore")));

        Assert.Equal(2, section.Lines.Count);
    }

    [Fact]
    public void ACommentBeforeADroppedInstrumentalLine_IsShownAsAComment()
    {
        var flow = Flow(Lines("{c: Intro}", "[G] [C] [D]", "", "[G]Verse"), LyricsOnly());

        Assert.Equal(new[] { SectionKind.Comment, SectionKind.Verse }, flow.Sections.Select(s => s.Kind));
        Assert.Equal("Intro", Single(OnlyRow(flow.Sections[0].Lines[0]), RunKind.Comment).Text);
        Assert.Equal(38, flow.Sections[1].Y);
    }

    [Fact]
    public void ALabel_KeepsLabellingTheSection_WhenItsFirstLineIsInstrumentalButLaterLinesRemain()
    {
        var section = OnlySection(Flow(Lines("{c: Intro}", "[G] [C]", "[G]Verse"), LyricsOnly()));

        Assert.Equal("Intro", section.Label);
        Assert.Equal(2, Assert.Single(section.Lines).SourceLineIndex);
        Assert.Equal(22, section.Lines[0].Y);
    }

    [Fact]
    public void ACommentThenADroppedLineThenAnotherComment_ShowsTheFirstAndLabelsWithTheSecond()
    {
        var flow = Flow(Lines("{c: A}", "[G]", "{c: B}", "Verse"), LyricsOnly());

        Assert.Equal(new[] { SectionKind.Comment, SectionKind.Verse }, flow.Sections.Select(s => s.Kind));
        Assert.Equal("A", Single(OnlyRow(flow.Sections[0].Lines[0]), RunKind.Comment).Text);
        Assert.Equal("B", flow.Sections[1].Label);
    }

    [Fact]
    public void ACommentBeforeADroppedLineAtTheEnd_IsShownAsAComment()
    {
        var flow = Flow(Lines("Verse", "", "{c: Outro}", "[G] [C]"), LyricsOnly());

        Assert.Equal(new[] { SectionKind.Verse, SectionKind.Comment }, flow.Sections.Select(s => s.Kind));
    }

    [Fact]
    public void ALabelIsNotCarriedOverToASectionOfAnotherKind()
    {
        var flow = Flow(Lines("{c: Intro}", "[G]", "{soc}", "Santo", "{eoc}"), LyricsOnly());

        Assert.Equal(new[] { SectionKind.Comment, SectionKind.Chorus }, flow.Sections.Select(s => s.Kind));
        Assert.Null(flow.Sections[1].Label);
    }

    [Fact]
    public void InChordsAndLyricsMode_TheLabelStaysOnTheInstrumentalSection()
    {
        var section = OnlySection(Flow(Lines("{c: Intro}", "[G] [C]", "[G]Verse")));

        Assert.Equal("Intro", section.Label);
        Assert.Equal(2, section.Lines.Count);
    }
}
