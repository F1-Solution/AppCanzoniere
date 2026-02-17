namespace AppCanzoniere.Application.Interfaces;

/// <summary>
/// Service for downloading and managing offline songs
/// </summary>
public interface IDownloadService
{
    /// <summary>
    /// Downloads a song for offline use
    /// </summary>
    Task<bool> DownloadSongAsync(string backendSongId, IProgress<double>? progress = null);

    /// <summary>
    /// Removes a downloaded song
    /// </summary>
    Task<bool> RemoveDownloadAsync(string backendSongId);

    /// <summary>
    /// Checks if a song is downloaded
    /// </summary>
    Task<bool> IsDownloadedAsync(string backendSongId);

    /// <summary>
    /// Gets total storage used by offline songs
    /// </summary>
    Task<long> GetStorageUsageAsync();

    /// <summary>
    /// Gets count of downloaded songs
    /// </summary>
    Task<int> GetDownloadedCountAsync();

    /// <summary>
    /// Downloads media attachments for a song (PDF, MP3)
    /// </summary>
    Task<bool> DownloadMediaAsync(string backendSongId, IProgress<double>? progress = null);

    /// <summary>
    /// Clears all offline data
    /// </summary>
    Task ClearAllDownloadsAsync();
}
