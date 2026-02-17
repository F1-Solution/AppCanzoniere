using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;

    public DatabaseSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await SeedLiturgicalMomentsAsync();
        await SeedDefaultTagsAsync();
        await SeedSampleSongsAsync();
    }

    private async Task SeedLiturgicalMomentsAsync()
    {
        if (await _context.LiturgicalMoments.AnyAsync())
            return; // Already seeded

        var moments = new[]
        {
            new LiturgicalMoment { Name = "Ingresso", NameKey = "entrance", DisplayOrder = 1 },
            new LiturgicalMoment { Name = "Atto Penitenziale", NameKey = "penitential_act", DisplayOrder = 2 },
            new LiturgicalMoment { Name = "Gloria", NameKey = "gloria", DisplayOrder = 3 },
            new LiturgicalMoment { Name = "Salmo Responsoriale", NameKey = "responsorial_psalm", DisplayOrder = 4 },
            new LiturgicalMoment { Name = "Acclamazione al Vangelo", NameKey = "gospel_acclamation", DisplayOrder = 5 },
            new LiturgicalMoment { Name = "Offertorio", NameKey = "offertory", DisplayOrder = 6 },
            new LiturgicalMoment { Name = "Santo", NameKey = "sanctus", DisplayOrder = 7 },
            new LiturgicalMoment { Name = "Agnello di Dio", NameKey = "agnus_dei", DisplayOrder = 8 },
            new LiturgicalMoment { Name = "Comunione", NameKey = "communion", DisplayOrder = 9 },
            new LiturgicalMoment { Name = "Ringraziamento", NameKey = "thanksgiving", DisplayOrder = 10 },
            new LiturgicalMoment { Name = "Finale", NameKey = "final", DisplayOrder = 11 }
        };

        _context.LiturgicalMoments.AddRange(moments);
        await _context.SaveChangesAsync();
    }

    private async Task SeedDefaultTagsAsync()
    {
        if (await _context.Tags.AnyAsync())
            return; // Already seeded

        var tags = new[]
        {
            // Genre tags
            new Tag { Name = "Adorazione", Category = "Genre", Color = "#FF6B6B" },
            new Tag { Name = "Lode", Category = "Genre", Color = "#4ECDC4" },
            new Tag { Name = "Meditazione", Category = "Genre", Color = "#45B7D1" },
            new Tag { Name = "Preghiera", Category = "Genre", Color = "#96CEB4" },
            
            // Liturgical Season
            new Tag { Name = "Avvento", Category = "Season", Color = "#9B59B6" },
            new Tag { Name = "Natale", Category = "Season", Color = "#E74C3C" },
            new Tag { Name = "Quaresima", Category = "Season", Color = "#95A5A6" },
            new Tag { Name = "Pasqua", Category = "Season", Color = "#F39C12" },
            
            // Special Occasions
            new Tag { Name = "Matrimonio", Category = "Occasion", Color = "#FFC0CB" },
            new Tag { Name = "Funerale", Category = "Occasion", Color = "#2C3E50" },
            new Tag { Name = "Battesimo", Category = "Occasion", Color = "#3498DB" },
            new Tag { Name = "Prima Comunione", Category = "Occasion", Color = "#E8F8F5" },
            
            // Style
            new Tag { Name = "Tradizionale", Category = "Style", Color = "#8E44AD" },
            new Tag { Name = "Moderno", Category = "Style", Color = "#1ABC9C" },
            new Tag { Name = "Gospel", Category = "Style", Color = "#E67E22" },
        };

        _context.Tags.AddRange(tags);
        await _context.SaveChangesAsync();
    }

    private async Task SeedSampleSongsAsync()
    {
        if (await _context.Songs.AnyAsync())
            return; // Already seeded

        var songs = new[]
        {
            new Song
            {
                Title = "Amazing Grace",
                Artist = "John Newton",
                Language = "en",
                Type = Domain.Enums.SongType.ChurchSong,
                OriginalKey = "G",
                PrimaryFormat = Domain.Enums.ContentFormat.ChordPro,
                ChordProContent = @"{title: Amazing Grace}
{artist: John Newton}
{key: G}

{start_of_verse}
[G]Amazing [G/B]grace how [C]sweet the [G]sound
That saved a [Em]wretch like [D]me
[G]I once was [G/B]lost but [C]now I'm [G]found
Was [Em]blind but [D]now I [G]see
{end_of_verse}

{start_of_verse}
'Twas [G]grace that [G/B]taught my [C]heart to [G]fear
And grace my [Em]fears re[D]lieved
How [G]precious [G/B]did that [C]grace ap[G]pear
The [Em]hour I [D]first be[G]lieved
{end_of_verse}",
                CopyrightStatus = Domain.Enums.CopyrightStatus.PublicDomain,
                IsPublic = true
            },
            new Song
            {
                Title = "Sei Tu Signore il Pane",
                Artist = "Tradizionale",
                Language = "it",
                Type = Domain.Enums.SongType.ChurchSong,
                OriginalKey = "D",
                PrimaryFormat = Domain.Enums.ContentFormat.ChordPro,
                ChordProContent = @"{title: Sei Tu Signore il Pane}
{artist: Tradizionale}
{key: D}

{start_of_chorus}
Sei [D]tu, Signore, il [A]pane
Sei [Bm]tu, Signore, il [F#m]vino
Sei [G]tu che ci [D]fai [Em]uno con [A]Te
{end_of_chorus}

{start_of_verse}
[D]Nella [A]notte del tra[Bm]dimento
[F#m]Spezzò il [G]pane fra le [D]mani
[Em]Questo è il [A]corpo [D]mio
{end_of_verse}",
                CopyrightStatus = Domain.Enums.CopyrightStatus.Unknown,
                IsPublic = true
            }
        };

        _context.Songs.AddRange(songs);
        await _context.SaveChangesAsync();
    }
}
