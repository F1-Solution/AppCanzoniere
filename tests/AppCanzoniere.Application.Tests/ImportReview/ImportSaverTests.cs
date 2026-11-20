using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.ImportReview;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Application.Services;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;
using AppCanzoniere.Domain.Interfaces;

namespace AppCanzoniere.Application.Tests.ImportReview;

public sealed class ImportSaverTests
{
    private sealed class FakeRepository : ISongRepository
    {
        public List<Song> Added { get; } = new();

        public Task<Song> AddAsync(Song song, CancellationToken cancellationToken = default)
        {
            Added.Add(song);
            return Task.FromResult(song);
        }

        public Task<Song?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IEnumerable<Song>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IEnumerable<Song>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IEnumerable<Song>> GetSongsByTagsAsync(List<Guid> tagIds, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task UpdateAsync(Song song, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private static ImportedSong Imported(string title, string chordPro) => new(
        title, chordPro, "x", 1, 1, Array.Empty<SheetWarning>(), 0, 0, SongOrigin.Text, Array.Empty<int>());

    private static ImportEntry EntryOf(ImportedSong song)
    {
        var session = new ImportSession();
        session.BeginReading("a.pdf");
        session.Complete(new SongbookImportResult(SongbookImportStatus.Success, "", SongbookImportFailure.None, new[] { song }, 1, 1,
            Array.Empty<int>(), Array.Empty<UnreadPage>(), 0, Array.Empty<int>(), null));
        return session.Entries[0];
    }

    [Fact]
    public async Task Saves_edited_text_with_rights_unknown_and_title_from_the_title_line()
    {
        var repo = new FakeRepository();
        var saver = new ImportSaver(repo, new ChordProParser());
        var entry = EntryOf(Imported("Vecchio", "{title: Vecchio}\n[Do]La"));
        entry.GetType().GetProperty(nameof(ImportEntry.ChordPro))!.SetValue(entry, "{title: Nuovo titolo}\n{key: G}\n[Sol]La");

        await saver.SaveAsync(entry);

        var song = Assert.Single(repo.Added);
        Assert.Equal("Nuovo titolo", song.Title);
        Assert.Equal("G", song.OriginalKey);
        Assert.Equal(CopyrightStatus.Unknown, song.CopyrightStatus);
        Assert.Contains("[Sol]La", song.ChordProContent);
    }

    [Fact]
    public void Falls_back_to_the_engine_title_when_the_title_line_is_gone()
    {
        var saver = new ImportSaver(new FakeRepository(), new ChordProParser());
        var entry = EntryOf(Imported("Dal motore", "[Do]La"));

        Assert.Equal("Dal motore", saver.Build(entry).Title);
    }
}
