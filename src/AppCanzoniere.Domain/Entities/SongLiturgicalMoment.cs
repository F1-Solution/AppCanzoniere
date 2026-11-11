namespace AppCanzoniere.Domain.Entities;

public class SongLiturgicalMoment
{
    public Guid SongId { get; set; }
    public Song Song { get; set; } = null!;

    public Guid LiturgicalMomentId { get; set; }
    public LiturgicalMoment LiturgicalMoment { get; set; } = null!;
}
