namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Bounds on the reader process; the document limits and the wall-clock time come from the extraction limits.</summary>
public sealed record IsolatedPdfOptions
{
    /// <summary>Limits applied to the input and, again, to the reader's answer.</summary>
    public PdfExtractionLimits Limits { get; init; } = new();

    /// <summary>Working set at which the reader is killed (default 1 GB, about four times what the document limits need).</summary>
    public long MaxMemoryBytes { get; init; } = 1024L * 1024 * 1024;

    /// <summary>Most bytes read from the reader's stdout (default 64 MB, about 90 bytes per character of the document limit).</summary>
    public long MaxOutputBytes { get; init; } = 64L * 1024 * 1024;

    /// <summary>How often the reader's memory is sampled (default 20 ms).</summary>
    public TimeSpan MemoryPollInterval { get; init; } = TimeSpan.FromMilliseconds(20);
}
