namespace AppCanzoniere.Domain.Entities;

public class YouTubeLink
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SongId { get; set; }
    public Song Song { get; set; } = null!;

    public string VideoId { get; set; } = string.Empty; // YouTube video ID
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
