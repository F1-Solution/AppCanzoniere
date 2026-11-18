using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public class PdfTextImporterFooterOnlyTests
{
    private sealed class Fake(IReadOnlyList<PdfPageWords> pages) : IPdfWordExtractor
    {
        public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default) =>
            Task.FromResult(PdfWordsResult.Success(pages));
    }

    [Fact]
    public async Task A_scan_whose_only_text_is_a_repeated_footer_has_no_text_layer()
    {
        var pages = Enumerable.Range(1, 3).Select(n => new PageBuilder().Text("Scanned with CamScanner", 50, 30).Page(n, hasImages: true)).ToArray();

        var result = await new PdfTextImporter(new Fake(pages)).ImportAsync(new MemoryStream());

        Assert.False(result.IsSuccess);
        Assert.Equal(PdfExtractionStatus.NoTextLayer, result.Status);
    }
}
