using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;

namespace AppCanzoniere.Domain.Interfaces;

public interface IOfflineSongRepository
{
    Task<OfflineSong?> GetByIdAsync(int id);
    Task<OfflineSong?> GetByBackendIdAsync(string backendSongId);
    Task<IEnumerable<OfflineSong>> GetAllAsync();
    Task<IEnumerable<OfflineSong>> GetBySyncStatusAsync(SyncStatus status);
    Task<OfflineSong> AddAsync(OfflineSong offlineSong);
    Task UpdateAsync(OfflineSong offlineSong);
    Task DeleteAsync(int id);
    Task<long> GetTotalStorageUsageAsync();
    Task<int> GetDownloadedCountAsync();
    Task<bool> IsDownloadedAsync(string backendSongId);
}
