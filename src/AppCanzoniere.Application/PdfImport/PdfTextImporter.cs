namespace AppCanzoniere.Application.PdfImport;

/// <summary>Aligned text of every page of a PDF, or the typed failure that prevented it.</summary>
public sealed record PdfTextResult(PdfExtractionStatus Status, string Reason, IReadOnlyList<PageText> Pages)
{
    public bool IsSuccess => Status == PdfExtractionStatus.Success;

    /// <summary>Technical detail of a failure, for logs only; never shown to the user.</summary>
    public string? Detail { get; init; }
}

/// <summary>Reads a PDF on the device and returns its text page by page and column by column.</summary>
public sealed class PdfTextImporter
{
    /// <summary>Default time limit for reading and laying out one file.</summary>
    public static readonly TimeSpan DefaultMaxDuration = TimeSpan.FromMinutes(2);

    private readonly IPdfWordExtractor _extractor;
    private readonly TimeSpan _maxDuration;

    /// <summary>Creates an importer; maxDuration bounds extraction and layout together (default two minutes).</summary>
    public PdfTextImporter(IPdfWordExtractor extractor, TimeSpan? maxDuration = null)
    {
        _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
        _maxDuration = maxDuration ?? DefaultMaxDuration;
        ArgumentOutOfRangeException.ThrowIfLessThan(_maxDuration, TimeSpan.Zero, nameof(maxDuration));
    }

    /// <summary>Extracts and lays out every page; failures and the time limit come back typed, cancellation throws.</summary>
    public async Task<PdfTextResult> ImportAsync(Stream pdf, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdf);
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(_maxDuration);
        var clock = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            return await ImportWithinAsync(pdf, deadline.Token, clock).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new PdfTextResult(PdfExtractionStatus.TimedOut, TimedOutReason(_maxDuration), Array.Empty<PageText>());
        }
    }

    /// <summary>The readable reason given when reading takes longer than the limit.</summary>
    public static string TimedOutReason(TimeSpan limit) =>
        string.Create(System.Globalization.CultureInfo.InvariantCulture, $"Reading the file took longer than the limit of {limit.TotalSeconds:0.#} seconds.");

    private async Task<PdfTextResult> ImportWithinAsync(Stream pdf, CancellationToken token, System.Diagnostics.Stopwatch clock)
    {
        var words = await _extractor.ExtractAsync(pdf, token).ConfigureAwait(false);
        var pages = new List<PageText>(words.Pages.Count);
        foreach (var page in words.Pages)
        {
            StopIfOver(token, clock);
            pages.Add(PdfPageLayout.Lay(page, token));
        }

        StopIfOver(token, clock);
        return new PdfTextResult(words.Status, words.Reason, pages);
    }

    // The clock is checked too, so a busy machine that delays the deadline timer still stops on time.
    private void StopIfOver(CancellationToken token, System.Diagnostics.Stopwatch clock)
    {
        token.ThrowIfCancellationRequested();
        if (clock.Elapsed > _maxDuration)
            throw new OperationCanceledException(token);
    }
}
