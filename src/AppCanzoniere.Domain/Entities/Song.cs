using AppCanzoniere.Domain.Enums;

namespace AppCanzoniere.Domain.Entities;

public class Song
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public SongType Type { get; set; }
    
    // Content - multiple formats supported
    public string? ChordProContent { get; set; }
    public string? MusicXmlContent { get; set; }
    public string? JsonContent { get; set; }
    public ContentFormat PrimaryFormat { get; set; } = ContentFormat.ChordPro;
    public string OriginalKey { get; set; } = "C";
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    
    // Privacy and sharing
    public bool IsPublic { get; set; }
    public CopyrightStatus CopyrightStatus { get; set; }
    
    // Sync
    public int Version { get; set; } = 1;
    public bool IsSynced { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    
    // Offline
    public bool IsDownloadedForOffline { get; set; }
    
    // Navigation properties
    public List<AlbumSong> AlbumSongs { get; set; } = new();
    public List<SongTag> SongTags { get; set; } = new();
    public List<SongLiturgicalMoment> SongLiturgicalMoments { get; set; } = new();
    public List<BibleReference> BibleReferences { get; set; } = new();
    public List<YouTubeLink> YouTubeLinks { get; set; } = new();
    public List<Mp3Attachment> Mp3Attachments { get; set; } = new();
    public List<PdfAttachment> PdfAttachments { get; set; } = new();
}
