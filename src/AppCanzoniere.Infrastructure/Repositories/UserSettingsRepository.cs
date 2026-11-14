using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Infrastructure.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly AppDbContext _context;

    public UserSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>The saved settings, untracked so that an unsaved edit elsewhere is never seen.</summary>
    public async Task<UserSettings?> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserSettings.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserSettings> GetOrCreateAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _context.UserSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings != null)
            return settings;

        settings = new UserSettings();
        _context.UserSettings.Add(settings);
        await SaveAsync(cancellationToken);
        return settings;
    }

    public async Task<UserSettings> ResetAsync(CancellationToken cancellationToken = default)
    {
        var settings = await GetOrCreateAsync(cancellationToken);
        _context.Entry(settings).CurrentValues.SetValues(new UserSettings { Id = settings.Id });
        await SaveAsync(cancellationToken);
        return settings;
    }

    public async Task UpdateAsync(UserSettings settings, CancellationToken cancellationToken = default)
    {
        _context.UserSettings.Update(settings);
        await SaveAsync(cancellationToken);
    }

    // A failed save would leave its entities tracked and fail every later save on the shared context.
    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            _context.ChangeTracker.Clear();
            throw;
        }
    }
}
