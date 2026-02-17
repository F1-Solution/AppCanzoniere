using AppCanzoniere.Domain.Enums;

namespace AppCanzoniere.Domain.Entities;

/// <summary>
/// Represents a song downloaded for offline use
/// </summary>
public class OfflineSong
{
    public int Id { get; set; }

    /// <summary>
    /// Backend song ID (from public songs API) - stored as string to handle Guid
    /// </summary>
    public string BackendSongId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string? OriginalKey { get; set; }
    public string? Lyrics { get; set; }

    /// <summary>
    /// Sync status with backend
    /// </summary>
    public SyncStatus SyncStatus { get; set; } = SyncStatus.OnlineOnly;

    /// <summary>
    /// When the song was downloaded
    /// </summary>
    public DateTime? DownloadedAt { get; set; }

    /// <summary>
    /// When the song was last synced with backend
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }

    /// <summary>
    /// Size of downloaded content in bytes (song + media)
    /// </summary>
    public long SizeInBytes { get; set; }

    /// <summary>
    /// Local file path for cached PDF
    /// </summary>
    public string? LocalPdfPath { get; set; }

    /// <summary>
    /// Local file path for cached MP3
    /// </summary>
    public string? LocalMp3Path { get; set; }

    /// <summary>
    /// Backend version timestamp (for conflict detection)
    /// </summary>
    public DateTime? BackendVersion { get; set; }

    /// <summary>
    /// Local version timestamp (for conflict detection)
    /// </summary>
    public DateTime? LocalVersion { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
