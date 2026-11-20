using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class SongbookImportServiceTests
{
    private static readonly string[] Lyrics = { "La ^luna sale ^piano", "sopra il ^mare ^scuro" };
    private static readonly string[] Chords = { "Do", "Sol", "La-", "Fa" };

    private sealed class FakeExtractor(PdfWordsResult result) : IPdfWordExtractor
    {
        public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default) => Task.FromResult(result);
    }

    private sealed class FakeRenderer(int missingPage = -1, bool unopenable = false) : IPageImageRenderer
    {
        private readonly int _missingPage = missingPage;
        private readonly bool _unopenable = unopenable;

        public List<int> Rendered { get; } = new();

        public int Opened { get; private set; }

        public Task<IPageImageSession?> OpenAsync(Stream pdf, CancellationToken cancellationToken = default)
        {
            Opened++;
            return Task.FromResult<IPageImageSession?>(_unopenable ? null : new Session(this));
        }

        private sealed class Session(FakeRenderer owner) : IPageImageSession
        {
            public int PageCount => 10;

            public Task<OcrPageImage?> RenderPageAsync(int pageNumber, CancellationToken cancellationToken = default)
            {
                owner.Rendered.Add(pageNumber);
                return Task.FromResult<OcrPageImage?>(pageNumber == owner._missingPage ? null : new OcrPageImage(new byte[] { 1 }, 1000, 1400));
            }

            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }

    private sealed class FakeOcr(Func<int, OcrPageResult> page) : IOcrEngine
    {
        private int _calls;

        public Task<OcrPageResult> RecognizeAsync(OcrPageImage image, CancellationToken cancellationToken = default) =>
            Task.FromResult(page(++_calls));
    }

    private static OcrLine Line(string text, double top) =>
        new(text.Split(' ').Select((w, i) => new OcrWord(w, 100 + (i * 120), top, 100, 30)).ToList());

    private static OcrPageResult ScannedSong() => new(1000, 1400, new[]
    {
        Line("IL VENTO", 100),
        Line("Re La", 300),
        Line("Il vento canta forte", 340),
        Line("Si- Sol", 420),
        Line("sulla collina verde", 460)
    });

    private static PdfPageWords TextSongPage(int number)
    {
        var builder = new PageBuilder();
        builder.Text("LA LUNA", 50, 800);
        builder.Song(Lyrics, Chords, 50, 740);
        return builder.Page(number);
    }

    private static PdfPageWords ScannedPage(int number) => new(number, 595, 842, Array.Empty<PositionedWord>(), HasImages: true);

    private static SongbookImportService Service(PdfWordsResult words, FakeRenderer? renderer = null, IOcrEngine? ocr = null, SongbookImportOptions? options = null) =>
        new(new FakeExtractor(words), renderer ?? new FakeRenderer(), ocr ?? new FakeOcr(_ => ScannedSong()), options);

    private static Task<SongbookImportResult> Run(SongbookImportService service, IProgress<SongbookImportProgress>? progress = null, CancellationToken token = default) =>
        service.ImportAsync(new MemoryStream(new byte[] { 1, 2, 3 }), "book.pdf", progress, token);

    [Fact]
    public async Task Text_pages_are_converted_without_rendering()
    {
        var renderer = new FakeRenderer();

        var result = await Run(Service(PdfWordsResult.Success(new[] { TextSongPage(1) }), renderer));

        Assert.True(result.IsSuccess);
        Assert.Equal("LA LUNA", Assert.Single(result.Songs).Title);
        Assert.Contains("{title:", result.Songs[0].ChordPro);
        Assert.Contains("La luna sale piano", result.Songs[0].SourceText);
        Assert.Equal(SongOrigin.Text, result.Songs[0].Origin);
        Assert.Equal(0, renderer.Opened);
        Assert.Empty(result.ScannedPages);
        Assert.Equal(1, result.TextPageCount);
    }

    [Fact]
    public async Task Scanned_page_is_recognised_and_joins_the_text_page()
    {
        var renderer = new FakeRenderer();
        var words = PdfWordsResult.Success(new[] { TextSongPage(1), ScannedPage(2) });

        var result = await Run(Service(words, renderer));

        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { 2 }, renderer.Rendered);
        Assert.Equal(new[] { 2 }, result.ScannedPages);
        Assert.Contains(result.Songs, s => s.Title == "LA LUNA" && s.Origin == SongOrigin.Text);
        Assert.Contains(result.Songs, s => s.Title == "IL VENTO" && s.Origin == SongOrigin.Scan && s.FirstPage == 2);
    }

    [Fact]
    public async Task Fully_scanned_file_is_read_by_ocr()
    {
        var words = PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "scan", new[] { ScannedPage(1) });

        var result = await Run(Service(words));

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.TextPageCount);
        Assert.Equal("IL VENTO", Assert.Single(result.Songs).Title);
    }

    [Fact]
    public async Task Blank_pages_without_images_are_not_rendered()
    {
        var renderer = new FakeRenderer();
        var blank = new PdfPageWords(2, 595, 842, Array.Empty<PositionedWord>(), HasImages: false);

        await Run(Service(PdfWordsResult.Success(new[] { TextSongPage(1), blank }), renderer));

        Assert.Empty(renderer.Rendered);
    }

    [Fact]
    public async Task Reader_failure_comes_back_typed()
    {
        var result = await Run(Service(PdfWordsResult.Failure(PdfExtractionStatus.Encrypted, "needs a password")));

        Assert.Equal(SongbookImportStatus.Failed, result.Status);
        Assert.Equal(SongbookImportFailure.ProtectedOrDamaged, result.Failure);
        Assert.Equal("needs a password", result.Reason);
    }

    [Fact]
    public async Task Failed_page_is_listed_and_the_rest_still_import()
    {
        var renderer = new FakeRenderer(missingPage: 3);
        var words = PdfWordsResult.Success(new[] { TextSongPage(1), ScannedPage(2), ScannedPage(3) });

        var result = await Run(Service(words, renderer));

        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { 3 }, result.UnreadPages.Select(p => p.Page));
        Assert.Contains("rendered", result.UnreadPages[0].Reason);
    }

    [Fact]
    public async Task Recognition_stops_after_consecutive_failures_and_reports_the_reason()
    {
        var pages = Enumerable.Range(1, 6).Select(ScannedPage).ToList();
        var words = PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "scan", pages);
        var renderer = new FakeRenderer();
        var ocr = new FakeOcr(_ => OcrPageResult.Failure("No OCR language installed"));

        var result = await Run(Service(words, renderer, ocr));

        Assert.Equal(SongbookImportStatus.Failed, result.Status);
        Assert.Contains("No OCR language installed", result.Reason);
        Assert.Equal(3, renderer.Rendered.Count);
        Assert.Equal(6, result.UnreadPages.Count);
        Assert.Equal(SongbookImportFailure.OcrUnavailable, result.Failure);
    }

    [Fact]
    public async Task Unopenable_pdf_fails_all_scanned_pages()
    {
        var words = PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "scan", new[] { ScannedPage(1) });

        var result = await Run(Service(words, new FakeRenderer(unopenable: true)));

        Assert.Equal(SongbookImportStatus.Failed, result.Status);
        Assert.Equal(new[] { 1 }, result.UnreadPages.Select(p => p.Page));
    }

    [Fact]
    public async Task Too_many_scanned_pages_are_refused()
    {
        var words = PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "scan", Enumerable.Range(1, 3).Select(ScannedPage).ToList());

        var result = await Run(Service(words, options: new SongbookImportOptions { MaxScannedPages = 2 }));

        Assert.Equal(SongbookImportFailure.OverLimits, result.Failure);
    }

    [Fact]
    public async Task Oversized_file_is_refused_before_reading()
    {
        var service = Service(PdfWordsResult.Success(new[] { TextSongPage(1) }), options: new SongbookImportOptions { MaxBytes = 2 });

        var result = await Run(service);

        Assert.Equal(SongbookImportFailure.OverLimits, result.Failure);
    }

    [Fact]
    public async Task Empty_file_is_refused()
    {
        var service = Service(PdfWordsResult.Success(Array.Empty<PdfPageWords>()));

        var result = await service.ImportAsync(new MemoryStream(), "x.pdf");

        Assert.Equal(SongbookImportFailure.ProtectedOrDamaged, result.Failure);
    }

    [Fact]
    public async Task Recognition_time_limit_is_reported()
    {
        var time = new ManualTime();
        var words = PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "scan", new[] { ScannedPage(1) });
        var options = new SongbookImportOptions { MaxRecognitionDuration = TimeSpan.FromSeconds(5) };
        var ocr = new FakeOcr(_ =>
        {
            time.Advance(TimeSpan.FromSeconds(6));
            return ScannedSong();
        });
        var service = new SongbookImportService(new FakeExtractor(words), new FakeRenderer(), ocr, options, time);

        var result = await Run(service);

        Assert.Equal(SongbookImportFailure.TimedOut, result.Failure);
    }

    [Fact]
    public async Task Progress_names_each_scanned_page_out_of_the_document()
    {
        var reports = new List<SongbookImportProgress>();
        var words = PdfWordsResult.Success(new[] { TextSongPage(1), ScannedPage(2) });

        await Run(Service(words), new SyncProgress(reports.Add));

        Assert.Contains(new SongbookImportProgress(SongbookImportStage.Recognizing, 2, 2), reports);
        Assert.Contains(reports, r => r.Stage == SongbookImportStage.Converting);
        Assert.Equal(SongbookImportStage.Reading, reports[0].Stage);
    }

    [Fact]
    public async Task Cancellation_throws()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var words = PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "scan", new[] { ScannedPage(1) });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Run(Service(words), token: cts.Token));
    }

    [Fact]
    public async Task Index_entries_are_counted_against_songs_found()
    {
        var result = await Run(Service(PdfWordsResult.Success(new[] { TextSongPage(1) })));

        Assert.Equal(result.IndexEntryCount > 0 && result.IndexEntryCount != 1, result.IndexCountMismatch);
    }

    [Fact]
    public async Task Unavailable_engine_fails_every_page_with_its_reason()
    {
        var words = PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "scan", new[] { ScannedPage(1) });

        var result = await Run(Service(words, ocr: new UnavailableOcrEngine("No OCR language")));

        Assert.Contains("No OCR language", result.Reason);
    }

    [Fact]
    public async Task Scanned_pages_without_words_are_reported_as_low_resolution()
    {
        var words = PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "scan", new[] { ScannedPage(1) });
        var ocr = new FakeOcr(_ => new OcrPageResult(1000, 1400, Array.Empty<OcrLine>()));

        var result = await Run(Service(words, ocr: ocr));

        Assert.Equal(SongbookImportFailure.LowResolution, result.Failure);
        Assert.Equal(1, result.UnreadPages.Single().Page);
    }

    [Fact]
    public async Task Blank_scanned_pages_do_not_stop_recognition()
    {
        var pages = Enumerable.Range(1, 5).Select(ScannedPage).ToList();
        var words = PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "scan", pages);
        var ocr = new FakeOcr(n => n == 5 ? ScannedSong() : new OcrPageResult(1000, 1400, Array.Empty<OcrLine>()));

        var result = await Run(Service(words, ocr: ocr));

        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.UnreadPages.Count);
    }

    [Fact]
    public async Task Pdf_without_any_text_or_images_reports_no_text()
    {
        var blank = new PdfPageWords(1, 595, 842, Array.Empty<PositionedWord>(), HasImages: false);

        var result = await Run(Service(PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, "none", new[] { blank })));

        Assert.Equal(SongbookImportFailure.NoText, result.Failure);
    }

    [Fact]
    public void Doubtful_line_numbers_point_into_the_source_text()
    {
        var song = new SplitSong("T", 1, 1, new[] { "Do        Sol", "La luna sale piano", "xx $$ ?? ##", "" }, Array.Empty<string>());

        var converted = PdfSongConverter.Convert(song);

        Assert.All(converted.DoubtfulLineNumbers, n => Assert.InRange(n, 1, song.Lines.Count));
        Assert.Equal(converted.DoubtfulLineCount, converted.DoubtfulLineNumbers.Count);
    }

    private sealed class SyncProgress(Action<SongbookImportProgress> report) : IProgress<SongbookImportProgress>
    {
        public void Report(SongbookImportProgress value) => report(value);
    }
}
