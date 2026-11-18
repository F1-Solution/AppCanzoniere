using System.Text;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

public sealed class PdfMetadataTitleTests
{
    private static byte[] WithInfoTitle(string? title)
    {
        var builder = new PdfDocumentBuilder();
        builder.DocumentInformation.Title = title;
        var font = builder.AddStandard14Font(Standard14Font.Helvetica);
        builder.AddPage(PageSize.A4).AddText("canto la mia gioia al Signore", 11, new PdfPoint(50, 700), font);
        return builder.Build();
    }

    private static byte[] WithXmp(string xml) => CraftedPdf.Write(new[]
    {
        new CraftedPdf.Obj("<< /Type /Catalog /Pages 2 0 R /Metadata 6 0 R >>"),
        new CraftedPdf.Obj("<< /Type /Pages /Kids [4 0 R] /Count 1 >>"),
        new CraftedPdf.Obj(CraftedPdf.Font),
        new CraftedPdf.Obj(CraftedPdf.Page(5)),
        new CraftedPdf.Obj(string.Empty, CraftedPdf.Ascii(CraftedPdf.Text)),
        new CraftedPdf.Obj("/Type /Metadata /Subtype /XML", Encoding.UTF8.GetBytes(xml)),
    });

    private static string Xmp(string title) =>
        "<?xpacket begin='' id='W5M0MpCehiHzreSzNTczkc9d'?><x:xmpmeta xmlns:x='adobe:ns:meta/'><rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'>"
        + $"<rdf:Description xmlns:dc='http://purl.org/dc/elements/1.1/'><dc:title><rdf:Alt><rdf:li xml:lang='x-default'>{title}</rdf:li></rdf:Alt></dc:title></rdf:Description></rdf:RDF></x:xmpmeta><?xpacket end='w'?>";

    private static Task<PdfWordsResult> Extract(byte[] pdf) => new PdfPigWordExtractor().ExtractAsync(new MemoryStream(pdf));

    [Fact]
    public async Task The_title_of_the_document_information_is_read()
    {
        var result = await Extract(WithInfoTitle("Il mio canto"));

        Assert.Equal("Il mio canto", result.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Microsoft Word - x.docx")]
    [InlineData("untitled")]
    public async Task A_missing_or_useless_info_title_gives_no_title(string? title)
    {
        var result = await Extract(WithInfoTitle(title));

        Assert.Null(result.Title);
    }

    [Fact]
    public async Task An_empty_xmp_title_does_not_take_the_next_field()
    {
        var xml = "<?xpacket begin='' id='W5M0MpCehiHzreSzNTczkc9d'?><x:xmpmeta xmlns:x='adobe:ns:meta/'><rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'>"
            + "<rdf:Description xmlns:dc='http://purl.org/dc/elements/1.1/'><dc:title/><dc:creator><rdf:Seq><rdf:li>Mario Rossi</rdf:li></rdf:Seq></dc:creator></rdf:Description></rdf:RDF></x:xmpmeta><?xpacket end='w'?>";

        var result = await Extract(WithXmp(xml));

        Assert.Null(result.Title);
    }

    [Fact]
    public async Task An_xmp_stream_over_the_decode_limit_gives_no_title_but_the_document_is_read()
    {
        var padded = Xmp("Il mio canto").Replace("<rdf:RDF", "<!--" + new string('x', 200_000) + "--><rdf:RDF");
        var pdf = CraftedPdf.Write(new[]
        {
            new CraftedPdf.Obj("<< /Type /Catalog /Pages 2 0 R /Metadata 6 0 R >>"),
            new CraftedPdf.Obj("<< /Type /Pages /Kids [4 0 R] /Count 1 >>"),
            new CraftedPdf.Obj(CraftedPdf.Font),
            new CraftedPdf.Obj(CraftedPdf.Page(5)),
            new CraftedPdf.Obj(string.Empty, CraftedPdf.Ascii(CraftedPdf.Text)),
            new CraftedPdf.Obj("/Type /Metadata /Subtype /XML /Filter /FlateDecode", CraftedPdf.Zlib(Encoding.UTF8.GetBytes(padded))),
        });

        var result = await new PdfPigWordExtractor(new PdfExtractionLimits { MaxDecodedStreamBytes = 64 * 1024 }).ExtractAsync(new MemoryStream(pdf));

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Null(result.Title);
    }

    [Fact]
    public async Task A_very_long_title_is_cut()
    {
        var result = await Extract(WithInfoTitle(new string('a', 9000)));

        Assert.Equal(PdfDocumentTitle.MaxLength, result.Title!.Length);
    }

    [Fact]
    public async Task The_xmp_title_is_used_when_the_information_dictionary_has_none()
    {
        var result = await Extract(WithXmp(Xmp("Canto di lode")));

        Assert.Equal("Canto di lode", result.Title);
    }

    [Fact]
    public async Task An_xmp_document_with_a_doctype_is_not_parsed()
    {
        var xml = "<!DOCTYPE x [<!ENTITY a 'Titolo'>]>" + Xmp("&a;");

        var result = await Extract(WithXmp(xml));

        Assert.Null(result.Title);
        Assert.Equal(PdfExtractionStatus.Success, result.Status);
    }

    [Fact]
    public async Task A_broken_xmp_document_gives_no_title_and_still_reads_the_pages()
    {
        var result = await Extract(WithXmp("<x:xmpmeta><dc:title>"));

        Assert.Null(result.Title);
        Assert.Equal(PdfExtractionStatus.Success, result.Status);
    }

    [Fact]
    public async Task The_importer_carries_the_title_to_the_text_result()
    {
        var result = await new PdfTextImporter(new PdfPigWordExtractor()).ImportAsync(new MemoryStream(WithInfoTitle("Il mio canto")));

        Assert.Equal("Il mio canto", result.Title);
    }

    [Fact]
    public void The_worker_payload_carries_the_title()
    {
        var sent = PdfWordsResult.Success(new[] { new PdfPageWords(1, 595, 842, new[] { new PositionedWord("ciao", new WordBox(10, 20, 40, 30), null) }, false) }) with { Title = "Il mio canto" };
        using var wire = new MemoryStream();

        PdfWorkerProtocol.Write(wire, sent);
        wire.Position = 0;

        Assert.Equal("Il mio canto", PdfWorkerProtocol.TryRead(wire, new PdfExtractionLimits())!.Title);
    }

    [Theory]
    [InlineData("Il mio\\u0000 canto\\u202E", "Il mio canto")]
    [InlineData("Microsoft Word - x.docx", null)]
    [InlineData("", null)]
    public void A_title_in_a_payload_is_cleaned_again_by_the_receiver(string wireTitle, string? expected)
    {
        var json = PayloadWith($"\"m\":\"{wireTitle}\",");

        Assert.Equal(expected, PdfWorkerProtocol.TryRead(json, new PdfExtractionLimits())!.Title);
    }

    [Fact]
    public void A_huge_title_in_a_payload_is_cut_to_the_limit()
    {
        var json = PayloadWith($"\"m\":\"{new string('a', 3_000_000)}\",");

        Assert.Equal(PdfDocumentTitle.MaxLength, PdfWorkerProtocol.TryRead(json, new PdfExtractionLimits())!.Title!.Length);
    }

    [Fact]
    public void A_payload_without_a_title_is_still_read()
    {
        Assert.Null(PdfWorkerProtocol.TryRead(PayloadWith(string.Empty), new PdfExtractionLimits())!.Title);
    }

    [Fact]
    public void A_payload_of_the_previous_version_is_refused()
    {
        var json = Encoding.UTF8.GetBytes("{\"v\":1,\"s\":\"Success\",\"r\":\"\",\"p\":[{\"n\":1,\"w\":595,\"h\":842,\"i\":false,\"x\":[{\"t\":\"ciao\",\"l\":10,\"b\":20,\"r\":40,\"u\":30}]}]}");

        Assert.Null(PdfWorkerProtocol.TryRead(json, new PdfExtractionLimits()));
    }

    private static byte[] PayloadWith(string titleMember) => Encoding.UTF8.GetBytes(
        "{\"v\":" + PdfWorkerProtocol.Version + ",\"s\":\"Success\",\"r\":\"\"," + titleMember + "\"p\":[{\"n\":1,\"w\":595,\"h\":842,\"i\":false,\"x\":[{\"t\":\"ciao\",\"l\":10,\"b\":20,\"r\":40,\"u\":30}]}]}");
}
