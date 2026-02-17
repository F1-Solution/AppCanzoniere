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

    public async Task<UserSettings?> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserSettings.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserSettings> GetOrCreateAsync(CancellationToken cancellationToken = default)
    {
        var settings = await GetAsync(cancellationToken);
        
        if (settings == null)
        {
            settings = new UserSettings();
            _context.UserSettings.Add(settings);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        return settings;
    }

    public async Task UpdateAsync(UserSettings settings, CancellationToken cancellationToken = default)
    {
        _context.UserSettings.Update(settings);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
