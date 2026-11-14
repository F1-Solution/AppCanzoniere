using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

// The lyrics below are written for these tests; '^' marks the character a chord sits over.
public class ColumnSplitterTests
{
    private static readonly string[] LeftLyrics =
    {
        "^Cammina con me per la ^strada",
        "^dove il sole ^nasce al mattino",
        "^porta la tua ^luce nel cuore",
        "^canta con noi la ^gioia del cielo",
    };

    private static readonly string[] RightLyrics =
    {
        "^Vento che soffia sul ^mare",
        "^onde che corrono ^verso la riva",
        "^voce che chiama il ^nome di tutti",
        "^sera che scende ^piano sul monte",
    };

    private static readonly string[] Chords = { "DO", "SOL" };

    private static string[] TextOf(LayoutRegion region) => region.Rows.SelectMany(r => r.Words).Select(w => w.Text).ToArray();

    private static PageBuilder TwoColumns(double rightOffset = 0, double leftX = 40, double rightX = 310, double top = 760)
    {
        var page = new PageBuilder();
        page.Song(LeftLyrics, Chords, leftX, top);
        page.Song(RightLyrics, Chords, rightX, top - rightOffset);
        return page;
    }

    [Fact]
    public void Split_ReturnsNoRegions_ForAnEmptyPage()
    {
        Assert.Empty(ColumnSplitter.Split(Array.Empty<PositionedWord>(), PageBuilder.PageWidth));
    }

    [Fact]
    public void Split_ReadsTheLeftColumnBeforeTheRightOne_WhenTwoSongsSitSideBySide()
    {
        var regions = ColumnSplitter.Split(TwoColumns().Words, PageBuilder.PageWidth);

        Assert.Equal(new[] { (0, 2), (1, 2) }, regions.Select(r => (r.ColumnIndex, r.ColumnCount)));
        Assert.Contains("Cammina", TextOf(regions[0]));
        Assert.DoesNotContain("Vento", TextOf(regions[0]));
        Assert.Contains("Vento", TextOf(regions[1]));
        Assert.DoesNotContain("Cammina", TextOf(regions[1]));
        Assert.Equal(8, regions[0].Rows.Count);
        Assert.Equal(8, regions[1].Rows.Count);
    }

    [Fact]
    public void Split_KeepsEachColumnsRows_WhenTheColumnsBaselinesAreOffset()
    {
        // In real booklets the right column's lines often sit a couple of points lower than the left ones,
        // which makes line-by-line extraction interleave them.
        var regions = ColumnSplitter.Split(TwoColumns(rightOffset: 2.2).Words, PageBuilder.PageWidth);

        Assert.Equal(2, regions.Count);
        Assert.Equal(8, regions[0].Rows.Count);
        Assert.Equal(8, regions[1].Rows.Count);
        Assert.All(regions[0].Rows, r => Assert.True(r.Right < PageBuilder.PageWidth / 2));
        Assert.All(regions[1].Rows, r => Assert.True(r.Left > PageBuilder.PageWidth / 2));
    }

    [Fact]
    public void Split_SeparatesColumns_AcrossANarrowGutter()
    {
        // One booklet in the corpus leaves only about five points between its columns.
        var widest = LeftLyrics.Max(l => PageBuilder.Width(l.Replace("^", string.Empty).Replace(" ", string.Empty)) + l.Count(c => c == ' ') * PageBuilder.SpaceWidth);
        const double rightX = 302;
        var regions = ColumnSplitter.Split(TwoColumns(leftX: rightX - 5 - widest, rightX: rightX).Words, PageBuilder.PageWidth);

        Assert.Equal(new[] { (0, 2), (1, 2) }, regions.Select(r => (r.ColumnIndex, r.ColumnCount)));
    }

    [Fact]
    public void Split_PutsAFullWidthTitleFirst_ThenTheTwoColumns()
    {
        var page = TwoColumns(top: 740).Text("UN TITOLO MOLTO LUNGO CHE ATTRAVERSA TUTTA LA PAGINA", 150, 800);

        var regions = ColumnSplitter.Split(page.Words, PageBuilder.PageWidth);

        Assert.Equal(new[] { (0, 1), (0, 2), (1, 2) }, regions.Select(r => (r.ColumnIndex, r.ColumnCount)));
        Assert.Equal("UN", TextOf(regions[0])[0]);
        Assert.Single(regions[0].Rows);
    }

    [Fact]
    public void Split_PutsAFullWidthParagraphBelowTheColumns_AfterBothColumns()
    {
        var page = TwoColumns().Text("una riga conclusiva che attraversa tutta la larghezza del foglio da un lato all'altro", 40, 120);

        var regions = ColumnSplitter.Split(page.Words, PageBuilder.PageWidth);

        Assert.Equal(new[] { (0, 2), (1, 2), (0, 1) }, regions.Select(r => (r.ColumnIndex, r.ColumnCount)));
        Assert.Equal("una", TextOf(regions[2])[0]);
    }

    [Fact]
    public void Split_KeepsOneColumn_WhenChordsFarApartLeaveAGapInTheMiddle()
    {
        var page = new PageBuilder();
        page.Song(
            new[]
            {
                "^Questa lunga strada che porta lontano dove nasce il ^giorno e finisce la sera",
                "^tutti insieme camminiamo cantando sotto il cielo con la ^luce che ci guida",
                "^ogni passo che facciamo ci avvicina un poco alla casa del ^padre che ci aspetta",
                "^e la notte non fa paura a chi ha nel cuore la speranza di un ^nuovo mattino",
            },
            new[] { "LA-", "MI7" },
            40,
            760);

        var regions = ColumnSplitter.Split(page.Words, PageBuilder.PageWidth);

        Assert.Single(regions);
        Assert.Equal((0, 1), (regions[0].ColumnIndex, regions[0].ColumnCount));
        Assert.Equal(8, regions[0].Rows.Count);
    }

    [Fact]
    public void Split_KeepsOneColumn_WhenOnlyChordsAtTheEndOfLinesReachTheRightHalf()
    {
        // Single-column songs often end a line with a chord placed well to the right; a stack of such chords
        // lines up like a narrow column but holds no lyrics, so it must not become one.
        var page = new PageBuilder();
        for (var i = 0; i < 8; i++)
        {
            var baseline = 760 - (i * 24);
            page.Word("RE", 40, baseline + 12).Word("SOL", 120, baseline + 12).Word("LA7", 330, baseline + 12);
            page.Text("canto per te la mia canzone", 40, baseline);
        }

        var regions = ColumnSplitter.Split(page.Words, PageBuilder.PageWidth);

        Assert.Single(regions);
        Assert.Equal(16, regions[0].Rows.Count);
    }
}
