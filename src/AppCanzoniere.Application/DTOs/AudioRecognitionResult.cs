namespace AppCanzoniere.Application.DTOs;

public class AudioRecognitionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Track Information
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public string? Album { get; set; }
    public string? ReleaseDate { get; set; }
    public string? Label { get; set; }
    
    // Identification Metadata
    public float Score { get; set; }  // 0-100, confidence score
    public int DurationMs { get; set; }
    public string? Isrc { get; set; }  // International Standard Recording Code
    public string? SpotifyId { get; set; }
    public string? YouTubeId { get; set; }
    
    // Additional Info
    public List<string> Genres { get; set; } = new();
    public string? CoverArtUrl { get; set; }
    public string? PreviewUrl { get; set; }
    
    // Service Info
    public string ServiceName { get; set; } = string.Empty;
    public DateTime RecognizedAt { get; set; } = DateTime.UtcNow;
}
