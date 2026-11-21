using AppCanzoniere.Application.Import;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>Where the import is: reading the text layer, recognising scanned pages, or converting songs.</summary>
public enum SongbookImportStage
{
    Reading,
    Recognizing,
    Converting
}

/// <summary>Progress of one import: the stage (Reading is the text pass, Recognizing the scan pass) and the document page being handled, out of PageCount pages.</summary>
public sealed record SongbookImportProgress(SongbookImportStage Stage, int Page, int PageCount);

/// <summary>How the import ended; a partial read with failed pages is still Success.</summary>
public enum SongbookImportStatus
{
    Success,

    /// <summary>The file was read but no song could be cut out of it.</summary>
    NoSongsFound,

    /// <summary>The file could not be read; see Failure and Reason.</summary>
    Failed
}

/// <summary>Where a song's text came from.</summary>
public enum SongOrigin
{
    Text,
    Scan,
    TextAndScan
}

/// <summary>Why an import failed, in the groups the screen explains to the user.</summary>
public enum SongbookImportFailure
{
    None,
    ProtectedOrDamaged,
    OverLimits,
    TimedOut,
    NoText,
    OcrUnavailable,
    LowResolution,
    Unreadable
}

/// <summary>A scanned page that could not be read, with the reason.</summary>
public sealed record UnreadPage(int Page, string Reason);

/// <summary>One song ready for review: its ChordPro next to the text it came from.</summary>
public sealed record ImportedSong(
    string Title,
    string ChordPro,
    string SourceText,
    int FirstPage,
    int LastPage,
    IReadOnlyList<SheetWarning> Warnings,
    int ChordLineCount,
    int DoubtfulLineCount,
    SongOrigin Origin,
    IReadOnlyList<int> DoubtfulLineNumbers);

/// <summary>Songs found in a PDF and how the document was read.</summary>
public sealed record SongbookImportResult(
    SongbookImportStatus Status,
    string Reason,
    SongbookImportFailure Failure,
    IReadOnlyList<ImportedSong> Songs,
    int PageCount,
    int TextPageCount,
    IReadOnlyList<int> ScannedPages,
    IReadOnlyList<UnreadPage> UnreadPages,
    int IndexEntryCount,
    IReadOnlyList<int> SkippedPages,
    string? DocumentTitle)
{
    public bool IsSuccess => Status == SongbookImportStatus.Success;

    /// <summary>True when the document has an index whose entries do not match the songs found.</summary>
    public bool IndexCountMismatch => IndexEntryCount > 0 && IndexEntryCount != Songs.Count;

    public static SongbookImportResult Failed(string reason, SongbookImportFailure failure, int pageCount = 0) =>
        new(SongbookImportStatus.Failed, reason, failure, Array.Empty<ImportedSong>(), pageCount, 0, Array.Empty<int>(), Array.Empty<UnreadPage>(), 0, Array.Empty<int>(), null);

    /// <summary>The failure group for a reader status.</summary>
    public static SongbookImportFailure FailureOf(PdfExtractionStatus status) => status switch
    {
        PdfExtractionStatus.Encrypted or PdfExtractionStatus.Damaged or PdfExtractionStatus.Empty => SongbookImportFailure.ProtectedOrDamaged,
        PdfExtractionStatus.TooLarge => SongbookImportFailure.OverLimits,
        PdfExtractionStatus.TimedOut => SongbookImportFailure.TimedOut,
        PdfExtractionStatus.NoTextLayer => SongbookImportFailure.NoText,
        _ => SongbookImportFailure.Unreadable
    };
}

/// <summary>Limits of one import beyond those of the PDF reader itself.</summary>
public sealed record SongbookImportOptions
{
    /// <summary>Largest accepted file in bytes (default 100 MB, as the reader).</summary>
    public long MaxBytes { get; init; } = 100L * 1024 * 1024;

    /// <summary>Most scanned pages recognised in one import (default 200).</summary>
    public int MaxScannedPages { get; init; } = 200;

    /// <summary>Time allowed for recognising all scanned pages (default 10 minutes).</summary>
    public TimeSpan MaxRecognitionDuration { get; init; } = TimeSpan.FromMinutes(10);

    /// <summary>Consecutive failed pages after which recognition stops, as when the engine has no language (default 3).</summary>
    public int MaxConsecutiveFailures { get; init; } = 3;
}

/// <summary>Turns a songbook PDF into converted songs, reading text pages directly and scanned pages with on-device OCR.</summary>
public interface ISongbookImportService
{
    /// <summary>Imports the PDF; failures come back typed, cancellation throws OperationCanceledException.</summary>
    Task<SongbookImportResult> ImportAsync(Stream pdf, string fileName, IProgress<SongbookImportProgress>? progress = null, CancellationToken cancellationToken = default);
}
