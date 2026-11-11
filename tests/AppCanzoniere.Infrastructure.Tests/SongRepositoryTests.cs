using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using AppCanzoniere.Infrastructure.Data;
using AppCanzoniere.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Infrastructure.Tests;

public class SongRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISongRepository _repository;

    public SongRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new SongRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddSongToDatabase()
    {
        // Arrange
        var song = new Song
        {
            Title = "Test Song",
            Artist = "Test Artist",
            Language = "en",
            OriginalKey = "C"
        };

        // Act
        var result = await _repository.AddAsync(song);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);

        var savedSong = await _context.Songs.FindAsync(result.Id);
        Assert.NotNull(savedSong);
        Assert.Equal("Test Song", savedSong.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSong_WhenExists()
    {
        // Arrange
        var song = new Song
        {
            Title = "Test Song 2",
            Artist = "Test Artist 2"
        };
        await _repository.AddAsync(song);

        // Act
        var result = await _repository.GetByIdAsync(song.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(song.Id, result.Id);
        Assert.Equal("Test Song 2", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingSongs()
    {
        // Arrange
        await _repository.AddAsync(new Song { Title = "Amazing Grace", Artist = "John Newton" });
        await _repository.AddAsync(new Song { Title = "Grace Greater", Artist = "Julia Johnston" });
        await _repository.AddAsync(new Song { Title = "Holy Holy", Artist = "Unknown" });

        // Act
        var results = await _repository.SearchAsync("Grace");

        // Assert
        Assert.Equal(2, results.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSong()
    {
        // Arrange
        var song = new Song { Title = "Original Title", Artist = "Artist" };
        await _repository.AddAsync(song);

        song.Title = "Updated Title";
        var originalVersion = song.Version;

        // Act
        await _repository.UpdateAsync(song);

        // Assert
        var updated = await _repository.GetByIdAsync(song.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Title", updated.Title);
        Assert.Equal(originalVersion + 1, updated.Version);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSong()
    {
        // Arrange
        var song = new Song { Title = "To Delete", Artist = "Artist" };
        await _repository.AddAsync(song);
        var songId = song.Id;

        // Act
        await _repository.DeleteAsync(songId);

        // Assert
        var deleted = await _repository.GetByIdAsync(songId);
        Assert.Null(deleted);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
