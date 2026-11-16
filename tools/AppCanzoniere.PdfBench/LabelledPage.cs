using System.Text.RegularExpressions;

namespace AppCanzoniere.PdfBench;

/// <summary>One physical line of the labelled pdftotext output: its 1-based number, text and label.</summary>
internal sealed record LabelledLine(int Number, string Text, bool IsChordLine);

/// <summary>The labelled lines of one PDF page, as pdftotext wrote them.</summary>
internal sealed record LabelledPage(int PageNumber, IReadOnlyList<LabelledLine> Lines);

/// <summary>A labelled text file (whole PDF or one page extract) split into pages.</summary>
internal sealed record LabelledText(string Name, string Text, IReadOnlySet<int> Labels, IReadOnlyList<LabelledPage> Pages)
{
    private const char FormFeed = '\f';

    /// <summary>Reads a .txt and its .chordlines; firstPage numbers the first page of the text within the PDF.</summary>
    public static LabelledText Read(string textPath, string labelPath, int firstPage) =>
        Parse(Path.GetFileNameWithoutExtension(textPath), File.ReadAllText(textPath), ReadLabels(labelPath), firstPage);

    /// <summary>Reads a whole-PDF .txt without labels, as the column reference of every page.</summary>
    public static LabelledText ReadUnlabelled(string textPath) =>
        Parse(Path.GetFileNameWithoutExtension(textPath), File.ReadAllText(textPath), new HashSet<int>(), 1);

    private static LabelledText Parse(string name, string text, HashSet<int> labels, int firstPage)
    {
        var pages = new List<LabelledPage>();
        var current = new List<LabelledLine>();
        var page = firstPage;
        var physical = PhysicalLines(text);
        for (var i = 0; i < physical.Count; i++)
        {
            var line = physical[i];
            var breaks = line.Count(c => c == FormFeed);
            if (breaks > 0)
            {
                pages.Add(new LabelledPage(page, current));
                current = new List<LabelledLine>();
                page += breaks;
            }

            current.Add(new LabelledLine(i + 1, line.Replace(FormFeed.ToString(), string.Empty), labels.Contains(i + 1)));
        }

        pages.Add(new LabelledPage(page, current));
        return new LabelledText(name, text, labels, pages.Where(p => p.Lines.Any(l => l.Text.Trim().Length > 0)).ToList());
    }

    private static HashSet<int> ReadLabels(string path) =>
        File.ReadAllText(path)
            .Split(new[] { ',', ' ', '\r', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(token => int.TryParse(token, out var n) ? n : 0)
            .Where(n => n > 0)
            .ToHashSet();

    // Newline-terminated lines, the numbering the labels use.
    private static List<string> PhysicalLines(string text) =>
        Regex.Split(text, "\r\n|\r|\n").ToList();
}
