using System.Net.Http.Headers;
using System.Net.Http.Json;
using AppCanzoniere.Application.DTOs.Backend;
using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Services;

public class BackendSongService : IBackendSongService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public BackendSongService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
        _httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
    }

    public async Task<IEnumerable<PublicSongDto>> GetPublicSongsAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            var response = await _httpClient.GetAsync($"songs?page={page}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<PublicSongDto>();

            var songs = await response.Content.ReadFromJsonAsync<IEnumerable<PublicSongDto>>();
            return songs ?? Enumerable.Empty<PublicSongDto>();
        }
        catch
        {
            return Enumerable.Empty<PublicSongDto>();
        }
    }

    public async Task<PublicSongDto?> GetSongByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"songs/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PublicSongDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<PublicSongDto>> SearchSongsAsync(string query)
    {
        try
        {
            var response = await _httpClient.GetAsync($"songs/search?query={Uri.EscapeDataString(query)}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<PublicSongDto>();

            var songs = await response.Content.ReadFromJsonAsync<IEnumerable<PublicSongDto>>();
            return songs ?? Enumerable.Empty<PublicSongDto>();
        }
        catch
        {
            return Enumerable.Empty<PublicSongDto>();
        }
    }

    public async Task<PublicSongDto?> CreateSongAsync(CreateSongRequest request)
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var response = await _httpClient.PostAsJsonAsync("songs", request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PublicSongDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateSongAsync(UpdateSongRequest request)
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var response = await _httpClient.PutAsJsonAsync($"songs/{request.Id}", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteSongAsync(Guid id)
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var response = await _httpClient.DeleteAsync($"songs/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task EnsureAuthenticatedAsync()
    {
        var token = await _authService.GetCurrentTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
