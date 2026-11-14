using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Services;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Infrastructure.Data;
using AppCanzoniere.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Infrastructure.Tests;

// Real SQLite in memory, because only a relational provider fails an UPDATE that matches no row.
public sealed class UserSettingsRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly AppDbContext _context;
    private readonly UserSettingsRepository _repository;

    public UserSettingsRepositoryTests()
    {
        _connection.Open();
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);
        _context.Database.EnsureCreated();
        _repository = new UserSettingsRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task AFreshSettingsRow_LeavesTheDeviceLanguageInCharge()
    {
        var settings = await _repository.GetOrCreateAsync();
        var presenter = new SongScreenPresenter(new SongDisplayService(new ChordProParser()));

        var screen = presenter.Present(new SongScreenRequest("{key:C}\n[C]Uno [G]due", null, 0, ChordDisplaySettings.Default, settings.PreferredLanguage, "it"));

        Assert.Equal("Do    Sol", screen.Text.Split('\n')[0].TrimEnd());
        Assert.Equal("Do", screen.Key);
    }
}
