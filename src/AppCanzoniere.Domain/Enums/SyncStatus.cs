namespace AppCanzoniere.Domain.Enums;

/// <summary>
/// Represents the sync status of a song with the backend
/// </summary>
public enum SyncStatus
{
    /// <summary>
    /// Song exists only online, not downloaded
    /// </summary>
    OnlineOnly,

    /// <summary>
    /// Song has been downloaded and is available offline
    /// </summary>
    Downloaded,

    /// <summary>
    /// Song was modified locally and needs to be synced
    /// </summary>
    LocallyModified,

    /// <summary>
    /// Song download is in progress
    /// </summary>
    Downloading,

    /// <summary>
    /// Song download failed
    /// </summary>
    DownloadFailed,

    /// <summary>
    /// Conflict between local and remote versions
    /// </summary>
    Conflict
}
