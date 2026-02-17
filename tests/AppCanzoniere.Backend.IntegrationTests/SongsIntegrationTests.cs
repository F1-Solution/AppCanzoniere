using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AppCanzoniere.Backend.IntegrationTests;

public class SongsIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SongsIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetPublicSongs_WithoutAuth_ReturnsOkWithPublicSongs()
    {
        // Act
        var response = await _client.GetAsync("/api/songs/public");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var songs = await response.Content.ReadFromJsonAsync<List<SongDto>>();
        songs.Should().NotBeNull();
        songs.Should().AllSatisfy(s => s.IsPublic.Should().BeTrue());
    }

    [Fact]
    public async Task CreateSong_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var createSongRequest = new
        {
            Title = "Test Song",
            Artist = "Test Artist",
            IsPublic = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/songs", createSongRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateSong_WithAuth_ReturnsCreated()
    {
        // Arrange - Register and login
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createSongRequest = new
        {
            Title = $"Integration Test Song {Guid.NewGuid()}",
            Artist = "Test Artist",
            IsPublic = true,
            OriginalKey = "C",
            Lyrics = "[C]Test lyrics [G]here"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/songs", createSongRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var song = await response.Content.ReadFromJsonAsync<SongDto>();
        song.Should().NotBeNull();
        song!.Title.Should().Be(createSongRequest.Title);
        song.Artist.Should().Be(createSongRequest.Artist);
    }

    [Fact]
    public async Task GetSong_PublicSongWithoutAuth_ReturnsOk()
    {
        // Arrange - Create a public song first
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createSongRequest = new
        {
            Title = $"Public Song {Guid.NewGuid()}",
            Artist = "Test Artist",
            IsPublic = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/songs", createSongRequest);
        var createdSong = await createResponse.Content.ReadFromJsonAsync<SongDto>();

        // Remove auth header
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync($"/api/songs/{createdSong!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var song = await response.Content.ReadFromJsonAsync<SongDto>();
        song.Should().NotBeNull();
        song!.Id.Should().Be(createdSong.Id);
    }

    [Fact]
    public async Task UpdateSong_AsNonAdmin_ReturnsForbidden()
    {
        // Arrange - Create a song
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createSongRequest = new
        {
            Title = $"Song to Update {Guid.NewGuid()}",
            Artist = "Original Artist",
            IsPublic = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/songs", createSongRequest);
        var createdSong = await createResponse.Content.ReadFromJsonAsync<SongDto>();

        var updateRequest = new
        {
            Title = "Updated Title",
            Artist = "Updated Artist",
            IsPublic = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/songs/{createdSong!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SearchSongs_WithQuery_ReturnsMatchingSongs()
    {
        // Arrange - Create some songs
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var uniqueTitle = $"SearchTest{Guid.NewGuid()}";
        await _client.PostAsJsonAsync("/api/songs", new
        {
            Title = uniqueTitle,
            Artist = "Test Artist",
            IsPublic = true
        });

        // Remove auth
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync($"/api/songs/search?query={uniqueTitle}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var songs = await response.Content.ReadFromJsonAsync<List<SongDto>>();
        songs.Should().NotBeNull();
        songs.Should().Contain(s => s.Title.Contains(uniqueTitle));
    }

    [Fact]
    public async Task DeleteSong_AsNonAdmin_ReturnsForbidden()
    {
        // Arrange - Create a song
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createSongRequest = new
        {
            Title = $"Song to Delete {Guid.NewGuid()}",
            Artist = "Test Artist",
            IsPublic = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/songs", createSongRequest);
        var createdSong = await createResponse.Content.ReadFromJsonAsync<SongDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/songs/{createdSong!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var registerRequest = new
        {
            Email = $"testuser{Guid.NewGuid()}@test.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };

        var response = await _client.PostAsJsonAsync("/api/account/register", registerRequest);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return authResponse!.Token;
    }
}
