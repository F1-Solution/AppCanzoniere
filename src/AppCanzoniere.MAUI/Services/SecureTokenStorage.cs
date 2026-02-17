using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.MAUI.Services;

/// <summary>
/// MAUI-specific implementation of ITokenStorage using SecureStorage
/// </summary>
public class SecureTokenStorage : ITokenStorage
{
    private const string TokenKey = "auth_token";
    private const string EmailKey = "user_email";
    private const string ExpirationKey = "token_expiration";

    public async Task SetTokenAsync(string token)
    {
        await SecureStorage.Default.SetAsync(TokenKey, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.Default.GetAsync(TokenKey);
    }

    public async Task SetEmailAsync(string email)
    {
        await SecureStorage.Default.SetAsync(EmailKey, email);
    }

    public async Task<string?> GetEmailAsync()
    {
        return await SecureStorage.Default.GetAsync(EmailKey);
    }

    public async Task SetExpirationAsync(DateTime expiration)
    {
        await SecureStorage.Default.SetAsync(ExpirationKey, expiration.ToString("O"));
    }

    public async Task<DateTime?> GetExpirationAsync()
    {
        var expirationStr = await SecureStorage.Default.GetAsync(ExpirationKey);
        if (string.IsNullOrEmpty(expirationStr))
            return null;

        if (DateTime.TryParse(expirationStr, out var expiration))
            return expiration;

        return null;
    }

    public async Task ClearAllAsync()
    {
        SecureStorage.Default.Remove(TokenKey);
        SecureStorage.Default.Remove(EmailKey);
        SecureStorage.Default.Remove(ExpirationKey);
        await Task.CompletedTask;
    }
}
