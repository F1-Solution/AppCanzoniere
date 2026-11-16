namespace AppCanzoniere.Application.PdfImport;

/// <summary>A run of rows read as one block: a full-width stretch (column 0 of 1) or one column of a two-column band.</summary>
public sealed record LayoutRegion(int ColumnIndex, int ColumnCount, IReadOnlyList<TextRow> Rows);

/// <summary>Splits a page into full-width stretches and two-column bands, in reading order.</summary>
public static class ColumnSplitter
{
    // The gutter must lie within this share of the page width from the page centre line.
    private const double CentreZoneShare = 0.12;

    // Each column of a band must span at least this share of the page text width.
    private const double MinColumnWidthShare = 0.15;

    // Two-column bands must hold at least this share of the page rows, otherwise the page reads across.
    private const double MinBandRowShare = 0.5;

    // A band needs at least this many rows with text on each side of the gutter.
    private const int MinRowsPerColumn = 6;

    // Each column needs at least this many lyric-like rows, so stacks of chords or line endings do not count.
    private const int MinLyricRowsPerColumn = 3;

    // A lyric-like row part has at least this many words.
    private const int MinLyricWords = 3;

    // A lyric-like row part has at least this many characters, so short chord runs do not count.
    private const int MinLyricCharacters = 12;

    // A lyric-like row part has words covering at least this share of its extent.
    private const double MinLyricCoverage = 0.6;

    // A gap at the gutter narrower than this many text heights is a word space rather than a gutter.
    private const double MinGutterGapInHeights = 1.0;

    // Fewer than this share of the rows with text on both sides may have a narrow gap at the gutter.
    private const double MaxNarrowGapShare = 0.5;

    // The gap test applies only when at least this many rows have text on both sides.
    private const int MinRowsForGapTest = 3;

    // Right-column rows whose first word starts within this distance (points) share a margin.
    private const double MarginTolerancePoints = 3.0;

    // At least this share of right-column rows, and never fewer than two, must start at a shared margin.
    private const double MinShareAtMargin = 0.25;
    private const int MinRowsAtMargin = 2;

    // The word-space test for a top or bottom row needs at least this many gaps, so its median is a word space...
    private const int MinEdgeLineGaps = 2;

    // ...and a gap at the gutter up to this multiple of that median still counts as a word space.
    private const double WordSpaceToleranceFactor = 1.5;

    // A header or footer stands further than this multiple of the usual row pitch from the next row.
    private const double EdgeApartPitchFactor = 1.4;

    public static IReadOnlyList<LayoutRegion> Split(IReadOnlyList<PositionedWord> words, double pageWidth, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(words);
        cancellationToken.ThrowIfCancellationRequested();
        if (words.Count == 0)
            return Array.Empty<LayoutRegion>();

        var rows = TextRows.Group(words);
        var left = words.Min(w => w.Box.Left);
        var right = words.Max(w => w.Box.Right);
        var minColumnWidth = (right - left) * MinColumnWidthShare;
        var centre = pageWidth / 2;
        var from = Math.Max(left + minColumnWidth, centre - (CentreZoneShare * pageWidth));
        var to = Math.Min(right - minColumnWidth, centre + (CentreZoneShare * pageWidth));

        // A position crossed by words in more rows than this leaves too few rows for the bands to win.
        var maxCrossed = rows.Count - (int)Math.Ceiling(MinBandRowShare * rows.Count);
        var candidates = GutterCandidates.Between(rows, from, to, maxCrossed);
        var gutter = FindGutter(rows, candidates, minColumnWidth, cancellationToken);
        return gutter is { } x ? Regions(rows, x, minColumnWidth) : new[] { new LayoutRegion(0, 1, rows) };
    }

    private static bool Crosses(TextRow row, double x) => row.Words.Any(w => w.Box.Left < x && w.Box.Right > x);

    // Rows read across the gutter: a word lies on it, or only a word space separates the two sides and the
    // right side does not start at the right column's margin (a full-width title or paragraph).
    private static bool[] Spanning(IReadOnlyList<TextRow> rows, double x)
    {
        var margin = RightMargin(rows.Where(r => !Crosses(r, x) && r.Right > x).Select(r => RightStart(r, x)).ToList());
        var edges = EdgeLines(rows, x);
        return rows.Select((r, i) => Crosses(r, x) || IsBridged(r, x, margin, edges.Contains(i))).ToArray();
    }

    private static bool IsBridged(TextRow row, double x, double? margin, bool edgeLine)
    {
        if (row.Left >= x || row.Right <= x)
            return false;

        var start = RightStart(row, x);
        var narrow = Gap(row, x) < MinGutterGapInHeights * row.Height;
        return narrow && (margin is not { } m || Math.Abs(start - m) > MarginTolerancePoints || edgeLine);
    }

    // Indexes of a running header or footer: the top or bottom row, set apart by extra space and crossing the gutter with
    // about its usual word space, whatever its size, since two larger column titles starting at the margin are not one.
    private static HashSet<int> EdgeLines(IReadOnlyList<TextRow> rows, double x)
    {
        var lines = new HashSet<int>();
        if (rows.Count < 2)
            return lines;

        var pitch = rows.Zip(rows.Skip(1), (a, b) => a.Baseline - b.Baseline).Order().ElementAt((rows.Count - 1) / 2);
        foreach (var (index, neighbour) in new[] { (0, 1), (rows.Count - 1, rows.Count - 2) })
        {
            var apart = Math.Abs(rows[index].Baseline - rows[neighbour].Baseline) > EdgeApartPitchFactor * pitch;
            if (apart && CrossesWithWordSpace(rows[index], x))
                lines.Add(index);
        }

        return lines;
    }

    private static bool CrossesWithWordSpace(TextRow row, double x)
    {
        var gaps = row.Words.Zip(row.Words.Skip(1), (a, b) => b.Box.Left - a.Box.Right).Order().ToList();
        return gaps.Count >= MinEdgeLineGaps && row.Left < x && row.Right > x && !Crosses(row, x)
            && Gap(row, x) <= WordSpaceToleranceFactor * gaps[gaps.Count / 2];
    }

    private static double RightStart(TextRow row, double x) => row.Words.First(w => w.Box.Left >= x).Box.Left;

    private static double Gap(TextRow row, double x) => RightStart(row, x) - row.Words.Last(w => w.Box.Right <= x).Box.Right;

    // The x whose two-column bands hold most rows; ties go to the middle of the longest run of adjacent such x.
    private static double? FindGutter(IReadOnlyList<TextRow> rows, IReadOnlyList<GutterCandidate> candidates, double minColumnWidth, CancellationToken cancellationToken)
    {
        int bestScore = 0, bestRun = 0, run = 0, lastStep = int.MinValue;
        double bestX = 0, runStart = 0;
        foreach (var (x, step) in candidates)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var score = Bands(rows, Spanning(rows, x)).Where(b => IsTwoColumnBand(b, x, minColumnWidth)).Sum(b => b.Count);
            var adjacent = step == lastStep + 1;
            lastStep = step;
            if (score > bestScore)
            {
                (bestScore, bestRun, run, bestX, runStart) = (score, 1, 1, x, x);
                continue;
            }

            if (score != bestScore)
            {
                run = 0;
                continue;
            }

            (run, runStart) = adjacent && run > 0 ? (run + 1, runStart) : (1, x);
            if (score > 0 && run > bestRun)
                (bestRun, bestX) = (run, (runStart + x) / 2);
        }

        return bestScore > 0 && bestScore >= MinBandRowShare * rows.Count ? bestX : null;
    }

    // Maximal runs of consecutive rows that are not read across the gutter.
    private static IEnumerable<IReadOnlyList<TextRow>> Bands(IReadOnlyList<TextRow> rows, bool[] spanning)
    {
        var band = new List<TextRow>();
        for (var i = 0; i < rows.Count; i++)
        {
            if (!spanning[i])
            {
                band.Add(rows[i]);
                continue;
            }

            if (band.Count > 0)
                yield return band;
            band = new List<TextRow>();
        }

        if (band.Count > 0)
            yield return band;
    }

    private static IReadOnlyList<LayoutRegion> Regions(IReadOnlyList<TextRow> rows, double gutter, double minColumnWidth)
    {
        var regions = new List<LayoutRegion>();
        var full = new List<PositionedWord>();
        var spanning = Spanning(rows, gutter);
        for (var index = 0; index < rows.Count;)
        {
            var band = rows.Skip(index).TakeWhile((_, i) => !spanning[index + i]).ToList();
            if (band.Count > 0 && IsTwoColumnBand(band, gutter, minColumnWidth))
            {
                Flush(regions, full);
                var words = band.SelectMany(r => r.Words).ToList();
                regions.Add(new LayoutRegion(0, 2, TextRows.Group(words.Where(w => w.Box.Right <= gutter))));
                regions.Add(new LayoutRegion(1, 2, TextRows.Group(words.Where(w => w.Box.Left >= gutter))));
            }
            else
            {
                full.AddRange(band.SelectMany(r => r.Words));
            }

            index += band.Count;
            if (index < rows.Count)
                full.AddRange(rows[index++].Words);
        }

        Flush(regions, full);
        return regions;
    }

    private static void Flush(List<LayoutRegion> regions, List<PositionedWord> full)
    {
        if (full.Count == 0)
            return;

        regions.Add(new LayoutRegion(0, 1, TextRows.Group(full.ToList())));
        full.Clear();
    }

    private static bool IsTwoColumnBand(IReadOnlyList<TextRow> band, double gutter, double minColumnWidth)
    {
        var leftRows = band.Count(r => r.Left < gutter);
        var rightStarts = band.Where(r => r.Right > gutter).Select(r => RightStart(r, gutter)).ToList();
        if (leftRows < MinRowsPerColumn || rightStarts.Count < MinRowsPerColumn)
            return false;

        var words = band.SelectMany(r => r.Words).ToList();
        var leftWidth = words.Where(w => w.Box.Right <= gutter).Max(w => w.Box.Right) - words.Min(w => w.Box.Left);
        var rightWidth = words.Max(w => w.Box.Right) - rightStarts.Min();
        return leftWidth >= minColumnWidth
            && rightWidth >= minColumnWidth
            && RightMargin(rightStarts) is not null
            && HasGutterGap(band, gutter)
            && LyricRows(band, w => w.Box.Right <= gutter) >= MinLyricRowsPerColumn
            && LyricRows(band, w => w.Box.Left >= gutter) >= MinLyricRowsPerColumn;
    }

    private static int LyricRows(IReadOnlyList<TextRow> band, Func<PositionedWord, bool> side) =>
        band.Select(r => r.Words.Where(side).ToList())
            .Count(part => part.Count >= MinLyricWords
                && part.Sum(w => w.Text.Length) >= MinLyricCharacters
                && new TextRow(part).Coverage >= MinLyricCoverage);

    // Rows split by the gutter must mostly show a real gap there, not the space between two words of a sentence.
    private static bool HasGutterGap(IReadOnlyList<TextRow> band, double gutter)
    {
        var split = band.Where(r => r.Left < gutter && r.Right > gutter).ToList();
        if (split.Count < MinRowsForGapTest)
            return true;

        var narrow = split.Count(r => Gap(r, gutter) < MinGutterGapInHeights * r.Height);
        return narrow < MaxNarrowGapShare * split.Count;
    }

    // Left edge shared by the most row starts within the margin tolerance, when enough rows share it.
    private static double? RightMargin(IReadOnlyList<double> starts)
    {
        var sorted = starts.OrderBy(s => s).ToList();
        int best = 0, bestFirst = 0;
        for (int first = 0, last = 0; last < sorted.Count; last++)
        {
            while (sorted[last] - sorted[first] > MarginTolerancePoints)
                first++;
            if (last - first + 1 > best)
                (best, bestFirst) = (last - first + 1, first);
        }

        return best >= Math.Max(MinRowsAtMargin, MinShareAtMargin * sorted.Count) ? sorted[bestFirst] : null;
    }
}
