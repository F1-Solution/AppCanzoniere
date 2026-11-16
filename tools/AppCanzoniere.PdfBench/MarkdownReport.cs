using System.Globalization;
using System.Text;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.PdfBench;

/// <summary>Writes the bench results as one Markdown page.</summary>
internal static class MarkdownReport
{
    // At most this many split errors are listed per PDF.
    private const int MaxListedSplitErrors = 15;

    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    private const string SplitMethod =
        "- **split check**: each output line with a letter or digit is matched word by word to a `pdftotext` line of the same page. "
        + "*Cut at a word space* (every page): the line is part of a `pdftotext` line and only one space separates it from the words left out, "
        + "so the app cut running text (a full-width line split into columns, or one row split in two); `pdftotext` puts at least two spaces between columns. "
        + "*Joined* (only on the labelled pages of the PDFs the corpus description lists as two-column: 03, 04, 13, 15, 18): the line holds words from both sides "
        + "of the right-column boundary of a two-column `pdftotext` line, a missed split. The boundary is the character column where most lines start a non-chord "
        + "piece after two spaces and other text, between 30% and 70% of the longest line, in at least 4 lines; it is listed per page above. "
        + "*Unverified*: no word-by-word match; a line that mixes two columns often matches no `pdftotext` line and lands here, so a rise in unverified lines is also a warning. The reference is pdftotext's layout, not a hand label, and the listed errors show what it flags; "
        + "on one-column PDFs a layout-only boundary mistakes aligned half-verses for columns, which is why joins are checked only on known two-column pages.";

    public static string Write(IReadOnlyList<PdfBenchResult> results, DateTimeOffset when)
    {
        var md = new StringBuilder();
        md.AppendLine("# PDF import bench").AppendLine();
        md.AppendLine(Invariant, $"Run on {when:yyyy-MM-dd HH:mm} UTC. Extraction and layout run in-process with the app's own code; nothing leaves the machine.").AppendLine();
        md.AppendLine("Three separate questions, three separate measures: does the converter find the chord lines (recall / precision), does each line keep to one column (split check), and does each chord stand over its syllable (alignment). A good chord-line score says nothing about the other two.").AppendLine();
        PerPdf(md, results);
        Summary(md, results);
        PerExtract(md, results);
        PageColumns(md, results);
        SplitErrors(md, results);
        Disagreements(md, results);
        Method(md);
        return md.ToString();
    }

    private static void PerPdf(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        md.AppendLine("## Per PDF").AppendLine();
        md.AppendLine("| PDF | pages | result | 1-col | 2-col | no text | time ms | labelled pages | pdftotext R / P | in-app R / P | split: checked / joined / cut / unverified | chords: measured / off by 2+ |");
        md.AppendLine("|---|---:|---|---:|---:|---:|---:|---|---|---|---|---|");
        foreach (var r in results)
        {
            var pages = r.Text.Pages;
            var labelled = string.Join(",", r.Extracts.SelectMany(e => e.Pages));
            var split = r.Split.Score;
            var a = r.Alignment;
            md.AppendLine(Invariant, $"| {r.File} | {pages.Count} | {Outcome(r.Text)} | {pages.Count(p => p.ColumnCount == 1)} | {pages.Count(p => p.ColumnCount == 2)} | {pages.Count(p => p.ColumnCount == 0)} | {r.Elapsed.TotalMilliseconds:0} | {(labelled.Length == 0 ? "-" : labelled)} | {Pair(Sum(r, e => e.Baseline))} | {Pair(Sum(r, e => e.Extracted))} | {split.Checked} / {split.Joined} / {split.WrongSplit} / {split.Unverified} | {a.Chords} / {Ratio(a.OffTwoRate, a.OffTwo, a.Chords)} |");
        }

        md.AppendLine();
    }

    private static void Summary(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        var booklets = results.Where(IsTwoColumnBooklet).ToList();
        var others = results.Where(r => !IsTwoColumnBooklet(r)).ToList();
        md.AppendLine("## Summary").AppendLine();
        md.AppendLine(Invariant, $"Two-column booklets, as the corpus description lists them: {string.Join(", ", booklets.Select(b => b.File))}.").AppendLine();
        md.AppendLine("### Chord lines (labelled pages only)").AppendLine();
        md.AppendLine("| group | pdftotext R | pdftotext P | in-app R, hand labels | in-app P, hand labels | in-app R, pattern labels | in-app P, pattern labels | in-app R, all | in-app P, all |");
        md.AppendLine("|---|---|---|---|---|---|---|---|---|");
        foreach (var (name, group) in Groups(booklets, others))
            md.AppendLine(Invariant, $"| {name} | {Rates(Sum(group, e => e.Baseline))} | {Rates(Sum(group, e => e.Human))} | {Rates(Sum(group, e => e.Pattern))} | {Rates(Sum(group, e => e.Extracted))} |");
        md.AppendLine();
        md.AppendLine("### Column split (every page with a pdftotext reference)").AppendLine();
        md.AppendLine("| group | pages checked for joins | lines checked | joined (missed split) | cut at a word space | error rate | unverified |");
        md.AppendLine("|---|---:|---:|---:|---:|---|---:|");
        foreach (var (name, group) in Groups(booklets, others))
        {
            var s = group.Aggregate(SplitScore.None, (t, r) => t + r.Split.Score);
            md.AppendLine(Invariant, $"| {name} | {group.Sum(r => r.Split.JoinPages.Count)} | {s.Checked} | {s.Joined} | {s.WrongSplit} | {Ratio(s.ErrorRate, s.Joined + s.WrongSplit, s.Checked)} | {s.Unverified} |");
        }

        md.AppendLine();
        var joinPages = results.Where(r => r.Split.JoinPages.Count > 0).Select(r => $"{r.File} {string.Join(" ", r.Split.JoinPages)}");
        md.AppendLine(Invariant, $"Pages checked for joins, with the reference boundary column: {string.Join("; ", joinPages)}.").AppendLine();
        Alignment(md, booklets, others);
    }

    private static void Alignment(StringBuilder md, IReadOnlyList<PdfBenchResult> booklets, IReadOnlyList<PdfBenchResult> others)
    {
        md.AppendLine("### Chord over syllable (every page)").AppendLine();
        md.AppendLine("| group | chords measured | exact | off by 1 | off by 2+ | off by 2+ over loose lyric rows | off by 2+ over tight lyric rows | chord rows separated by a blank line |");
        md.AppendLine("|---|---:|---:|---:|---|---|---|---:|");
        foreach (var (name, group) in Groups(booklets, others))
        {
            var a = group.Aggregate(AlignmentScore.None, (t, r) => t + r.Alignment);
            var tight = a.Chords - a.LooseChords;
            md.AppendLine(Invariant, $"| {name} | {a.Chords} | {a.Exact} | {a.OffOne} | {Ratio(a.OffTwoRate, a.OffTwo, a.Chords)} | {Ratio(a.LooseOffTwoRate, a.LooseOffTwo, a.LooseChords)} | {Ratio(a.TightOffTwoRate, a.OffTwo - a.LooseOffTwo, tight)} | {a.Separated} |");
        }

        md.AppendLine();
    }

    private static void PerExtract(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        md.AppendLine("## Per labelled text").AppendLine();
        md.AppendLine("| labelled text | page(s) | pdftotext R / P | in-app R / P, all | hand labels | pattern labels | lines scored | of which fragment | of which unmatched |");
        md.AppendLine("|---|---|---|---|---|---|---:|---:|---:|");
        foreach (var e in results.SelectMany(r => r.Extracts))
            md.AppendLine(Invariant, $"| {e.Name} | {string.Join(",", e.Pages)} | {Pair(e.Baseline)} | {Pair(e.Extracted)} | {Pair(e.Human)} | {Pair(e.Pattern)} | {e.Lines} | {e.Fragments} | {e.Unmatched} |");
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

    private static void SplitErrors(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        md.AppendLine(Invariant, $"## Split errors (first {MaxListedSplitErrors} per PDF)").AppendLine();
        foreach (var r in results.Where(r => r.Split.Errors.Count > 0))
        {
            md.AppendLine(Invariant, $"### {r.File} ({r.Split.Errors.Count})").AppendLine();
            foreach (var error in r.Split.Errors.Take(MaxListedSplitErrors))
                md.AppendLine(Invariant, $"- {error}");
            md.AppendLine();
        }
    }

    private static void Disagreements(StringBuilder md, IReadOnlyList<PdfBenchResult> results)
    {
        md.AppendLine("## Lines where the converter and the label disagree (in-app text)").AppendLine();
        foreach (var e in results.SelectMany(r => r.Extracts).Where(e => e.Disagreements.Count + e.UnmatchedLines.Count > 0))
        {
            md.AppendLine(Invariant, $"### {e.Name}").AppendLine();
            foreach (var d in e.Disagreements)
                md.AppendLine(Invariant, $"- p{d.Page} {(d.Labelled ? "missed chord line" : "false chord line")} ({d.Source}): `{d.Line.Trim()}`");
            foreach (var u in e.UnmatchedLines)
                md.AppendLine(Invariant, $"- unmatched, scored by the chord pattern: `{u}`");
            md.AppendLine();
        }
    }

    private static void Method(StringBuilder md)
    {
        md.AppendLine("## How the numbers are computed").AppendLine();
        md.AppendLine("- **pdftotext R / P**: the converter on the labelled `pdftotext -layout` text, scored by line number against the `.chordlines` hand labels, exactly as the corpus tests do. On two-column pages these lines interleave the two songs.");
        md.AppendLine("- **in-app R / P**: each labelled page is extracted and laid out by the app, the page text goes through the converter, and every non-blank output line is scored. Its label is found by content:");
        md.AppendLine("  1. the line's words must appear, in order and contiguous, in a `pdftotext` line of the same page (or, with spaces removed and at least 6 characters, inside one); the nearest in page position wins;");
        md.AppendLine("  2. *hand labels*: a labelled chord line labels its parts as chord lines, an unlabelled whole line is text;");
        md.AppendLine("  3. *pattern labels*: a line that is only one column of an unlabelled two-column `pdftotext` line (*fragment*), or that matches no `pdftotext` line (*unmatched*), counts as a chord line when every word matches a strict chord pattern, otherwise as text. Unmatched lines are scored, not dropped.");
        md.AppendLine("- Recall = chord lines the converter marked / chord lines; precision = correct / marked. The in-app text has more, shorter lines on two-column pages than pdftotext, so compare rates, not counts. The pdftotext and in-app recall measure the converter on two different texts; neither measures column splitting or chord placement.");
        md.AppendLine(SplitMethod);
        md.AppendLine("- **chord over syllable**: on every page, each output line that is a chord row (80% of its words chords) followed directly by a lyric row is checked chord by chord. The truth is the lyric letter whose PdfPig glyph box contains the chord's left edge (chords between words or outside the row are not measured); the error is the distance, in characters, between the chord's column and that letter's column in the output. *Loose* lyric rows have fewer than two words, words covering under 60% of the row, or a gap wider than the text height; the others are *tight*.");
        md.AppendLine("- Time is wall-clock for opening, extracting and laying out the whole PDF, on this machine; the first PDF includes start-up.");
    }

    private static IEnumerable<(string Name, IReadOnlyList<PdfBenchResult> Group)> Groups(IReadOnlyList<PdfBenchResult> booklets, IReadOnlyList<PdfBenchResult> others)
    {
        yield return ("two-column booklets", booklets);
        yield return ("other PDFs", others);
        yield return ("all PDFs", booklets.Concat(others).ToList());
    }

    private static Score Sum(PdfBenchResult r, Func<ExtractScore, Score> pick) => r.Extracts.Aggregate(Score.None, (s, e) => s + pick(e));

    private static Score Sum(IReadOnlyList<PdfBenchResult> group, Func<ExtractScore, Score> pick) => group.Aggregate(Score.None, (s, r) => s + Sum(r, pick));

    private static bool IsTwoColumnBooklet(PdfBenchResult r) => CorpusFolder.IsTwoColumn(r.File);

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

    private static string Rates(Score s) => $"{Ratio(s.Recall, s.Found, s.Labelled)} | {Ratio(s.Precision, s.Found, s.Predicted)}";

    private static string Ratio(double? value, int part, int whole) => $"{Percent(value)} ({part}/{whole})";

    private static string Percent(double? value) => value is { } v ? v.ToString("P1", Invariant) : "-";
}
