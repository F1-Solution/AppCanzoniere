using AppCanzoniere.Application.DTOs.Backend;

namespace AppCanzoniere.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(string email, string password);
    Task<AuthResponse?> RegisterAsync(string email, string password, string confirmPassword);
    Task<bool> IsAuthenticatedAsync();
    Task LogoutAsync();
    Task<string?> GetCurrentTokenAsync();
    Task<string?> RefreshTokenAsync();
}
