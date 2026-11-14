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

        Assert.Equal(new[] { "Do", "Sol" }, screen.Text.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal("Do", screen.Key);
    }

    [Fact]
    public async Task Reset_RestoresTheDefaultsOnTheStoredRow_AndKeepsItsIdentity()
    {
        var stored = await _repository.GetOrCreateAsync();
        stored.PreferredLanguage = "it";
        stored.ThemeMode = "Dark";
        await _repository.UpdateAsync(stored);

        var reset = await _repository.ResetAsync();

        Assert.Equal(stored.Id, reset.Id);
        Assert.Equal(string.Empty, reset.PreferredLanguage);
        Assert.Equal("System", reset.ThemeMode);
        _context.ChangeTracker.Clear();
        var reread = await _repository.GetAsync();
        Assert.Equal(stored.Id, reread!.Id);
        Assert.Equal(string.Empty, reread.PreferredLanguage);
        Assert.Equal(1, await _context.UserSettings.CountAsync());
    }

    [Fact]
    public async Task AFailedSave_DoesNotMakeLaterSavesFail()
    {
        var stored = await _repository.GetOrCreateAsync();
        var phantom = new UserSettings { PreferredLanguage = "it" };

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _repository.UpdateAsync(phantom));

        _context.Songs.Add(new Song { Title = "After the failure", Language = "it" });
        await _context.SaveChangesAsync();
        stored.ThemeMode = "Dark";
        await _repository.UpdateAsync(stored);
        Assert.Equal(1, await _context.Songs.CountAsync());
    }

    [Fact]
    public async Task ReadingForTheSongScreen_ShowsOnlySavedValues()
    {
        var stored = await _repository.GetOrCreateAsync();
        stored.PreferredLanguage = "it";
        await _repository.UpdateAsync(stored);

        stored.PreferredLanguage = "en";

        var seen = await _repository.GetAsync();
        Assert.Equal("it", seen!.PreferredLanguage);
    }
}
