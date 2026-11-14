using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.PdfBench;

/// <summary>Split outcomes of one PDF, the pages checked for joins with their boundary column, and examples of each error.</summary>
internal sealed record SplitResult(SplitScore Score, IReadOnlyList<string> JoinPages, IReadOnlyList<string> Errors)
{
    public static readonly SplitResult None = new(SplitScore.None, Array.Empty<string>(), Array.Empty<string>());
}

/// <summary>
/// Checks every non-blank output line against its pdftotext reference line: cuts at a word space on every page, joins
/// across the column boundary only on the given pages, known to have two columns.
/// </summary>
internal static class SplitCheck
{
    public static SplitResult Measure(PdfTextResult text, IReadOnlyList<LabelledPage> reference, IReadOnlySet<int> twoColumnPages)
    {
        var score = SplitScore.None;
        var joinPages = new List<string>();
        var errors = new List<string>();
        foreach (var page in text.Pages.Where(p => p.Blocks.Count > 0))
        {
            var source = reference.FirstOrDefault(r => r.PageNumber == page.PageNumber);
            if (source is null)
                continue;

            var known = twoColumnPages.Contains(page.PageNumber);
            var boundary = known ? ColumnReference.Boundary(source.Lines.Select(l => l.Text).ToList()) : null;
            if (known)
                joinPages.Add(boundary is { } b ? $"p{page.PageNumber}@{b}" : $"p{page.PageNumber}@none");
            foreach (var (line, outcome) in Judge(page, source, boundary))
            {
                score += SplitScore.Of(outcome);
                if (outcome is SplitOutcome.Joined or SplitOutcome.WrongSplit)
                    errors.Add($"p{page.PageNumber} {(outcome == SplitOutcome.Joined ? "joined" : "wrong split")}: `{line.Trim()}`");
            }
        }

        return new SplitResult(score, joinPages, errors);
    }

    private static IEnumerable<(string Line, SplitOutcome Outcome)> Judge(PageText page, LabelledPage source, int? boundary)
    {
        // Lines of symbols only (staff glyphs in tablature) have nothing pdftotext can match.
        var lines = page.Blocks.SelectMany(b => b.Lines).Where(l => l.Row is not null && l.Text.Any(char.IsLetterOrDigit)).ToList();
        if (lines.Count == 0)
            yield break;

        var top = lines.Max(l => l.Row!.Baseline);
        var span = Math.Max(1, top - lines.Min(l => l.Row!.Baseline));
        foreach (var line in lines)
        {
            var truth = LineTruth.Of(line.Text, (top - line.Row!.Baseline) / span, source);
            var match = source.Lines.FirstOrDefault(l => l.Number == truth.SourceLine);
            var outcome = match is null
                ? SplitOutcome.Unverified
                : ColumnReference.Judge(match.Text, truth.TokenStart, LineTruth.Tokens(line.Text).Length, boundary);
            yield return (line.Text, outcome);
        }
    }
}
