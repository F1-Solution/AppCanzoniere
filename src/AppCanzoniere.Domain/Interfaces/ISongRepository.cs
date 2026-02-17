using AppCanzoniere.Domain.Entities;

namespace AppCanzoniere.Domain.Interfaces;

public interface ISongRepository
{
    Task<Song?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Song>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Song>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Song>> GetSongsByTagsAsync(List<Guid> tagIds, CancellationToken cancellationToken = default);
    Task<Song> AddAsync(Song song, CancellationToken cancellationToken = default);
    Task UpdateAsync(Song song, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Advanced search with multiple criteria and filtering
    /// </summary>
    Task<AdvancedSearchResult<Song>> AdvancedSearchAsync(AdvancedSearchCriteria criteria, CancellationToken cancellationToken = default);
}
