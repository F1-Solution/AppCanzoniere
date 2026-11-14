namespace AppCanzoniere.Application.PdfImport;

/// <summary>One monospaced output line and the row it came from; Row is null for a blank line between stanzas.</summary>
public sealed record AlignedLine(string Text, TextRow? Row);

/// <summary>Turns the rows of one region into monospaced lines that keep each word over the character it sits over on the page.</summary>
public static class AlignedTextRenderer
{
    // A row is dense (lyrics) when it has at least two words covering at least this share of its extent...
    private const double DenseCoverageShare = 0.6;
    private const int MinDenseWords = 2;

    // ...and no gap between words wider than this many row heights.
    private const double MaxDenseGapInHeights = 1.0;

    // A sparse row (chords) borrows the character positions of a dense neighbour at most this many row heights away.
    private const double MaxAnchorDistanceInHeights = 2.5;

    // A vertical gap larger than this multiple of the usual row pitch becomes a blank line.
    private const double BlankLinePitchFactor = 1.6;

    public static IReadOnlyList<AlignedLine> Render(IReadOnlyList<TextRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        if (rows.Count == 0)
            return Array.Empty<AlignedLine>();

        var grid = CharacterGrid.For(rows);
        var dense = rows.Select(IsDense).ToArray();
        var placed = rows.Select((row, i) => dense[i] ? PlacedRow.Natural(row, grid) : null).ToArray();
        for (var i = 0; i < rows.Count; i++)
            placed[i] ??= PlacedRow.Mapped(rows[i], Anchor(rows, dense, placed, i), grid);

        return WithBlankLines(rows, placed.Select(p => p!.Text).ToList());
    }

    private static bool IsDense(TextRow row) =>
        row.Words.Count >= MinDenseWords
        && row.Coverage >= DenseCoverageShare
        && row.Words.Zip(row.Words.Skip(1), (a, b) => b.Box.Left - a.Box.Right).Max() <= MaxDenseGapInHeights * row.Height;

    // The dense row just below, or else just above, close enough to be the lyric a chord row belongs to.
    private static PlacedRow? Anchor(IReadOnlyList<TextRow> rows, bool[] dense, PlacedRow?[] placed, int index)
    {
        foreach (var neighbour in new[] { index + 1, index - 1 })
        {
            if (neighbour < 0 || neighbour >= rows.Count || !dense[neighbour])
                continue;

            var distance = Math.Abs(rows[index].Baseline - rows[neighbour].Baseline);
            if (distance <= MaxAnchorDistanceInHeights * rows[index].Height)
                return placed[neighbour];
        }

        return null;
    }

    private static IReadOnlyList<AlignedLine> WithBlankLines(IReadOnlyList<TextRow> rows, IReadOnlyList<string> texts)
    {
        var pitches = rows.Zip(rows.Skip(1), (a, b) => a.Baseline - b.Baseline).Where(p => p > 0).OrderBy(p => p).ToList();
        var pitch = pitches.Count == 0 ? double.MaxValue : pitches[pitches.Count / 2];
        var lines = new List<AlignedLine> { new(texts[0], rows[0]) };
        for (var i = 1; i < rows.Count; i++)
        {
            if (rows[i - 1].Baseline - rows[i].Baseline > BlankLinePitchFactor * pitch)
                lines.Add(new AlignedLine(string.Empty, null));
            lines.Add(new AlignedLine(texts[i], rows[i]));
        }

        return lines;
    }
}
