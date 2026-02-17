namespace AppCanzoniere.Domain.Entities;

public class Mp3Attachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SongId { get; set; }
    public Song Song { get; set; } = null!;
    
    public string FileName { get; set; } = string.Empty;
    public string? LocalPath { get; set; }
    public string? Url { get; set; }
    public bool IsDownloadedForOffline { get; set; }
    public long? FileSize { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
