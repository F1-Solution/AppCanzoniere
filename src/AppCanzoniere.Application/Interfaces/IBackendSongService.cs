using AppCanzoniere.Application.DTOs.Backend;

namespace AppCanzoniere.Application.Interfaces;

public interface IBackendSongService
{
    Task<IEnumerable<PublicSongDto>> GetPublicSongsAsync(int page = 1, int pageSize = 20);
    Task<PublicSongDto?> GetSongByIdAsync(Guid id);
    Task<IEnumerable<PublicSongDto>> SearchSongsAsync(string query);
    Task<PublicSongDto?> CreateSongAsync(CreateSongRequest request);
    Task<bool> UpdateSongAsync(UpdateSongRequest request);
    Task<bool> DeleteSongAsync(Guid id);
}
