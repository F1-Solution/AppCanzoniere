using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Infrastructure.Repositories;

public class AlbumRepository : IAlbumRepository
{
    private readonly AppDbContext _context;

    public AlbumRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Album?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Albums
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Album?> GetByIdWithSongsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Albums
            .Include(a => a.AlbumSongs)
                .ThenInclude(als => als.Song)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Album>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Albums
            .Include(a => a.AlbumSongs)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Album>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Albums
            .Include(a => a.AlbumSongs)
            .Where(a => a.Name.Contains(searchTerm) || a.Description.Contains(searchTerm))
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Album> AddAsync(Album album, CancellationToken cancellationToken = default)
    {
        if (album.Id == Guid.Empty)
            album.Id = Guid.NewGuid();

        _context.Albums.Add(album);
        await _context.SaveChangesAsync(cancellationToken);
        return album;
    }

    public async Task UpdateAsync(Album album, CancellationToken cancellationToken = default)
    {
        album.UpdatedAt = DateTime.UtcNow;
        _context.Albums.Update(album);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var album = await _context.Albums.FindAsync(new object[] { id }, cancellationToken);
        if (album != null)
        {
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Song>> GetSongsInAlbumAsync(Guid albumId, CancellationToken cancellationToken = default)
    {
        return await _context.AlbumSongs
            .Where(als => als.AlbumId == albumId)
            .OrderBy(als => als.OrderIndex)
            .Select(als => als.Song)
            .ToListAsync(cancellationToken);
    }

    public async Task AddSongAsync(Guid albumId, Guid songId, int orderIndex, CancellationToken cancellationToken = default)
    {
        var albumSong = new AlbumSong
        {
            AlbumId = albumId,
            SongId = songId,
            OrderIndex = orderIndex,
            AddedAt = DateTime.UtcNow
        };

        _context.AlbumSongs.Add(albumSong);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveSongAsync(Guid albumId, Guid songId, CancellationToken cancellationToken = default)
    {
        var albumSong = await _context.AlbumSongs
            .FirstOrDefaultAsync(als => als.AlbumId == albumId && als.SongId == songId, cancellationToken);

        if (albumSong != null)
        {
            _context.AlbumSongs.Remove(albumSong);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateSongOrderAsync(Guid albumId, Dictionary<Guid, int> songOrders, CancellationToken cancellationToken = default)
    {
        var albumSongs = await _context.AlbumSongs
            .Where(als => als.AlbumId == albumId)
            .ToListAsync(cancellationToken);

        foreach (var albumSong in albumSongs)
        {
            if (songOrders.TryGetValue(albumSong.SongId, out int newOrder))
            {
                albumSong.OrderIndex = newOrder;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
