using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

/// <summary>
/// The layout turns distances into spaces; crafted coordinates must not turn into huge lines, huge pages or a crash.
/// The songbook corpus has lines of at most 141 characters, pages of at most 6,125 and documents of at most 323,410.
/// </summary>
public class PdfLayoutBoundsTests
{
    // The whitespace a line may hold, plus room for the words themselves.
    private const int MaxLineLength = PdfPageLayout.MaxLineColumns + 100;

    private sealed class FakeExtractor(PdfWordsResult result) : IPdfWordExtractor
    {
        public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default) => Task.FromResult(result);
    }

    private static Task<PdfTextResult> Import(params PdfPageWords[] pages) =>
        new PdfTextImporter(new FakeExtractor(PdfWordsResult.Success(pages))).ImportAsync(new MemoryStream());

    // Rows of two half-point letters at the two edges of an A4 page: on a grid that fine every line is mostly spaces.
    private static PdfPageWords WideRows(int rows, int number = 1)
    {
        var words = Enumerable.Range(0, rows).SelectMany(r => new[]
        {
            new PositionedWord("a", new WordBox(0, 830 - (r * 2), 0.5, 831 - (r * 2))),
            new PositionedWord("b", new WordBox(594.5, 830 - (r * 2), 595, 831 - (r * 2))),
        });
        return new PdfPageWords(number, 595, 842, words.ToList(), false);
    }

    [Theory]
    [InlineData(2e6, 700)]
    [InlineData(2e6, 680)]
    [InlineData(-2e6, 680)]
    public void Lay_KeepsEveryLineShort_WhenAWordLiesFarFromTheRest(double x, double baseline)
    {
        var page = new PageBuilder().Text("canto la mia gioia al Signore", 50, 700).Word("vento", x, baseline).Page();

        var text = PdfPageLayout.Lay(page);

        Assert.All(text.Text.Split('\n'), line => Assert.True(line.Length <= MaxLineLength, $"a line of {line.Length} characters"));
        Assert.Contains("vento", text.Text, StringComparison.Ordinal);
        Assert.Contains("canto la mia gioia al Signore", text.Text, StringComparison.Ordinal);
    }

    [Fact]
    public void Lay_KeepsAChordRowShort_WhenAChordLiesFarFromItsLyric()
    {
        var page = new PageBuilder().Word("DO", 50, 712).Word("SOL", 3e6, 712).Text("canto la mia gioia al Signore", 50, 700).Page();

        var text = PdfPageLayout.Lay(page);

        Assert.All(text.Text.Split('\n'), line => Assert.True(line.Length <= MaxLineLength, $"a line of {line.Length} characters"));
        Assert.Contains("SOL", text.Text, StringComparison.Ordinal);
    }

    public static TheoryData<double, double, double, double> ExtremeBoxes() => new()
    {
        { double.NaN, 680, 10, 688 },
        { 50, double.NaN, 60, 688 },
        { double.NegativeInfinity, 680, double.PositiveInfinity, 688 },
        { 50, 680, double.PositiveInfinity, 688 },
        { 1e300, 680, 2e300, 688 },
        { -2e300, 680, -1e300, 688 },
        { 50, -1e300, 60, 1e300 },
        { 60, 680, 50, 688 },
        { 50, 688, 60, 680 },
        { 50, 680, 50, 680 },
        { 50, 680, 50 + 1e-300, 680 + 1e-300 },
    };

    [Theory]
    [MemberData(nameof(ExtremeBoxes))]
    public void Lay_NeitherThrowsNorLosesTheText_ForAWordWithAnExtremeBox(double left, double bottom, double right, double top)
    {
        var page = new PageBuilder { WithLetters = true }.Text("canto la mia gioia al Signore", 50, 700).Page();
        var odd = new PositionedWord("x", new WordBox(left, bottom, right, top), new[] { left });
        page = page with { Words = page.Words.Append(odd).ToList() };

        var text = PdfPageLayout.Lay(page);

        Assert.Contains("canto la mia gioia al Signore", text.Text, StringComparison.Ordinal);
        Assert.All(text.Text.Split('\n'), line => Assert.True(line.Length <= MaxLineLength, $"a line of {line.Length} characters"));
    }

    [Fact]
    public async Task ImportAsync_RefusesAPageThatLaysOutToTooMuchText()
    {
        var result = await Import(WideRows(rows: 300));

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains($"{PdfPageLayout.MaxPageCharacters}", result.Reason, StringComparison.Ordinal);
        Assert.Empty(result.Pages);
    }

    [Fact]
    public async Task ImportAsync_RefusesADocumentThatLaysOutToTooMuchText()
    {
        // Each page stays under the page limit; together they pass the document limit.
        var pageRows = (PdfPageLayout.MaxPageCharacters / (PdfPageLayout.MaxLineColumns + 2)) - 1;
        var pageCount = (PdfTextImporter.MaxTextCharacters / (pageRows * PdfPageLayout.MaxLineColumns)) + 1;
        var pages = Enumerable.Range(1, pageCount).Select(n => WideRows(pageRows, n)).ToArray();

        var result = await Import(pages);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains($"{PdfTextImporter.MaxTextCharacters}", result.Reason, StringComparison.Ordinal);
        Assert.Empty(result.Pages);
    }
}
