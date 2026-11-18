using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

// Rule: in the top or bottom tenth of a page, a row with a link is dropped at the bottom, or at the top when repeated;
// any such row (digits ignored, two words or a number) repeated on three pages and a quarter of them is dropped too.
public class RunningLinesTests
{
    private sealed class Fake(IReadOnlyList<PdfPageWords> pages) : IPdfWordExtractor
    {
        public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default) =>
            Task.FromResult(PdfWordsResult.Success(pages));
    }

    private const double Footer = 30;
    private const double Header = 810;

    private static async Task<IReadOnlyList<string>> Import(params PdfPageWords[] pages)
    {
        var result = await new PdfTextImporter(new Fake(pages)).ImportAsync(new MemoryStream());
        return result.Pages.Select(p => p.Text).ToList();
    }

    private static PdfPageWords Page(int number, params (string Text, double Baseline)[] rows)
    {
        var builder = new PageBuilder();
        foreach (var (text, baseline) in rows)
            builder.Text(text, 50, baseline);
        return builder.Page(number);
    }

    [Theory]
    [InlineData("www.example.it | Canti per tutti 12")]
    [InlineData("https://example.it/canti/pagina-3")]
    [InlineData("example.com/canti Pagina 3")]
    [InlineData("scrivici info@example.it")]
    public async Task FooterWithALink_IsDropped_WhateverTheLinkLooksLike(string footer)
    {
        var texts = await Import(Page(1, ("Alleluia cantate al Signore", 400), (footer, Footer)));

        Assert.Equal("Alleluia cantate al Signore", texts[0]);
    }

    [Fact]
    public async Task HeaderWithALinkRepeatedOnThreePages_IsDropped()
    {
        var pages = Enumerable.Range(1, 3).Select(n => Page(n, ("www.example.it", Header), ("Alleluia cantate al Signore", 400))).ToArray();

        var texts = await Import(pages);

        Assert.All(texts, t => Assert.Equal("Alleluia cantate al Signore", t));
    }

    [Fact]
    public async Task LinkUnderASongTitleAtTheTopOfOnePage_IsKept()
    {
        var texts = await Import(Page(1, ("https://example.com/watch?v=abc", Header), ("Alleluia cantate al Signore", 400)));

        Assert.Contains("https://example.com/watch?v=abc", texts[0]);
    }

    [Fact]
    public async Task LineMentioningASiteInTheMiddleOfThePage_IsKept()
    {
        var texts = await Import(Page(1, ("Visita www.example.it per il testo", 400), ("Alleluia cantate al Signore", 300)));

        Assert.Contains("www.example.it", texts[0]);
    }

    [Fact]
    public async Task UniqueFooterWithoutALink_IsKept()
    {
        var texts = await Import(Page(1, ("Alleluia cantate al Signore", 400), ("Canti per tutti 12", Footer)));

        Assert.Contains("Canti per tutti 12", texts[0]);
    }

    [Fact]
    public async Task FooterRepeatedOnThreePagesWithoutALink_IsDropped_EvenWhenThePageNumberChanges()
    {
        var pages = Enumerable.Range(1, 3).Select(n => Page(n, ("Alleluia cantate al Signore", 400), ($"Canti per tutti {n}", Footer))).ToArray();

        var texts = await Import(pages);

        Assert.All(texts, t => Assert.Equal("Alleluia cantate al Signore", t));
    }

    [Fact]
    public async Task FooterRepeatedOnTwoPagesOnly_IsKept()
    {
        var pages = Enumerable.Range(1, 2).Select(n => Page(n, ("Alleluia cantate al Signore", 400), ($"Canti per tutti {n}", Footer))).ToArray();

        var texts = await Import(pages);

        Assert.All(texts, t => Assert.Contains("Canti per tutti", t));
    }

    [Fact]
    public async Task SingleWordAtTheBottomOfThreePages_IsKept()
    {
        var pages = Enumerable.Range(1, 3).Select(n => Page(n, ("Alleluia cantate al Signore", 400), ("Amen", Footer))).ToArray();

        var texts = await Import(pages);

        Assert.All(texts, t => Assert.Contains("Amen", t));
    }

    [Fact]
    public async Task RepeatedRowInTheMiddleOfThreePages_IsKept()
    {
        var pages = Enumerable.Range(1, 3).Select(n => Page(n, ("Ritornello ripetere due volte", 400), ("Alleluia cantate al Signore", 300))).ToArray();

        var texts = await Import(pages);

        Assert.All(texts, t => Assert.Contains("Ritornello ripetere due volte", t));
    }
}
