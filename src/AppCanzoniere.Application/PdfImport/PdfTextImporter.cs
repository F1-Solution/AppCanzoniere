namespace AppCanzoniere.Application.PdfImport;

/// <summary>Aligned text of every page of a PDF, or the typed failure that prevented it.</summary>
public sealed record PdfTextResult(PdfExtractionStatus Status, string Reason, IReadOnlyList<PageText> Pages)
{
    public bool IsSuccess => Status == PdfExtractionStatus.Success;

    /// <summary>Technical detail of a failure, for logs only; never shown to the user.</summary>
    public string? Detail { get; init; }

    /// <summary>The cleaned title from the PDF metadata, null when there is none or it says nothing about the song.</summary>
    public string? Title { get; init; }
}

/// <summary>Reads a PDF on the device and returns its text page by page and column by column.</summary>
public sealed class PdfTextImporter
{
    /// <summary>Default time limit for reading and laying out one file.</summary>
    public static readonly TimeSpan DefaultMaxDuration = TimeSpan.FromMinutes(2);

    /// <summary>Longest time limit a timer can wait for, about 49.7 days.</summary>
    public static readonly TimeSpan MaxSupportedDuration = TimeSpan.FromMilliseconds(uint.MaxValue - 1);

    /// <summary>Most characters a whole document may lay out to (the longest corpus document has 323,410).</summary>
    public const int MaxTextCharacters = 1_000_000;

    private readonly IPdfWordExtractor _extractor;
    private readonly TimeSpan _maxDuration;
    private readonly TimeProvider _time;

    /// <summary>Creates an importer; maxDuration bounds extraction and layout together (default two minutes), timed by timeProvider.</summary>
    public PdfTextImporter(IPdfWordExtractor extractor, TimeSpan? maxDuration = null, TimeProvider? timeProvider = null)
    {
        _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
        _maxDuration = maxDuration ?? DefaultMaxDuration;
        _time = timeProvider ?? TimeProvider.System;
        ArgumentOutOfRangeException.ThrowIfLessThan(_maxDuration, TimeSpan.Zero, nameof(maxDuration));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(_maxDuration, MaxSupportedDuration, nameof(maxDuration));
    }

    /// <summary>Extracts and lays out every page; failures and the time limit come back typed, cancellation throws.</summary>
    public async Task<PdfTextResult> ImportAsync(Stream pdf, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdf);
        using var timer = new CancellationTokenSource(_maxDuration, _time);
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timer.Token);
        var started = _time.GetTimestamp();
        try
        {
            return await ImportWithinAsync(pdf, deadline.Token, started).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new PdfTextResult(PdfExtractionStatus.TimedOut, TimedOutReason(_maxDuration), Array.Empty<PageText>());
        }
        catch (PageTextTooLargeException tooLarge)
        {
            return new PdfTextResult(PdfExtractionStatus.TooLarge, tooLarge.Message, Array.Empty<PageText>());
        }
    }

    /// <summary>The readable reason given when reading takes longer than the limit.</summary>
    public static string TimedOutReason(TimeSpan limit) =>
        string.Create(System.Globalization.CultureInfo.InvariantCulture, $"Reading the file took longer than the limit of {limit.TotalSeconds:0.#} seconds.");

    private async Task<PdfTextResult> ImportWithinAsync(Stream pdf, CancellationToken token, long started)
    {
        var words = await _extractor.ExtractAsync(pdf, token).ConfigureAwait(false);
        var pages = new List<PageText>(words.Pages.Count);
        long characters = 0;
        var stripped = RunningLines.Strip(words.Pages);
        foreach (var page in stripped)
        {
            StopIfOver(token, started);
            var text = PdfPageLayout.Lay(page, token);
            characters += text.Blocks.Sum(b => b.Lines.Sum(l => (long)l.Text.Length + 1));
            if (characters > MaxTextCharacters)
                return new PdfTextResult(PdfExtractionStatus.TooLarge, $"The document lays out to more than {MaxTextCharacters} characters, the limit.", Array.Empty<PageText>());
            pages.Add(text);
        }

        StopIfOver(token, started);
        if (words.Status == PdfExtractionStatus.Success && words.Pages.Any(p => p.HasText) && !stripped.Any(p => p.HasText))
            return new PdfTextResult(PdfExtractionStatus.NoTextLayer, "No page has a text layer once the repeated footers are removed.", pages) { Detail = words.Detail, Title = words.Title };

        return new PdfTextResult(words.Status, words.Reason, pages) { Detail = words.Detail, Title = words.Title };
    }

    // The clock is checked too, so a busy machine that delays the deadline timer still stops on time.
    private void StopIfOver(CancellationToken token, long started)
    {
        token.ThrowIfCancellationRequested();
        if (_time.GetElapsedTime(started) >= _maxDuration)
            throw new OperationCanceledException(token);
    }
}
