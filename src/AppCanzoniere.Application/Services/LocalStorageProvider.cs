using AppCanzoniere.Application.DTOs;
using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Services;

public class LocalStorageProvider : ICloudStorageProvider
{
    private readonly string _backupFolder;

    public string ProviderName => "Local Storage";

    public LocalStorageProvider()
    {
        // Default backup folder in app data
        _backupFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AppCanzoniere",
            "Backups"
        );

        // Ensure backup folder exists
        Directory.CreateDirectory(_backupFolder);
    }

    public Task<bool> IsAuthenticatedAsync()
    {
        // Local storage doesn't require authentication
        return Task.FromResult(true);
    }

    public Task<CloudAuthResult> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        // No authentication needed for local storage
        return Task.FromResult(new CloudAuthResult
        {
            Success = true,
            UserId = "local",
            UserEmail = "local@device"
        });
    }

    public Task SignOutAsync()
    {
        // No-op for local storage
        return Task.CompletedTask;
    }

    public async Task<CloudFileInfo?> UploadFileAsync(
        string localFilePath,
        string remoteFilePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var destinationPath = GetFullPath(remoteFilePath);
            var destinationDir = Path.GetDirectoryName(destinationPath);

            if (!string.IsNullOrEmpty(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            File.Copy(localFilePath, destinationPath, overwrite: true);

            return await GetFileInfoAsync(remoteFilePath, cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<CloudFileInfo?> UploadFileAsync(
        Stream fileStream,
        string remoteFilePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var destinationPath = GetFullPath(remoteFilePath);
            var destinationDir = Path.GetDirectoryName(destinationPath);

            if (!string.IsNullOrEmpty(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            using var fileStreamOut = File.Create(destinationPath);
            await fileStream.CopyToAsync(fileStreamOut, cancellationToken);

            return await GetFileInfoAsync(remoteFilePath, cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public Task<bool> DownloadFileAsync(
        string remoteFilePath,
        string localFilePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sourcePath = GetFullPath(remoteFilePath);

            if (!File.Exists(sourcePath))
            {
                return Task.FromResult(false);
            }

            var localDir = Path.GetDirectoryName(localFilePath);
            if (!string.IsNullOrEmpty(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            File.Copy(sourcePath, localFilePath, overwrite: true);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<Stream?> DownloadFileAsync(
        string remoteFilePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sourcePath = GetFullPath(remoteFilePath);

            if (!File.Exists(sourcePath))
            {
                return Task.FromResult<Stream?>(null);
            }

            var stream = File.OpenRead(sourcePath);
            return Task.FromResult<Stream?>(stream);
        }
        catch
        {
            return Task.FromResult<Stream?>(null);
        }
    }

    public Task<bool> DeleteFileAsync(
        string remoteFilePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = GetFullPath(remoteFilePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<List<CloudFileInfo>> ListFilesAsync(
        string remoteFolderPath = "/",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var folderPath = GetFullPath(remoteFolderPath);

            if (!Directory.Exists(folderPath))
            {
                return Task.FromResult(new List<CloudFileInfo>());
            }

            var files = new List<CloudFileInfo>();

            // Add files
            foreach (var filePath in Directory.GetFiles(folderPath))
            {
                var fileInfo = new FileInfo(filePath);
                files.Add(new CloudFileInfo
                {
                    Id = filePath,
                    Name = fileInfo.Name,
                    Path = GetRelativePath(filePath),
                    Size = fileInfo.Length,
                    CreatedAt = fileInfo.CreationTimeUtc,
                    ModifiedAt = fileInfo.LastWriteTimeUtc,
                    IsFolder = false
                });
            }

            // Add folders
            foreach (var dirPath in Directory.GetDirectories(folderPath))
            {
                var dirInfo = new DirectoryInfo(dirPath);
                files.Add(new CloudFileInfo
                {
                    Id = dirPath,
                    Name = dirInfo.Name,
                    Path = GetRelativePath(dirPath),
                    Size = 0,
                    CreatedAt = dirInfo.CreationTimeUtc,
                    ModifiedAt = dirInfo.LastWriteTimeUtc,
                    IsFolder = true
                });
            }

            return Task.FromResult(files);
        }
        catch
        {
            return Task.FromResult(new List<CloudFileInfo>());
        }
    }

    public Task<CloudFileInfo?> GetFileInfoAsync(
        string remoteFilePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = GetFullPath(remoteFilePath);

            if (File.Exists(fullPath))
            {
                var fileInfo = new FileInfo(fullPath);
                return Task.FromResult<CloudFileInfo?>(new CloudFileInfo
                {
                    Id = fullPath,
                    Name = fileInfo.Name,
                    Path = remoteFilePath,
                    Size = fileInfo.Length,
                    CreatedAt = fileInfo.CreationTimeUtc,
                    ModifiedAt = fileInfo.LastWriteTimeUtc,
                    IsFolder = false
                });
            }

            if (Directory.Exists(fullPath))
            {
                var dirInfo = new DirectoryInfo(fullPath);
                return Task.FromResult<CloudFileInfo?>(new CloudFileInfo
                {
                    Id = fullPath,
                    Name = dirInfo.Name,
                    Path = remoteFilePath,
                    Size = 0,
                    CreatedAt = dirInfo.CreationTimeUtc,
                    ModifiedAt = dirInfo.LastWriteTimeUtc,
                    IsFolder = true
                });
            }

            return Task.FromResult<CloudFileInfo?>(null);
        }
        catch
        {
            return Task.FromResult<CloudFileInfo?>(null);
        }
    }

    public Task<bool> CreateFolderAsync(
        string remoteFolderPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var folderPath = GetFullPath(remoteFolderPath);
            Directory.CreateDirectory(folderPath);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> FileExistsAsync(
        string remoteFilePath,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(remoteFilePath);
        return Task.FromResult(File.Exists(fullPath) || Directory.Exists(fullPath));
    }

    private string GetFullPath(string relativePath)
    {
        // Remove leading slash if present
        relativePath = relativePath.TrimStart('/', '\\');
        return Path.Combine(_backupFolder, relativePath);
    }

    private string GetRelativePath(string fullPath)
    {
        return fullPath.Replace(_backupFolder, "").TrimStart('/', '\\');
    }
}
