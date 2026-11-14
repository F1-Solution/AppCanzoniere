using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

public class PdfPigWordExtractorTests
{
    private static Task<PdfWordsResult> Extract(byte[] pdf, PdfExtractionLimits? limits = null, CancellationToken token = default) =>
        new PdfPigWordExtractor(limits).ExtractAsync(new MemoryStream(pdf), token);

    private static async Task<PdfTextResult> Import(byte[] pdf) =>
        await new PdfTextImporter(new PdfPigWordExtractor()).ImportAsync(new MemoryStream(pdf));

    private static string[] Lines(PageText page) => page.Text.Split('\n');

    [Fact]
    public async Task ExtractAsync_ReturnsEveryWordWithItsBox()
    {
        var result = await Extract(PdfFixtures.ManyPages(2));

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Equal(new[] { 1, 2 }, result.Pages.Select(p => p.PageNumber));
        var words = result.Pages[1].Words;
        Assert.Equal(new[] { "pagina", "2" }, words.Select(w => w.Text));
        Assert.Equal(50, words[0].Box.Left, 1);
        Assert.Equal(700, words[0].Box.Bottom, 1);
        Assert.True(words[1].Box.Left > words[0].Box.Right);
        Assert.Equal(595, result.Pages[0].Width, 0);
    }

    [Fact]
    public async Task ExtractAsync_GivesEachWordTheLeftEdgeOfEveryLetter()
    {
        var result = await Extract(PdfFixtures.TwoColumns());

        var word = result.Pages[0].Words.First(w => w.Text == "Cammina");
        Assert.NotNull(word.LetterLefts);
        Assert.Equal(word.Text.Length, word.LetterLefts!.Count);
        Assert.Equal(word.Box.Left, word.LetterLefts[0], 1);
        Assert.True(word.LetterLefts.Zip(word.LetterLefts.Skip(1)).All(p => p.Second > p.First));
        Assert.True(word.LetterLefts[^1] < word.Box.Right);

        // In Helvetica "m" is far wider than "i", so the letters are not evenly spaced.
        var widths = word.LetterLefts.Zip(word.LetterLefts.Skip(1), (a, b) => b - a).ToList();
        Assert.True(widths[2] > 2 * widths[4], string.Join(", ", widths));
    }

    [Fact]
    public async Task Import_ReadsTwoColumns_LeftColumnFirst_WithChordsOverTheirSyllables()
    {
        var result = await Import(PdfFixtures.TwoColumns());

        var page = Assert.Single(result.Pages);
        Assert.Equal(2, page.ColumnCount);
        var lines = Lines(page);
        var left = Array.IndexOf(lines, "Cammina con me per la strada");
        var right = Array.IndexOf(lines, "Vento che soffia sul mare");
        Assert.True(left > 0 && right > left, page.Text);
        Assert.DoesNotContain(lines, l => l.Contains("Cammina", StringComparison.Ordinal) && l.Contains("Vento", StringComparison.Ordinal));
        Assert.Equal(lines[left].IndexOf("strada", StringComparison.Ordinal), lines[left - 1].IndexOf("SOL", StringComparison.Ordinal));
        Assert.Equal(lines[right].IndexOf("mare", StringComparison.Ordinal), lines[right - 1].IndexOf("LA", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Import_PutsAFullWidthTitleBeforeBothColumns()
    {
        var result = await Import(PdfFixtures.TwoColumns(withTitle: true));

        var page = Assert.Single(result.Pages);
        Assert.Equal(new[] { (0, 1), (0, 2), (1, 2) }, page.Blocks.Select(b => (b.ColumnIndex, b.ColumnCount)));
        Assert.StartsWith("UN TITOLO", Lines(page)[0], StringComparison.Ordinal);
    }

    [Fact]
    public async Task Import_KeepsOneColumn_WhenChordsFarApartLeaveAGapInTheMiddle()
    {
        var result = await Import(PdfFixtures.OneColumnWithWideChordGaps());

        var page = Assert.Single(result.Pages);
        Assert.Equal(1, page.ColumnCount);
        var lines = Lines(page);
        var lyric = Array.FindIndex(lines, l => l.StartsWith("Questa lunga strada", StringComparison.Ordinal));
        Assert.Equal(lines[lyric].IndexOf("giorno", StringComparison.Ordinal), lines[lyric - 1].IndexOf("MI7", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ExtractAsync_ReportsAScan_AsNoTextLayer_ListingItsPagesForOcr()
    {
        var result = await Extract(PdfFixtures.ScannedPages(2));

        Assert.Equal(PdfExtractionStatus.NoTextLayer, result.Status);
        Assert.Equal(2, result.Pages.Count);
        Assert.All(result.Pages, p => Assert.True(p.HasImages && !p.HasText));
        Assert.False(string.IsNullOrWhiteSpace(result.Reason));
    }

    [Fact]
    public async Task ExtractAsync_ReportsABlankDocument_AsNoTextLayer()
    {
        var result = await Extract(PdfFixtures.BlankPage());

        Assert.Equal(PdfExtractionStatus.NoTextLayer, result.Status);
        Assert.False(Assert.Single(result.Pages).HasImages);
    }

    [Fact]
    public async Task ExtractAsync_Succeeds_WhenOnlySomePagesAreScanned()
    {
        var result = await Extract(PdfFixtures.TextThenScan());

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.True(result.Pages[0].HasText);
        Assert.True(result.Pages[1].HasImages && !result.Pages[1].HasText);
    }

    [Fact]
    public async Task ExtractAsync_ReportsAnEmptyFile_AsEmpty()
    {
        var result = await Extract(Array.Empty<byte>());

        Assert.Equal(PdfExtractionStatus.Empty, result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.Reason));
    }

    [Theory]
    [InlineData("random")]
    [InlineData("truncated")]
    [InlineData("header")]
    public async Task ExtractAsync_ReportsUnreadableInput_AsDamaged_WithAReason(string kind)
    {
        var valid = PdfFixtures.ManyPages(3);
        var input = kind switch
        {
            "random" => Enumerable.Range(0, 5000).Select(i => (byte)(i * 7919 % 251)).ToArray(),
            "truncated" => valid.Take(valid.Length / 2).ToArray(),
            _ => System.Text.Encoding.ASCII.GetBytes("%PDF-1.7\n%%EOF\n"),
        };

        var result = await Extract(input);

        Assert.Equal(PdfExtractionStatus.Damaged, result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.Reason));
        Assert.Empty(result.Pages);
    }

    [Theory]
    [InlineData(1e7, 842)]
    [InlineData(595, 1e7)]
    public async Task ExtractAsync_ReportsAPageLargerThanPdfAllows_AsDamaged(double width, double height)
    {
        // The PDF format limits a page side to 14,400 units; a bigger page box is crafted or broken.
        var result = await Extract(PdfFixtures.PageOfSize(width, height));

        Assert.Equal(PdfExtractionStatus.Damaged, result.Status);
        Assert.Empty(result.Pages);
    }

    [Fact]
    public async Task ExtractAsync_AcceptsALargeButValidPage()
    {
        var result = await Extract(PdfFixtures.PageOfSize(14400, 14400));

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_ReportsAPasswordProtectedFile_AsEncrypted()
    {
        var result = await Extract(PdfFixtures.Encrypted());

        Assert.Equal(PdfExtractionStatus.Encrypted, result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.Reason));
    }

    [Fact]
    public async Task ExtractAsync_RefusesAFileOverTheSizeLimit_WithoutParsingIt()
    {
        var result = await Extract(PdfFixtures.ManyPages(1), new PdfExtractionLimits { MaxBytes = 100 });

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains("100", result.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExtractAsync_RefusesAFileOverTheSizeLimit_FromAStreamWithoutLength()
    {
        var stream = new UnseekableStream(PdfFixtures.ManyPages(1));

        var result = await new PdfPigWordExtractor(new PdfExtractionLimits { MaxBytes = 100 }).ExtractAsync(stream);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_RefusesADocumentOverThePageLimit()
    {
        var result = await Extract(PdfFixtures.ManyPages(5), new PdfExtractionLimits { MaxPages = 4 });

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains("5", result.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExtractAsync_StopsWithTimedOut_WhenTheTimeLimitIsUsedUp()
    {
        var result = await Extract(PdfFixtures.ManyPages(3), new PdfExtractionLimits { MaxDuration = TimeSpan.Zero });

        Assert.Equal(PdfExtractionStatus.TimedOut, result.Status);
        Assert.Empty(result.Pages);
    }

    [Fact]
    public async Task ExtractAsync_Throws_WhenTheCallerCancels()
    {
        using var source = new CancellationTokenSource();
        await source.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Extract(PdfFixtures.ManyPages(3), token: source.Token));
    }

    [Fact]
    public async Task ExtractAsync_ReadsAStreamWithoutLength()
    {
        var result = await new PdfPigWordExtractor().ExtractAsync(new UnseekableStream(PdfFixtures.ManyPages(2)));

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Equal(2, result.Pages.Count);
    }

    [Fact]
    public async Task ExtractAsync_RejectsANullStream()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new PdfPigWordExtractor().ExtractAsync(null!));
    }

    [Fact]
    public void Limits_RejectNonPositiveValues()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfPigWordExtractor(new PdfExtractionLimits { MaxBytes = 0 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfPigWordExtractor(new PdfExtractionLimits { MaxPages = 0 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfPigWordExtractor(new PdfExtractionLimits { MaxDuration = TimeSpan.FromSeconds(-1) }));
    }

    private sealed class UnseekableStream : MemoryStream
    {
        public UnseekableStream(byte[] bytes)
            : base(bytes)
        {
        }

        public override bool CanSeek => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin loc) => throw new NotSupportedException();
    }
}
