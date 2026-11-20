using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

/// <summary>The import service on top of the in-process PdfPig reader, the way the Android app wires it.</summary>
public sealed class SongbookImportWithPdfPigTests
{
    private sealed class RecordingRenderer : IPageImageRenderer
    {
        public List<int> Rendered { get; } = new();

        public Task<IPageImageSession?> OpenAsync(Stream pdf, CancellationToken cancellationToken = default) =>
            Task.FromResult<IPageImageSession?>(new Session(Rendered));

        private sealed class Session(List<int> rendered) : IPageImageSession
        {
            public int PageCount => 2;

            public Task<OcrPageImage?> RenderPageAsync(int pageNumber, CancellationToken cancellationToken = default)
            {
                rendered.Add(pageNumber);
                return Task.FromResult<OcrPageImage?>(new OcrPageImage(new byte[] { 1 }, 100, 100));
            }

            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }

    private sealed class EmptyEngine : IOcrEngine
    {
        public Task<OcrPageResult> RecognizeAsync(OcrPageImage image, CancellationToken cancellationToken = default) =>
            Task.FromResult(new OcrPageResult(100, 100, Array.Empty<OcrLine>()));
    }

    private static SongbookImportService Service(RecordingRenderer renderer) =>
        new(new PdfPigWordExtractor(new PdfExtractionLimits()), renderer, new EmptyEngine());

    [Fact]
    public async Task Scanned_page_after_a_text_page_is_rendered_and_reported_unread_when_ocr_finds_nothing()
    {
        var renderer = new RecordingRenderer();

        var result = await Service(renderer).ImportAsync(new MemoryStream(PdfFixtures.TextThenScan()), "book.pdf");

        Assert.Equal(new[] { 2 }, renderer.Rendered);
        Assert.Equal(new[] { 2 }, result.ScannedPages);
        Assert.Equal(1, result.TextPageCount);
        Assert.Equal(2, result.UnreadPages.Single().Page);
    }

    [Fact]
    public async Task Text_only_pdf_never_opens_the_renderer()
    {
        var renderer = new RecordingRenderer();

        var result = await Service(renderer).ImportAsync(new MemoryStream(PdfFixtures.ManyPages(2)), "book.pdf");

        Assert.Empty(renderer.Rendered);
        Assert.Empty(result.ScannedPages);
    }

    [Fact]
    public async Task Encrypted_pdf_is_reported_as_protected()
    {
        var result = await Service(new RecordingRenderer()).ImportAsync(new MemoryStream(PdfFixtures.Encrypted()), "book.pdf");

        Assert.Equal(SongbookImportFailure.ProtectedOrDamaged, result.Failure);
    }

    [Fact]
    public async Task Garbage_is_reported_as_damaged_not_thrown()
    {
        var result = await Service(new RecordingRenderer()).ImportAsync(new MemoryStream(new byte[] { 1, 2, 3, 4 }), "x.pdf");

        Assert.Equal(SongbookImportStatus.Failed, result.Status);
        Assert.Equal(SongbookImportFailure.ProtectedOrDamaged, result.Failure);
    }
}
