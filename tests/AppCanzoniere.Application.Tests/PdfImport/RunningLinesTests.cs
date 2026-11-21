using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

// Rule: in the top or bottom tenth of a page, a row with a link is dropped at the bottom, or at the top when repeated;
// any such row (digits ignored, two words or a number) repeated on three pages and a quarter of them, or on three nearby pages, is dropped too.
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
    private static PdfPageWords SectionPage(int number, string section) =>
        Page(number, ($"Alleluia cantate al Signore {number}", 400), (number % 2 == 1 ? $"{section} {number}" : $"{number} {section}", Footer));

    // Five sections of four pages each: no footer reaches a quarter of the booklet, but each repeats on consecutive pages.
    private static PdfPageWords[] SectionedBooklet(params string[] sections) =>
        sections.SelectMany((section, s) => Enumerable.Range(1, 4).Select(i => SectionPage(s * 4 + i, section))).ToArray();

    [Fact]
    public async Task FooterThatChangesPerSection_IsDropped_OnEveryPageOfItsSection()
    {
        var texts = await Import(SectionedBooklet("Canti di ingresso", "Salmi e cantici", "Comunione", "Maria", "Finali"));

        Assert.All(texts, t => Assert.StartsWith("Alleluia cantate al Signore", t));
        Assert.All(texts, t => Assert.DoesNotContain("ingresso", t));
        Assert.All(texts, t => Assert.DoesNotContain("cantici", t));
        Assert.All(texts, t => Assert.DoesNotContain("Maria", t));
        Assert.All(texts, t => Assert.DoesNotContain("Comunione", t));
        Assert.All(texts, t => Assert.DoesNotContain("Finali", t));
    }

    // A 30-page booklet whose pages carry the given extra row, so that no signature reaches a quarter of the pages.
    private static PdfPageWords[] Booklet(Func<int, (string Text, double Baseline)?> extra) =>
        Enumerable.Range(1, 30).Select(n => extra(n) is { } row
            ? Page(n, ($"Alleluia cantate al Signore {n}", 400), row)
            : Page(n, ($"Alleluia cantate al Signore {n}", 400))).ToArray();

    [Fact]
    public async Task SectionFooter_OnTwoPagesOnly_IsKept_OnBothPages()
    {
        var texts = await Import(Booklet(n => n is 1 or 2 ? ($"Maria {n}", Footer) : null));

        Assert.Contains("Maria 1", texts[0]);
        Assert.Contains("Maria 2", texts[1]);
    }

    [Fact]
    public async Task SectionFooter_OnThreePagesTwoApart_IsDropped()
    {
        var texts = await Import(Booklet(n => n is 1 or 3 or 5 ? ($"Maria {n}", Footer) : null));

        Assert.All(new[] { 0, 2, 4 }, i => Assert.DoesNotContain("Maria", texts[i]));
    }

    [Fact]
    public async Task SectionFooter_OnThreePagesThreeApart_IsKept()
    {
        var texts = await Import(Booklet(n => n is 1 or 4 or 7 ? ($"Maria {n}", Footer) : null));

        Assert.All(new[] { 0, 3, 6 }, i => Assert.Contains("Maria", texts[i]));
    }

    [Fact]
    public async Task SameTitleAtTheTopOfThreeConsecutivePages_IsKept()
    {
        var texts = await Import(Booklet(n => n is >= 10 and <= 12 ? ("AGNELLO DI DIO", Header) : null));

        Assert.All(new[] { 9, 10, 11 }, i => Assert.StartsWith("AGNELLO DI DIO", texts[i]));
    }

    [Fact]
    public async Task NumberedTitlesWithTheSameWord_AtTheTopOfThreeConsecutivePages_AreKept()
    {
        var texts = await Import(Booklet(n => n is >= 10 and <= 12 ? ($"{100 + n} SANTO", Header) : null));

        Assert.All(new[] { 9, 10, 11 }, i => Assert.StartsWith($"{101 + i} SANTO", texts[i]));
    }

    [Fact]
    public async Task NumberedTitlesWithTheSameWord_InATenPageBooklet_AreKept()
    {
        var pages = Enumerable.Range(1, 10)
            .Select(n => n <= 3 ? Page(n, ($"Alleluia cantate al Signore {n}", 400), ($"{100 + n} SANTO", Header)) : Page(n, ($"Alleluia cantate al Signore {n}", 400))).ToArray();

        var texts = await Import(pages);

        Assert.All(new[] { 0, 1, 2 }, i => Assert.StartsWith($"{101 + i} SANTO", texts[i]));
    }

    [Fact]
    public async Task RefrainAtTheBottomOfThreeConsecutivePages_IsKept()
    {
        var texts = await Import(Booklet(n => n is >= 10 and <= 12 ? ("Alleluia, alleluia", Footer) : null));

        Assert.All(new[] { 9, 10, 11 }, i => Assert.EndsWith("Alleluia, alleluia", texts[i]));
    }

    [Theory]
    [InlineData("1. Signore")]
    [InlineData("1 Signore pietà")]
    public async Task VerseLineAtTheBottomOfThreeConsecutivePages_IsKept(string verse)
    {
        var texts = await Import(Booklet(n => n is >= 10 and <= 12 ? (verse, Footer) : null));

        Assert.All(new[] { 9, 10, 11 }, i => Assert.EndsWith(verse, texts[i]));
    }

    [Fact]
    public async Task BottomRowWithAFixedNumber_OnThreeConsecutivePages_IsKept()
    {
        var texts = await Import(Booklet(n => n is >= 10 and <= 12 ? ("Maria 7", Footer) : null));

        Assert.All(new[] { 9, 10, 11 }, i => Assert.Contains("Maria 7", texts[i]));
    }

    [Fact]
    public async Task SetApartLineAtTheBottomOfSpreadOutPages_IsKept_AsARefrain()
    {
        var pages = Enumerable.Range(1, 20)
            .Select(n => n % 5 == 0 ? Page(n, ($"Alleluia cantate al Signore {n}", 400), ("Rit. lode al Signore", Footer)) : Page(n, ($"Alleluia cantate al Signore {n}", 400)))
            .ToArray();

        var texts = await Import(pages);

        Assert.Equal(4, texts.Count(t => t.Contains("Rit. lode al Signore")));
    }
}
