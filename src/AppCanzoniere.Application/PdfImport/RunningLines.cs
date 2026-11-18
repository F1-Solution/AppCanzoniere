using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>Removes running footers and headers: edge rows with a link, or the same edge row repeated on several pages.</summary>
internal static partial class RunningLines
{
    // A row is an edge row when it lies in the top or bottom tenth of the page.
    private const double EdgeShare = 0.10;

    // The same edge row (digits ignored) on at least this many pages, and on this share of them, is a running line.
    private const int MinRepeatPages = 3;
    private const double MinRepeatShare = 0.25;

    [GeneratedRegex(@"^(?:https?://|www\.)\S+|^[^\s@]+@[^\s@]+\.[A-Za-z]{2,}|^[\w-]+(?:\.[\w-]+)*\.[A-Za-z]{2,6}/\S*", RegexOptions.IgnoreCase)]
    private static partial Regex LinkWord();

    [GeneratedRegex(@"[0-9]")]
    private static partial Regex Digit();

    public static IReadOnlyList<PdfPageWords> Strip(IReadOnlyList<PdfPageWords> pages)
    {
        var edges = pages.Select(EdgeRows).ToList();
        var repeated = edges.SelectMany(rows => rows.Select(Signature).Distinct()).GroupBy(s => s)
            .Where(g => g.Key.Length > 0 && g.Count() >= MinRepeatPages && g.Count() >= MinRepeatShare * pages.Count).Select(g => g.Key).ToHashSet();
        return pages.Select((page, i) => Without(page, edges[i].SelectMany(r => Dropped(r, page, repeated)).ToList())).ToList();
    }

    // A repeated row goes whole, and so does a bottom row with a link; a link at the top may belong to the song below its title, so it stays unless repeated.
    private static IEnumerable<PositionedWord> Dropped(TextRow row, PdfPageWords page, HashSet<string> repeated)
    {
        var hasLink = row.Words.Any(w => LinkWord().IsMatch(w.Text));
        var atBottom = row.Words.All(w => w.Box.Bottom < EdgeShare * page.Height);
        var running = (IsRepeatable(row) || hasLink) && repeated.Contains(Signature(row));
        return running || (atBottom && hasLink) ? row.Words : Array.Empty<PositionedWord>();
    }

    private static IReadOnlyList<TextRow> EdgeRows(PdfPageWords page)
    {
        var words = page.Words.Where(w => !string.IsNullOrWhiteSpace(w.Text)).ToList();
        if (words.Count == 0 || page.Height <= 0)
            return Array.Empty<TextRow>();

        return TextRows.Group(words).Where(r => r.Words.All(w => w.Box.Top > (1 - EdgeShare) * page.Height || w.Box.Bottom < EdgeShare * page.Height)).ToList();
    }

    // A single plain word such as "Amen" could be lyrics; two words or a page number mark a footer.
    private static bool IsRepeatable(TextRow row) => row.Words.Count >= 2 || row.Words.Any(w => Digit().IsMatch(w.Text));

    private static string Signature(TextRow row) =>
        Digit().Replace(string.Join(' ', row.Words.Select(w => w.Text)), string.Empty).Trim().ToLowerInvariant();

    private static PdfPageWords Without(PdfPageWords page, IReadOnlyList<PositionedWord> rows)
    {
        if (rows.Count == 0)
            return page;

        var dropped = rows.ToHashSet();
        return page with { Words = page.Words.Where(w => !dropped.Contains(w)).ToList() };
    }
}
