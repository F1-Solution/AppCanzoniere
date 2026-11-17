namespace AppCanzoniere.Application.PdfImport;

/// <summary>Outcome of reading a PDF; every value other than Success says why no usable text came back.</summary>
public enum PdfExtractionStatus
{
    Success,

    /// <summary>The pages carry no text layer, as in a scan; the pages are still listed for OCR.</summary>
    NoTextLayer,

    /// <summary>The input has no bytes or no pages.</summary>
    Empty,

    /// <summary>The input is not a readable PDF.</summary>
    Damaged,

    /// <summary>The PDF needs a password.</summary>
    Encrypted,

    /// <summary>The file or its page count is over the configured limit.</summary>
    TooLarge,

    /// <summary>Reading took longer than the configured time limit.</summary>
    TimedOut,

    /// <summary>The file could not be read: the reader stopped unexpectedly or its answer could not be trusted.</summary>
    Unreadable
}

/// <summary>Positioned words of every page, or a typed failure with a readable reason.</summary>
public sealed record PdfWordsResult(PdfExtractionStatus Status, string Reason, IReadOnlyList<PdfPageWords> Pages)
{
    public bool IsSuccess => Status == PdfExtractionStatus.Success;

    /// <summary>Technical detail of a failure, such as the parser's message, for logs only; never shown to the user.</summary>
    public string? Detail { get; init; }

    public static PdfWordsResult Success(IReadOnlyList<PdfPageWords> pages) => new(PdfExtractionStatus.Success, string.Empty, pages);

    public static PdfWordsResult Failure(PdfExtractionStatus status, string reason, IReadOnlyList<PdfPageWords>? pages = null) =>
        new(status, reason, pages ?? Array.Empty<PdfPageWords>());
}

/// <summary>Reads the positioned words of a PDF on the device; implementations never throw for bad input.</summary>
public interface IPdfWordExtractor
{
    /// <summary>Reads every page; throws only OperationCanceledException when the token is cancelled.</summary>
    Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default);
}
