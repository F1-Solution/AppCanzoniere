namespace AppCanzoniere.Domain.Entities;

public class AlbumSong
{
    public Guid AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    public Guid SongId { get; set; }
    public Song Song { get; set; } = null!;

    public int OrderIndex { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
