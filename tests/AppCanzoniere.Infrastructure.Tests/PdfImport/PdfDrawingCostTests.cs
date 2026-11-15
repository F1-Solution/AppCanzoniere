using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;
using static AppCanzoniere.Infrastructure.Tests.PdfImport.CraftedPdf;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

/// <summary>
/// What a page draws is costed before PdfPig parses it: forms count once per use, nested forms multiply, and the
/// whole document has a budget too. The limits are lowered so that the fixtures stay a few kilobytes.
/// </summary>
public class PdfDrawingCostTests
{
    private const int PageLimitBytes = 16 * 1024;

    // High enough that PdfPig's own letter count, taken after parsing, never refuses these pages.
    private const int ParsedLetterLimit = 1_000_000;

    private static readonly PdfExtractionLimits Limits = new() { MaxContentBytesPerPage = PageLimitBytes, MaxLettersPerPage = ParsedLetterLimit };

    private static Task<PdfWordsResult> Extract(byte[] pdf, PdfExtractionLimits? limits = null) =>
        new PdfPigWordExtractor(limits ?? Limits).ExtractAsync(new MemoryStream(pdf));

    private static string Letters(int count) => $"BT /F1 1 Tf 10 10 Td ({new string('a', count)}) Tj ET\n";

    private static string Draws(string name, int times) => string.Concat(Enumerable.Repeat($"/{name} Do\n", times));

    private static Obj Form(string content, string resources = "<< /Font << /F1 3 0 R >> >>") =>
        new($"/Type /XObject /Subtype /Form /BBox [0 0 595 842] /Resources {resources}", Ascii(content));

    [Fact]
    public async Task ExtractAsync_RefusesAPageThatWouldDrawTooManyLetters_BeforeParsingIt()
    {
        var pdf = OnePage(string.Empty, Ascii(Letters(20_000)));

        var result = await Extract(pdf, new PdfExtractionLimits { MaxLettersPerPage = 5_000 });

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains("would draw more than 5000 characters", result.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExtractAsync_CountsAFormOncePerUse()
    {
        // The form holds 1,000 letters and the page, a few hundred bytes long, draws it 50 times.
        var pdf = OnePage(string.Empty, Ascii(Text + Draws("X0", 50)), "/XObject << /X0 6 0 R >>", new[] { Form(Letters(1_000)) });

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains($"{PageLimitBytes}", result.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExtractAsync_MultipliesTheUsesOfNestedForms()
    {
        // Three levels of forms, each drawing the next five times: 125 copies of 400 letters from a file of a few kilobytes.
        var forms = new[]
        {
            Form(Draws("Y", 5), "<< /XObject << /Y 7 0 R >> >>"),
            Form(Draws("Z", 5), "<< /XObject << /Z 8 0 R >> >>"),
            Form(Letters(400)),
        };
        var pdf = OnePage(string.Empty, Ascii(Text + Draws("X", 5)), "/XObject << /X 6 0 R >>", forms);

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_RefusesFormsThatDrawEachOther()
    {
        // A draws B and B draws A: PdfPig would recurse until the process dies of a stack overflow.
        var forms = new[]
        {
            Form("/B Do\n", "<< /XObject << /B 7 0 R >> >>"),
            Form("/A Do\n", "<< /XObject << /A 6 0 R >> >>"),
        };
        var pdf = OnePage(string.Empty, Ascii(Text + "/A Do\n"), "/XObject << /A 6 0 R >>", forms);

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_ReadsAPageThatDrawsAFormAFewTimes()
    {
        // Each use is moved down 30 points, so the three copies land on three rows.
        var draws = string.Concat(Enumerable.Range(0, 3).Select(k => $"q 1 0 0 1 0 {-30 * k} cm /X0 Do Q\n"));
        var pdf = OnePage(string.Empty, Ascii(draws), "/XObject << /X0 6 0 R >>", new[] { Form(Text) });

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Equal(3, result.Pages[0].Words.Count(w => w.Text == "gioia"));
    }

    [Fact]
    public async Task ExtractAsync_ReadsNestedFormsThatReuseOneName()
    {
        // Each level calls its child /X, as generated files often do: the nearest resources decide, so this is no cycle.
        var forms = new[]
        {
            Form("/X Do\n", "<< /XObject << /X 7 0 R >> >>"),
            Form(Text),
        };
        var pdf = OnePage(string.Empty, Ascii("/X Do\n"), "/XObject << /X 6 0 R >>", forms);

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Single(result.Pages[0].Words, w => w.Text == "gioia");
    }

    [Fact]
    public async Task ExtractAsync_ReadsAFormThatCallsItself_AsPdfPigDropsThatCall()
    {
        var forms = new[] { Form(Text + "/X Do\n", "<< /Font << /F1 3 0 R >> /XObject << /X 6 0 R >> >>") };
        var pdf = OnePage(string.Empty, Ascii("/X Do\n"), "/XObject << /X 6 0 R >>", forms);

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Single(result.Pages[0].Words, w => w.Text == "gioia");
    }

    [Fact]
    public async Task ExtractAsync_DoesNotCountImagesAsDrawnContent()
    {
        // An image is drawn 2,000 times; PdfPig does not unpack images to read the text, so they cost nothing here.
        var image = new Obj("/Type /XObject /Subtype /Image /Width 1 /Height 1 /ColorSpace /DeviceGray /BitsPerComponent 8", new byte[] { 0 });
        var pdf = OnePage(string.Empty, Ascii(Text + Draws("Im", 2_000)), "/XObject << /Im 6 0 R >>", new[] { image });

        var result = await Extract(pdf, Limits with { MaxContentBytesPerPage = 64 * 1024 });

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.True(result.Pages[0].HasImages);
    }

    // A chain of page-tree nodes, each with its own resources, above a single page that draws the given content.
    private static byte[] ResourceChain(int nodes, byte[] content)
    {
        // Objects: 1 catalog, 2 content, 3 font, 4 to 3 + nodes the chain (the font in the top node), then the page.
        var objects = new List<Obj> { new("<< /Type /Catalog /Pages 4 0 R >>"), new("/Filter /FlateDecode", Zlib(content)), new(Font) };
        for (var i = 0; i < nodes; i++)
        {
            var resources = i == 0 ? "/Font << /F1 3 0 R >>" : "/ProcSet [/PDF]";
            objects.Add(new Obj($"<< /Type /Pages /Kids [{5 + i} 0 R] /Count 1 /Resources << {resources} >> >>"));
        }

        objects.Add(new Obj("<< /Type /Page /MediaBox [0 0 595 842] /Resources << /ProcSet [/PDF] >> /Contents 2 0 R >>"));
        return Write(objects);
    }

    [Fact]
    public async Task ExtractAsync_RefusesAPage_ThatInheritsMoreResourceDictionariesThanTheLimit()
    {
        // Forty ancestors with resources; real page trees pass on one or two, and the cost walk scans all of them per name.
        var result = await Extract(ResourceChain(40, Ascii(Text)));

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains("32", result.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExtractAsync_ReadsAPage_ThatInheritsAFewResourceDictionaries()
    {
        var result = await Extract(ResourceChain(8, Ascii(Text)));

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Contains(result.Pages[0].Words, w => w.Text == "gioia");
    }

    [Fact]
    public async Task ExtractAsync_RefusesADocument_ThatCallsMoreDrawingsByNameThanTheLimit()
    {
        // Four pages share one content calling 30,000 different names: each is costed, 120,000 lookups in all.
        const int Pages = 4;
        var calls = string.Concat(Enumerable.Range(0, 30_000).Select(i => $"/a{i} Do\n"));
        var objects = new List<Obj>
        {
            new("<< /Type /Catalog /Pages 2 0 R >>"),
            new($"<< /Type /Pages /Kids [{string.Join(' ', Enumerable.Range(5, Pages).Select(n => $"{n} 0 R"))}] /Count {Pages} >>"),
            new(Font),
            new("/Filter /FlateDecode", Zlib(Ascii(Text + calls))),
        };
        objects.AddRange(Enumerable.Range(0, Pages).Select(_ => new Obj(Page(4))));

        var result = await Extract(Write(objects), new PdfExtractionLimits());

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains("100000", result.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExtractAsync_RefusesADocumentOverTheWholeContentBudget()
    {
        // Forty pages share one content stream of 2 KB: each page is small, together they are not.
        const int Pages = 40;
        var objects = new List<Obj>
        {
            new("<< /Type /Catalog /Pages 2 0 R >>"),
            new($"<< /Type /Pages /Kids [{string.Join(' ', Enumerable.Range(5, Pages).Select(n => $"{n} 0 R"))}] /Count {Pages} >>"),
            new(Font),
            new(string.Empty, Padded(2 * 1024)),
        };
        objects.AddRange(Enumerable.Range(0, Pages).Select(_ => new Obj(Page(4))));

        var result = await Extract(Write(objects), Limits with { MaxContentBytesPerDocument = 32 * 1024 });

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains("32768", result.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExtractAsync_RefusesADocumentOverTheWholeLetterBudget()
    {
        var result = await Extract(PdfFixtures.ManyPages(5), new PdfExtractionLimits { MaxLettersPerDocument = 20 });

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains("20", result.Reason, StringComparison.Ordinal);
        Assert.Empty(result.Pages);
    }

    [Fact]
    public async Task ExtractAsync_RefusesByDefaultAPageOfTwoMegabytesOfContent()
    {
        // The largest page of the songbook corpus holds 215 KB of content; 2 MB is far beyond any real page.
        var result = await new PdfPigWordExtractor().ExtractAsync(new MemoryStream(PdfFixtures.CompressedPage(2 * 1024 * 1024)));

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_RefusesByDefaultADocumentOfMoreThanAMillionLetters()
    {
        // Eleven pages share one stream of 96,000 letters; the corpus's largest document has 334,000.
        const int Pages = 11;
        var objects = new List<Obj>
        {
            new("<< /Type /Catalog /Pages 2 0 R >>"),
            new($"<< /Type /Pages /Kids [{string.Join(' ', Enumerable.Range(5, Pages).Select(n => $"{n} 0 R"))}] /Count {Pages} >>"),
            new(Font),
            new("/Filter /FlateDecode", Zlib(Ascii(Letters(96_000)))),
        };
        objects.AddRange(Enumerable.Range(0, Pages).Select(_ => new Obj(Page(4))));

        var result = await new PdfPigWordExtractor().ExtractAsync(new MemoryStream(Write(objects)));

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public void Limits_RejectNonPositiveDocumentBudgets()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfPigWordExtractor(new PdfExtractionLimits { MaxContentBytesPerDocument = 0 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfPigWordExtractor(new PdfExtractionLimits { MaxLettersPerDocument = 0 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfPigWordExtractor(new PdfExtractionLimits { MaxDecodedStreamBytes = 0 }));
    }
}
