using System;
using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace AppCanzoniere.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.2");

            // Song Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.Song", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Artist")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("ChordProContent")
                        .HasColumnType("TEXT");

                    b.Property<int>("CopyrightStatus")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDownloadedForOffline")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSynced")
                        .HasColumnType("INTEGER");

                    b.Property<string>("JsonContent")
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastSyncedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("MusicXmlContent")
                        .HasColumnType("TEXT");

                    b.Property<string>("OriginalKey")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<int>("PrimaryFormat")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");
                    b.HasIndex("Artist");
                    b.HasIndex("IsPublic");
                    b.HasIndex("Title");

                    b.ToTable("Songs");
                });

            // UserSettings Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.UserSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ChordNotation")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("CloudProvider")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EnableAIFeatures")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EnableCloudSync")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PreferredInstrument")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PreferredLanguage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("ShowSharpInsteadOfFlat")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ThemeMode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");
                    b.ToTable("UserSettings");
                });

            // Album Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.Album", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CoverImagePath")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");
                    b.HasIndex("Name");
                    b.ToTable("Albums");
                });

            // AlbumSong Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.AlbumSong", b =>
                {
                    b.Property<Guid>("AlbumId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SongId")
                        .HasColumnType("TEXT");

                    b.Property<int>("OrderIndex")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AddedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("AlbumId", "SongId");
                    b.HasIndex("SongId");
                    b.ToTable("AlbumSongs");
                });

            // Tag Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Category")
                        .HasColumnType("TEXT");

                    b.Property<string>("Color")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");
                    b.HasIndex("Name").IsUnique();
                    b.ToTable("Tags");
                });

            // SongTag Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.SongTag", b =>
                {
                    b.Property<Guid>("SongId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TagId")
                        .HasColumnType("TEXT");

                    b.HasKey("SongId", "TagId");
                    b.HasIndex("TagId");
                    b.ToTable("SongTags");
                });

            // LiturgicalMoment Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.LiturgicalMoment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("NameKey")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");
                    b.HasIndex("NameKey").IsUnique();
                    b.ToTable("LiturgicalMoments");
                });

            // SongLiturgicalMoment Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.SongLiturgicalMoment", b =>
                {
                    b.Property<Guid>("SongId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("LiturgicalMomentId")
                        .HasColumnType("TEXT");

                    b.HasKey("SongId", "LiturgicalMomentId");
                    b.HasIndex("LiturgicalMomentId");
                    b.ToTable("SongLiturgicalMoments");
                });

            // BibleReference Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.BibleReference", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SongId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Book")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<int?>("Chapter")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("VerseStart")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("VerseEnd")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DisplayText")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");
                    b.HasIndex("SongId");
                    b.ToTable("BibleReferences");
                });

            // YouTubeLink Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.YouTubeLink", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SongId")
                        .HasColumnType("TEXT");

                    b.Property<string>("VideoId")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<string>("ThumbnailUrl")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("AddedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");
                    b.HasIndex("SongId");
                    b.ToTable("YouTubeLinks");
                });

            // Mp3Attachment Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.Mp3Attachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SongId")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("LocalPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDownloadedForOffline")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("FileSize")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AddedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");
                    b.HasIndex("SongId");
                    b.ToTable("Mp3Attachments");
                });

            // PdfAttachment Entity
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.PdfAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SongId")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("LocalPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDownloadedForOffline")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("FileSize")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AddedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");
                    b.HasIndex("SongId");
                    b.ToTable("PdfAttachments");
                });

            // Navigation Properties
            modelBuilder.Entity("AppCanzoniere.Domain.Entities.AlbumSong", b =>
                {
                    b.HasOne("AppCanzoniere.Domain.Entities.Album", "Album")
                        .WithMany("AlbumSongs")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppCanzoniere.Domain.Entities.Song", "Song")
                        .WithMany("AlbumSongs")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");
                    b.Navigation("Song");
                });

            modelBuilder.Entity("AppCanzoniere.Domain.Entities.SongTag", b =>
                {
                    b.HasOne("AppCanzoniere.Domain.Entities.Song", "Song")
                        .WithMany("SongTags")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppCanzoniere.Domain.Entities.Tag", "Tag")
                        .WithMany("SongTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Song");
                    b.Navigation("Tag");
                });

            modelBuilder.Entity("AppCanzoniere.Domain.Entities.SongLiturgicalMoment", b =>
                {
                    b.HasOne("AppCanzoniere.Domain.Entities.Song", "Song")
                        .WithMany("SongLiturgicalMoments")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppCanzoniere.Domain.Entities.LiturgicalMoment", "LiturgicalMoment")
                        .WithMany("SongLiturgicalMoments")
                        .HasForeignKey("LiturgicalMomentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Song");
                    b.Navigation("LiturgicalMoment");
                });

            modelBuilder.Entity("AppCanzoniere.Domain.Entities.BibleReference", b =>
                {
                    b.HasOne("AppCanzoniere.Domain.Entities.Song", "Song")
                        .WithMany("BibleReferences")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Song");
                });

            modelBuilder.Entity("AppCanzoniere.Domain.Entities.YouTubeLink", b =>
                {
                    b.HasOne("AppCanzoniere.Domain.Entities.Song", "Song")
                        .WithMany("YouTubeLinks")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Song");
                });

            modelBuilder.Entity("AppCanzoniere.Domain.Entities.Mp3Attachment", b =>
                {
                    b.HasOne("AppCanzoniere.Domain.Entities.Song", "Song")
                        .WithMany("Mp3Attachments")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Song");
                });

            modelBuilder.Entity("AppCanzoniere.Domain.Entities.PdfAttachment", b =>
                {
                    b.HasOne("AppCanzoniere.Domain.Entities.Song", "Song")
                        .WithMany("PdfAttachments")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Song");
                });

            modelBuilder.Entity("AppCanzoniere.Domain.Entities.Album", b =>
                {
                    b.Navigation("AlbumSongs");
                });

            modelBuilder.Entity("AppCanzoniere.Domain.Entities.Song", b =>
                {
                    b.Navigation("AlbumSongs");
                    b.Navigation("SongTags");
                    b.Navigation("SongLiturgicalMoments");
                    b.Navigation("BibleReferences");
                    b.Navigation("YouTubeLinks");
                    b.Navigation("Mp3Attachments");
                    b.Navigation("PdfAttachments");
                });

            modelBuilder.Entity("AppCanzoniere.Domain.Entities.Tag", b =>
                {
                    b.Navigation("SongTags");
                });

            modelBuilder.Entity("AppCanzoniere.Domain.Entities.LiturgicalMoment", b =>
                {
                    b.Navigation("SongLiturgicalMoments");
                });
#pragma warning restore 612, 618
        }
    }
}
