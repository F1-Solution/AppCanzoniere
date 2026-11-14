namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Bounds applied to an untrusted PDF before and while it is read.</summary>
public sealed record PdfExtractionLimits
{
    /// <summary>Largest accepted file, in bytes (default 100 MB).</summary>
    public long MaxBytes { get; init; } = 100L * 1024 * 1024;

    /// <summary>Largest accepted page count (default 2000).</summary>
    public int MaxPages { get; init; } = 2000;

    /// <summary>Most characters on one page (default 100,000, about twenty times a dense songbook page).</summary>
    public int MaxLettersPerPage { get; init; } = 100_000;

    /// <summary>Largest unpacked content of one page, in bytes, checked before the page is parsed (default 4 MB).</summary>
    public long MaxContentBytesPerPage { get; init; } = 4L * 1024 * 1024;

    /// <summary>Largest size any single stream of the file may unpack to, in bytes.</summary>
    public long MaxDecodedStreamBytes { get; init; } = 32L * 1024 * 1024;

    /// <summary>Longest time spent reading one file (default 2 minutes, the importer's default).</summary>
    public TimeSpan MaxDuration { get; init; } = AppCanzoniere.Application.PdfImport.PdfTextImporter.DefaultMaxDuration;
}
