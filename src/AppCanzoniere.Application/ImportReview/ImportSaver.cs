using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;
using AppCanzoniere.Domain.Interfaces;

namespace AppCanzoniere.Application.ImportReview;

/// <summary>Writes an imported song into the library with the rights status unknown.</summary>
public sealed class ImportSaver(ISongRepository songs, IChordProParser parser)
{
    private const string DefaultLanguage = "it";

    /// <summary>Saves the entry as edited and returns the id of the new song.</summary>
    public async Task<Guid> SaveAsync(ImportEntry entry, CancellationToken cancellationToken = default)
    {
        var song = Build(entry);
        var added = await songs.AddAsync(song, cancellationToken).ConfigureAwait(false);
        return added.Id;
    }

    /// <summary>The library song for an entry; the title and key come from the edited ChordPro.</summary>
    public Song Build(ImportEntry entry)
    {
        var document = parser.Parse(entry.ChordPro);
        return new Song
        {
            Title = string.IsNullOrWhiteSpace(document.Title) ? entry.Song.Title : document.Title.Trim(),
            Artist = document.Artist ?? string.Empty,
            Language = DefaultLanguage,
            Type = SongType.Other,
            ChordProContent = entry.ChordPro,
            PrimaryFormat = ContentFormat.ChordPro,
            OriginalKey = string.IsNullOrWhiteSpace(document.Key) ? "C" : document.Key.Trim(),
            CopyrightStatus = CopyrightStatus.Unknown
        };
    }
}
