using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class SectionTests
{
    [Fact]
    public void Case16_TwoLinesInASection_AreSeparatedByTheLineSpacing()
    {
        var section = OnlySection(Flow(Lines("[Do]Signore", "Pietà")));

        Assert.Equal(2, section.Lines.Count);
        Assert.Equal((0d, 44d), (section.Lines[0].Y, section.Lines[0].Height));
        Assert.Equal((48d, 24d), (section.Lines[1].Y, section.Lines[1].Height));
        Assert.Equal(72, section.Height);
    }

    [Fact]
    public void Case17_ABlankLine_SeparatesSections()
    {
        var flow = Flow(Lines("Signore", "", "Pietà"));

        Assert.Equal(new[] { 0d, 40 }, flow.Sections.Select(s => s.Y));
        Assert.Equal(64, flow.Height);
    }

    [Fact]
    public void Case18_RunsOfBlankLines_CollapseAndEdgesProduceNothing()
    {
        var flow = Flow(Lines("", "", "Signore", "", "", "", "Pietà", "", ""));

        Assert.Equal(new[] { 0d, 40 }, flow.Sections.Select(s => s.Y));
        Assert.Equal(64, flow.Height);
    }

    [Fact]
    public void Case19_ATypeChangeWithoutABlank_StartsANewSection()
    {
        var flow = Flow(Lines("Signore", "{soc}", "Santo", "{eoc}"));

        Assert.Equal(new[] { SectionKind.Verse, SectionKind.Chorus }, flow.Sections.Select(s => s.Kind));
        Assert.Equal(new[] { 0d, 40 }, flow.Sections.Select(s => s.Y));
    }

    [Fact]
    public void BridgeLines_FormABridgeSection()
    {
        var flow = Flow(Lines("{sob}", "Santo", "{eob}"));

        Assert.Equal(SectionKind.Bridge, OnlySection(flow).Kind);
        Assert.Equal(TextRole.Lyric, Single(OnlyRow(flow.Sections[0].Lines[0]), RunKind.Lyric).Role);
    }

    [Fact]
    public void Case20_ACommentRightBeforeASection_BecomesItsLabel()
    {
        var section = OnlySection(Flow(Lines("{c: Rit.}", "Santo")));

        Assert.Equal(SectionKind.Verse, section.Kind);
        Assert.Equal("Rit.", section.Label);
        var label = section.LabelRun!;
        Assert.Equal(RunKind.SectionLabel, label.Kind);
        Assert.Equal(TextRole.SectionLabel, label.Role);
        AssertBox(label, 0, 32, 0, 18, 14);
        Assert.Equal(22, Assert.Single(section.Lines).Y);
        Assert.Equal(46, section.Height);
    }

    [Fact]
    public void Case21_AStandaloneComment_IsItsOwnSection()
    {
        var flow = Flow(Lines("Signore", "", "{c: Ripetere}", "", "Pietà"));

        Assert.Equal(new[] { SectionKind.Verse, SectionKind.Comment, SectionKind.Verse }, flow.Sections.Select(s => s.Kind));
        Assert.Equal(new[] { 0d, 40, 78 }, flow.Sections.Select(s => s.Y));
        var run = Single(OnlyRow(flow.Sections[1].Lines[0]), RunKind.Comment);
        Assert.Equal("Ripetere", run.Text);
        Assert.Equal(TextRole.Comment, run.Role);
        AssertBox(run, 0, 72, 40, 22);
    }

    [Fact]
    public void Case22_TheTitle_IsDroppedFromTheLayout()
    {
        var section = OnlySection(Flow(Lines("{title: Santo}", "Signore")));

        var line = Assert.Single(section.Lines);
        Assert.Equal(0, line.Y);
        Assert.Equal(0, line.SourceLineIndex);
    }

    [Fact]
    public void SeveralComments_OnlyTheLastOneLabelsTheSection()
    {
        var flow = Flow(Lines("{c: Uno}", "{c: Due}", "Santo"));

        Assert.Equal(new[] { SectionKind.Comment, SectionKind.Verse }, flow.Sections.Select(s => s.Kind));
        Assert.Equal("Uno", Single(OnlyRow(flow.Sections[0].Lines[0]), RunKind.Comment).Text);
        Assert.Equal("Due", flow.Sections[1].Label);
        Assert.Equal(22 + 16, flow.Sections[1].Y);
    }

    [Fact]
    public void ConsecutiveCommentsWithoutASectionAfter_ShareOneCommentSection()
    {
        var section = OnlySection(Flow(Lines("{c: Uno}", "{c: Due}")));

        Assert.Equal(2, section.Lines.Count);
        Assert.Equal(22 + 4, section.Lines[1].Y);
    }

    [Fact]
    public void ACommentLabel_WinsOverATabLabel()
    {
        var section = OnlySection(Flow(Lines("{c: Intro}", "{sot: Chitarra}", "e|--0--|", "{eot}")));

        Assert.Equal(SectionKind.Tab, section.Kind);
        Assert.Equal("Intro", section.Label);
    }

    [Fact]
    public void ACommentFollowedByABlank_DoesNotLabelTheNextSection()
    {
        var flow = Flow(Lines("{c: Ripetere}", "", "Santo"));

        Assert.Equal(new[] { SectionKind.Comment, SectionKind.Verse }, flow.Sections.Select(s => s.Kind));
        Assert.All(flow.Sections, s => Assert.Null(s.LabelRun));
    }

    [Fact]
    public void CommentTextAndLabels_AreTrimmed()
    {
        var flow = Flow(Lines("{c:   Rit.  }", "Santo", "", "{comment:  Ripetere }"));

        Assert.Equal("Rit.", flow.Sections[0].Label);
        Assert.Equal("Ripetere", Single(OnlyRow(flow.Sections[1].Lines[0]), RunKind.Comment).Text);
    }

    [Fact]
    public void AnEmptyComment_ProducesNothing()
    {
        var flow = Flow(Lines("{c: }", "Santo"));

        var section = OnlySection(flow);
        Assert.Null(section.Label);
    }

    [Fact]
    public void ALabelledChorus_KeepsItsKindAndRole()
    {
        var section = OnlySection(Flow(Lines("{c: Ritornello}", "{soc}", "[Do]Santo", "{eoc}")));

        Assert.Equal(SectionKind.Chorus, section.Kind);
        Assert.Equal("Ritornello", section.Label);
    }

    [Fact]
    public void AnEmptyDocument_HasNoSections()
    {
        var flow = Flow(string.Empty);

        Assert.Empty(flow.Sections);
        Assert.Equal(0, flow.Height);
    }

    [Fact]
    public void SectionSpacing_ComesFromTheOptions()
    {
        var flow = Flow(Lines("Signore", "", "Pietà"), Options() with { SectionSpacing = 30 });

        Assert.Equal(54, flow.Sections[1].Y);
    }
}
