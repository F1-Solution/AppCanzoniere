using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public class PdfTextImporterTests
{
    private sealed class FakeExtractor : IPdfWordExtractor
    {
        private readonly PdfWordsResult _result;

        public FakeExtractor(PdfWordsResult result) => _result = result;

        public CancellationToken ReceivedToken { get; private set; }

        public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default)
        {
            ReceivedToken = cancellationToken;
            return Task.FromResult(_result);
        }
    }

    private static PdfPageWords TextPage(int number) =>
        new PageBuilder().Text("canto la mia gioia al Signore", 50, 700).Page(number);

    [Fact]
    public async Task ImportAsync_LaysOutEveryPage_WhenExtractionSucceeds()
    {
        var pages = new[] { TextPage(1), new PdfPageWords(2, 595, 842, Array.Empty<PositionedWord>(), true) };
        var importer = new PdfTextImporter(new FakeExtractor(PdfWordsResult.Success(pages)));

        var result = await importer.ImportAsync(new MemoryStream());

        Assert.True(result.IsSuccess);
        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Equal(2, result.Pages.Count);
        Assert.Equal("canto la mia gioia al Signore", result.Pages[0].Text);
        Assert.True(result.Pages[1].IsScanned);
    }

    [Theory]
    [InlineData(PdfExtractionStatus.NoTextLayer)]
    [InlineData(PdfExtractionStatus.Damaged)]
    [InlineData(PdfExtractionStatus.Encrypted)]
    [InlineData(PdfExtractionStatus.Empty)]
    [InlineData(PdfExtractionStatus.TooLarge)]
    [InlineData(PdfExtractionStatus.TimedOut)]
    public async Task ImportAsync_PassesATypedFailureOn_WithItsReason(PdfExtractionStatus status)
    {
        var importer = new PdfTextImporter(new FakeExtractor(PdfWordsResult.Failure(status, "reason given")));

        var result = await importer.ImportAsync(new MemoryStream());

        Assert.False(result.IsSuccess);
        Assert.Equal(status, result.Status);
        Assert.Equal("reason given", result.Reason);
    }

    [Fact]
    public async Task ImportAsync_LaysOutScannedPages_WhenTheWholeDocumentHasNoText()
    {
        var pages = new[] { new PdfPageWords(1, 595, 842, Array.Empty<PositionedWord>(), true) };
        var importer = new PdfTextImporter(new FakeExtractor(PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "scanned", pages)));

        var result = await importer.ImportAsync(new MemoryStream());

        Assert.Equal(PdfExtractionStatus.NoTextLayer, result.Status);
        Assert.True(Assert.Single(result.Pages).IsScanned);
    }

    [Fact]
    public async Task ImportAsync_HandsTheCancellationTokenToTheExtractor()
    {
        var extractor = new FakeExtractor(PdfWordsResult.Success(Array.Empty<PdfPageWords>()));
        using var source = new CancellationTokenSource();

        await new PdfTextImporter(extractor).ImportAsync(new MemoryStream(), source.Token);

        Assert.Equal(source.Token, extractor.ReceivedToken);
    }

    [Fact]
    public async Task ImportAsync_RejectsANullStream()
    {
        var importer = new PdfTextImporter(new FakeExtractor(PdfWordsResult.Success(Array.Empty<PdfPageWords>())));

        await Assert.ThrowsAsync<ArgumentNullException>(() => importer.ImportAsync(null!));
    }
}
