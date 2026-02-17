namespace AppCanzoniere.Domain.Interfaces;

/// <summary>
/// Result of an advanced search with metadata
/// </summary>
public class AdvancedSearchResult<T>
{
    /// <summary>
    /// Matching items
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>
    /// Total count of matching items (before pagination)
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Search execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Search query used
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Number of filters applied
    /// </summary>
    public int FiltersApplied { get; set; }

    /// <summary>
    /// Fuzzy search enabled
    /// </summary>
    public bool FuzzySearchUsed { get; set; }
}
