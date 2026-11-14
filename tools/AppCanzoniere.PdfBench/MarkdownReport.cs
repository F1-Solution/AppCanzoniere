using System.Globalization;
using System.Text;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.PdfBench;

/// <summary>Writes the bench results as one Markdown page.</summary>
internal static class MarkdownReport
{
    // A PDF counts as a two-column booklet when at least this share of its text pages has two columns.
    private const double TwoColumnBookletShare = 0.5;

    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    public static string Write(IReadOnlyList<PdfBenchResult> results, DateTimeOffset when)
    {
        var md = new StringBuilder();
        md.AppendLine("# PDF import bench").AppendLine();
        md.AppendLine(Invariant, $"Run on {when:yyyy-MM-dd HH:mm} UTC. Extraction and layout run in-process with the app's own code; nothing leaves the machine.").AppendLine();
        PerPdf(md, results);
        TwoColumnSummary(md, results);
        PerExtract(md, results);
        PageColumns(md, results);
        Disagreements(md, results);
        Method(md);
        return md.ToString();
    }

    private static void PerPdf(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        md.AppendLine("## Per PDF").AppendLine();
        md.AppendLine("| PDF | pages | result | 1-col | 2-col | no text | time ms | labelled pages | pdftotext R / P | in-app R / P | lines scored / unmatched / fragment |");
        md.AppendLine("|---|---:|---|---:|---:|---:|---:|---|---|---|---|");
        foreach (var r in results)
        {
            var pages = r.Text.Pages;
            var baseline = r.Extracts.Aggregate(Score.None, (s, e) => s + e.Baseline);
            var extracted = r.Extracts.Aggregate(Score.None, (s, e) => s + e.Extracted);
            var labelled = string.Join(",", r.Extracts.SelectMany(e => e.Pages));
            var lines = r.Extracts.Sum(e => e.Lines);
            var counts = r.Extracts.Count == 0 ? "-" : $"{lines - r.Extracts.Sum(e => e.Unmatched)} / {r.Extracts.Sum(e => e.Unmatched)} / {r.Extracts.Sum(e => e.Fragments)}";
            md.AppendLine(Invariant, $"| {r.File} | {pages.Count} | {Outcome(r.Text)} | {pages.Count(p => p.ColumnCount == 1)} | {pages.Count(p => p.ColumnCount == 2)} | {pages.Count(p => p.ColumnCount == 0)} | {r.Elapsed.TotalMilliseconds:0} | {(labelled.Length == 0 ? "-" : labelled)} | {Pair(baseline)} | {Pair(extracted)} | {counts} |");
        }

        md.AppendLine();
    }

    private static void TwoColumnSummary(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        var booklets = results.Where(IsTwoColumnBooklet).Where(r => r.Extracts.Count > 0).ToList();
        var others = results.Where(r => !IsTwoColumnBooklet(r)).Where(r => r.Extracts.Count > 0).ToList();
        md.AppendLine("## Two-column booklets against the pdftotext baseline").AppendLine();
        md.AppendLine(Invariant, $"Two-column booklets (at least {TwoColumnBookletShare:P0} of text pages split): {string.Join(", ", booklets.Select(b => b.File))}.").AppendLine();
        md.AppendLine("| group | pdftotext lines mixing both columns | pdftotext recall | pdftotext precision | in-app recall | in-app precision |");
        md.AppendLine("|---|---|---|---|---|---|");
        Group(md, "two-column booklets", booklets);
        Group(md, "other labelled PDFs", others);
        Group(md, "all labelled PDFs", booklets.Concat(others).ToList());
        md.AppendLine();
    }

    private static void Group(StringBuilder md, string name, IReadOnlyList<PdfBenchResult> results)
    {
        var baseline = results.SelectMany(r => r.Extracts).Aggregate(Score.None, (s, e) => s + e.Baseline);
        var extracted = results.SelectMany(r => r.Extracts).Aggregate(Score.None, (s, e) => s + e.Extracted);
        var source = results.SelectMany(r => r.Extracts).Sum(e => e.SourceLines);
        var mixed = results.SelectMany(r => r.Extracts).Sum(e => e.MixedSourceLines);
        var share = source == 0 ? (double?)null : (double)mixed / source;
        md.AppendLine(Invariant, $"| {name} | {Ratio(share, mixed, source)} | {Ratio(baseline.Recall, baseline.Found, baseline.Labelled)} | {Ratio(baseline.Precision, baseline.Found, baseline.Predicted)} | {Ratio(extracted.Recall, extracted.Found, extracted.Labelled)} | {Ratio(extracted.Precision, extracted.Found, extracted.Predicted)} |");
    }

    private static void PerExtract(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        md.AppendLine("## Per labelled text").AppendLine();
        md.AppendLine("| labelled text | page(s) | pdftotext R / P | in-app R / P | lines scored | unmatched | fragment |");
        md.AppendLine("|---|---|---|---|---:|---:|---:|");
        foreach (var e in results.SelectMany(r => r.Extracts))
            md.AppendLine(Invariant, $"| {e.Name} | {string.Join(",", e.Pages)} | {Pair(e.Baseline)} | {Pair(e.Extracted)} | {e.Lines - e.Unmatched} | {e.Unmatched} | {e.Fragments} |");
        md.AppendLine();
    }

    private static void PageColumns(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        md.AppendLine("## Columns per page").AppendLine();
        md.AppendLine("One character per page: `1` one column, `2` two columns somewhere on the page, `S` scanned (images, no text), `-` blank.").AppendLine();
        foreach (var r in results)
            md.AppendLine(Invariant, $"- {r.File}: `{string.Concat(r.Text.Pages.Select(Code))}`");
        md.AppendLine();
    }

    private static void Disagreements(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        md.AppendLine("## Lines where the converter and the label disagree, and unmatched lines (in-app text)").AppendLine();
        foreach (var e in results.SelectMany(r => r.Extracts).Where(e => e.Disagreements.Count + e.UnmatchedLines.Count > 0))
        {
            md.AppendLine(Invariant, $"### {e.Name}").AppendLine();
            foreach (var d in e.Disagreements)
                md.AppendLine(Invariant, $"- p{d.Page} {(d.Labelled ? "missed chord line" : "false chord line")} ({d.Source}): `{d.Line.Trim()}`");
            foreach (var u in e.UnmatchedLines)
                md.AppendLine(Invariant, $"- unmatched, not scored: `{u}`");
            md.AppendLine();
        }
    }

    private static void Method(StringBuilder md)
    {
        md.AppendLine("## How the numbers are computed").AppendLine();
        md.AppendLine("- **pdftotext R / P**: the converter on the labelled `pdftotext -layout` text, scored by line number against the `.chordlines` labels, exactly as the corpus tests do. On two-column pages these lines interleave the two songs.");
        md.AppendLine("- **in-app R / P**: each labelled page is extracted and laid out by the app, the page text (columns in reading order, a blank line between blocks) goes through the converter, and every non-blank output line gets a label by content:");
        md.AppendLine("  1. the line's words (split on whitespace) must appear, in order and contiguous, in a `pdftotext` line of the same page; failing that, the line with spaces removed (at least 6 characters) must be a substring of a `pdftotext` line with spaces removed; otherwise the line is *unmatched* and not scored;");
        md.AppendLine("  2. when several `pdftotext` lines match, the one at the nearest relative vertical position on the page is used;");
        md.AppendLine("  3. a labelled chord line labels all its parts as chord lines (the labelling rule: a two-column line is a chord line only if every column holds chords); an unlabelled line labels a whole-line match as text;");
        md.AppendLine("  4. a line that is only one column of an unlabelled two-column `pdftotext` line (*fragment*) has no label of its own; it counts as a chord line when every word matches a strict chord pattern (Italian or English root, accidental, suffixes, optional bass), otherwise as text.");
        md.AppendLine("- Recall = labelled chord lines the converter marked / labelled chord lines; precision = correct chord lines / lines the converter marked. The two columns score different line sets (the in-app text has more, shorter lines on two-column pages), so compare the rates, not the counts.");
        md.AppendLine("- **lines mixing both columns**: non-blank `pdftotext` lines of the labelled pages whose words the app placed in two different columns, that is lines where `pdftotext` interleaved two songs; the app splits them.");
        md.AppendLine("- Time is wall-clock for opening, extracting and laying out the whole PDF, on this machine.");
    }

    private static bool IsTwoColumnBooklet(PdfBenchResult r)
    {
        var text = r.Text.Pages.Where(p => p.ColumnCount > 0).ToList();
        return text.Count > 0 && text.Count(p => p.ColumnCount == 2) >= TwoColumnBookletShare * text.Count;
    }

    private static string Outcome(PdfTextResult text) =>
        text.Status switch
        {
            PdfExtractionStatus.Success => "text",
            PdfExtractionStatus.NoTextLayer => "scan (no text layer)",
            _ => $"{text.Status}: {text.Reason}",
        };

    private static char Code(PageText page) => page.ColumnCount switch
    {
        0 => page.IsScanned ? 'S' : '-',
        1 => '1',
        _ => '2',
    };

    private static string Pair(Score s) =>
        s.Labelled == 0 && s.Predicted == 0 ? "-" : $"{Percent(s.Recall)} / {Percent(s.Precision)}";

    private static string Ratio(double? value, int part, int whole) => $"{Percent(value)} ({part}/{whole})";

    private static string Percent(double? value) => value is { } v ? v.ToString("P1", Invariant) : "-";
}
