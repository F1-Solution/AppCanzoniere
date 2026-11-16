using System.Text.RegularExpressions;

namespace AppCanzoniere.PdfBench;

/// <summary>The local corpus layout: pdf\*.pdf, testo\*.txt with *.chordlines labels, and the report folder.</summary>
internal sealed partial class CorpusFolder
{
    private const string SongMarker = "--song-";

    // PDFs that fonti.md describes as two-column booklets (04 as corrected by review), independent of the app's output.
    private static readonly string[] TwoColumnPrefixes = { "03-", "04-", "13-", "15-", "18-" };

    public CorpusFolder(string root)
    {
        Root = Path.GetFullPath(root);
        PdfFolder = Path.Combine(Root, "pdf");
        TextFolder = Path.Combine(Root, "testo");
        ReportFolder = Path.Combine(Root, "report-pdf");
    }

    public string Root { get; }

    public string PdfFolder { get; }

    public string TextFolder { get; }

    public string ReportFolder { get; }

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();

    /// <summary>True for the PDFs the corpus description lists as two-column.</summary>
    public static bool IsTwoColumn(string pdfFile) => TwoColumnPrefixes.Any(Path.GetFileName(pdfFile).StartsWith);

    public IEnumerable<string> Pdfs() => Directory.EnumerateFiles(PdfFolder, "*.pdf").Order(StringComparer.Ordinal);

    /// <summary>Labelled texts of a PDF: the whole file, or page extracts located in the full pdftotext output.</summary>
    public IReadOnlyList<LabelledText> LabelsFor(string pdfPath)
    {
        var stem = Path.GetFileNameWithoutExtension(pdfPath);
        var whole = Path.Combine(TextFolder, stem + ".txt");
        if (File.Exists(Path.ChangeExtension(whole, ".chordlines")))
            return new[] { LabelledText.Read(whole, Path.ChangeExtension(whole, ".chordlines"), 1) };

        if (!File.Exists(whole))
            return Array.Empty<LabelledText>();

        var pages = File.ReadAllText(whole).Split('\f').Select(Normalize).ToList();
        return Directory.EnumerateFiles(TextFolder, stem + SongMarker + "*.txt")
            .Order(StringComparer.Ordinal)
            .Where(t => File.Exists(Path.ChangeExtension(t, ".chordlines")))
            .Select(t => (Text: t, Page: pages.IndexOf(Normalize(File.ReadAllText(t))) + 1))
            .Where(t => t.Page > 0)
            .Select(t => LabelledText.Read(t.Text, Path.ChangeExtension(t.Text, ".chordlines"), t.Page))
            .ToList();
    }

    /// <summary>Every page of the PDF's whole pdftotext output, unlabelled; empty when there is no such file.</summary>
    public IReadOnlyList<LabelledPage> ReferencePages(string pdfPath)
    {
        var whole = Path.Combine(TextFolder, Path.GetFileNameWithoutExtension(pdfPath) + ".txt");
        return File.Exists(whole) ? LabelledText.ReadUnlabelled(whole).Pages : Array.Empty<LabelledPage>();
    }

    private static string Normalize(string text) => Whitespace().Replace(text, " ").Trim();
}
