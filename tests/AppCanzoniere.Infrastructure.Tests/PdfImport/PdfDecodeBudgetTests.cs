using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;
using static AppCanzoniere.Infrastructure.Tests.PdfImport.CraftedPdf;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

/// <summary>
/// PdfPig unpacks some streams again every time they are used (patterns on every form draw, a Type3 glyph program per
/// code), so the bytes unpacked over a whole document are bounded, not only each stream. The budget is lowered so the
/// fixtures stay small; a stream here unpacks to well under the per-stream cap.
/// </summary>
public class PdfDecodeBudgetTests
{
    private const int BudgetBytes = 1024 * 1024;
    private const int StreamBytes = 256 * 1024;

    private static readonly PdfExtractionLimits Limits = new() { MaxDecodedBytesPerDocument = BudgetBytes, MaxDuration = TimeSpan.FromSeconds(10) };

    private static Task<PdfWordsResult> Extract(byte[] pdf, PdfExtractionLimits? limits = null) =>
        new PdfPigWordExtractor(limits ?? Limits).ExtractAsync(new MemoryStream(pdf));

    // A tiling pattern whose content is spaces: harmless, and it packs to almost nothing.
    private static Obj Pattern(int bytes) =>
        new("/PatternType 1 /PaintType 1 /TilingType 1 /BBox [0 0 1 1] /XStep 1 /YStep 1 /Resources << >> /Filter /FlateDecode", Zlib(Ascii(new string(' ', bytes))));

    [Fact]
    public async Task ExtractAsync_RefusesAPage_WhosePatternsUnpackOneStreamOverTheDocumentBudget()
    {
        // Twenty pattern names for one stream of 256 KB: PdfPig unpacks it at least once per name.
        var names = string.Concat(Enumerable.Range(0, 20).Select(i => $"/P{i} 6 0 R "));
        var pdf = OnePage(string.Empty, Ascii(Text), $"/Pattern << {names}>>", new[] { Pattern(StreamBytes) });

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains($"{BudgetBytes}", result.Reason, StringComparison.Ordinal);
        Assert.Empty(result.Pages);
    }

    [Fact]
    public async Task ExtractAsync_StopsPromptly_WhenAFormDrawnThousandsOfTimesUnpacksItsPatternEachTime()
    {
        // 10,000 draws of an empty form whose resources hold a 256 KB pattern: 2.5 GB unpacked without a budget.
        var content = Zlib(Ascii(Text + string.Concat(Enumerable.Repeat("/X Do\n", 10_000))));
        var form = new Obj("/Type /XObject /Subtype /Form /BBox [0 0 1 1] /Resources << /Pattern << /P 7 0 R >> >>", Ascii(" "));
        var pdf = OnePage("/Filter /FlateDecode", content, "/XObject << /X 6 0 R >>", new[] { form, Pattern(StreamBytes) });

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains($"{BudgetBytes}", result.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExtractAsync_RefusesAType3Font_WhoseCodesAllUnpackOneGlyphProgram()
    {
        // Sixty-four codes name one glyph program of 256 KB, which PdfPig unpacks once per code drawn.
        var codes = Enumerable.Range(0, 64).ToArray();
        var differences = "0 " + string.Concat(codes.Select(_ => "/g "));
        var widths = string.Join(' ', codes.Select(_ => 500));
        var font = $"<< /Type /Font /Subtype /Type3 /FontBBox [0 0 1000 1000] /FontMatrix [0.001 0 0 0.001 0 0] /CharProcs << /g 6 0 R >> /Encoding << /Type /Encoding /Differences [{differences}] >> /FirstChar 0 /LastChar 63 /Widths [{widths}] >>";
        var glyph = new System.Text.StringBuilder("1000 0 0 0 750 750 d1\n");
        while (glyph.Length < StreamBytes)
            glyph.Append("0 0 m\n");
        var shown = Convert.ToHexString(codes.Select(c => (byte)c).ToArray());
        var pdf = Write(new[]
        {
            new Obj("<< /Type /Catalog /Pages 2 0 R >>"),
            new Obj("<< /Type /Pages /Kids [4 0 R] /Count 1 >>"),
            new Obj(font),
            new Obj(Page(5)),
            new Obj(string.Empty, Ascii($"BT /F1 11 Tf 50 700 Td <{shown}> Tj ET\n")),
            new Obj("/Filter /FlateDecode", Zlib(Ascii(glyph.ToString()))),
        });

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains($"{BudgetBytes}", result.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExtractAsync_ReadsAPageWithAPattern_WithinTheDocumentBudget()
    {
        // PdfPig unpacks a page's patterns twice while reading it: 512 KB of the 1 MB budget.
        var pdf = OnePage(string.Empty, Ascii(Text), "/Pattern << /P0 6 0 R >>", new[] { Pattern(StreamBytes) });

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Contains(result.Pages[0].Words, w => w.Text == "gioia");
    }

    [Fact]
    public void Limits_RejectANonPositiveDocumentDecodeBudget()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfPigWordExtractor(new PdfExtractionLimits { MaxDecodedBytesPerDocument = 0 }));
    }
}
