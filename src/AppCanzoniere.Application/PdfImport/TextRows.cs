namespace AppCanzoniere.Application.PdfImport;

/// <summary>Words sharing a baseline, ordered left to right.</summary>
public sealed record TextRow(IReadOnlyList<PositionedWord> Words)
{
    public double Baseline => Words[0].Box.Bottom;

    public double Left => Words[0].Box.Left;

    public double Right => Words.Max(w => w.Box.Right);

    public double Height => Words.Max(w => w.Box.Height);

    /// <summary>Share of the row extent covered by words: low for chords far apart, high for lyrics.</summary>
    public double Coverage => Right - Left <= 0 ? 1 : Words.Sum(w => w.Box.Width) / (Right - Left);
}

/// <summary>Groups words into rows by baseline, top of the page first.</summary>
public static class TextRows
{
    // A word joins a row when its baseline is within this share of the taller word's height.
    private const double BaselineToleranceShare = 0.3;

    // Lower bound of the height used for the tolerance, in points, for words with a degenerate box.
    private const double MinHeightPoints = 1.0;

    public static IReadOnlyList<TextRow> Group(IEnumerable<PositionedWord> words)
    {
        ArgumentNullException.ThrowIfNull(words);
        var rows = new List<List<PositionedWord>>();
        foreach (var word in words.OrderByDescending(w => w.Box.Bottom).ThenBy(w => w.Box.Left))
        {
            if (rows.Count > 0 && SameBaseline(rows[^1][0], word))
                rows[^1].Add(word);
            else
                rows.Add(new List<PositionedWord> { word });
        }

        return rows.Select(r => new TextRow(r.OrderBy(w => w.Box.Left).ToList())).ToList();
    }

    private static bool SameBaseline(PositionedWord first, PositionedWord word)
    {
        var height = Math.Max(MinHeightPoints, Math.Max(first.Box.Height, word.Box.Height));
        return Math.Abs(first.Box.Bottom - word.Box.Bottom) <= BaselineToleranceShare * height;
    }
}
