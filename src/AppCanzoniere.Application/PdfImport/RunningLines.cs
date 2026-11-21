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

    // A footer that changes per section is a bottom row whose page number follows the page sequence, on at least this many pages of a run whose members are at most MaxRunGap pages apart.
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
        var sectionPages = SectionFooters(pages, candidates, repeated);
        return pages.Select((page, i) => Without(page, candidates[i].SelectMany(r => Dropped(r, page, repeated, sectionPages, i)).ToList())).ToList();
    }

    // Per signature, the pages whose bottom row carries a page number that moves with the page; a title or refrain without such a number is never a section footer.
    private static HashSet<(string Signature, int Page)> SectionFooters(IReadOnlyList<PdfPageWords> pages, List<IReadOnlyList<TextRow>> candidates, HashSet<string> repeated)
    {
        var result = new HashSet<(string, int)>();
        var groups = candidates.SelectMany((rows, i) => rows.Where(r => (IsRepeatable(r) || HasLink(r)) && IsAtBottom(r, pages[i])).Select(r => (Row: r, Page: i)))
            .Select(x => (Signature: Signature(x.Row), x.Page, Offset: PageNumberOf(x.Row) is { } n ? n - pages[x.Page].PageNumber : (int?)null))
            .Where(x => x.Signature.Length > 0 && x.Offset is not null && !repeated.Contains(x.Signature))
            .GroupBy(x => (x.Signature, x.Offset));
        foreach (var group in groups)
        {
            foreach (var page in SectionRunPages(group.Select(x => x.Page).Distinct().Order().ToList()))
                result.Add((group.Key.Signature, page));
        }

        return result;
    }

    private static bool IsAtBottom(TextRow row, PdfPageWords page) => row.Words.All(w => w.Box.Bottom < EdgeShare * page.Height);

    // The pages of the runs long enough to be one section's footer: occurrences close together, at least MinSectionPages of them.
    private static List<int> SectionRunPages(IReadOnlyList<int> pages)
    {
        var result = new List<int>();
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

    private static void Flush(List<int> run, List<int> result)
    {
        if (run.Count >= MinSectionPages)
            result.AddRange(run);

        run.Clear();
    }

    // A repeated row goes whole, and so does a bottom row with a link; a link at the top may belong to the song below its title, so it stays unless repeated.
    private static IEnumerable<PositionedWord> Dropped(TextRow row, PdfPageWords page, HashSet<string> repeated, HashSet<(string Signature, int Page)> sectionPages, int pageIndex)
    {
        var hasLink = HasLink(row);
        var atBottom = IsAtBottom(row, page);
        var signature = Signature(row);
        var running = (IsRepeatable(row) || hasLink) && (repeated.Contains(signature) || (atBottom && sectionPages.Contains((signature, pageIndex))));
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

    // The page number of a row in the same shapes Signature strips; null when the row has none.
    private static int? PageNumberOf(TextRow row)
    {
        var text = string.Join(' ', row.Words.Select(w => w.Text)).Trim();
        var match = NumberOnly().IsMatch(text) ? Digits().Match(text) : Digits().Match(LeadingPageNumber().Match(text) is { Success: true } lead ? lead.Value
            : DashedPageNumber().Match(text) is { Success: true } dashed ? dashed.Value
            : TrailingPageNumber().Match(text) is { Success: true } tail ? tail.Value : string.Empty);
        return match.Success && int.TryParse(match.Value, out var number) ? number : null;
    }

    [GeneratedRegex(@"[0-9]{1,4}")]
    private static partial Regex Digits();

    private static PdfPageWords Without(PdfPageWords page, IReadOnlyList<PositionedWord> rows)
    {
        if (rows.Count == 0)
            return page;

        var dropped = rows.ToHashSet();
        return page with { Words = page.Words.Where(w => !dropped.Contains(w)).ToList() };
    }
}
