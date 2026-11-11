using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Infrastructure.Repositories;

public class SongRepository : ISongRepository
{
    private readonly AppDbContext _context;

    public SongRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Song?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Songs.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Song>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Songs
            .OrderBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Song>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Songs
            .Where(s => s.Title.Contains(searchTerm) || s.Artist.Contains(searchTerm))
            .OrderBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Song>> GetSongsByTagsAsync(List<Guid> tagIds, CancellationToken cancellationToken = default)
    {
        return await _context.Songs
            .Where(s => s.SongTags.Any(st => tagIds.Contains(st.TagId)))
            .Include(s => s.SongTags)
                .ThenInclude(st => st.Tag)
            .OrderBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<Song> AddAsync(Song song, CancellationToken cancellationToken = default)
    {
        _context.Songs.Add(song);
        await _context.SaveChangesAsync(cancellationToken);
        return song;
    }

    public async Task UpdateAsync(Song song, CancellationToken cancellationToken = default)
    {
        song.UpdatedAt = DateTime.UtcNow;
        song.Version++;
        _context.Songs.Update(song);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var song = await GetByIdAsync(id, cancellationToken);
        if (song != null)
        {
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
