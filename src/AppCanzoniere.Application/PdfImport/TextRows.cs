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
    public static IReadOnlyList<TextRow> Group(IEnumerable<PositionedWord> words) => Array.Empty<TextRow>();
}
