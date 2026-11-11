namespace AppCanzoniere.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Color { get; set; }

    // Navigation properties
    public List<SongTag> SongTags { get; set; } = new();
}
