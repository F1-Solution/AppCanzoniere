using System.Diagnostics;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

// Windows only: the Job Object is what reaches a descendant whose parent has already exited.
public class PdfReaderJobTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "pdfjob-" + Guid.NewGuid().ToString("N"));

    public PdfReaderJobTests() => Directory.CreateDirectory(_folder);

    public void Dispose()
    {
        KillLeftover(PidFile + ".child");
        KillLeftover(PidFile);
        Directory.Delete(_folder, recursive: true);
    }

    private string PidFile => Path.Combine(_folder, "pid.txt");

    private IsolatedPdfWordExtractor Fake(string mode, TimeSpan? duration = null) =>
        new(new PdfWorkerLaunch(PdfWorkerLaunch.DotnetHost(), new[] { Path.Combine(AppContext.BaseDirectory, "AppCanzoniere.FakePdfWorker.dll"), mode, PidFile }),
            new IsolatedPdfOptions { Limits = new PdfExtractionLimits { MaxDuration = duration ?? TimeSpan.FromSeconds(30) } });

    private static Task<PdfWordsResult> Run(IsolatedPdfWordExtractor extractor, CancellationToken token = default) =>
        extractor.ExtractAsync(new MemoryStream(new byte[] { 1, 2, 3 }), token);

    private static async Task WaitForFile(string path)
    {
        for (var i = 0; i < 200 && !File.Exists(path); i++)
            await Task.Delay(50);
        Assert.True(File.Exists(path), $"{path} was never written");
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

    private void AssertGrandchildGone()
    {
        var pid = int.Parse(File.ReadAllText(PidFile + ".child"));
        Assert.True(IsGone(pid), $"grandchild {pid} is still alive");
    }

    private static void KillLeftover(string pidFile)
    {
        if (!File.Exists(pidFile))
            return;
        try
        {
            using var process = Process.GetProcessById(int.Parse(File.ReadAllText(pidFile)));
            process.Kill();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or System.ComponentModel.Win32Exception or IOException or FormatException)
        {
            // Already gone.
        }
    }

    [Fact]
    public async Task ExtractAsync_LeavesNoDescendant_AfterATimeout()
    {
        if (!OperatingSystem.IsWindows())
            return;

        using var isolated = Fake("orphan-then-hang", TimeSpan.FromSeconds(3));

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.TimedOut, result.Status);
        AssertGrandchildGone();
    }

    [Fact]
    public async Task ExtractAsync_LeavesNoDescendant_AfterCancellation()
    {
        if (!OperatingSystem.IsWindows())
            return;

        using var isolated = Fake("orphan-then-hang");
        using var cancel = new CancellationTokenSource();
        var run = Run(isolated, cancel.Token);
        await WaitForFile(PidFile + ".child");

        cancel.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => run);
        AssertGrandchildGone();
    }

    [Fact]
    public async Task ExtractAsync_LeavesNoDescendant_AfterANormalCompletion()
    {
        if (!OperatingSystem.IsWindows())
            return;

        using var isolated = Fake("orphan-then-ok");

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        AssertGrandchildGone();
    }

    [Fact]
    public async Task Dispose_LeavesNoDescendant_OfAReadInProgress()
    {
        if (!OperatingSystem.IsWindows())
            return;

        var isolated = Fake("orphan-then-hang");
        var run = Run(isolated);
        await WaitForFile(PidFile + ".child");

        isolated.Dispose();

        try
        {
            await run;
        }
        catch (Exception exception) when (exception is OperationCanceledException or ObjectDisposedException)
        {
            // Expected once the extractor is disposed.
        }

        AssertGrandchildGone();
    }
}
