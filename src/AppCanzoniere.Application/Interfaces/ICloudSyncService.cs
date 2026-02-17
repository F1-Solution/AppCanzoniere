using AppCanzoniere.Application.DTOs;

namespace AppCanzoniere.Application.Interfaces;

public interface ICloudSyncService
{
    /// <summary>
    /// Current active cloud provider
    /// </summary>
    ICloudStorageProvider? CurrentProvider { get; }

    /// <summary>
    /// Set the active cloud provider
    /// </summary>
    void SetProvider(ICloudStorageProvider provider);

    /// <summary>
    /// Sync database to cloud
    /// </summary>
    Task<CloudSyncResult> SyncDatabaseAsync(
        bool uploadLocal = true,
        bool downloadRemote = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Export database to file
    /// </summary>
    Task<string?> ExportDatabaseAsync(
        string destinationPath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Import database from file
    /// </summary>
    Task<bool> ImportDatabaseAsync(
        string sourcePath,
        bool mergeWithExisting = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get last sync time
    /// </summary>
    Task<DateTime?> GetLastSyncTimeAsync();

    /// <summary>
    /// Detect conflicts between local and remote
    /// </summary>
    Task<List<SyncConflict>> DetectConflictsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolve conflicts with specified strategy
    /// </summary>
    Task<CloudSyncResult> ResolveConflictsAsync(
        List<SyncConflict> conflicts,
        CancellationToken cancellationToken = default);
}
