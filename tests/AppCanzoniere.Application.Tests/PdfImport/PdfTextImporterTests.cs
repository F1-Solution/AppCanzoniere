using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public class PdfTextImporterTests
{
    // Blocks the calling thread like a parser that cannot be interrupted, then returns the result.
    private sealed class SlowExtractor : IPdfWordExtractor
    {
        private readonly PdfWordsResult _result;
        private readonly TimeSpan _delay;

        public SlowExtractor(PdfWordsResult result, TimeSpan delay) => (_result, _delay) = (result, delay);

        public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default)
        {
            Thread.Sleep(_delay);
            return Task.FromResult(_result);
        }
    }

    // Waits cooperatively, as the real extractor does, and honours the token.
    private sealed class WaitingExtractor : IPdfWordExtractor
    {
        public async Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            return PdfWordsResult.Success(Array.Empty<PdfPageWords>());
        }
    }

    private sealed class FakeExtractor : IPdfWordExtractor
    {
        private readonly PdfWordsResult _result;

        public FakeExtractor(PdfWordsResult result) => _result = result;

        public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default) =>
            Task.FromResult(_result);
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
    public async Task ImportAsync_ReturnsTimedOut_WhenTheTimeLimitRunsOutBeforeTheLayout()
    {
        // The time limit covers the layout too: once extraction has used it up, no page is laid out.
        var pages = Enumerable.Range(1, 3).Select(TextPage).ToArray();
        var extractor = new SlowExtractor(PdfWordsResult.Success(pages), TimeSpan.FromMilliseconds(300));
        var importer = new PdfTextImporter(extractor, TimeSpan.FromMilliseconds(50));

        var result = await importer.ImportAsync(new MemoryStream());

        Assert.Equal(PdfExtractionStatus.TimedOut, result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.Reason));
        Assert.Empty(result.Pages);
    }

    [Fact]
    public async Task ImportAsync_ReturnsTimedOut_WhenTheExtractorIsStillWaitingAtTheTimeLimit()
    {
        var importer = new PdfTextImporter(new WaitingExtractor(), TimeSpan.FromMilliseconds(100));

        var result = await importer.ImportAsync(new MemoryStream()).WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Equal(PdfExtractionStatus.TimedOut, result.Status);
    }

    [Fact]
    public async Task ImportAsync_Throws_WhenTheCallerCancels_EvenWithATimeLimit()
    {
        using var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        var importer = new PdfTextImporter(new WaitingExtractor(), TimeSpan.FromSeconds(30));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => importer.ImportAsync(new MemoryStream(), source.Token));
    }

    [Fact]
    public void Constructor_RejectsANegativeTimeLimit()
    {
        var extractor = new FakeExtractor(PdfWordsResult.Success(Array.Empty<PdfPageWords>()));

        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfTextImporter(extractor, TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public async Task ImportAsync_RejectsANullStream()
    {
        var importer = new PdfTextImporter(new FakeExtractor(PdfWordsResult.Success(Array.Empty<PdfPageWords>())));

        await Assert.ThrowsAsync<ArgumentNullException>(() => importer.ImportAsync(null!));
    }
}
