namespace AppCanzoniere.Application.Interfaces;

/// <summary>
/// Interface for secure token storage across platforms
/// </summary>
public interface ITokenStorage
{
    /// <summary>
    /// Stores the authentication token securely
    /// </summary>
    Task SetTokenAsync(string token);

    /// <summary>
    /// Retrieves the stored authentication token
    /// </summary>
    Task<string?> GetTokenAsync();

    /// <summary>
    /// Stores the user email
    /// </summary>
    Task SetEmailAsync(string email);

    /// <summary>
    /// Retrieves the stored user email
    /// </summary>
    Task<string?> GetEmailAsync();

    /// <summary>
    /// Stores the token expiration datetime
    /// </summary>
    Task SetExpirationAsync(DateTime expiration);

    /// <summary>
    /// Retrieves the token expiration datetime
    /// </summary>
    Task<DateTime?> GetExpirationAsync();

    /// <summary>
    /// Clears all stored authentication data
    /// </summary>
    Task ClearAllAsync();
}
