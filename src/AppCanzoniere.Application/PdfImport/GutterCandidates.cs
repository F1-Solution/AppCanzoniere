namespace AppCanzoniere.Application.PdfImport;

/// <summary>A position worth testing for a gutter: its x and its step index, so adjacent positions can be recognised.</summary>
internal sealed record GutterCandidate(double X, int Step);

/// <summary>
/// Lists the positions worth testing for a gutter: a grid of fixed steps over the search range, coarser only when the
/// range is absurdly wide, without the positions where too many rows have a word across, which cannot win.
/// </summary>
internal static class GutterCandidates
{
    // Positions are tried every this many points...
    private const double StepPoints = 1.0;

    // ...but never more than this many positions, so a crafted page size cannot make the search slow.
    private const int MaxSteps = 1000;

    /// <summary>Grid positions in [from, to] where at most maxCrossedRows rows have a word across, left to right.</summary>
    public static IReadOnlyList<GutterCandidate> Between(IReadOnlyList<TextRow> rows, double from, double to, int maxCrossedRows)
    {
        if (!(from <= to) || double.IsInfinity(to - from))
            return Array.Empty<GutterCandidate>();

        var step = Math.Max(StepPoints, (to - from) / MaxSteps);
        var spans = rows.SelectMany(RowSpans).ToList();
        var starts = spans.Select(s => s.Left).Order().ToList();
        var ends = spans.Select(s => s.Right).Order().ToList();
        var candidates = new List<GutterCandidate>();
        int started = 0, ended = 0;
        for (var k = 0; k <= MaxSteps && from + (k * step) <= to; k++)
        {
            var x = from + (k * step);
            while (started < starts.Count && starts[started] < x)
                started++;
            while (ended < ends.Count && ends[ended] <= x)
                ended++;
            if (started - ended <= maxCrossedRows)
                candidates.Add(new GutterCandidate(x, k));
        }

        return candidates;
    }

    // The x ranges a row covers with words, overlapping words merged, so each row counts once.
    private static IEnumerable<(double Left, double Right)> RowSpans(TextRow row)
    {
        var merged = new List<(double Left, double Right)>();
        foreach (var box in row.Words.Select(w => w.Box).OrderBy(b => b.Left))
        {
            if (merged.Count > 0 && box.Left < merged[^1].Right)
                merged[^1] = (merged[^1].Left, Math.Max(merged[^1].Right, box.Right));
            else
                merged.Add((box.Left, box.Right));
        }

        return merged;
    }
}
