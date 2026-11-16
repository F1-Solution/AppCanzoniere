namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Bounds on the reader process; the document limits and the wall-clock time come from the extraction limits.</summary>
public sealed record IsolatedPdfOptions
{
    /// <summary>Limits applied to the input and, again, to the reader's answer.</summary>
    public PdfExtractionLimits Limits { get; init; } = new();

    /// <summary>Working set at which the reader is killed (default 1 GB, about four times what the document limits need).</summary>
    public long MaxMemoryBytes { get; init; } = 1024L * 1024 * 1024;

    /// <summary>Most bytes read from the reader's stdout (default 64 MB: 700,000 letters at the 48 bytes per letter measured on the corpus, times a safety factor of 2).</summary>
    public long MaxOutputBytes { get; init; } = 64L * 1024 * 1024;

    /// <summary>How often the reader's memory is sampled; must be positive (default 100 ms, as each sample snapshots the process list on Windows).</summary>
    public TimeSpan MemoryPollInterval { get; init; } = TimeSpan.FromMilliseconds(100);
}
