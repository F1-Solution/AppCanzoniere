using AppCanzoniere.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Song> Songs => Set<Song>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<AlbumSong> AlbumSongs => Set<AlbumSong>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<SongTag> SongTags => Set<SongTag>();
    public DbSet<LiturgicalMoment> LiturgicalMoments => Set<LiturgicalMoment>();
    public DbSet<SongLiturgicalMoment> SongLiturgicalMoments => Set<SongLiturgicalMoment>();
    public DbSet<BibleReference> BibleReferences => Set<BibleReference>();
    public DbSet<YouTubeLink> YouTubeLinks => Set<YouTubeLink>();
    public DbSet<Mp3Attachment> Mp3Attachments => Set<Mp3Attachment>();
    public DbSet<PdfAttachment> PdfAttachments => Set<PdfAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Song configuration
        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Artist).HasMaxLength(200);
            entity.Property(e => e.Language).HasMaxLength(10);
            entity.Property(e => e.OriginalKey).HasMaxLength(10);

            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.Artist);
            entity.HasIndex(e => e.IsPublic);
        });

        // UserSettings configuration
        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // Album configuration
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Name);
        });

        // AlbumSong configuration (many-to-many with payload)
        modelBuilder.Entity<AlbumSong>(entity =>
        {
            entity.HasKey(e => new { e.AlbumId, e.SongId });

            entity.HasOne(e => e.Album)
                .WithMany(a => a.AlbumSongs)
                .HasForeignKey(e => e.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Song)
                .WithMany(s => s.AlbumSongs)
                .HasForeignKey(e => e.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Tag configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // SongTag configuration (many-to-many)
        modelBuilder.Entity<SongTag>(entity =>
        {
            entity.HasKey(e => new { e.SongId, e.TagId });

            entity.HasOne(e => e.Song)
                .WithMany(s => s.SongTags)
                .HasForeignKey(e => e.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tag)
                .WithMany(t => t.SongTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // LiturgicalMoment configuration
        modelBuilder.Entity<LiturgicalMoment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NameKey).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.NameKey).IsUnique();
        });

        // SongLiturgicalMoment configuration (many-to-many)
        modelBuilder.Entity<SongLiturgicalMoment>(entity =>
        {
            entity.HasKey(e => new { e.SongId, e.LiturgicalMomentId });

            entity.HasOne(e => e.Song)
                .WithMany(s => s.SongLiturgicalMoments)
                .HasForeignKey(e => e.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.LiturgicalMoment)
                .WithMany(lm => lm.SongLiturgicalMoments)
                .HasForeignKey(e => e.LiturgicalMomentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // BibleReference configuration
        modelBuilder.Entity<BibleReference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Book).IsRequired().HasMaxLength(10);
            entity.Property(e => e.DisplayText).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Song)
                .WithMany(s => s.BibleReferences)
                .HasForeignKey(e => e.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // YouTubeLink configuration
        modelBuilder.Entity<YouTubeLink>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VideoId).IsRequired().HasMaxLength(20);

            entity.HasOne(e => e.Song)
                .WithMany(s => s.YouTubeLinks)
                .HasForeignKey(e => e.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Mp3Attachment configuration
        modelBuilder.Entity<Mp3Attachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);

            entity.HasOne(e => e.Song)
                .WithMany(s => s.Mp3Attachments)
                .HasForeignKey(e => e.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PdfAttachment configuration
        modelBuilder.Entity<PdfAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);

            entity.HasOne(e => e.Song)
                .WithMany(s => s.PdfAttachments)
                .HasForeignKey(e => e.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
