using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using AppCanzoniere.Infrastructure.Data;
using AppCanzoniere.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Infrastructure.Tests;

public class AlbumRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IAlbumRepository _albumRepository;
    private readonly ISongRepository _songRepository;

    public AlbumRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _albumRepository = new AlbumRepository(_context);
        _songRepository = new SongRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAlbumToDatabase()
    {
        // Arrange
        var album = new Album
        {
            Name = "Test Album",
            Description = "Test Description"
        };

        // Act
        var result = await _albumRepository.AddAsync(album);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test Album", result.Name);
    }

    [Fact]
    public async Task AddSongAsync_ShouldCreateRelationship()
    {
        // Arrange
        var album = await _albumRepository.AddAsync(new Album { Name = "Album 1" });
        var song = await _songRepository.AddAsync(new Song { Title = "Song 1", Artist = "Artist" });

        // Act
        await _albumRepository.AddSongAsync(album.Id, song.Id, 1);

        // Assert
        var songs = await _albumRepository.GetSongsInAlbumAsync(album.Id);
        Assert.Single(songs);
        Assert.Equal(song.Id, songs.First().Id);
    }

    [Fact]
    public async Task GetSongsInAlbumAsync_ShouldReturnSongsInOrder()
    {
        // Arrange
        var album = await _albumRepository.AddAsync(new Album { Name = "Album" });
        var song1 = await _songRepository.AddAsync(new Song { Title = "Song 1", Artist = "A" });
        var song2 = await _songRepository.AddAsync(new Song { Title = "Song 2", Artist = "A" });
        var song3 = await _songRepository.AddAsync(new Song { Title = "Song 3", Artist = "A" });

        await _albumRepository.AddSongAsync(album.Id, song2.Id, 2);
        await _albumRepository.AddSongAsync(album.Id, song1.Id, 1);
        await _albumRepository.AddSongAsync(album.Id, song3.Id, 3);

        // Act
        var songs = await _albumRepository.GetSongsInAlbumAsync(album.Id);

        // Assert
        var songList = songs.ToList();
        Assert.Equal(3, songList.Count);
        Assert.Equal(song1.Id, songList[0].Id);
        Assert.Equal(song2.Id, songList[1].Id);
        Assert.Equal(song3.Id, songList[2].Id);
    }

    [Fact]
    public async Task RemoveSongAsync_ShouldRemoveRelationship()
    {
        // Arrange
        var album = await _albumRepository.AddAsync(new Album { Name = "Album" });
        var song = await _songRepository.AddAsync(new Song { Title = "Song", Artist = "A" });
        await _albumRepository.AddSongAsync(album.Id, song.Id, 1);

        // Act
        await _albumRepository.RemoveSongAsync(album.Id, song.Id);

        // Assert
        var songs = await _albumRepository.GetSongsInAlbumAsync(album.Id);
        Assert.Empty(songs);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
