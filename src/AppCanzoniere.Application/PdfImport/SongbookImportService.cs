namespace AppCanzoniere.Application.PdfImport;

/// <summary>Reads text pages with the PDF word extractor and scanned pages with the renderer and OCR engine, then splits and converts songs.</summary>
public sealed class SongbookImportService : ISongbookImportService
{
    private const int CopyBufferBytes = 81920;

    private readonly IPdfWordExtractor _extractor;
    private readonly IPageImageRenderer _renderer;
    private readonly IOcrEngine _ocr;
    private readonly SongbookImportOptions _options;
    private readonly TimeProvider _time;

    public SongbookImportService(
        IPdfWordExtractor extractor, IPageImageRenderer renderer, IOcrEngine ocr, SongbookImportOptions? options = null, TimeProvider? timeProvider = null)
    {
        _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _ocr = ocr ?? throw new ArgumentNullException(nameof(ocr));
        _options = options ?? new SongbookImportOptions();
        _time = timeProvider ?? TimeProvider.System;
    }

    public async Task<SongbookImportResult> ImportAsync(
        Stream pdf, string fileName, IProgress<SongbookImportProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdf);
        using var source = await ReadBoundedAsync(pdf, cancellationToken).ConfigureAwait(false);
        if (source is null)
            return SongbookImportResult.Failed($"The file is larger than the limit of {_options.MaxBytes} bytes.", SongbookImportFailure.OverLimits);
        if (source.Length == 0)
            return SongbookImportResult.Failed("The file is empty.", SongbookImportFailure.ProtectedOrDamaged);

        progress?.Report(new SongbookImportProgress(SongbookImportStage.Reading, 0, 0));
        var words = await _extractor.ExtractAsync(source, cancellationToken).ConfigureAwait(false);
        if (words.Status is not (PdfExtractionStatus.Success or PdfExtractionStatus.NoTextLayer))
            return SongbookImportResult.Failed(words.Reason, SongbookImportResult.FailureOf(words.Status), words.Pages.Count);

        return await ImportWordsAsync(source, words, fileName, progress, cancellationToken).ConfigureAwait(false);
    }

    private async Task<SongbookImportResult> ImportWordsAsync(
        MemoryStream source, PdfWordsResult words, string fileName, IProgress<SongbookImportProgress>? progress, CancellationToken cancellationToken)
    {
        var pageCount = words.Pages.Count;
        var scanned = words.Pages.Where(p => !p.HasText && p.HasImages).Select(p => p.PageNumber).ToList();
        if (scanned.Count > _options.MaxScannedPages)
            return SongbookImportResult.Failed($"The file has {scanned.Count} scanned pages, more than the limit of {_options.MaxScannedPages}.", SongbookImportFailure.OverLimits, pageCount);

        var outcome = ScannedPagesOutcome.None;
        if (scanned.Count > 0)
        {
            source.Position = 0;
            outcome = await new ScannedPageReader(_renderer, _ocr, _options, _time).ReadAsync(source, scanned, pageCount, progress, cancellationToken).ConfigureAwait(false);
        }

        var merged = words.Pages.Select(p => outcome.Recognised.TryGetValue(p.PageNumber, out var read) ? read : p).ToList();
        var textPages = words.Pages.Count(p => p.HasText);
        if (merged.All(p => !p.HasText))
            return NoText(outcome, words, pageCount, scanned);

        var layout = await new PdfTextImporter(new PrecomputedWordExtractor(PdfWordsResult.Success(merged)), timeProvider: _time)
            .ImportAsync(Stream.Null, cancellationToken).ConfigureAwait(false);
        if (!layout.IsSuccess)
            return SongbookImportResult.Failed(layout.Reason, SongbookImportResult.FailureOf(layout.Status), pageCount);

        return Convert(layout, fileName, progress, pageCount, textPages, scanned, outcome);
    }

    private static SongbookImportResult NoText(ScannedPagesOutcome outcome, PdfWordsResult words, int pageCount, IReadOnlyList<int> scanned)
    {
        var reason = outcome.StopReason ?? words.Reason;
        var failure = outcome.StopFailure == SongbookImportFailure.None ? SongbookImportFailure.NoText : outcome.StopFailure;
        if (string.IsNullOrEmpty(reason))
            reason = "No page has any text.";
        return SongbookImportResult.Failed(reason, failure, pageCount) with { ScannedPages = scanned, UnreadPages = outcome.Unread };
    }

    private static SongbookImportResult Convert(
        PdfTextResult layout, string fileName, IProgress<SongbookImportProgress>? progress, int pageCount, int textPages,
        IReadOnlyList<int> scanned, ScannedPagesOutcome outcome)
    {
        var split = PdfSongSplitter.Split(layout.Pages, layout.Title, fileName);
        var songs = new List<ImportedSong>(split.Songs.Count);
        foreach (var song in split.Songs)
        {
            progress?.Report(new SongbookImportProgress(SongbookImportStage.Converting, song.FirstPage, pageCount));
            var converted = PdfSongConverter.Convert(song);
            var origin = OriginOf(song, outcome);
            songs.Add(new ImportedSong(converted.Title, converted.ChordPro, string.Join('\n', song.Lines), converted.FirstPage, converted.LastPage,
                converted.Warnings, converted.ChordLineCount, converted.DoubtfulLineCount, origin, converted.DoubtfulLineNumbers));
        }

        var found = songs.Count > 0;
        return new SongbookImportResult(
            found ? SongbookImportStatus.Success : SongbookImportStatus.NoSongsFound,
            found ? outcome.StopReason ?? string.Empty : "No song could be found in the text of the file.",
            SongbookImportFailure.None, songs, pageCount, textPages, scanned, outcome.Unread, split.IndexEntryCount, split.SkippedPages, layout.Title);
    }

    // A song is from the scan when every page it spans was read by OCR, mixed when only some were.
    private static SongOrigin OriginOf(SplitSong song, ScannedPagesOutcome outcome)
    {
        var pages = Enumerable.Range(song.FirstPage, song.LastPage - song.FirstPage + 1).ToList();
        var scanned = pages.Count(outcome.Recognised.ContainsKey);
        return scanned == 0 ? SongOrigin.Text : scanned == pages.Count ? SongOrigin.Scan : SongOrigin.TextAndScan;
    }

    // Reads at most MaxBytes into one buffer that the readers share without copying it; null means the input is larger than that.
    private async Task<MemoryStream?> ReadBoundedAsync(Stream pdf, CancellationToken cancellationToken)
    {
        if (pdf.CanSeek)
        {
            var remaining = pdf.Length - pdf.Position;
            if (remaining > _options.MaxBytes || remaining > Array.MaxLength)
                return null;

            var exact = new byte[remaining];
            var count = await pdf.ReadAtLeastAsync(exact, exact.Length, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(false);
            return new MemoryStream(exact, 0, count, writable: false, publiclyVisible: true);
        }

        using var memory = new MemoryStream();
        var buffer = new byte[CopyBufferBytes];
        int read;
        while ((read = await pdf.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
        {
            if (memory.Length + read > _options.MaxBytes)
                return null;
            memory.Write(buffer, 0, read);
        }

        return new MemoryStream(memory.GetBuffer(), 0, (int)memory.Length, writable: false, publiclyVisible: true);
    }
}
