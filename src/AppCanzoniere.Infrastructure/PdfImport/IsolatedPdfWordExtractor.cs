using System.Collections.Concurrent;
using System.Diagnostics;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Reads PDFs in a separate process, so a crash, hang or memory blow-up of the parser only kills that process.</summary>
public sealed class IsolatedPdfWordExtractor : IPdfWordExtractor, IDisposable
{
    private const int CopyBufferBytes = 81920;
    private const int ErrorBytes = 4096;
    private static readonly TimeSpan ExitWait = TimeSpan.FromSeconds(5);

    // Once the reader has exited its pipes hold nothing more; only a stray descendant can keep them open.
    private static readonly TimeSpan PipeGrace = TimeSpan.FromSeconds(1);

    private static readonly string[] KeptVariables = { "PATH", "SystemRoot", "TEMP", "TMP", "TMPDIR", "HOME", "USERPROFILE" };

    private readonly PdfWorkerLaunch _launch;
    private readonly IsolatedPdfOptions _options;
    private readonly ConcurrentDictionary<Process, ReaderJob?> _running = new();
    private readonly CancellationTokenSource _disposed = new();

    public IsolatedPdfWordExtractor(PdfWorkerLaunch launch, IsolatedPdfOptions? options = null)
    {
        _launch = launch ?? throw new ArgumentNullException(nameof(launch));
        _options = options ?? new IsolatedPdfOptions();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_options.MaxMemoryBytes, nameof(options));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_options.MaxOutputBytes, nameof(options));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(_options.MemoryPollInterval, TimeSpan.Zero, nameof(options));
        ArgumentOutOfRangeException.ThrowIfLessThan(_options.Limits.MaxDuration, TimeSpan.Zero, nameof(options));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(_options.Limits.MaxDuration, PdfTextImporter.MaxSupportedDuration, nameof(options));
    }

    public async Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdf);
        ObjectDisposedException.ThrowIf(_disposed.IsCancellationRequested, this);
        cancellationToken.ThrowIfCancellationRequested();

        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposed.Token);
        stop.CancelAfter(_options.Limits.MaxDuration);
        if (!TryStart(out var process, out var startFailure))
            return startFailure!;

        using (process)
        {
            // A descendant started before this assignment escapes the job; the reader needs far longer than that to start one.
            using var job = ReaderJob.TryAssign(process);
            _running[process] = job;
            try
            {
                return await RunAsync(process, pdf, stop, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await StopAsync(process).ConfigureAwait(false);
                _running.TryRemove(process, out _);
            }
        }
    }

    /// <summary>Kills every reader still running.</summary>
    public void Dispose()
    {
        _disposed.Cancel();
        foreach (var process in _running.Keys)
            Stop(process);
        _disposed.Dispose();
    }

    private async Task<PdfWordsResult> RunAsync(Process process, Stream pdf, CancellationTokenSource stop, CancellationToken caller)
    {
        var abort = new ChildAbort(stop.Token);
        var feed = FeedAsync(process.StandardInput.BaseStream, pdf, abort);
        var output = ReadCappedAsync(process.StandardOutput.BaseStream, _options.MaxOutputBytes, abort, ChildVerdict.OutputTooLarge);
        var error = ReadCappedAsync(process.StandardError.BaseStream, ErrorBytes, abort, null);
        var watch = WatchMemoryAsync(process, abort);
        try
        {
            await process.WaitForExitAsync(abort.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Handled below from the abort reason.
        }

        var aborted = abort.Token.IsCancellationRequested;

        // On abort the tree dies first; after a normal exit the pipes get a short grace before anything left over is killed.
        if (aborted)
            Stop(process);
        else
            await WaitBoundedAsync(Task.WhenAll(output, error), PipeGrace, abort.Token).ConfigureAwait(false);

        Stop(process);
        abort.Finish();
        await Task.WhenAny(Task.WhenAll(feed, watch), Task.Delay(ExitWait)).ConfigureAwait(false);
        caller.ThrowIfCancellationRequested();
        ObjectDisposedException.ThrowIf(_disposed.IsCancellationRequested, this);
        return Outcome(process, abort.Verdict, aborted, await Completed(output), await Completed(error));
    }

    private PdfWordsResult Outcome(Process process, ChildVerdict verdict, bool aborted, MemoryStream? output, MemoryStream? error)
    {
        var limits = _options.Limits;
        switch (verdict)
        {
            case ChildVerdict.InputTooLarge:
                return PdfWordsResult.Failure(PdfExtractionStatus.TooLarge, $"The file is larger than the limit of {limits.MaxBytes} bytes.");
            case ChildVerdict.OutputTooLarge:
                return Unreadable("The reader answered with more data than allowed.");
            case ChildVerdict.OverMemory:
                return MemoryExceeded();
        }

        if (aborted)
            return PdfWordsResult.Failure(PdfExtractionStatus.TimedOut, PdfTextImporter.TimedOutReason(limits.MaxDuration));

        if (process.ExitCode != 0)
            return ExitFailure(process.ExitCode, error);

        if (output is not null)
            output.Position = 0;
        var result = output is null ? null : PdfWorkerProtocol.TryRead(output, limits);
        return result ?? Unreadable("The reader gave an answer that could not be trusted.");
    }

    private static PdfWordsResult MemoryExceeded() =>
        PdfWordsResult.Failure(PdfExtractionStatus.TooLarge, "The file needs more memory than the device can give.");

    private static PdfWordsResult ExitFailure(int code, MemoryStream? error)
    {
        var text = error is null ? string.Empty : System.Text.Encoding.UTF8.GetString(error.GetBuffer(), 0, (int)error.Length);
        if (text.Contains("OutOfMemoryException", StringComparison.Ordinal))
            return MemoryExceeded();

        return Unreadable("The file could not be read.") with { Detail = $"The reader exited with code {code}. {text}".Trim() };
    }

    private static PdfWordsResult Unreadable(string reason) => PdfWordsResult.Failure(PdfExtractionStatus.Unreadable, reason);

    private static async Task<MemoryStream?> Completed(Task<MemoryStream> task) => task.IsCompletedSuccessfully ? await task.ConfigureAwait(false) : null;

    private bool TryStart(out Process process, out PdfWordsResult? failure)
    {
        var info = new ProcessStartInfo(_launch.FileName)
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var argument in _launch.LeadingArguments)
            info.ArgumentList.Add(argument);
        info.ArgumentList.Add("-");
        info.ArgumentList.Add("--deadline-seconds");
        info.ArgumentList.Add(((int)Math.Ceiling(_options.Limits.MaxDuration.TotalSeconds) + 5).ToString(System.Globalization.CultureInfo.InvariantCulture));
        info.ArgumentList.Add(PdfWorkerLimits.Switch);
        info.ArgumentList.Add(PdfWorkerLimits.Serialize(_options.Limits));
        MinimalEnvironment(info.Environment);
        info.Environment["DOTNET_GCHeapHardLimit"] = _options.MaxMemoryBytes.ToString("X", System.Globalization.CultureInfo.InvariantCulture);

        try
        {
            process = Process.Start(info) ?? throw new InvalidOperationException("The reader did not start.");
            failure = null;
            return true;
        }
        catch (Exception exception) when (exception is System.ComponentModel.Win32Exception or InvalidOperationException or IOException)
        {
            process = null!;
            failure = Unreadable("The PDF reader could not be started.") with { Detail = exception.Message };
            return false;
        }
    }

    // The reader is not a sandbox, but it need not see the host's secrets: only what .NET needs is passed on.
    private static void MinimalEnvironment(IDictionary<string, string?> target)
    {
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var kept = new List<KeyValuePair<string, string?>>();
        foreach (System.Collections.DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            var name = (string)entry.Key;
            if (KeptVariables.Any(k => string.Equals(k, name, comparison)) || name.StartsWith("DOTNET_", comparison))
                kept.Add(new(name, entry.Value as string));
        }

        target.Clear();
        foreach (var variable in kept)
            target[variable.Key] = variable.Value;
    }

    private static async Task WaitBoundedAsync(Task work, TimeSpan limit, CancellationToken token)
    {
        try
        {
            await Task.WhenAny(work, Task.Delay(limit, token)).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // The run was aborted while waiting; the caller decides from the abort reason.
        }
    }

    private async Task FeedAsync(Stream stdin, Stream pdf, ChildAbort abort)
    {
        try
        {
            var buffer = new byte[CopyBufferBytes];
            long total = 0;
            int read;
            while ((read = await pdf.ReadAsync(buffer, abort.Token).ConfigureAwait(false)) > 0)
            {
                total += read;
                if (total > _options.Limits.MaxBytes)
                {
                    abort.Cancel(ChildVerdict.InputTooLarge);
                    return;
                }

                await stdin.WriteAsync(buffer.AsMemory(0, read), abort.Token).ConfigureAwait(false);
            }

            await stdin.FlushAsync(abort.Token).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is IOException or OperationCanceledException or ObjectDisposedException)
        {
            // A reader that stops early closes the pipe; its exit code tells the rest.
        }
        finally
        {
            stdin.Close();
        }
    }

    // A null onExceed keeps the first cap bytes and discards the rest.
    private static async Task<MemoryStream> ReadCappedAsync(Stream stream, long cap, ChildAbort abort, ChildVerdict? onExceed)
    {
        var data = new MemoryStream();
        var buffer = new byte[CopyBufferBytes];
        try
        {
            int read;
            while ((read = await stream.ReadAsync(buffer, abort.Token).ConfigureAwait(false)) > 0)
            {
                if (data.Length + read > cap)
                {
                    if (onExceed is not { } verdict)
                    {
                        await DrainAsync(stream, abort.Token).ConfigureAwait(false);
                        break;
                    }

                    abort.Cancel(verdict);
                    return new MemoryStream();
                }

                data.Write(buffer, 0, read);
            }
        }
        catch (Exception exception) when (exception is IOException or OperationCanceledException or ObjectDisposedException)
        {
            // The pipe closed or the run was aborted; what was read so far stands.
        }

        return data;
    }

    private static async Task DrainAsync(Stream stream, CancellationToken token)
    {
        var buffer = new byte[CopyBufferBytes];
        while (await stream.ReadAsync(buffer, token).ConfigureAwait(false) > 0)
        {
        }
    }

    private async Task WatchMemoryAsync(Process process, ChildAbort abort)
    {
        try
        {
            while (!abort.Token.IsCancellationRequested && !process.HasExited)
            {
                process.Refresh();
                if (process.WorkingSet64 > _options.MaxMemoryBytes)
                {
                    abort.Cancel(ChildVerdict.OverMemory);
                    return;
                }

                await Task.Delay(_options.MemoryPollInterval, abort.Token).ConfigureAwait(false);
            }
        }
        catch (Exception exception) when (exception is InvalidOperationException or OperationCanceledException)
        {
            // The process ended or the run was aborted.
        }
    }

    private void Stop(Process process)
    {
        if (_running.TryGetValue(process, out var job))
            job?.Terminate();

        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch (Exception exception) when (exception is InvalidOperationException or System.ComponentModel.Win32Exception or NotSupportedException)
        {
            // Already gone.
        }
    }

    private async Task StopAsync(Process process)
    {
        Stop(process);
        try
        {
            using var wait = new CancellationTokenSource(ExitWait);
            await process.WaitForExitAsync(wait.Token).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is InvalidOperationException or OperationCanceledException)
        {
            // Nothing more can be done for a process that will not die.
        }
    }
}
