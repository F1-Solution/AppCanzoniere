namespace AppCanzoniere.Backend.IntegrationTests;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}

public class SongDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public string? OriginalKey { get; set; }
    public string? Lyrics { get; set; }
}
