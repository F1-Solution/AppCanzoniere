namespace AppCanzoniere.Domain.Entities;

public class LiturgicalMoment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string NameKey { get; set; } = string.Empty; // For localization
    public int DisplayOrder { get; set; }
    
    // Navigation properties
    public List<SongLiturgicalMoment> SongLiturgicalMoments { get; set; } = new();
}
