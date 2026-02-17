using AppCanzoniere.Application.DTOs.Backend;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;
using AppCanzoniere.Domain.Interfaces;

namespace AppCanzoniere.Application.Services;

public class DownloadService : IDownloadService
{
    private readonly IBackendSongService _backendSongService;
    private readonly IOfflineSongRepository _offlineSongRepository;
    private readonly IFileService _fileService;
    private const long MaxStorageBytes = 500 * 1024 * 1024; // 500 MB

    public DownloadService(
        IBackendSongService backendSongService,
        IOfflineSongRepository offlineSongRepository,
        IFileService fileService)
    {
        _backendSongService = backendSongService;
        _offlineSongRepository = offlineSongRepository;
        _fileService = fileService;
    }

    public async Task<bool> DownloadSongAsync(string backendSongId, IProgress<double>? progress = null)
    {
        try
        {
            progress?.Report(0.1);

            // Check if already downloaded
            if (await IsDownloadedAsync(backendSongId))
                return true;

            // Check storage limit
            var currentUsage = await GetStorageUsageAsync();
            if (currentUsage >= MaxStorageBytes)
                return false;

            progress?.Report(0.2);

            // Fetch song from backend by searching
            var searchResults = await _backendSongService.SearchSongsAsync(backendSongId);
            var publicSong = searchResults.FirstOrDefault();

            if (publicSong == null)
                return false;

            progress?.Report(0.5);

            // Create offline song entity
            var offlineSong = new OfflineSong
            {
                BackendSongId = publicSong.Id.ToString(),
                Title = publicSong.Title,
                Artist = publicSong.Artist,
                OriginalKey = publicSong.OriginalKey,
                Lyrics = publicSong.ChordProContent,
                SyncStatus = SyncStatus.Downloaded,
                DownloadedAt = DateTime.UtcNow,
                LastSyncedAt = DateTime.UtcNow,
                BackendVersion = publicSong.UpdatedAt,
                SizeInBytes = CalculateSongSize(publicSong)
            };

            progress?.Report(0.8);

            // Save to local database
            await _offlineSongRepository.AddAsync(offlineSong);

            progress?.Report(1.0);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveDownloadAsync(string backendSongId)
    {
        try
        {
            var offlineSong = await _offlineSongRepository.GetByBackendIdAsync(backendSongId);
            if (offlineSong == null)
                return false;

            // Delete media files if they exist
            if (!string.IsNullOrEmpty(offlineSong.LocalPdfPath))
            {
                await _fileService.DeleteFileAsync(offlineSong.LocalPdfPath);
            }

            if (!string.IsNullOrEmpty(offlineSong.LocalMp3Path))
            {
                await _fileService.DeleteFileAsync(offlineSong.LocalMp3Path);
            }

            // Delete from database
            await _offlineSongRepository.DeleteAsync(offlineSong.Id);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsDownloadedAsync(string backendSongId)
    {
        return await _offlineSongRepository.IsDownloadedAsync(backendSongId);
    }

    public async Task<long> GetStorageUsageAsync()
    {
        return await _offlineSongRepository.GetTotalStorageUsageAsync();
    }

    public async Task<int> GetDownloadedCountAsync()
    {
        return await _offlineSongRepository.GetDownloadedCountAsync();
    }

    public async Task<bool> DownloadMediaAsync(string backendSongId, IProgress<double>? progress = null)
    {
        // TODO: Implement media download (PDF, MP3) in future phase
        await Task.CompletedTask;
        progress?.Report(1.0);
        return true;
    }

    public async Task ClearAllDownloadsAsync()
    {
        var allSongs = await _offlineSongRepository.GetAllAsync();
        foreach (var song in allSongs)
        {
            await RemoveDownloadAsync(song.BackendSongId);
        }
    }

    private long CalculateSongSize(PublicSongDto publicSong)
    {
        // Estimate: lyrics text + metadata ~= 10KB per song
        var textSize = (publicSong.ChordProContent?.Length ?? 0) + 
                      (publicSong.Title?.Length ?? 0) + 
                      (publicSong.Artist?.Length ?? 0);
        return Math.Max(textSize * 2, 10240); // Minimum 10KB
    }
}
