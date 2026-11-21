using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>Removes running footers and headers: edge rows with a link, or the same outermost, set-apart row repeated on several pages or on a run of nearby pages.</summary>
internal static partial class RunningLines
{
    // A row is an edge row when it lies in the top or bottom tenth of the page.
    private const double EdgeShare = 0.10;

    // The same edge row (page number ignored) on at least this many pages, and on this share of them, is a running line.
    private const int MinRepeatPages = 3;
    private const double MinRepeatShare = 0.25;

    // A footer that changes per section repeats on at least this many pages in a run whose members are at most MaxRunGap pages apart.
    private const int MinSectionPages = 3;
    private const int MaxRunGap = 2;

    // A running line is set apart: its gap to the next row is more than this multiple of the page's usual row gap.
    private const double SetApartFactor = 2.0;

    // Row heights stand in for the usual gap on a page with too few rows to measure it.
    private const double FallbackGapHeights = 1.5;

    [GeneratedRegex(@"^(?:https?://|www\.)\S+|^[^\s@]+@[^\s@]+\.[A-Za-z]{2,}|^[\w-]+(?:\.[\w-]+)*\.[A-Za-z]{2,6}/\S*", RegexOptions.IgnoreCase)]
    private static partial Regex LinkWord();

    [GeneratedRegex(@"[0-9]")]
    private static partial Regex Digit();

    // A page number in one of its usual shapes: alone, between dashes, after "pag.", or at the end of the row.
    [GeneratedRegex(@"^[-–\s]*[0-9]{1,4}[-–\s]*$", RegexOptions.IgnoreCase)]
    private static partial Regex NumberOnly();

    [GeneratedRegex(@"[-–\s]*\b(?:pag\.?\s*)?[0-9]{1,4}(?:\s*[-–])?\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex TrailingPageNumber();

    [GeneratedRegex(@"\s*[-–]\s*[0-9]{1,4}\s*[-–]\s*", RegexOptions.IgnoreCase)]
    private static partial Regex DashedPageNumber();

    // A page number before the text, as on facing pages; it only counts when letters follow.
    [GeneratedRegex(@"^(?:pag\.?\s*)?[0-9]{1,4}\s+(?=\p{L})", RegexOptions.IgnoreCase)]
    private static partial Regex LeadingPageNumber();

    public static IReadOnlyList<PdfPageWords> Strip(IReadOnlyList<PdfPageWords> pages)
    {
        var candidates = pages.Select(Candidates).ToList();
        var bySignature = candidates.SelectMany((rows, i) => rows.Where(r => IsRepeatable(r) || HasLink(r)).Select(Signature).Where(s => s.Length > 0).Distinct().Select(s => (Signature: s, Page: i)))
            .GroupBy(x => x.Signature).ToList();
        var repeated = bySignature.Where(g => g.Count() >= MinRepeatPages && g.Count() >= MinRepeatShare * pages.Count).Select(g => g.Key).ToHashSet();
        var sectionPages = bySignature.Where(g => !repeated.Contains(g.Key)).ToDictionary(g => g.Key, g => SectionRunPages(g.Select(x => x.Page).Order().ToList()));
        return pages.Select((page, i) => Without(page, candidates[i].SelectMany(r => Dropped(r, page, repeated, sectionPages, i)).ToList())).ToList();
    }

    // The pages of the runs long enough to be one section's footer: occurrences close together, at least MinSectionPages of them.
    private static HashSet<int> SectionRunPages(IReadOnlyList<int> pages)
    {
        var result = new HashSet<int>();
        var run = new List<int>();
        foreach (var page in pages)
        {
            if (run.Count > 0 && page - run[^1] > MaxRunGap)
                Flush(run, result);

            run.Add(page);
        }

        Flush(run, result);
        return result;
    }

    private static void Flush(List<int> run, HashSet<int> result)
    {
        if (run.Count >= MinSectionPages)
            result.UnionWith(run);

        run.Clear();
    }

    // A repeated row goes whole, and so does a bottom row with a link; a link at the top may belong to the song below its title, so it stays unless repeated.
    private static IEnumerable<PositionedWord> Dropped(TextRow row, PdfPageWords page, HashSet<string> repeated, Dictionary<string, HashSet<int>> sectionPages, int pageIndex)
    {
        var hasLink = HasLink(row);
        var atBottom = row.Words.All(w => w.Box.Bottom < EdgeShare * page.Height);
        var signature = Signature(row);
        var running = (IsRepeatable(row) || hasLink) && (repeated.Contains(signature) || (sectionPages.TryGetValue(signature, out var run) && run.Contains(pageIndex)));
        return running || (atBottom && hasLink) ? row.Words : Array.Empty<PositionedWord>();
    }

    private static bool HasLink(TextRow row) => row.Words.Any(w => LinkWord().IsMatch(w.Text));

    // Edge rows that may be running lines: any with a link, and the outermost row of the page when it is set apart and is not a chord row.
    private static IReadOnlyList<TextRow> Candidates(PdfPageWords page)
    {
        var words = page.Words.Where(w => !string.IsNullOrWhiteSpace(w.Text)).ToList();
        if (words.Count == 0 || page.Height <= 0)
            return Array.Empty<TextRow>();

        var rows = TextRows.Group(words);
        var result = new List<TextRow>();
        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var inTop = row.Words.All(w => w.Box.Top > (1 - EdgeShare) * page.Height);
            var inBottom = row.Words.All(w => w.Box.Bottom < EdgeShare * page.Height);
            if (!inTop && !inBottom)
                continue;

            var outermost = (inTop && i == 0) || (inBottom && i == rows.Count - 1);
            if (HasLink(row) || (outermost && IsSetApart(rows, i) && !IsChordRow(row)))
                result.Add(row);
        }

        return result;
    }

    // The gap to the neighbouring row inside the page is more than twice the usual gap between rows.
    private static bool IsSetApart(IReadOnlyList<TextRow> rows, int i)
    {
        if (rows.Count == 1)
            return true;

        var neighbour = i == 0 ? 1 : i - 1;
        var gap = Math.Abs(rows[i].Baseline - rows[neighbour].Baseline);
        var others = Enumerable.Range(1, rows.Count - 1).Where(j => j != Math.Max(i, neighbour)).Select(j => Math.Abs(rows[j - 1].Baseline - rows[j].Baseline)).Order().ToList();
        var usual = others.Count > 0 ? others[others.Count / 2] : FallbackGapHeights * rows[i].Height;
        return gap > SetApartFactor * usual;
    }

    private static bool IsChordRow(TextRow row) =>
        Region.Of(0, new[] { string.Join(' ', row.Words.Select(w => w.Text)) }).IsChord[0];

    // A single plain word such as "Amen" could be lyrics; two words or a page number mark a footer.
    private static bool IsRepeatable(TextRow row) => row.Words.Count >= 2 || row.Words.Any(w => Digit().IsMatch(w.Text));

    // The row text without its page number; digits elsewhere (a song number, the 7 of a chord) stay.
    private static string Signature(TextRow row)
    {
        var text = string.Join(' ', row.Words.Select(w => w.Text)).Trim();
        if (NumberOnly().IsMatch(text))
            return "#";

        text = LeadingPageNumber().Replace(text, string.Empty);
        text = DashedPageNumber().Replace(text, " ");
        return TrailingPageNumber().Replace(text, string.Empty).Trim().ToLowerInvariant();
    }

    private static PdfPageWords Without(PdfPageWords page, IReadOnlyList<PositionedWord> rows)
    {
        if (rows.Count == 0)
            return page;

        var dropped = rows.ToHashSet();
        return page with { Words = page.Words.Where(w => !dropped.Contains(w)).ToList() };
    }
}
