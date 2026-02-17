namespace AppCanzoniere.Application.DTOs;

public class CloudFileInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string? Checksum { get; set; }
    public bool IsFolder { get; set; }
}

public class CloudSyncResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int FilesUploaded { get; set; }
    public int FilesDownloaded { get; set; }
    public int FilesDeleted { get; set; }
    public int Conflicts { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

public class CloudAuthResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }
}

public enum ConflictResolution
{
    KeepLocal,
    KeepRemote,
    KeepBoth,
    Merge
}

public class SyncConflict
{
    public string FilePath { get; set; } = string.Empty;
    public CloudFileInfo? LocalFile { get; set; }
    public CloudFileInfo? RemoteFile { get; set; }
    public ConflictResolution Resolution { get; set; } = ConflictResolution.KeepLocal;
}
