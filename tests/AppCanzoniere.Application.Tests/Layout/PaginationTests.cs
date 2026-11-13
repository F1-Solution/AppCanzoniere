using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class PaginationTests
{
    private static string Chords(int count) => Lines(Enumerable.Repeat("[Do]Santo", count).ToArray());

    private static string Plain(int count) => Lines(Enumerable.Repeat("Santo", count).ToArray());

    private static string Chorus(string body) => Lines("{soc}", body, "{eoc}");

    private static SongLayout Paged(string song, double height, LayoutOptions? options = null) =>
        LayoutKit.Layout(song, (options ?? Options()) with { PageHeight = height });

    private static double[] LineYs(LayoutSection s) => s.Lines.Select(l => l.Y).ToArray();

    [Fact]
    public void Case39_ScrollMode_IsOnePageOfTheFlowHeight()
    {
        var layout = LayoutKit.Layout(Lines("[Do]Signore", "Pietà"), Options());

        var page = Assert.Single(layout.Pages);
        Assert.Equal(72, page.Height);
        Assert.False(page.Overflow);
        Assert.Equal(0, page.Index);
    }

    [Fact]
    public void Case40_ASectionThatDoesNotFit_MovesWholeToTheNextPage()
    {
        var layout = Paged(Lines(Chords(3), "", Chorus(Chords(3))), 200);

        Assert.Equal(2, layout.Pages.Count);
        var first = Assert.Single(layout.Pages[0].Sections);
        Assert.Equal(SectionKind.Verse, first.Kind);
        Assert.Equal(140, layout.Pages[0].Height);
        var second = Assert.Single(layout.Pages[1].Sections);
        Assert.Equal(SectionKind.Chorus, second.Kind);
        Assert.Equal(0, second.Y);
        Assert.False(second.ContinuedFromPrevious);
        Assert.Equal(new[] { 0, 1 }, layout.Pages.Select(p => p.Index));
    }

    [Fact]
    public void Case41_ALongVerse_SplitsAtThePageBottom()
    {
        var layout = Paged(Chords(6), 200);

        Assert.Equal(2, layout.Pages.Count);
        var top = Assert.Single(layout.Pages[0].Sections);
        Assert.Equal(4, top.Lines.Count);
        Assert.Equal(188, top.Height);
        Assert.True(top.ContinuesOnNext);
        Assert.False(top.ContinuedFromPrevious);
        var rest = Assert.Single(layout.Pages[1].Sections);
        Assert.Equal(new[] { 0d, 48 }, LineYs(rest));
        Assert.True(rest.ContinuedFromPrevious);
        Assert.False(rest.ContinuesOnNext);
        Assert.Equal(new[] { 4, 5 }, rest.Lines.Select(l => l.SourceLineIndex));
    }

    [Fact]
    public void Case42_WidowRule_LeavesTwoLinesForTheNextPage()
    {
        var layout = Paged(Chords(5), 200);

        Assert.Equal(new[] { 3, 2 }, layout.Pages.Select(p => p.Sections.Sum(s => s.Lines.Count)));
    }

    [Fact]
    public void Case43_OrphanRule_MovesTheSectionToANewPageInsteadOfLeavingOneLine()
    {
        var layout = Paged(Lines(Chords(3), "", Chords(5)), 200);

        Assert.Equal(3, layout.Pages.Count);
        Assert.Equal(new[] { 3 }, layout.Pages[0].Sections.Select(s => s.Lines.Count));
        Assert.Equal(new[] { 3 }, layout.Pages[1].Sections.Select(s => s.Lines.Count));
        Assert.Equal(new[] { 2 }, layout.Pages[2].Sections.Select(s => s.Lines.Count));
        Assert.Equal(0, layout.Pages[1].Sections[0].Y);
    }

    [Fact]
    public void Case44_ASmallChorus_IsKeptWholeOnTheNextPage()
    {
        var layout = Paged(Lines(Chords(2), "", Chorus(Chords(6))), 300);

        Assert.Equal(2, layout.Pages.Count);
        Assert.Equal(SectionKind.Verse, Assert.Single(layout.Pages[0].Sections).Kind);
        var chorus = Assert.Single(layout.Pages[1].Sections);
        Assert.Equal(6, chorus.Lines.Count);
        Assert.False(chorus.ContinuedFromPrevious);
        Assert.False(chorus.ContinuesOnNext);
    }

    [Fact]
    public void Case45_ALongChorus_IsSplitAfterTheFirstSectionOnThePage()
    {
        var layout = Paged(Lines(Chords(2), "", Chorus(Chords(7))), 300);

        Assert.Equal(2, layout.Pages.Count);
        Assert.Equal(2, layout.Pages[0].Sections.Count);
        var fragment = layout.Pages[0].Sections[1];
        Assert.Equal(SectionKind.Chorus, fragment.Kind);
        Assert.Equal(108, fragment.Y);
        Assert.Equal(188, fragment.Height);
        Assert.Equal(4, fragment.Lines.Count);
        Assert.True(fragment.ContinuesOnNext);
        var rest = Assert.Single(layout.Pages[1].Sections);
        Assert.Equal(SectionKind.Chorus, rest.Kind);
        Assert.Equal(3, rest.Lines.Count);
        Assert.True(rest.ContinuedFromPrevious);
    }

    [Fact]
    public void Case46_TheLabelStaysWithTheFirstLine()
    {
        var song = Lines("Uno", "", "{c: Strofa}", Plain(5));
        var options = Options() with { MinLinesAtBreak = 1 };

        var small = Paged(song, 80, options);
        Assert.Equal(3, small.Pages.Count);
        var onPage1 = Assert.Single(small.Pages[1].Sections);
        Assert.Equal("Strofa", onPage1.Label);
        Assert.Equal(0, onPage1.LabelRun!.Y);
        Assert.Equal(2, onPage1.Lines.Count);
        Assert.Equal(3, Assert.Single(small.Pages[2].Sections).Lines.Count);

        var tall = Paged(song, 100, options);
        var onPage0 = tall.Pages[0].Sections[1];
        Assert.Equal("Strofa", onPage0.Label);
        Assert.Equal(40, onPage0.Y);
        Assert.Single(onPage0.Lines);
    }

    [Fact]
    public void TheContinuation_HasNoLabel()
    {
        var song = Lines("{c: Strofa}", Plain(5));

        var layout = Paged(song, 80, Options() with { MinLinesAtBreak = 1 });

        var rest = layout.Pages[1].Sections[0];
        Assert.Null(rest.Label);
        Assert.Null(rest.LabelRun);
        Assert.True(rest.ContinuedFromPrevious);
        Assert.Equal(0, rest.Lines[0].Y);
    }

    [Fact]
    public void Case47_ALineTallerThanThePage_SplitsBetweenRows()
    {
        var layout = Paged("[Do]Signore [Re]Signore [Mi]Signore", 100, Options(100));

        Assert.Equal(2, layout.Pages.Count);
        var first = Assert.Single(layout.Pages[0].Sections);
        var firstLine = Assert.Single(first.Lines);
        Assert.Equal(new[] { 0, 1 }, firstLine.Rows.Select(r => r.Index));
        Assert.Equal(92, layout.Pages[0].Height);
        Assert.Equal(1, firstLine.Number);
        Assert.True(first.ContinuesOnNext);
        var second = Assert.Single(layout.Pages[1].Sections);
        var secondLine = Assert.Single(second.Lines);
        Assert.Equal(new[] { 2 }, secondLine.Rows.Select(r => r.Index));
        Assert.Equal(0, secondLine.Rows[0].Y);
        Assert.Equal(0, secondLine.Y);
        Assert.Null(secondLine.Number);
        Assert.False(layout.Pages[0].Overflow);
        Assert.False(layout.Pages[1].Overflow);
    }

    [Fact]
    public void Case48_ARowTallerThanThePage_IsPlacedAloneAndFlaggedAsOverflow()
    {
        var layout = Paged("[Do]Santo", 30);

        var page = Assert.Single(layout.Pages);
        Assert.Equal(44, page.Height);
        Assert.True(page.Overflow);
    }

    [Fact]
    public void Case50_AnEmptyDocument_IsOnePageOfHeightZero()
    {
        var flow = Flow(string.Empty);
        Assert.Empty(flow.Sections);
        Assert.Equal(0, flow.Height);

        foreach (var height in new double?[] { null, 100 })
        {
            var page = Assert.Single(LayoutKit.Layout(string.Empty, Options(400, height)).Pages);
            Assert.Equal(0, page.Height);
            Assert.False(page.Overflow);
            Assert.Empty(page.Sections);
        }
    }

    [Fact]
    public void Everything_FittingOnOnePage_KeepsTheFlowGeometry()
    {
        var song = Lines(Chords(2), "", Chords(2));
        var flow = Flow(song);
        var layout = Paged(song, 1000);

        var page = Assert.Single(layout.Pages);
        Assert.Equal(flow.Sections.Select(s => s.Y), page.Sections.Select(s => s.Y));
        Assert.Equal(flow.Height, page.Height);
    }

    [Fact]
    public void ASectionFittingExactlyOnThePage_IsNotSplit()
    {
        var layout = Paged(Chords(3), 140);

        var page = Assert.Single(layout.Pages);
        Assert.Equal(140, page.Height);
        Assert.False(page.Overflow);
    }

    [Fact]
    public void ASecondSection_StartsAfterTheSectionSpacingOnTheSamePage()
    {
        var layout = Paged(Lines(Chords(1), "", Chords(1)), 200);

        var page = Assert.Single(layout.Pages);
        Assert.Equal(new[] { 0d, 60 }, page.Sections.Select(s => s.Y));
    }

    [Fact]
    public void ShortSections_AreNeverSplit()
    {
        var layout = Paged(Lines(Chords(1), "", Chords(3)), 150);

        Assert.Equal(2, layout.Pages.Count);
        Assert.Equal(3, Assert.Single(layout.Pages[1].Sections).Lines.Count);
    }

    [Fact]
    public void Tabs_SplitWithoutLineSpacing()
    {
        var tab = Lines("{sot}", Lines(Enumerable.Repeat("e|--0--|", 12).ToArray()), "{eot}");
        var options = Options() with { KeepWholeMaxLines = 0 };

        var layout = Paged(tab, 100, options);

        Assert.Equal(2, layout.Pages.Count);
        Assert.Equal(96, layout.Pages[0].Sections[0].Height);
        Assert.Equal(6, layout.Pages[0].Sections[0].Lines.Count);
        Assert.Equal(new[] { 0d, 16 }, LineYs(layout.Pages[1].Sections[0]).Take(2));
    }

    [Fact]
    public void PaginationNeverMeasures()
    {
        var measurer = new FixedAdvanceMeasurer();
        var flow = Flow(Chords(6), Options(), measurer);
        var calls = measurer.TotalCalls;

        SongLayoutEngine.Paginate(flow, Options(400, 200));

        Assert.Equal(calls, measurer.TotalCalls);
    }

    [Fact]
    public void PaginateIsReusableForDifferentPageHeights()
    {
        var flow = Flow(Chords(6));

        var tall = SongLayoutEngine.Paginate(flow, Options(400, 300));
        var short200 = SongLayoutEngine.Paginate(flow, Options(400, 200));

        Assert.Single(tall.Pages);
        Assert.Equal(2, short200.Pages.Count);
        Assert.Equal(6, flow.Sections[0].Lines.Count);
        Assert.Equal(0, flow.Sections[0].Y);
    }

    [Fact]
    public void LineNumbersAndGutter_SurviveAPageSplit()
    {
        var layout = Paged(Chords(6), 200, Options() with { LineNumbers = true });

        Assert.Equal(14, layout.GutterWidth);
        Assert.Equal(6, layout.NumberedLineCount);
        Assert.Equal(new int?[] { 5, 6 }, layout.Pages[1].Sections[0].Lines.Select(l => l.Number));
    }
}
