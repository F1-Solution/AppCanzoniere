using System.Globalization;
using System.Text;

namespace AppCanzoniere.PdfBench;

/// <summary>Writes the song splitting results: the totals per PDF and one table of songs per PDF.</summary>
internal static class SongsReport
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    /// <summary>The "songs found" cell of the per-PDF table, with the index count when the PDF has an index.</summary>
    public static string Cell(PdfBenchResult r) =>
        r.Songs.IndexEntries > 0 ? $"{r.Songs.Songs.Count} (index {r.Songs.IndexEntries})" : r.Songs.Songs.Count.ToString(Invariant);

    public static void Append(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        md.AppendLine("## Songs found per PDF").AppendLine();
        md.AppendLine("Index entries are the numbered or page-numbered lines of the pages recognised as an index; where a PDF has one, the difference to the songs found shows missed or extra songs.").AppendLine();
        md.AppendLine("| PDF | songs found | index entries | difference | songs without chords | songs with warnings | pages without a song |");
        md.AppendLine("|---|---:|---:|---:|---:|---:|---|");
        foreach (var r in results.Where(r => r.Text.IsSuccess))
        {
            var s = r.Songs;
            var diff = s.IndexEntries > 0 ? (s.Songs.Count - s.IndexEntries).ToString("+0;-0;0", Invariant) : "-";
            md.AppendLine(Invariant, $"| {r.File} | {s.Songs.Count} | {(s.IndexEntries > 0 ? s.IndexEntries.ToString(Invariant) : "-")} | {diff} | {s.Songs.Count(x => x.ChordLineCount == 0)} | {s.Songs.Count(x => x.Warnings.Count > 0)} | {Pages(s.SkippedPages)} |");
        }

        md.AppendLine();
        foreach (var r in results.Where(r => r.Songs.Songs.Count > 0))
            Songs(md, r);
    }

    private static void Songs(StringBuilder md, PdfBenchResult r)
    {
        md.AppendLine(Invariant, $"### {r.File}").AppendLine();
        md.AppendLine("| # | title | pages | chord lines | warnings |");
        md.AppendLine("|---:|---|---|---:|---|");
        var n = 0;
        foreach (var s in r.Songs.Songs)
        {
            var pages = s.FirstPage == s.LastPage ? $"{s.FirstPage}" : $"{s.FirstPage}-{s.LastPage}";
            var warnings = s.Warnings.Count == 0 ? "-" : string.Join(", ", s.Warnings) + $" ({s.DoubtfulLineCount} lines)";
            md.AppendLine(Invariant, $"| {++n} | {s.Title.Replace("|", "/", StringComparison.Ordinal)} | {pages} | {s.ChordLineCount} | {warnings} |");
        }

        md.AppendLine();
    }

    private static string Pages(IReadOnlyList<int> pages) => pages.Count == 0 ? "-" : string.Join(",", pages);
}
