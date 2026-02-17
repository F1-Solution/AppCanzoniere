namespace AppCanzoniere.Application.Interfaces;

/// <summary>
/// Service for file operations (cross-platform)
/// </summary>
public interface IFileService
{
    Task<bool> FileExistsAsync(string path);
    Task<string> ReadTextAsync(string path);
    Task WriteTextAsync(string path, string content);
    Task<byte[]> ReadBytesAsync(string path);
    Task WriteBytesAsync(string path, byte[] bytes);
    Task DeleteFileAsync(string path);
    Task<string> GetCacheDirectoryAsync();
    Task<long> GetFileSizeAsync(string path);
}
