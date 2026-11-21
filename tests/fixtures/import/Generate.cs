#:package PdfPig@0.1.16
#:package System.Drawing.Common@9.0.0
#:property ManagePackageVersionsCentrally=false
#:property NuGetAudit=false
#:property PublishAot=false

// Regenerates the PDFs in the folder given as the first argument (Windows only: the scan is drawn with GDI).
// Run: dotnet run tests/fixtures/import/Generate.cs -- tests/fixtures/import
using System.Drawing;
using System.Drawing.Imaging;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;

var folder = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

(string Title, string[] Lyrics, string[] Chords)[] songs =
[
    ("CAMMINA CON ME", ["^Cammina con me per la ^strada", "^dove il sole ^nasce al mattino", "^porta la tua ^luce nel cuore", "^canta con noi la ^gioia del cielo"], ["DO", "SOL"]),
    ("VENTO SUL MARE", ["^Vento che soffia sul ^mare", "^onde che corrono ^verso la riva", "^voce che chiama il ^nome di tutti", "^sera che scende ^piano sul monte"], ["RE", "LA"]),
    ("LUCE DEL MATTINO", ["^Luce che sorge sul ^monte", "^canto di pace e di ^festa", "^tutti insieme lodiamo il ^Signore", "^oggi e per sempre nel ^cuore"], ["MI", "SI"]),
];

File.WriteAllBytes(Path.Combine(folder, "single-song.pdf"), Build(b => SongPage(b, songs[0])));

File.WriteAllBytes(Path.Combine(folder, "mini-songbook.pdf"), Build(b =>
{
    var font = b.AddStandard14Font(Standard14Font.Helvetica);
    var index = b.AddPage(PageSize.A4);
    index.AddText("INDICE", 16, new PdfPoint(230, 780), font);
    index.AddText("Piccola raccolta di canti", 11, new PdfPoint(60, 750), font);
    for (var i = 0; i < songs.Length; i++)
        index.AddText($"{songs[i].Title}  {i + 2}", 11, new PdfPoint(60, 720 - (i * 22)), font);
    foreach (var song in songs)
        SongPage(b, song);
}));

File.WriteAllBytes(Path.Combine(folder, "scanned-page.pdf"), Build(b =>
{
    using var bitmap = new Bitmap(1240, 1754);
    using (var g = Graphics.FromImage(bitmap))
    {
        g.Clear(Color.White);
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        using var title = new Font("Arial", 64, FontStyle.Bold);
        using var body = new Font("Arial", 48);
        g.DrawString("ALLELUIA CANTO NUOVO", title, Brushes.Black, 80, 150);
        var lines = new[] { "Alleluia cantiamo al Signore", "con gioia nel nostro cuore", "Alleluia lodiamo il suo nome", "per sempre nella sua casa" };
        for (var i = 0; i < lines.Length; i++)
            g.DrawString(lines[i], body, Brushes.Black, 80, 400 + (i * 120));
    }

    using var png = new MemoryStream();
    bitmap.Save(png, ImageFormat.Png);
    b.AddPage(PageSize.A4).AddPng(png.ToArray(), new PdfRectangle(0, 0, 595, 842));
}));

File.WriteAllText(Path.Combine(folder, "damaged.pdf"), "%PDF-1.4\nthis is not a real pdf, the file is cut short\n");
Console.WriteLine("Fixtures written to " + folder);

static byte[] Build(Action<PdfDocumentBuilder> fill)
{
    var builder = new PdfDocumentBuilder();
    fill(builder);
    return builder.Build();
}

static void SongPage(PdfDocumentBuilder b, (string Title, string[] Lyrics, string[] Chords) song)
{
    var font = b.AddStandard14Font(Standard14Font.Helvetica);
    var page = b.AddPage(PageSize.A4);
    page.AddText(song.Title, 14, new PdfPoint(60, 780), font);
    var baseline = 740.0;
    foreach (var marked in song.Lyrics)
    {
        var letters = page.AddText(marked.Replace("^", string.Empty), 11, new PdfPoint(60, baseline), font);
        var chord = 0;
        for (int i = 0, plain = 0; i < marked.Length; i++)
        {
            if (marked[i] == '^')
                page.AddText(song.Chords[chord++], 11, new PdfPoint(letters[plain].BoundingBox.Left, baseline + 13), font);
            else
                plain++;
        }

        baseline -= 26;
    }
}
