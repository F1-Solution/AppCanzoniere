using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

public class PdfTexGlyphTests
{
    // PdfPig reports a font's name only for embedded and Type 3 fonts, so the tests use Type 3 fonts.
    private static readonly (int Code, string Name)[] Glyphs =
    {
        (68, "D"), (82, "R"), (83, "S"), (91, "bracketleft"), (92, "backslash"), (93, "bracketright"),
        (101, "e"), (105, "i"), (111, "o"),
    };

    private const string ChordLines = "BT /F1 12 Tf 100 700 Td (Si[) Tj 0 -20 Td (Do]) Tj 0 -20 Td (Re\\\\) Tj ET\n";

    // Three chords: Si with code 0x5B, Do with 0x5D, Re with 0x5C, written in a Type 3 font of the given name.
    private static byte[] ChordsIn(string fontName)
    {
        var differences = string.Join(' ', Glyphs.Select(g => $"{g.Code} /{g.Name}"));
        var procs = string.Join(' ', Glyphs.Select(g => $"/{g.Name} 6 0 R"));
        var widths = string.Join(' ', Enumerable.Repeat(500, 111 - 68 + 1));
        return CraftedPdf.Write(new[]
        {
            new CraftedPdf.Obj("<< /Type /Catalog /Pages 2 0 R >>"),
            new CraftedPdf.Obj("<< /Type /Pages /Kids [4 0 R] /Count 1 >>"),
            new CraftedPdf.Obj($"<< /Type /Font /Subtype /Type3 /Name /{fontName} /FontBBox [0 0 500 700] /FontMatrix [0.001 0 0 0.001 0 0] /CharProcs << {procs} >> /Encoding << /Type /Encoding /Differences [{differences}] >> /FirstChar 68 /LastChar 111 /Widths [{widths}] >>"),
            new CraftedPdf.Obj(CraftedPdf.Page(5)),
            new CraftedPdf.Obj(string.Empty, CraftedPdf.Ascii(ChordLines)),
            new CraftedPdf.Obj(string.Empty, CraftedPdf.Ascii("500 0 0 0 500 700 d1 0 0 500 700 re f\n")),
        });
    }

    // The same three chords in an ordinary Type 1 font.
    private static byte[] ChordsInHelvetica() => CraftedPdf.Write(new[]
    {
        new CraftedPdf.Obj("<< /Type /Catalog /Pages 2 0 R >>"),
        new CraftedPdf.Obj("<< /Type /Pages /Kids [4 0 R] /Count 1 >>"),
        new CraftedPdf.Obj(CraftedPdf.Font),
        new CraftedPdf.Obj(CraftedPdf.Page(5)),
        new CraftedPdf.Obj(string.Empty, CraftedPdf.Ascii(ChordLines)),
    });

    private static async Task<string[]> Words(byte[] pdf)
    {
        var result = await new PdfPigWordExtractor().ExtractAsync(new MemoryStream(pdf));
        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        return result.Pages[0].Words.Select(w => w.Text).ToArray();
    }

    [Theory]
    [InlineData("ABCDEF+CMMI10")]
    [InlineData("CMMI7")]
    [InlineData("ABCDEF+LMMathItalic10-Regular")]
    [InlineData("QWERTY+LMMathItalic12-Regular")]
    [InlineData("LMMathItalic8-Regular")]
    public async Task ExtractAsync_ReadsCodesOfTexMathItalicAsFlatSharpAndNatural(string font)
    {
        var words = await Words(ChordsIn(font));

        Assert.Equal(new[] { "Sib", "Do#", "Re♮" }, words);
    }

    [Fact]
    public async Task ExtractAsync_LeavesTheSameCodesAloneInOrdinaryFonts()
    {
        var words = await Words(ChordsInHelvetica());

        Assert.Equal(new[] { "Si[", "Do]", "Re\\" }, words);
    }

    [Theory]
    [InlineData("ABCDEF+CMR10")]
    [InlineData("ABCDEF+LMRoman10-Regular")]
    [InlineData("abcdef+cmmi10")]
    [InlineData("ABCDEF+lmmathitalic10-regular")]
    [InlineData("Helvetica")]
    public async Task ExtractAsync_LeavesOtherFontNamesUntouched(string font)
    {
        var words = await Words(ChordsIn(font));

        Assert.Equal(new[] { "Si[", "Do]", "Re\\" }, words);
    }
}
