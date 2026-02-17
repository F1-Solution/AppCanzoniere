using AppCanzoniere.Application.DTOs;

namespace AppCanzoniere.Application.Interfaces;

public interface ICloudStorageProvider
{
    /// <summary>
    /// Provider name (e.g., "OneDrive", "Google Drive", "Dropbox")
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    Task<bool> IsAuthenticatedAsync();

    /// <summary>
    /// Authenticate user with OAuth 2.0
    /// </summary>
    Task<CloudAuthResult> AuthenticateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sign out and clear tokens
    /// </summary>
    Task SignOutAsync();

    /// <summary>
    /// Upload a file to cloud storage
    /// </summary>
    Task<CloudFileInfo?> UploadFileAsync(
        string localFilePath,
        string remoteFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload file from stream
    /// </summary>
    Task<CloudFileInfo?> UploadFileAsync(
        Stream fileStream,
        string remoteFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a file from cloud storage
    /// </summary>
    Task<bool> DownloadFileAsync(
        string remoteFilePath,
        string localFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download file to stream
    /// </summary>
    Task<Stream?> DownloadFileAsync(
        string remoteFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file from cloud storage
    /// </summary>
    Task<bool> DeleteFileAsync(
        string remoteFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List files in a folder
    /// </summary>
    Task<List<CloudFileInfo>> ListFilesAsync(
        string remoteFolderPath = "/",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get file metadata
    /// </summary>
    Task<CloudFileInfo?> GetFileInfoAsync(
        string remoteFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a folder
    /// </summary>
    Task<bool> CreateFolderAsync(
        string remoteFolderPath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if file exists
    /// </summary>
    Task<bool> FileExistsAsync(
        string remoteFilePath,
        CancellationToken cancellationToken = default);
}
