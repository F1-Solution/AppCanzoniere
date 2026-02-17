using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;
using AppCanzoniere.Domain.Interfaces;
using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Infrastructure.Repositories;

public class OfflineSongRepository : IOfflineSongRepository
{
    private readonly AppDbContext _context;

    public OfflineSongRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OfflineSong?> GetByIdAsync(int id)
    {
        return await _context.OfflineSongs.FindAsync(id);
    }

    public async Task<OfflineSong?> GetByBackendIdAsync(string backendSongId)
    {
        return await _context.OfflineSongs
            .FirstOrDefaultAsync(s => s.BackendSongId == backendSongId);
    }

    public async Task<IEnumerable<OfflineSong>> GetAllAsync()
    {
        return await _context.OfflineSongs
            .OrderByDescending(s => s.DownloadedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OfflineSong>> GetBySyncStatusAsync(SyncStatus status)
    {
        return await _context.OfflineSongs
            .Where(s => s.SyncStatus == status)
            .ToListAsync();
    }

    public async Task<OfflineSong> AddAsync(OfflineSong offlineSong)
    {
        offlineSong.CreatedAt = DateTime.UtcNow;
        offlineSong.UpdatedAt = DateTime.UtcNow;
        _context.OfflineSongs.Add(offlineSong);
        await _context.SaveChangesAsync();
        return offlineSong;
    }

    public async Task UpdateAsync(OfflineSong offlineSong)
    {
        offlineSong.UpdatedAt = DateTime.UtcNow;
        _context.OfflineSongs.Update(offlineSong);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var offlineSong = await GetByIdAsync(id);
        if (offlineSong != null)
        {
            _context.OfflineSongs.Remove(offlineSong);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<long> GetTotalStorageUsageAsync()
    {
        return await _context.OfflineSongs
            .Where(s => s.SyncStatus == SyncStatus.Downloaded || s.SyncStatus == SyncStatus.LocallyModified)
            .SumAsync(s => s.SizeInBytes);
    }

    public async Task<int> GetDownloadedCountAsync()
    {
        return await _context.OfflineSongs
            .Where(s => s.SyncStatus == SyncStatus.Downloaded || s.SyncStatus == SyncStatus.LocallyModified)
            .CountAsync();
    }

    public async Task<bool> IsDownloadedAsync(string backendSongId)
    {
        return await _context.OfflineSongs
            .AnyAsync(s => s.BackendSongId == backendSongId && 
                          (s.SyncStatus == SyncStatus.Downloaded || s.SyncStatus == SyncStatus.LocallyModified));
    }
}
