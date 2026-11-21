using System.Diagnostics;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

// Windows only: the Job Object is what reaches a descendant whose parent has already exited.
[Collection(nameof(ProcessCollection))]
public class PdfReaderJobTests : IDisposable
{
    private readonly DateTime _startedAt = DateTime.Now.AddSeconds(-1);
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
            new IsolatedPdfOptions { Limits = new PdfExtractionLimits { MaxDuration = duration ?? TimeSpan.FromSeconds(60) } });

    private static Task<PdfWordsResult> Run(IsolatedPdfWordExtractor extractor, CancellationToken token = default) =>
        extractor.ExtractAsync(new MemoryStream(new byte[] { 1, 2, 3 }), token);

    private static async Task WaitForFile(string path)
    {
        for (var i = 0; i < 600 && !File.Exists(path); i++)
            await Task.Delay(50);
        Assert.True(File.Exists(path), $"{path} was never written");
    }

    // A pid can be reused once the test's process died, so only a dotnet process started after the test began counts as ours.
    private bool IsOurs(Process process)
    {
        try
        {
            return process.ProcessName.Equals("dotnet", StringComparison.OrdinalIgnoreCase) && process.StartTime >= _startedAt;
        }
        catch (Exception exception) when (exception is InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            return false;
        }
    }

    private bool IsGone(int pid)
    {
        var deadline = DateTime.UtcNow.AddSeconds(15);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                using var process = Process.GetProcessById(pid);
                if (process.HasExited || !IsOurs(process))
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

    private void KillLeftover(string pidFile)
    {
        if (!File.Exists(pidFile))
            return;
        try
        {
            using var process = Process.GetProcessById(int.Parse(File.ReadAllText(pidFile)));
            if (IsOurs(process))
                process.Kill();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or System.ComponentModel.Win32Exception or IOException or FormatException)
        {
            // Already gone.
        }
    }

    [WindowsFact]
    public async Task ExtractAsync_LeavesNoDescendant_AfterATimeout()
    {
        using var isolated = Fake("orphan-then-hang", TimeSpan.FromSeconds(20));
        var run = Run(isolated);
        await WaitForFile(PidFile + ".child");

        var result = await run;

        Assert.Equal(PdfExtractionStatus.TimedOut, result.Status);
        AssertGrandchildGone();
    }

    [WindowsFact]
    public async Task ExtractAsync_LeavesNoDescendant_AfterCancellation()
    {
        using var isolated = Fake("orphan-then-hang");
        using var cancel = new CancellationTokenSource();
        var run = Run(isolated, cancel.Token);
        await WaitForFile(PidFile + ".child");

        cancel.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => run);
        AssertGrandchildGone();
    }

    [WindowsFact]
    public async Task ExtractAsync_LeavesNoDescendant_AfterANormalCompletion()
    {
        using var isolated = Fake("orphan-then-ok");

        var result = await Run(isolated);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        AssertGrandchildGone();
    }

    [WindowsFact]
    public async Task Dispose_LeavesNoDescendant_OfAReadInProgress()
    {
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
