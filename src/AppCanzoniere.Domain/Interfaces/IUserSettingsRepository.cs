using AppCanzoniere.Domain.Entities;

namespace AppCanzoniere.Domain.Interfaces;

public interface IUserSettingsRepository
{
    Task<UserSettings?> GetAsync(CancellationToken cancellationToken = default);
    Task<UserSettings> GetOrCreateAsync(CancellationToken cancellationToken = default);
    /// <summary>Puts every setting back to its default on the stored row, keeping its identity.</summary>
    Task<UserSettings> ResetAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(UserSettings settings, CancellationToken cancellationToken = default);
}
