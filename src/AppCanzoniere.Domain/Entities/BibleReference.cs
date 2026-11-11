namespace AppCanzoniere.Domain.Entities;

public class BibleReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SongId { get; set; }
    public Song Song { get; set; } = null!;

    public string Book { get; set; } = string.Empty; // e.g., "Mt", "Gv", "Gen"
    public int? Chapter { get; set; }
    public int? VerseStart { get; set; }
    public int? VerseEnd { get; set; }
    public string DisplayText { get; set; } = string.Empty; // e.g., "Mt 5,1-12"
}
