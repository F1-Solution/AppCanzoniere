using AppCanzoniere.Application.DTOs;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Application.Services;

public class CloudSyncService : ICloudSyncService
{
    private readonly AppDbContext _dbContext;
    private readonly string _databasePath;
    private const string RemoteDatabaseName = "appcanzoniere.db";

    public ICloudStorageProvider? CurrentProvider { get; private set; }

    public CloudSyncService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        
        // Get database path from connection string
        var connection = _dbContext.Database.GetConnectionString();
        if (!string.IsNullOrEmpty(connection) && connection.Contains("Data Source="))
        {
            var dataSource = connection.Split("Data Source=")[1].Split(';')[0];
            _databasePath = dataSource;
        }
        else
        {
            _databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AppCanzoniere",
                "appcanzoniere.db"
            );
        }
    }

    public void SetProvider(ICloudStorageProvider provider)
    {
        CurrentProvider = provider;
    }

    public async Task<CloudSyncResult> SyncDatabaseAsync(
        bool uploadLocal = true,
        bool downloadRemote = true,
        CancellationToken cancellationToken = default)
    {
        if (CurrentProvider == null)
        {
            return new CloudSyncResult
            {
                Success = false,
                ErrorMessage = "No cloud provider configured"
            };
        }

        if (!await CurrentProvider.IsAuthenticatedAsync())
        {
            return new CloudSyncResult
            {
                Success = false,
                ErrorMessage = $"{CurrentProvider.ProviderName} not authenticated"
            };
        }

        try
        {
            var result = new CloudSyncResult { Success = true };

            // Check if remote file exists
            var remoteExists = await CurrentProvider.FileExistsAsync(RemoteDatabaseName, cancellationToken);

            if (!remoteExists && uploadLocal)
            {
                // First sync: upload local database
                await UploadDatabaseAsync(cancellationToken);
                result.FilesUploaded = 1;
            }
            else if (remoteExists)
            {
                // Detect conflicts
                var conflicts = await DetectConflictsAsync(cancellationToken);

                if (conflicts.Count > 0)
                {
                    result.Conflicts = conflicts.Count;
                    result.Success = false;
                    result.ErrorMessage = $"Found {conflicts.Count} conflicts. Please resolve manually.";
                    return result;
                }

                // Get remote file info
                var remoteInfo = await CurrentProvider.GetFileInfoAsync(RemoteDatabaseName, cancellationToken);
                var localInfo = new FileInfo(_databasePath);

                if (remoteInfo != null && remoteInfo.ModifiedAt > localInfo.LastWriteTimeUtc && downloadRemote)
                {
                    // Remote is newer, download
                    await DownloadDatabaseAsync(cancellationToken);
                    result.FilesDownloaded = 1;
                }
                else if (uploadLocal)
                {
                    // Local is newer or same, upload
                    await UploadDatabaseAsync(cancellationToken);
                    result.FilesUploaded = 1;
                }
            }

            result.SyncedAt = DateTime.UtcNow;
            return result;
        }
        catch (Exception ex)
        {
            return new CloudSyncResult
            {
                Success = false,
                ErrorMessage = $"Sync failed: {ex.Message}"
            };
        }
    }

    public async Task<string?> ExportDatabaseAsync(
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Close all connections
            await _dbContext.Database.CloseConnectionAsync();

            // Wait a bit to ensure file is not locked
            await Task.Delay(500, cancellationToken);

            // Copy database file
            var destinationDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            File.Copy(_databasePath, destinationPath, overwrite: true);

            return destinationPath;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> ImportDatabaseAsync(
        string sourcePath,
        bool mergeWithExisting = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(sourcePath))
            {
                return false;
            }

            if (mergeWithExisting)
            {
                // TODO: Implement merge logic
                // For now, just replace
                return false;
            }

            // Close all connections
            await _dbContext.Database.CloseConnectionAsync();

            // Wait a bit
            await Task.Delay(500, cancellationToken);

            // Backup current database
            var backupPath = _databasePath + ".backup";
            File.Copy(_databasePath, backupPath, overwrite: true);

            try
            {
                // Replace database
                File.Copy(sourcePath, _databasePath, overwrite: true);

                // Verify by opening connection
                await _dbContext.Database.OpenConnectionAsync(cancellationToken);
                await _dbContext.Database.CloseConnectionAsync();

                // Delete backup
                File.Delete(backupPath);

                return true;
            }
            catch
            {
                // Restore backup on failure
                File.Copy(backupPath, _databasePath, overwrite: true);
                File.Delete(backupPath);
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    public async Task<DateTime?> GetLastSyncTimeAsync()
    {
        if (CurrentProvider == null)
        {
            return null;
        }

        try
        {
            var remoteInfo = await CurrentProvider.GetFileInfoAsync(RemoteDatabaseName);
            return remoteInfo?.ModifiedAt;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<SyncConflict>> DetectConflictsAsync(
        CancellationToken cancellationToken = default)
    {
        var conflicts = new List<SyncConflict>();

        if (CurrentProvider == null)
        {
            return conflicts;
        }

        try
        {
            var remoteExists = await CurrentProvider.FileExistsAsync(RemoteDatabaseName, cancellationToken);
            
            if (!remoteExists)
            {
                return conflicts; // No conflict if remote doesn't exist
            }

            var remoteInfo = await CurrentProvider.GetFileInfoAsync(RemoteDatabaseName, cancellationToken);
            var localInfo = new FileInfo(_databasePath);

            if (remoteInfo != null)
            {
                // Check if both were modified after last sync
                // For simplicity, consider conflict if times differ by more than 1 minute
                var timeDiff = Math.Abs((remoteInfo.ModifiedAt - localInfo.LastWriteTimeUtc).TotalMinutes);

                if (timeDiff > 1)
                {
                    conflicts.Add(new SyncConflict
                    {
                        FilePath = RemoteDatabaseName,
                        LocalFile = new CloudFileInfo
                        {
                            Path = _databasePath,
                            ModifiedAt = localInfo.LastWriteTimeUtc,
                            Size = localInfo.Length
                        },
                        RemoteFile = remoteInfo,
                        Resolution = ConflictResolution.KeepLocal // Default
                    });
                }
            }

            return conflicts;
        }
        catch
        {
            return conflicts;
        }
    }

    public async Task<CloudSyncResult> ResolveConflictsAsync(
        List<SyncConflict> conflicts,
        CancellationToken cancellationToken = default)
    {
        var result = new CloudSyncResult { Success = true };

        foreach (var conflict in conflicts)
        {
            try
            {
                switch (conflict.Resolution)
                {
                    case ConflictResolution.KeepLocal:
                        await UploadDatabaseAsync(cancellationToken);
                        result.FilesUploaded++;
                        break;

                    case ConflictResolution.KeepRemote:
                        await DownloadDatabaseAsync(cancellationToken);
                        result.FilesDownloaded++;
                        break;

                    case ConflictResolution.KeepBoth:
                        // Rename local and download remote
                        var backupPath = _databasePath + $".conflict.{DateTime.Now:yyyyMMddHHmmss}";
                        File.Copy(_databasePath, backupPath, overwrite: true);
                        await DownloadDatabaseAsync(cancellationToken);
                        result.FilesDownloaded++;
                        break;

                    case ConflictResolution.Merge:
                        // TODO: Implement merge logic
                        result.Success = false;
                        result.ErrorMessage = "Merge not yet implemented";
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
        }

        return result;
    }

    private async Task UploadDatabaseAsync(CancellationToken cancellationToken)
    {
        if (CurrentProvider == null) return;

        // Close connections
        await _dbContext.Database.CloseConnectionAsync();
        await Task.Delay(500, cancellationToken);

        // Upload
        await CurrentProvider.UploadFileAsync(_databasePath, RemoteDatabaseName, cancellationToken);
    }

    private async Task DownloadDatabaseAsync(CancellationToken cancellationToken)
    {
        if (CurrentProvider == null) return;

        // Close connections
        await _dbContext.Database.CloseConnectionAsync();
        await Task.Delay(500, cancellationToken);

        // Download
        var success = await CurrentProvider.DownloadFileAsync(RemoteDatabaseName, _databasePath, cancellationToken);
        
        if (success)
        {
            // Reopen connection to reload
            await _dbContext.Database.OpenConnectionAsync(cancellationToken);
        }
    }
}
