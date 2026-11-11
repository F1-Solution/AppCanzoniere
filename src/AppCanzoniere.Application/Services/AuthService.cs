using System.Net.Http.Json;
using AppCanzoniere.Application.DTOs.Backend;
using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStorage _tokenStorage;

    public AuthService(HttpClient httpClient, ITokenStorage tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        try
        {
            var request = new LoginRequest(email, password);
            var response = await _httpClient.PostAsJsonAsync("account/login", request);

            if (!response.IsSuccessStatusCode)
                return null;

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResponse != null)
            {
                await SaveTokenAsync(authResponse);
            }

            return authResponse;
        }
        catch
        {
            return null;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(string email, string password, string confirmPassword)
    {
        try
        {
            var request = new RegisterRequest(email, password, confirmPassword);
            var response = await _httpClient.PostAsJsonAsync("account/register", request);

            if (!response.IsSuccessStatusCode)
                return null;

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResponse != null)
            {
                await SaveTokenAsync(authResponse);
            }

            return authResponse;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _tokenStorage.GetTokenAsync();
        var expiration = await _tokenStorage.GetExpirationAsync();

        if (string.IsNullOrEmpty(token) || !expiration.HasValue)
            return false;

        // Check if token is expired
        if (expiration.Value <= DateTime.UtcNow)
        {
            await LogoutAsync();
            return false;
        }

        return true;
    }

    public async Task LogoutAsync()
    {
        await _tokenStorage.ClearAllAsync();
    }

    public async Task<string?> GetCurrentTokenAsync()
    {
        return await _tokenStorage.GetTokenAsync();
    }

    public async Task<string?> RefreshTokenAsync()
    {
        // TODO: Implement token refresh logic when backend supports it
        // For now, just return current token if still valid
        var isValid = await IsAuthenticatedAsync();
        return isValid ? await GetCurrentTokenAsync() : null;
    }

    private async Task SaveTokenAsync(AuthResponse authResponse)
    {
        await _tokenStorage.SetTokenAsync(authResponse.Token);
        await _tokenStorage.SetEmailAsync(authResponse.Email);
        await _tokenStorage.SetExpirationAsync(authResponse.Expiration);
    }
}
