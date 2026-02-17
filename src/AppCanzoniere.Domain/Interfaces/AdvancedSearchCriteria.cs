using AppCanzoniere.Domain.Entities;

namespace AppCanzoniere.Domain.Interfaces;

/// <summary>
/// Advanced search criteria for songs with multi-filter support
/// </summary>
public class AdvancedSearchCriteria
{
    /// <summary>
    /// Full-text search query (searches in Title, Artist, Lyrics, ChordProContent)
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// Filter by language
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Filter by original key
    /// </summary>
    public string? OriginalKey { get; set; }

    /// <summary>
    /// Filter by tag IDs (returns songs that have ANY of these tags)
    /// </summary>
    public List<int>? TagIds { get; set; }

    /// <summary>
    /// Filter by album IDs (returns songs in ANY of these albums)
    /// </summary>
    public List<int>? AlbumIds { get; set; }

    /// <summary>
    /// Filter by liturgical moment (e.g., "Ingresso", "Offertorio")
    /// </summary>
    public string? LiturgicalMoment { get; set; }

    /// <summary>
    /// Filter by minimum number of verses
    /// </summary>
    public int? MinVerses { get; set; }

    /// <summary>
    /// Filter by maximum number of verses
    /// </summary>
    public int? MaxVerses { get; set; }

    /// <summary>
    /// Filter by creation date range (from)
    /// </summary>
    public DateTime? CreatedFrom { get; set; }

    /// <summary>
    /// Filter by creation date range (to)
    /// </summary>
    public DateTime? CreatedTo { get; set; }

    /// <summary>
    /// Filter by last modified date range (from)
    /// </summary>
    public DateTime? ModifiedFrom { get; set; }

    /// <summary>
    /// Filter by last modified date range (to)
    /// </summary>
    public DateTime? ModifiedTo { get; set; }

    /// <summary>
    /// Include songs with PDF attachments only
    /// </summary>
    public bool? HasPdf { get; set; }

    /// <summary>
    /// Include songs with MP3 attachments only
    /// </summary>
    public bool? HasMp3 { get; set; }

    /// <summary>
    /// Include songs with YouTube links only
    /// </summary>
    public bool? HasYouTube { get; set; }

    /// <summary>
    /// Include songs with Bible references only
    /// </summary>
    public bool? HasBibleReferences { get; set; }

    /// <summary>
    /// Enable fuzzy search (tolerates typos)
    /// </summary>
    public bool EnableFuzzySearch { get; set; } = false;

    /// <summary>
    /// Maximum Levenshtein distance for fuzzy search (default: 2)
    /// </summary>
    public int FuzzyDistance { get; set; } = 2;

    /// <summary>
    /// Sort order (CreatedAt, Title, Artist, LastModified)
    /// </summary>
    public SongSortOrder SortBy { get; set; } = SongSortOrder.CreatedAt;

    /// <summary>
    /// Sort ascending (default: false = descending)
    /// </summary>
    public bool SortAscending { get; set; } = false;

    /// <summary>
    /// Maximum results to return (default: 100, max: 1000)
    /// </summary>
    public int MaxResults { get; set; } = 100;

    /// <summary>
    /// Skip N results (for pagination)
    /// </summary>
    public int Skip { get; set; } = 0;
}

/// <summary>
/// Sort order options for advanced search
/// </summary>
public enum SongSortOrder
{
    CreatedAt,
    Title,
    Artist,
    LastModified,
    Relevance // For full-text search scoring
}
