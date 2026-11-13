using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class TabLayoutTests
{
    private const string Tab14 = "e|-----0-----|";
    private const string Tab20 = "e|--0--2--3--2--0--|";
    private const string Tab10 = "e|--0--2-|";

    private static string Tab(params string[] lines) => Lines(["{sot}", .. lines, "{eot}"]);

    [Fact]
    public void Case23_ATabLineThatFits_KeepsFullScale()
    {
        var section = OnlySection(Flow(Tab(Tab14), Options(400)));

        Assert.Equal(SectionKind.Tab, section.Kind);
        Assert.Equal(1, section.TabScale);
        Assert.False(section.TabClipped);
        var run = Single(OnlyRow(section.Lines[0]), RunKind.Tab);
        Assert.Equal(TextRole.Tab, run.Role);
        Assert.Equal(Tab14, run.Text);
        AssertBox(run, 0, 84, 0, 16, 12);
    }

    [Fact]
    public void Case24_AWideTabBlock_IsScaledToFit()
    {
        var section = OnlySection(Flow(Tab(Tab20), Options(90)));

        Assert.Equal(0.75, section.TabScale, 9);
        Assert.False(section.TabClipped);
        AssertBox(Single(OnlyRow(section.Lines[0]), RunKind.Tab), 0, 90, 0, 12, 9);
    }

    [Fact]
    public void Case25_ATabBelowTheMinimumScale_IsClipped()
    {
        var section = OnlySection(Flow(Tab(Tab20), Options(50)));

        Assert.Equal(0.5, section.TabScale, 9);
        Assert.True(section.TabClipped);
        AssertBox(Single(OnlyRow(section.Lines[0]), RunKind.Tab), 0, 60);
    }

    [Fact]
    public void Case26_AllLinesOfABlockShareTheScale_AndAreStackedWithoutSpacing()
    {
        var section = OnlySection(Flow(Tab(Tab20, Tab10), Options(90)));

        Assert.Equal(2, section.Lines.Count);
        var second = Single(OnlyRow(section.Lines[1]), RunKind.Tab);
        AssertBox(second, 0, 45, 12, 12);
        Assert.Equal(24, section.Height, 9);
    }

    [Fact]
    public void Case27_ATabLabel_IsDrawnAboveTheFirstLine()
    {
        var section = OnlySection(Flow(Lines("{sot: Intro}", Tab14, "{eot}"), Options(400)));

        Assert.Equal("Intro", section.Label);
        AssertBox(section.LabelRun!, 0, 40, 0, 18, 14);
        Assert.Equal(22, section.Lines[0].Y);
        Assert.Equal(RunKind.SectionLabel, section.LabelRun!.Kind);
    }

    [Fact]
    public void TabLines_KeepTheirWhitespaceAndNeverWrap()
    {
        var section = OnlySection(Flow(Tab("  e|--0--|  "), Options(30) with { MinTabScale = 0.1 }));

        var row = OnlyRow(section.Lines[0]);
        Assert.Equal("  e|--0--|  ", Single(row, RunKind.Tab).Text);
        Assert.Equal(30d / 72, section.TabScale, 9);
    }

    [Fact]
    public void ABlankLineInsideATabBlock_StaysInTheBlock()
    {
        var flow = Flow(Tab("e|--0--|", "", "B|--1--|"));

        Assert.Single(flow.Sections);
        Assert.Equal(3, flow.Sections[0].Lines.Count);
    }

    [Fact]
    public void TwoTabBlocks_AreTwoSections()
    {
        var flow = Flow(Lines(Tab("e|--0--|"), Tab("e|--1--|")));

        Assert.Equal(2, flow.Sections.Count);
        Assert.Equal(16 + 16, flow.Sections[1].Y);
    }

    [Fact]
    public void TabLines_HaveNoNumbers_AndTheirOffsetStartsAtZero()
    {
        var line = OnlySection(Flow(Tab(Tab14))).Lines[0];

        Assert.Null(line.Number);
        Assert.Equal(0, Single(OnlyRow(line), RunKind.Tab).SourceOffset);
    }
}
