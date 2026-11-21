using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

/// <summary>The committed PDFs under tests/fixtures/import give the song counts the UI tests rely on.</summary>
public sealed class ImportFixtureTests
{
    private static string Fixture(string name)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "AppCanzoniere.Core.slnx")))
            directory = directory.Parent;

        return Path.Combine(directory!.FullName, "tests", "fixtures", "import", name);
    }

    private static Task<SongbookImportResult> ImportAsync(string name)
    {
        var service = new SongbookImportService(new PdfPigWordExtractor(new PdfExtractionLimits()), new NoRenderer(), new NoEngine());
        return service.ImportAsync(File.OpenRead(Fixture(name)), name);
    }

    private sealed class NoRenderer : IPageImageRenderer
    {
        public Task<IPageImageSession?> OpenAsync(Stream pdf, CancellationToken cancellationToken = default) => Task.FromResult<IPageImageSession?>(null);
    }

    private sealed class NoEngine : IOcrEngine
    {
        public Task<OcrPageResult> RecognizeAsync(OcrPageImage image, CancellationToken cancellationToken = default) =>
            Task.FromResult(new OcrPageResult(1, 1, Array.Empty<OcrLine>()));
    }

    [Fact]
    public async Task Single_song_sheet_gives_one_text_song()
    {
        var result = await ImportAsync("single-song.pdf");

        var song = Assert.Single(result.Songs);
        Assert.Equal(SongOrigin.Text, song.Origin);
    }

    [Fact]
    public async Task Mini_songbook_gives_three_songs()
    {
        var result = await ImportAsync("mini-songbook.pdf");

        Assert.True(result.Songs.Count == 3, string.Join(" | ", result.Songs.Select(s => $"{s.Title} p{s.FirstPage}-{s.LastPage}")));
    }

    [Fact]
    public async Task Damaged_file_fails_without_throwing()
    {
        var result = await ImportAsync("damaged.pdf");

        Assert.Equal(SongbookImportStatus.Failed, result.Status);
    }
}
