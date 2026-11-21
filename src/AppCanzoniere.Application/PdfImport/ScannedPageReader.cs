namespace AppCanzoniere.Application.PdfImport;

/// <summary>Pages read by OCR, the pages that could not be read, and why recognition stopped when it did.</summary>
internal sealed record ScannedPagesOutcome(
    IReadOnlyDictionary<int, PdfPageWords> Recognised,
    IReadOnlyList<UnreadPage> Unread,
    string? StopReason,
    SongbookImportFailure StopFailure)
{
    public static ScannedPagesOutcome None { get; } = new(new Dictionary<int, PdfPageWords>(), Array.Empty<UnreadPage>(), null, SongbookImportFailure.None);
}

/// <summary>Renders scanned pages and reads them with an OCR engine, one page at a time.</summary>
internal sealed class ScannedPageReader(IPageImageRenderer renderer, IOcrEngine engine, SongbookImportOptions options, TimeProvider time)
{
    private enum PageFault
    {
        None,
        Render,
        Engine,
        Empty
    }

    private sealed record PageRead(PdfPageWords? Words, PageFault Fault, string Reason);

    public async Task<ScannedPagesOutcome> ReadAsync(
        Stream pdf, IReadOnlyList<int> pages, int pageCount, IProgress<SongbookImportProgress>? progress, CancellationToken cancellationToken)
    {
        using var timer = new CancellationTokenSource(options.MaxRecognitionDuration, time);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timer.Token);
        var recognised = new Dictionary<int, PdfPageWords>();
        var unread = new List<UnreadPage>();
        try
        {
            await using var session = await renderer.OpenAsync(pdf, linked.Token).ConfigureAwait(false);
            if (session is null)
                return Stopped(recognised, pages, "The scanned pages could not be opened for recognition.", SongbookImportFailure.Unreadable);

            return await ReadPagesAsync(session, pages, pageCount, progress, recognised, unread, linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            var reason = $"Recognising the scanned pages took longer than the limit of {options.MaxRecognitionDuration.TotalSeconds:0.#} seconds.";
            return Stopped(recognised, pages, reason, SongbookImportFailure.TimedOut);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Stopped(recognised, pages, "The scanned pages could not be rendered: " + ex.Message, SongbookImportFailure.Unreadable);
        }
    }

    private static ScannedPagesOutcome Stopped(Dictionary<int, PdfPageWords> recognised, IReadOnlyList<int> pages, string reason, SongbookImportFailure failure) =>
        new(recognised, pages.Where(p => !recognised.ContainsKey(p)).Select(p => new UnreadPage(p, reason)).ToList(), reason, failure);

    private async Task<ScannedPagesOutcome> ReadPagesAsync(
        IPageImageSession session, IReadOnlyList<int> pages, int pageCount, IProgress<SongbookImportProgress>? progress,
        Dictionary<int, PdfPageWords> recognised, List<UnreadPage> unread, CancellationToken token)
    {
        var streak = 0;
        var faults = new List<PageFault>();
        for (var i = 0; i < pages.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            progress?.Report(new SongbookImportProgress(SongbookImportStage.Recognizing, pages[i], pageCount));
            var read = await ReadPageAsync(session, pages[i], token).ConfigureAwait(false);
            if (read.Words is not null)
            {
                streak = 0;
                recognised[pages[i]] = read.Words;
                continue;
            }

            faults.Add(read.Fault);
            unread.Add(new UnreadPage(pages[i], read.Reason));
            streak = read.Fault == PageFault.Empty ? streak : streak + 1;
            if (streak >= options.MaxConsecutiveFailures)
            {
                unread.AddRange(pages.Skip(i + 1).Select(p => new UnreadPage(p, "Not read: recognition stopped.")));
                return new ScannedPagesOutcome(recognised, unread, $"Recognition stopped after {streak} failed pages in a row: {read.Reason}", FailureOf(faults));
            }
        }

        return recognised.Count > 0 || unread.Count == 0
            ? new ScannedPagesOutcome(recognised, unread, null, SongbookImportFailure.None)
            : new ScannedPagesOutcome(recognised, unread, unread[^1].Reason, FailureOf(faults));
    }

    // An engine that failed means OCR is not available; pages that read as empty point to a low resolution.
    private static SongbookImportFailure FailureOf(List<PageFault> faults) =>
        faults.Contains(PageFault.Engine) ? SongbookImportFailure.OcrUnavailable
        : faults.Contains(PageFault.Empty) ? SongbookImportFailure.LowResolution
        : SongbookImportFailure.Unreadable;

    private async Task<PageRead> ReadPageAsync(IPageImageSession session, int number, CancellationToken token)
    {
        var image = await session.RenderPageAsync(number, token).ConfigureAwait(false);
        if (image is null)
            return new PageRead(null, PageFault.Render, $"Page {number} could not be rendered.");

        var result = await engine.RecognizeAsync(image, token).ConfigureAwait(false);
        token.ThrowIfCancellationRequested();
        var adapted = OcrWordAdapter.ToPageWords(result, number);
        if (adapted.IsFailure)
            return new PageRead(null, PageFault.Engine, adapted.FailureReason ?? "Recognition failed.");
        if (adapted.TooManyWords)
            return new PageRead(null, PageFault.Render, $"Page {number} has too many recognised words.");
        if (!adapted.Page.HasText)
            return new PageRead(null, PageFault.Empty, $"No readable text on page {number}; the scan may be too blurred or too small.");

        return new PageRead(adapted.Page, PageFault.None, string.Empty);
    }
}
