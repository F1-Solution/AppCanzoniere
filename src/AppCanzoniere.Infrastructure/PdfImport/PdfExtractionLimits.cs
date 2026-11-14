namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Bounds applied to an untrusted PDF before and while it is read.</summary>
public sealed record PdfExtractionLimits
{
    /// <summary>Largest accepted file, in bytes (default 100 MB).</summary>
    public long MaxBytes { get; init; } = 100L * 1024 * 1024;

    /// <summary>Largest accepted page count (default 2000).</summary>
    public int MaxPages { get; init; } = 2000;

    /// <summary>Longest time spent reading one file (default 2 minutes).</summary>
    public TimeSpan MaxDuration { get; init; } = TimeSpan.FromMinutes(2);
}
