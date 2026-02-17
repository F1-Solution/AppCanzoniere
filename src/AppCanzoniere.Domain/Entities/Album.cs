using AppCanzoniere.Domain.Enums;

namespace AppCanzoniere.Domain.Entities;

public class Album
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AlbumType Type { get; set; } = AlbumType.Manual;
    public string? CoverImagePath { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublic { get; set; }
    
    // Navigation properties
    public List<AlbumSong> AlbumSongs { get; set; } = new();
}
