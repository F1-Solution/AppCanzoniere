namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>
/// Bounds applied to an untrusted PDF before and while it is read, with defaults well above the songbook corpus, whose
/// largest page holds 215 KB of content and 4,798 characters and largest document 4.4 MB and 334,000 characters.
/// </summary>
public sealed record PdfExtractionLimits
{
    /// <summary>Largest accepted file, in bytes (default 100 MB).</summary>
    public long MaxBytes { get; init; } = 100L * 1024 * 1024;

    /// <summary>Largest accepted page count (default 2000).</summary>
    public int MaxPages { get; init; } = 2000;

    /// <summary>Most characters on one page, bounded before parsing and counted after (default 100,000, twenty times the corpus maximum).</summary>
    public int MaxLettersPerPage { get; init; } = 100_000;

    /// <summary>Largest unpacked content of one page, forms counted once per use, in bytes, checked before parsing (default 1 MB, about five times the corpus maximum).</summary>
    public long MaxContentBytesPerPage { get; init; } = 1L * 1024 * 1024;

    /// <summary>Largest unpacked content of the whole document, in bytes, checked before the first page is parsed (default 64 MB, fourteen times the corpus maximum).</summary>
    public long MaxContentBytesPerDocument { get; init; } = 64L * 1024 * 1024;

    /// <summary>Most characters in the whole document (default 1,000,000, three times the corpus maximum; each costs about 190 bytes while read).</summary>
    public long MaxLettersPerDocument { get; init; } = 1_000_000;

    /// <summary>Largest size any single stream may unpack to while PdfPig reads it, in bytes (default 16 MB; the corpus maximum is 440 KB).</summary>
    public long MaxDecodedStreamBytes { get; init; } = 16L * 1024 * 1024;

    /// <summary>Most bytes PdfPig may unpack over the whole document, a stream counted each time it is unpacked (default 64 MB; the corpus maximum is 5 MB).</summary>
    public long MaxDecodedBytesPerDocument { get; init; } = 64L * 1024 * 1024;

    /// <summary>Longest time spent reading one file (default 2 minutes, the importer's default).</summary>
    public TimeSpan MaxDuration { get; init; } = AppCanzoniere.Application.PdfImport.PdfTextImporter.DefaultMaxDuration;
}
