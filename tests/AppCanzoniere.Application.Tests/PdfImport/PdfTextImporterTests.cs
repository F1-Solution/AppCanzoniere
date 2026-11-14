using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public class PdfTextImporterTests
{
    // Like a parser that cannot be interrupted: the clock moves on while it works, then it returns its result anyway.
    // With fireTimers false the device is too busy to run the deadline timer on time.
    private sealed class SlowExtractor(PdfWordsResult result, ManualTime time, TimeSpan takes, bool fireTimers = true) : IPdfWordExtractor
    {
        public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default)
        {
            time.Advance(takes, fireTimers);
            return Task.FromResult(result);
        }
    }

    // Waits cooperatively, as the real extractor does, until the token is cancelled; nothing else ends the wait.
    private sealed class WaitingExtractor : IPdfWordExtractor
    {
        public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default)
        {
            Started.TrySetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            return PdfWordsResult.Success(Array.Empty<PdfPageWords>());
        }
    }

    // Only a guard against a hang if the code under test is broken; no assertion depends on it.
    private static readonly TimeSpan HangGuard = TimeSpan.FromMinutes(1);

    private static readonly TimeSpan Limit = TimeSpan.FromSeconds(10);

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
    public async Task ImportAsync_PassesTheDetailForLogsOn()
    {
        var failure = PdfWordsResult.Failure(PdfExtractionStatus.Damaged, "The file is not a readable PDF.") with { Detail = "xref not found" };
        var importer = new PdfTextImporter(new FakeExtractor(failure));

        var result = await importer.ImportAsync(new MemoryStream());

        Assert.Equal("xref not found", result.Detail);
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
        var time = new ManualTime();
        var pages = Enumerable.Range(1, 3).Select(TextPage).ToArray();
        var importer = new PdfTextImporter(new SlowExtractor(PdfWordsResult.Success(pages), time, 3 * Limit), Limit, time);

        var result = await importer.ImportAsync(new MemoryStream());

        Assert.Equal(PdfExtractionStatus.TimedOut, result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.Reason));
        Assert.Empty(result.Pages);
    }

    [Fact]
    public async Task ImportAsync_ReturnsTimedOut_WhenTheExtractorIsStillWaitingAtTheTimeLimit()
    {
        var time = new ManualTime();
        var extractor = new WaitingExtractor();
        var importer = new PdfTextImporter(extractor, Limit, time);

        var import = importer.ImportAsync(new MemoryStream());
        await extractor.Started.Task.WaitAsync(HangGuard);
        time.Advance(Limit);
        var result = await import.WaitAsync(HangGuard);

        Assert.Equal(PdfExtractionStatus.TimedOut, result.Status);
    }

    [Fact]
    public async Task ImportAsync_KeepsWaiting_WhileTheTimeLimitHasNotRunOut()
    {
        var time = new ManualTime();
        var extractor = new WaitingExtractor();
        var importer = new PdfTextImporter(extractor, Limit, time);

        var import = importer.ImportAsync(new MemoryStream());
        await extractor.Started.Task.WaitAsync(HangGuard);
        time.Advance(Limit - TimeSpan.FromTicks(1));

        Assert.False(import.IsCompleted);
        time.Advance(TimeSpan.FromTicks(1));
        Assert.Equal(PdfExtractionStatus.TimedOut, (await import.WaitAsync(HangGuard)).Status);
    }

    [Fact]
    public async Task ImportAsync_ReturnsTimedOut_WhenABusyDeviceRunsTheTimerLate()
    {
        // The deadline timer has not fired yet, but the clock says the time is up: the clock alone must stop the import.
        var time = new ManualTime();
        var extractor = new SlowExtractor(PdfWordsResult.Success(new[] { TextPage(1) }), time, Limit, fireTimers: false);
        var importer = new PdfTextImporter(extractor, Limit, time);

        var result = await importer.ImportAsync(new MemoryStream());

        Assert.Equal(PdfExtractionStatus.TimedOut, result.Status);
        Assert.Empty(result.Pages);
    }

    [Fact]
    public async Task ImportAsync_LaysOutThePages_WhenExtractionEndsJustInsideTheTimeLimit()
    {
        var time = new ManualTime();
        var extractor = new SlowExtractor(PdfWordsResult.Success(new[] { TextPage(1) }), time, Limit - TimeSpan.FromTicks(1), fireTimers: false);
        var importer = new PdfTextImporter(extractor, Limit, time);

        var result = await importer.ImportAsync(new MemoryStream());

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Single(result.Pages);
    }

    [Fact]
    public async Task ImportAsync_Throws_WhenTheCallerCancels_EvenWithATimeLimit()
    {
        using var source = new CancellationTokenSource();
        var extractor = new WaitingExtractor();
        var importer = new PdfTextImporter(extractor, Limit, new ManualTime());

        var import = importer.ImportAsync(new MemoryStream(), source.Token);
        await extractor.Started.Task.WaitAsync(HangGuard);
        await source.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => import.WaitAsync(HangGuard));
    }

    [Fact]
    public void Constructor_RejectsANegativeTimeLimit()
    {
        var extractor = new FakeExtractor(PdfWordsResult.Success(Array.Empty<PdfPageWords>()));

        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfTextImporter(extractor, TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void Constructor_RejectsATimeLimitLongerThanATimerCanWait()
    {
        // A timer waits at most about 49.7 days; a longer limit would only fail later, inside ImportAsync.
        var extractor = new FakeExtractor(PdfWordsResult.Success(Array.Empty<PdfPageWords>()));

        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfTextImporter(extractor, TimeSpan.FromDays(60)));
        _ = new PdfTextImporter(extractor, TimeSpan.FromDays(49));
    }

    [Fact]
    public async Task ImportAsync_RejectsANullStream()
    {
        var importer = new PdfTextImporter(new FakeExtractor(PdfWordsResult.Success(Array.Empty<PdfPageWords>())));

        await Assert.ThrowsAsync<ArgumentNullException>(() => importer.ImportAsync(null!));
    }
}
