using AppCanzoniere.Backend.Controllers;
using AppCanzoniere.Backend.DTOs;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;
using AppCanzoniere.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppCanzoniere.Backend.Tests.Controllers;

public class SongsControllerTests
{
    private readonly Mock<ISongRepository> _mockRepository;
    private readonly Mock<ILogger<SongsController>> _mockLogger;
    private readonly SongsController _controller;

    public SongsControllerTests()
    {
        _mockRepository = new Mock<ISongRepository>();
        _mockLogger = new Mock<ILogger<SongsController>>();
        _controller = new SongsController(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetPublicSongs_ReturnsOnlyPublicSongs()
    {
        // Arrange
        var songs = new List<Song>
        {
            new() { Id = Guid.NewGuid(), Title = "Public Song 1", Artist = "Artist 1", IsPublic = true },
            new() { Id = Guid.NewGuid(), Title = "Private Song", Artist = "Artist 2", IsPublic = false },
            new() { Id = Guid.NewGuid(), Title = "Public Song 2", Artist = "Artist 3", IsPublic = true }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(songs);

        // Act
        var result = await _controller.GetPublicSongs();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedSongs = okResult!.Value as IEnumerable<SongResponseDto>;
        returnedSongs.Should().HaveCount(2);
        returnedSongs.Should().OnlyContain(s => s.IsPublic);
    }

    [Fact]
    public async Task GetPublicSongs_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var songs = Enumerable.Range(1, 50)
            .Select(i => new Song { Id = Guid.NewGuid(), Title = $"Song {i}", Artist = "Artist", IsPublic = true })
            .ToList();
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(songs);

        // Act
        var result = await _controller.GetPublicSongs(page: 2, pageSize: 20);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedSongs = okResult!.Value as IEnumerable<SongResponseDto>;
        returnedSongs.Should().HaveCount(20);
    }

    [Fact]
    public async Task GetSong_PublicSong_ReturnsOk()
    {
        // Arrange
        var song = new Song { Id = Guid.NewGuid(), Title = "Public Song", Artist = "Artist", IsPublic = true };
        _mockRepository.Setup(r => r.GetByIdAsync(song.Id)).ReturnsAsync(song);

        // Act
        var result = await _controller.GetSong(song.Id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedSong = okResult!.Value as SongResponseDto;
        returnedSong.Should().NotBeNull();
        returnedSong!.Id.Should().Be(song.Id);
    }

    [Fact]
    public async Task GetSong_NonExistent_ReturnsNotFound()
    {
        // Arrange
        var songId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(songId)).ReturnsAsync((Song?)null);

        // Act
        var result = await _controller.GetSong(songId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task SearchSongs_ReturnsMatchingSongs()
    {
        // Arrange
        var songs = new List<Song>
        {
            new() { Id = Guid.NewGuid(), Title = "Amazing Grace", Artist = "John Newton", IsPublic = true },
            new() { Id = Guid.NewGuid(), Title = "Grace Flows Down", Artist = "Passion", IsPublic = true },
            new() { Id = Guid.NewGuid(), Title = "How Great Thou Art", Artist = "Unknown", IsPublic = true }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(songs);

        // Act
        var result = await _controller.SearchSongs("grace");

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedSongs = okResult!.Value as IEnumerable<SongResponseDto>;
        returnedSongs.Should().HaveCount(2);
        returnedSongs.Should().Contain(s => s.Title.Contains("Grace", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CreateSong_ValidSong_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateSongDto(
            Title: "New Song",
            Artist: "New Artist",
            Language: "en",
            Type: SongType.ChurchSong,
            ChordProContent: "[C]Hello [G]World",
            MusicXmlContent: null,
            JsonContent: null,
            PrimaryFormat: ContentFormat.ChordPro,
            OriginalKey: "C",
            IsPublic: true,
            CopyrightStatus: CopyrightStatus.Unknown
        );
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Song>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Song s, CancellationToken _) => s);

        // Act
        var result = await _controller.CreateSong(dto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(SongsController.GetSong));
        var returnedSong = createdResult.Value as SongResponseDto;
        returnedSong.Should().NotBeNull();
        returnedSong!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task DeleteSong_ExistingSong_ReturnsNoContent()
    {
        // Arrange
        var song = new Song { Id = Guid.NewGuid(), Title = "Song to Delete", Artist = "Artist", IsPublic = true };
        _mockRepository.Setup(r => r.GetByIdAsync(song.Id)).ReturnsAsync(song);
        _mockRepository.Setup(r => r.DeleteAsync(song.Id)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteSong(song.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteSong_NonExistent_ReturnsNotFound()
    {
        // Arrange
        var songId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(songId)).ReturnsAsync((Song?)null);

        // Act
        var result = await _controller.DeleteSong(songId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
