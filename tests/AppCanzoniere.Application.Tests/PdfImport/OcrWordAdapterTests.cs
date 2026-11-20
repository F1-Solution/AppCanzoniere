using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public class OcrWordAdapterTests
{
    private const int Width = 1190;
    private const int Height = 1684;

    private static OcrWord W(string text, double left, double top, double width, double height = 20) => new(text, left, top, width, height);

    private static OcrPageResult Page(params OcrWord[] words) => new(Width, Height, new[] { new OcrLine(words) });

    private sealed class FakeEngine(OcrPageResult result) : IOcrEngine
    {
        public Task<OcrPageResult> RecognizeAsync(OcrPageImage image, CancellationToken cancellationToken = default) => Task.FromResult(result);
    }

    [Fact]
    public async Task Lay_KeepsEachChordOverItsLyricColumn_WhenWordsComeFromAnEngine()
    {
        var engine = new FakeEngine(new OcrPageResult(Width, Height, new[]
        {
            new OcrLine(new[] { W("DO", 100, 400, 40), W("SOL", 360, 400, 60) }),
            new OcrLine(new[] { W("canto", 100, 430, 100), W("la", 220, 430, 30), W("mia", 270, 430, 70), W("gioia", 360, 430, 100) }),
        }));

        var recognised = await engine.RecognizeAsync(new OcrPageImage(Array.Empty<byte>(), Width, Height));
        var lines = PdfPageLayout.Lay(OcrWordAdapter.ToPageWords(recognised, 1).Page).Text.Split('\n');

        Assert.Equal(2, lines.Length);
        Assert.Equal(lines[1].IndexOf("canto", StringComparison.Ordinal), lines[0].IndexOf("DO", StringComparison.Ordinal));
        Assert.Equal(lines[1].IndexOf("gioia", StringComparison.Ordinal), lines[0].IndexOf("SOL", StringComparison.Ordinal));
    }

    [Fact]
    public void ToPageWords_FlipsYSoTheUpperLineIsHigher()
    {
        var result = OcrWordAdapter.ToPageWords(new OcrPageResult(Width, Height, new[]
        {
            new OcrLine(new[] { W("alto", 100, 100, 80) }),
            new OcrLine(new[] { W("basso", 100, 900, 80) }),
        }), 3);

        Assert.Equal(3, result.Page.PageNumber);
        Assert.True(result.Page.Words[0].Box.Bottom > result.Page.Words[1].Box.Bottom);
        Assert.True(result.Page.HasImages);
        Assert.Equal(OcrWordAdapter.PageWidthPoints, result.Page.Width);
    }

    [Fact]
    public void ToPageWords_ReportsTheFailure_WhenTheEngineCouldNotReadThePage()
    {
        var failed = OcrPageResult.Failure("model missing", "it");

        var result = OcrWordAdapter.ToPageWords(failed, 2);

        Assert.True(failed.IsFailure);
        Assert.Equal(OcrPageStatus.Failed, failed.Status);
        Assert.True(result.IsFailure);
        Assert.Equal("model missing", result.FailureReason);
        Assert.Empty(result.Page.Words);
    }

    [Fact]
    public void ToPageWords_DoesNotReportAFailure_WhenThePageIsSimplyBlank()
    {
        var blank = new OcrPageResult(Width, Height, Array.Empty<OcrLine>());

        Assert.False(blank.IsFailure);
        Assert.False(OcrWordAdapter.ToPageWords(blank, 1).IsFailure);
    }

    [Fact]
    public void Failure_GivesAReasonEvenWhenNoneWasSupplied()
    {
        Assert.False(string.IsNullOrWhiteSpace(OcrPageResult.Failure(" ").FailureReason));
    }

    [Fact]
    public void ToPageWords_ReturnsNoText_WhenThePageIsEmpty()
    {
        var result = OcrWordAdapter.ToPageWords(new OcrPageResult(Width, Height, Array.Empty<OcrLine>()), 1);

        Assert.Empty(result.Page.Words);
        Assert.Equal(string.Empty, PdfPageLayout.Lay(result.Page).Text);
    }

    [Theory]
    [InlineData(double.NaN, 10, 10, 10)]
    [InlineData(10, double.PositiveInfinity, 10, 10)]
    [InlineData(10, 10, double.NaN, 10)]
    [InlineData(10, 10, 10, double.NegativeInfinity)]
    [InlineData(10, 10, -5, 10)]
    [InlineData(10, 10, 10, 0)]
    [InlineData(-500, 10, 100, 10)]
    [InlineData(5000, 10, 100, 10)]
    [InlineData(10, 9e9, 100, 10)]
    public void ToPageWords_RejectsTheWord_WhenItsBoxIsUnusable(double left, double top, double width, double height)
    {
        var result = OcrWordAdapter.ToPageWords(Page(new OcrWord("canto", left, top, width, height), W("ok", 100, 100, 40)), 1);

        Assert.Equal(new[] { "ok" }, result.Page.Words.Select(w => w.Text));
        Assert.Equal(1, result.RejectedWords);
    }

    [Fact]
    public void ToPageWords_ClampsABoxThatLeavesThePage()
    {
        var result = OcrWordAdapter.ToPageWords(Page(W("canto", -50, 10, 200), W("fine", Width - 20, 10, 5000)), 1);

        Assert.Equal(2, result.Page.Words.Count);
        Assert.All(result.Page.Words, w =>
        {
            Assert.InRange(w.Box.Left, 0, result.Page.Width);
            Assert.InRange(w.Box.Right, 0, result.Page.Width);
        });
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(100, -1)]
    [InlineData(OcrWordAdapter.MaxSidePixels + 1, 100)]
    public void ToPageWords_ReturnsNoText_WhenThePageSizeIsUnusable(int width, int height)
    {
        var result = OcrWordAdapter.ToPageWords(new OcrPageResult(width, height, new[] { new OcrLine(new[] { W("canto", 1, 1, 10) }) }), 1);

        Assert.Empty(result.Page.Words);
        Assert.Equal(1, result.RejectedWords);
    }

    [Fact]
    public void ToPageWords_RejectsThePage_WhenItHasAbsurdlyManyWords()
    {
        var words = Enumerable.Range(0, OcrWordAdapter.MaxWordsPerPage + 1).Select(i => W("a", i % 1000, i % 1600, 5)).ToArray();

        var result = OcrWordAdapter.ToPageWords(Page(words), 1);

        Assert.True(result.TooManyWords);
        Assert.Empty(result.Page.Words);
    }

    [Fact]
    public void ToPageWords_RejectsBlankAndOverlongText()
    {
        var result = OcrWordAdapter.ToPageWords(Page(W("  ", 10, 10, 20), W(new string('x', OcrWordAdapter.MaxWordLength + 1), 10, 40, 20), W(" Sol ", 10, 70, 30)), 1);

        Assert.Equal(new[] { "Sol" }, result.Page.Words.Select(w => w.Text));
        Assert.Equal(2, result.RejectedWords);
    }
}
