using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

/// <summary>
/// Small PDFs generated with the library's own document builder, so the tests need no corpus.
/// Helvetica is proportional, which is what makes chord alignment hard in real booklets.
/// The lyrics are written for these tests; '^' marks the character a chord sits over.
/// </summary>
internal static class PdfFixtures
{
    public const double FontSize = 11;
    public const double ChordRise = 13;
    public const double Pitch = 26;

    public static readonly string[] LeftLyrics =
    {
        "^Cammina con me per la ^strada",
        "^dove il sole ^nasce al mattino",
        "^porta la tua ^luce nel cuore",
        "^canta con noi la ^gioia del cielo",
    };

    public static readonly string[] RightLyrics =
    {
        "^Vento che soffia sul ^mare",
        "^onde che corrono ^verso la riva",
        "^voce che chiama il ^nome di tutti",
        "^sera che scende ^piano sul monte",
    };

    // A 1x1 pixel PNG, enough to make a page carry an image and nothing else, as a scan does.
    private static readonly byte[] Pixel = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");

    public static byte[] TwoColumns(bool withTitle = false)
    {
        var builder = new PdfDocumentBuilder();
        var font = builder.AddStandard14Font(Standard14Font.Helvetica);
        var page = builder.AddPage(PageSize.A4);
        if (withTitle)
            page.AddText("UN TITOLO MOLTO LUNGO CHE ATTRAVERSA TUTTA LA PAGINA", 14, new PdfPoint(110, 800), font);

        Song(page, font, LeftLyrics, new[] { "DO", "SOL" }, 40, 740);
        Song(page, font, RightLyrics, new[] { "RE", "LA" }, 310, 740);
        return builder.Build();
    }

    public static byte[] OneColumnWithWideChordGaps()
    {
        var builder = new PdfDocumentBuilder();
        var font = builder.AddStandard14Font(Standard14Font.Helvetica);
        var page = builder.AddPage(PageSize.A4);
        Song(
            page,
            font,
            new[]
            {
                "^Questa lunga strada che porta lontano dove nasce il ^giorno e finisce la sera",
                "^tutti noi insieme camminiamo cantando sotto il cielo con la ^luce che ci guida",
                "^ogni passo che facciamo ci avvicina un poco alla casa del ^padre che ci aspetta",
                "^e la notte buia non fa paura a chi ha nel cuore la speranza di un ^nuovo mattino",
            },
            new[] { "LA-", "MI7" },
            40,
            740);
        return builder.Build();
    }

    /// <summary>One page with a line of text on a page box of the given size, in points.</summary>
    public static byte[] PageOfSize(double width, double height)
    {
        var builder = new PdfDocumentBuilder();
        var font = builder.AddStandard14Font(Standard14Font.Helvetica);
        builder.AddPage(width, height).AddText("canto la mia gioia al Signore", FontSize, new PdfPoint(50, 700), font);
        return builder.Build();
    }

    /// <summary>A title in small capitals: a larger initial followed, touching it, by smaller capitals, then a word space.</summary>
    public static byte[] SmallCapsTitle()
    {
        var builder = new PdfDocumentBuilder();
        var font = builder.AddStandard14Font(Standard14Font.Helvetica);
        var page = builder.AddPage(PageSize.A4);
        var initial = page.AddText("C", 14, new PdfPoint(100, 700), font);
        var rest = page.AddText("ANTIAMO", 10, new PdfPoint(initial[^1].EndBaseLine.X, 700), font);
        page.AddText("TE", 14, new PdfPoint(rest[^1].EndBaseLine.X + 4, 700), font);
        return builder.Build();
    }

    /// <summary>
    /// "of Elijah" in Times Italic with a real space, narrowed by kerning as justified text is: the italic f overhangs
    /// its advance past the start of the E, while the pen still leaves more than a hair between the two words.
    /// </summary>
    public static byte[] ItalicWordsOneSpaceApart() => CraftedPdf.Write(new[]
    {
        new CraftedPdf.Obj("<< /Type /Catalog /Pages 2 0 R >>"),
        new CraftedPdf.Obj("<< /Type /Pages /Kids [4 0 R] /Count 1 >>"),
        new CraftedPdf.Obj("<< /Type /Font /Subtype /Type1 /BaseFont /Times-Italic >>"),
        new CraftedPdf.Obj(CraftedPdf.Page(5)),
        new CraftedPdf.Obj(string.Empty, CraftedPdf.Ascii("BT /F1 11 Tf 100 700 Td [(of ) 150 (Elijah)] TJ ET\n")),
    });

    public static byte[] ScannedPages(int pages = 1)
    {
        var builder = new PdfDocumentBuilder();
        for (var i = 0; i < pages; i++)
            builder.AddPage(PageSize.A4).AddPng(Pixel, new PdfRectangle(0, 0, 595, 842));
        return builder.Build();
    }

    public static byte[] BlankPage()
    {
        var builder = new PdfDocumentBuilder();
        builder.AddPage(PageSize.A4);
        return builder.Build();
    }

    public static byte[] TextThenScan()
    {
        var builder = new PdfDocumentBuilder();
        var font = builder.AddStandard14Font(Standard14Font.Helvetica);
        builder.AddPage(PageSize.A4).AddText("canto la mia gioia al Signore", FontSize, new PdfPoint(50, 700), font);
        builder.AddPage(PageSize.A4).AddPng(Pixel, new PdfRectangle(0, 0, 595, 842));
        return builder.Build();
    }

    public static byte[] ManyPages(int pages)
    {
        var builder = new PdfDocumentBuilder();
        var font = builder.AddStandard14Font(Standard14Font.Helvetica);
        for (var i = 0; i < pages; i++)
            builder.AddPage(PageSize.A4).AddText($"pagina {i + 1}", FontSize, new PdfPoint(50, 700), font);
        return builder.Build();
    }

    /// <summary>
    /// One page whose content stream is Flate-compressed and unpacks to about decodedBytes bytes: a line of text
    /// followed by comment lines, which compress to almost nothing, as a decompression bomb does.
    /// </summary>
    public static byte[] CompressedPage(int decodedBytes)
    {
        var content = new System.Text.StringBuilder("BT /F1 11 Tf 50 700 Td (canto la mia gioia) Tj ET\n");
        while (content.Length < decodedBytes)
            content.Append('%').Append('x', 98).Append('\n');

        using var packed = new MemoryStream();
        using (var zlib = new System.IO.Compression.ZLibStream(packed, System.IO.Compression.CompressionLevel.SmallestSize, leaveOpen: true))
            zlib.Write(System.Text.Encoding.ASCII.GetBytes(content.ToString()));

        return RawPdf(packed.ToArray(), "/Filter /FlateDecode ");
    }

    // A one-page PDF around the given content stream, with Helvetica as /F1 and a correct cross-reference table.
    private static byte[] RawPdf(byte[] stream, string filter)
    {
        var objects = new[]
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>",
            null,
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
        };
        using var pdf = new MemoryStream();
        void Write(string text) => pdf.Write(System.Text.Encoding.ASCII.GetBytes(text));
        Write("%PDF-1.4\n");
        var offsets = new List<long>();
        for (var i = 0; i < objects.Length; i++)
        {
            offsets.Add(pdf.Position);
            if (objects[i] is { } body)
            {
                Write($"{i + 1} 0 obj\n{body}\nendobj\n");
                continue;
            }

            Write($"{i + 1} 0 obj\n<< {filter}/Length {stream.Length} >>\nstream\n");
            pdf.Write(stream);
            Write("\nendstream\nendobj\n");
        }

        var xref = pdf.Position;
        Write($"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n");
        foreach (var offset in offsets)
            Write($"{offset:D10} 00000 n \n");
        Write($"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
        return pdf.ToArray();
    }

    /// <summary>A valid PDF whose trailer declares standard encryption with keys no password opens.</summary>
    public static byte[] Encrypted()
    {
        var text = System.Text.Encoding.Latin1.GetString(ManyPages(1));
        const string encrypt = " /Encrypt << /Filter /Standard /V 2 /R 3 /Length 128 /P -4 /O <"
            + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA> /U <"
            + "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB> >>";
        var index = text.IndexOf("trailer", StringComparison.Ordinal);
        var open = text.IndexOf("<<", index, StringComparison.Ordinal) + 2;
        return System.Text.Encoding.Latin1.GetBytes(text.Insert(open, encrypt));
    }

    private static void Song(PdfPageBuilder page, PdfDocumentBuilder.AddedFont font, string[] lyrics, string[] chords, double x, double top)
    {
        var baseline = top;
        foreach (var marked in lyrics)
        {
            ChordsOver(page, font, marked, chords, x, baseline);
            baseline -= Pitch;
        }
    }

    private static void ChordsOver(PdfPageBuilder page, PdfDocumentBuilder.AddedFont font, string marked, string[] chords, double x, double baseline)
    {
        var letters = page.AddText(marked.Replace("^", string.Empty), FontSize, new PdfPoint(x, baseline), font);
        var chord = 0;
        for (int i = 0, plain = 0; i < marked.Length; i++)
        {
            if (marked[i] == '^')
                page.AddText(chords[chord++], FontSize, new PdfPoint(letters[plain].BoundingBox.Left, baseline + ChordRise), font);
            else
                plain++;
        }
    }
}
