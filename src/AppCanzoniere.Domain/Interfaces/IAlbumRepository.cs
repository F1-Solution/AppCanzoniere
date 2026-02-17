using AppCanzoniere.Domain.Entities;

namespace AppCanzoniere.Domain.Interfaces;

public interface IAlbumRepository
{
    Task<Album?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Album?> GetByIdWithSongsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Album>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Album>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Album> AddAsync(Album album, CancellationToken cancellationToken = default);
    Task UpdateAsync(Album album, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Song>> GetSongsInAlbumAsync(Guid albumId, CancellationToken cancellationToken = default);
    Task AddSongAsync(Guid albumId, Guid songId, int orderIndex, CancellationToken cancellationToken = default);
    Task RemoveSongAsync(Guid albumId, Guid songId, CancellationToken cancellationToken = default);
    Task UpdateSongOrderAsync(Guid albumId, Dictionary<Guid, int> songOrders, CancellationToken cancellationToken = default);
}
