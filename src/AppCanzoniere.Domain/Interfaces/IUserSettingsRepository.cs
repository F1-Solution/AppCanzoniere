using AppCanzoniere.Domain.Entities;

namespace AppCanzoniere.Domain.Interfaces;

public interface IUserSettingsRepository
{
    Task<UserSettings?> GetAsync(CancellationToken cancellationToken = default);
    Task<UserSettings> GetOrCreateAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(UserSettings settings, CancellationToken cancellationToken = default);
}
