namespace AppCanzoniere.Application.DTOs.Backend;

// Auth DTOs matching Backend
public record LoginRequest(string Email, string Password);

public record RegisterRequest(string Email, string Password, string ConfirmPassword);

public record AuthResponse(string Token, string Email, DateTime Expiration);

// Song DTOs for API communication
public record PublicSongDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public string Language { get; init; } = "en";
    public string? ChordProContent { get; init; }
    public string OriginalKey { get; init; } = "C";
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool IsPublic { get; init; }
}

public record CreateSongRequest
{
    public string Title { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public string Language { get; init; } = "en";
    public string? ChordProContent { get; init; }
    public string OriginalKey { get; init; } = "C";
    public bool IsPublic { get; init; }
}

public record UpdateSongRequest
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public string Language { get; init; } = "en";
    public string? ChordProContent { get; init; }
    public string OriginalKey { get; init; } = "C";
    public bool IsPublic { get; init; }
}
