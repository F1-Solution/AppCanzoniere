using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;
using static AppCanzoniere.Infrastructure.Tests.PdfImport.CraftedPdf;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

/// <summary>
/// The content of a page is measured, unpacked, before PdfPig parses the page; no encoding or page-tree shape may
/// slip a large page past the limit. The fixtures are small: the limit is lowered instead of the content raised.
/// </summary>
public class PdfContentSizeTests
{
    private const int LimitBytes = 64 * 1024;
    private const int OverLimitBytes = 256 * 1024;
    private const int SmallBytes = 2 * 1024;

    private static readonly PdfExtractionLimits Limits = new() { MaxContentBytesPerPage = LimitBytes };

    private static Task<PdfWordsResult> Extract(byte[] pdf, PdfExtractionLimits? limits = null) =>
        new PdfPigWordExtractor(limits ?? Limits).ExtractAsync(new MemoryStream(pdf));

    // Each encoding PdfPig decodes, applied to the given content.
    private static byte[] Encoded(string encoding, byte[] content) => encoding switch
    {
        "flate twice" => OnePage("/Filter [/FlateDecode /FlateDecode]", Zlib(Zlib(content))),
        "abbreviated flate twice" => OnePage("/Filter [/Fl /Fl]", Zlib(Zlib(content))),
        "hex then flate" => OnePage("/Filter [/ASCIIHexDecode /FlateDecode]", Hex(Zlib(content))),
        "ascii85 then flate" => OnePage("/Filter [/ASCII85Decode /FlateDecode]", Ascii85(Zlib(content))),
        "lzw" => OnePage("/Filter /LZWDecode", Lzw(content)),
        "run length" => OnePage("/Filter /RunLengthDecode", RunLength(content)),
        "indirect filter" => OnePage("/Filter 6 0 R", Zlib(content), extra: new[] { new Obj("/FlateDecode") }),
        "indirect filter array" => OnePage("/Filter 6 0 R", Zlib(Zlib(content)), extra: new[] { new Obj("[/FlateDecode /FlateDecode]") }),
        "zlib header zeroed" => OnePage("/Filter /FlateDecode", ZlibWithoutHeader(content)),
        _ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null),
    };

    public static TheoryData<string> Encodings() => new()
    {
        "flate twice", "abbreviated flate twice", "hex then flate", "ascii85 then flate", "lzw", "run length",
        "indirect filter", "indirect filter array", "zlib header zeroed",
    };

    [Theory]
    [MemberData(nameof(Encodings))]
    public async Task ExtractAsync_ReadsAPageInEveryEncoding_WithinTheLimit(string encoding)
    {
        var result = await Extract(Encoded(encoding, Padded(SmallBytes)));

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Contains(result.Pages[0].Words, w => w.Text == "gioia");
    }

    [Theory]
    [MemberData(nameof(Encodings))]
    public async Task ExtractAsync_RefusesAPageInEveryEncoding_WhenItUnpacksOverTheLimit(string encoding)
    {
        var pdf = Encoded(encoding, Padded(OverLimitBytes));

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
        Assert.Contains($"{LimitBytes}", result.Reason, StringComparison.Ordinal);
        Assert.Empty(result.Pages);
    }

    [Theory]
    [InlineData("/Filter /NoSuchDecode")]
    [InlineData("/Filter [/FlateDecode /NoSuchDecode]")]
    [InlineData("/Filter /DCTDecode")]
    [InlineData("/Filter 6 0 R")]
    public async Task ExtractAsync_RefusesAPage_WhoseFilterCannotBeMeasured(string filter)
    {
        // Content in a filter the measurement does not decode, or a filter entry that is not a name, counts as over the limit.
        var pdf = OnePage(filter, Zlib(Ascii(Text)), extra: new[] { new Obj("<< /Not /AName >>") });

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_RefusesAPage_WhoseCompressedContentIsCorrupt()
    {
        var corrupt = Enumerable.Range(0, 400).Select(i => (byte)(i * 7919 % 251)).ToArray();

        var result = await Extract(OnePage("/Filter /FlateDecode", corrupt));

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_ReadsAPageWhoseContentUsesAPngPredictor()
    {
        // Each row of 64 bytes is prefixed with predictor type 0 ("none"), so the rows unpack to the content unchanged.
        const int Columns = 64;
        var content = Padded(SmallBytes);
        var rows = content.Chunk(Columns).SelectMany(row => row.Prepend((byte)0).Concat(new byte[Columns - row.Length])).ToArray();

        var result = await Extract(OnePage($"/Filter /FlateDecode /DecodeParms << /Predictor 12 /Columns {Columns} >>", Zlib(rows)));

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Contains(result.Pages[0].Words, w => w.Text == "gioia");
    }

    [Fact]
    public async Task ExtractAsync_RefusesAPredictorWhoseRowIsLargerThanTheLimit()
    {
        // A sub-kilobyte file asking for predictor rows of 4 MB: PdfPig allocates the rows before it reads a byte.
        var pdf = OnePage($"/Filter /FlateDecode /DecodeParms << /Predictor 12 /Columns {4 * 1024 * 1024} >>", Zlib(Ascii(Text)));

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_MeasuresTheLastPage_WhenTheTreeHasAnEmptyNodeWithoutKids()
    {
        // PdfPig skips the empty node, so the bomb is its page 2; a walk that numbered the node as a page would miss it.
        var pdf = Write(new[]
        {
            new Obj("<< /Type /Catalog /Pages 2 0 R >>"),
            new Obj("<< /Type /Pages /Kids [4 0 R 5 0 R 6 0 R] /Count 2 >>"),
            new Obj(Font),
            new Obj("<< /Type /Pages /Parent 2 0 R /Count 0 >>"),
            new Obj(Page(7)),
            new Obj(Page(8)),
            new Obj(string.Empty, Ascii(Text)),
            new Obj("/Filter /FlateDecode", Zlib(Padded(OverLimitBytes))),
        });

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_MeasuresAPageLeafThatAlsoListsKids()
    {
        // A /Page node is a page whatever else it holds: PdfPig parses its own content and never visits the kids.
        var pdf = Write(new[]
        {
            new Obj("<< /Type /Catalog /Pages 2 0 R >>"),
            new Obj("<< /Type /Pages /Kids [4 0 R] /Count 1 >>"),
            new Obj(Font),
            new Obj(Page(6, "/Kids [5 0 R] ")),
            new Obj(Page(7)),
            new Obj("/Filter /FlateDecode", Zlib(Padded(OverLimitBytes))),
            new Obj(string.Empty, Ascii(Text)),
        });

        var result = await Extract(pdf);

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }

    [Fact]
    public async Task ExtractAsync_RefusesAnyStreamThatUnpacksOverTheDecodeLimit_EvenOutsideThePageContent()
    {
        // An embedded font program PdfPig unpacks while it reads the page; the page content itself is tiny.
        var widths = string.Join(' ', Enumerable.Repeat(500, 95));
        var font = $"<< /Type /Font /Subtype /TrueType /BaseFont /Bomb /FirstChar 32 /LastChar 126 /Widths [{widths}] /FontDescriptor 6 0 R >>";
        var pdf = Write(new[]
        {
            new Obj("<< /Type /Catalog /Pages 2 0 R >>"),
            new Obj("<< /Type /Pages /Kids [4 0 R] /Count 1 >>"),
            new Obj(font),
            new Obj(Page(5)),
            new Obj(string.Empty, Ascii(Text)),
            new Obj("<< /Type /FontDescriptor /FontName /Bomb /Flags 32 /FontBBox [0 0 1000 1000] /ItalicAngle 0 /Ascent 800 /Descent -200 /CapHeight 700 /StemV 80 /FontFile2 7 0 R >>"),
            new Obj("/Filter /FlateDecode", Zlib(new byte[OverLimitBytes])),
        });

        var result = await Extract(pdf, new PdfExtractionLimits { MaxDecodedStreamBytes = LimitBytes });

        Assert.Equal(PdfExtractionStatus.TooLarge, result.Status);
    }
}
