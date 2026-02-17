using AppCanzoniere.Domain.Enums;

namespace AppCanzoniere.Domain.Entities;

public class PdfAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SongId { get; set; }
    public Song Song { get; set; } = null!;
    
    public string FileName { get; set; } = string.Empty;
    public string? LocalPath { get; set; }
    public string? Url { get; set; }
    public PdfType Type { get; set; } = PdfType.Sheet;
    public bool IsDownloadedForOffline { get; set; }
    public long? FileSize { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
