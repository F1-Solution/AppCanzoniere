using System.Diagnostics;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

public class IsolatedPdfWordExtractorTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "pdfworker-" + Guid.NewGuid().ToString("N"));

    public IsolatedPdfWordExtractorTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, recursive: true);

    private string PidFile => Path.Combine(_folder, "pid.txt");

    private static IsolatedPdfOptions Options(TimeSpan? duration = null, long memory = 1L << 30, long output = 1L << 20) => new()
    {
        Limits = new PdfExtractionLimits { MaxDuration = duration ?? TimeSpan.FromSeconds(10) },
        MaxMemoryBytes = memory,
        MaxOutputBytes = output
    };

    private IsolatedPdfWordExtractor Fake(string mode, IsolatedPdfOptions? options = null) =>
        new(new PdfWorkerLaunch(PdfWorkerLaunch.DotnetHost(), new[] { Helper("AppCanzoniere.FakePdfWorker"), mode, PidFile }), options ?? Options());

    private static IsolatedPdfWordExtractor Real(PdfExtractionLimits? limits = null) =>
        new(new PdfWorkerLaunch(PdfWorkerLaunch.DotnetHost(), new[] { Helper("AppCanzoniere.PdfWorker") }), new IsolatedPdfOptions { Limits = limits ?? new PdfExtractionLimits() });

    private static string Helper(string name) => Path.Combine(AppContext.BaseDirectory, name + ".dll");

    private static Task<PdfWordsResult> Run(IsolatedPdfWordExtractor extractor, byte[]? pdf = null, CancellationToken token = default) =>
        extractor.ExtractAsync(new MemoryStream(pdf ?? new byte[] { 1, 2, 3 }), token);

    private void AssertChildGone(string? pidFile = null)
    {
        var path = pidFile ?? PidFile;
        Assert.True(File.Exists(path), "the fake worker never started");
        var pid = int.Parse(File.ReadAllText(path));
        Assert.True(IsGone(pid), $"process {pid} is still alive");
    }

    private static bool IsGone(int pid)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                using var process = Process.GetProcessById(pid);
                if (process.HasExited)
                    return true;
            }
            catch (ArgumentException)
            {
                return true;
            }

            Thread.Sleep(50);
        }

        return false;
    }

    [Fact]
    public async Task ExtractAsync_GivesTheSameWordsAsReadingInProcess()
    {
        var pdf = PdfFixtures.TwoColumns();
        var inProcess = await new PdfPigWordExtractor().ExtractAsync(new MemoryStream(pdf));

        using var isolated = Real();
        var result = await Run(isolated, pdf);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Equal(inProcess.Pages.Count, result.Pages.Count);
        for (var i = 0; i < inProcess.Pages.Count; i++)
        {
            Assert.Equal(inProcess.Pages[i].Words.Select(w => (w.Text, w.Box)), result.Pages[i].Words.Select(w => (w.Text, w.Box)));
            Assert.Equal(inProcess.Pages[i].Words.Select(w => w.LetterLefts), result.Pages[i].Words.Select(w => w.LetterLefts), new LeftsComparer());
        }
    }

    [Fact]
    public async Task ExtractAsync_PassesTheReadersOwnFailuresThrough()
    {
        using var isolated = Real();

        var result = await Run(isolated, "not a pdf at all"u8.ToArray());

        Assert.Equal(PdfExtractionStatus.Damaged, result.Status);
        Assert.False(string.IsNullOrEmpty(result.Reason));
    }

    [Fact]
    public async Task ExtractAsync_ReportsAnEmptyFileAsEmpty()
    {
        using var isolated = Real();

        var result = await isolated.ExtractAsync(new MemoryStream());

        Assert.Equal(PdfExtractionStatus.Empty, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_RefusesAFileOverTheByteLimitWithoutKeepingTheChild()
    {
        using var isolated = Fake("hang", new IsolatedPdfOptions { Limits = new PdfExtractionLimits { MaxBytes = 100, MaxDuration = TimeSpan.FromSeconds(10) } });

        var result = await Run(isolated, new byte[10_000]);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);

        // The child may be killed before it writes its pid.
        if (File.Exists(PidFile))
            AssertChildGone();
    }

    [Fact]
    public async Task ExtractAsync_ReadsAWellFormedAnswer()
    {
        using var isolated = Fake("ok");

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Equal("ciao", result.Pages.Single().Words.Single().Text);
    }

    [Theory]
    [InlineData("crash")]
    [InlineData("empty")]
    [InlineData("garbage")]
    [InlineData("bad-values")]
    [InlineData("bad-version")]
    [InlineData("too-many-pages")]
    public async Task ExtractAsync_ReportsUnreadable_WhenTheChildFailsOrAnswersBadly(string mode)
    {
        using var isolated = Fake(mode);

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.Unreadable, result.Status);
        Assert.False(string.IsNullOrEmpty(result.Reason));
        AssertChildGone();
    }

    [Fact]
    public async Task ExtractAsync_ReportsTooLarge_WhenTheChildRunsOutOfManagedMemory()
    {
        using var isolated = Fake("oom");

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_ReportsTimedOutAndKillsAHangingChild()
    {
        using var isolated = Fake("hang", Options(TimeSpan.FromSeconds(1.5)));
        var clock = Stopwatch.StartNew();

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.TimedOut, result.Status);
        Assert.True(clock.Elapsed < TimeSpan.FromSeconds(6), $"took {clock.Elapsed}");
        AssertChildGone();
    }

    [Fact]
    public async Task ExtractAsync_KillsTheWholeTreeOfAHangingChild()
    {
        using var isolated = Fake("hang-with-grandchild", Options(TimeSpan.FromSeconds(1.5)));

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.TimedOut, result.Status);
        AssertChildGone();
        AssertChildGone(PidFile + ".child");
    }

    [Fact]
    public async Task ExtractAsync_StopsAChildThatFloodsStdout()
    {
        using var isolated = Fake("flood", Options(output: 100_000));

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.Unreadable, result.Status);
        AssertChildGone();
    }

    [Fact]
    public async Task ExtractAsync_ReportsTooLarge_WhenTheChildExceedsTheMemoryCeiling()
    {
        using var isolated = Fake("memory", Options(memory: 300L << 20));

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        AssertChildGone();
    }

    [Fact]
    public async Task ExtractAsync_ThrowsAndKillsTheChild_WhenTheCallerCancels()
    {
        using var isolated = Fake("hang");
        using var cancel = new CancellationTokenSource();
        var running = Run(isolated, token: cancel.Token);
        await WaitForFile(PidFile);

        await cancel.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => running);
        AssertChildGone();
    }

    [Fact]
    public async Task ExtractAsync_ThrowsWithoutStartingAnything_WhenTheTokenIsAlreadyCancelled()
    {
        using var isolated = Fake("hang");
        using var cancel = new CancellationTokenSource();
        await cancel.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Run(isolated, token: cancel.Token));

        Assert.False(File.Exists(PidFile));
    }

    [Fact]
    public async Task Dispose_KillsAReaderThatIsStillRunning()
    {
        var isolated = Fake("hang");
        var running = Run(isolated);
        await WaitForFile(PidFile);

        isolated.Dispose();

        await Assert.ThrowsAnyAsync<Exception>(() => running);
        AssertChildGone();
    }

    [Fact]
    public async Task ExtractAsync_ReportsUnreadable_WhenTheWorkerIsMissing()
    {
        using var isolated = new IsolatedPdfWordExtractor(new PdfWorkerLaunch(Path.Combine(_folder, "missing.exe"), Array.Empty<string>()));

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.Unreadable, result.Status);
    }

    [Fact]
    public void Locate_FindsTheWorkerNextToTheApp()
    {
        Assert.NotNull(PdfWorkerLaunch.Locate(AppContext.BaseDirectory));
        Assert.Null(PdfWorkerLaunch.Locate(_folder));
    }

    private static async Task WaitForFile(string path)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!File.Exists(path) && DateTime.UtcNow < deadline)
            await Task.Delay(20);
        Assert.True(File.Exists(path));
        await Task.Delay(50);
    }

    private sealed class LeftsComparer : IEqualityComparer<IReadOnlyList<double>?>
    {
        public bool Equals(IReadOnlyList<double>? x, IReadOnlyList<double>? y) =>
            x is null ? y is null : y is not null && x.SequenceEqual(y);

        public int GetHashCode(IReadOnlyList<double>? obj) => obj?.Count ?? 0;
    }
}
