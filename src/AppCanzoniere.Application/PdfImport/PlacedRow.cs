using System.Text;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>Average character width of a region, used to turn x distances into character counts.</summary>
internal sealed record CharacterGrid(double Left, double CharWidth)
{
    // Lower bound of the character width, in points, so degenerate boxes never divide by zero.
    private const double MinCharWidthPoints = 0.5;

    // Column counts are clamped to this before rounding, far past any line yet well inside the range of an int.
    internal const double ColumnArithmeticLimit = 1_000_000;

    public static CharacterGrid For(IReadOnlyList<TextRow> rows)
    {
        var words = rows.SelectMany(r => r.Words).ToList();
        var chars = words.Sum(w => w.Text.Length);
        var width = words.Sum(w => w.Box.Width);
        return new CharacterGrid(words.Min(w => w.Box.Left), Math.Max(MinCharWidthPoints, chars == 0 ? 0 : width / chars));
    }

    public int Columns(double distance) => SafeRound(distance / CharWidth);

    internal static int SafeRound(double value) =>
        double.IsNaN(value) ? 0 : (int)Math.Round(Math.Clamp(value, -ColumnArithmeticLimit, ColumnArithmeticLimit), MidpointRounding.AwayFromZero);

    public int ColumnOf(double x) => Math.Max(0, Columns(x - Left));
}

/// <summary>A rendered row and where each of its words starts, so that other rows can map x positions onto it.</summary>
internal sealed record PlacedRow(TextRow Row, IReadOnlyList<int> Starts, string Text)
{

    // A chord within this share of an average letter width from a word start is placed at the start (OCR words only).
    private const double WordStartSnapShare = 0.6;

    // Lyrics: words keep their own text and are separated by as many spaces as their gap holds, at least one.
    public static PlacedRow Natural(TextRow row, CharacterGrid grid)
    {
        var starts = new List<int>();
        var column = Within(grid.ColumnOf(row.Left), 0, 0);
        var written = 0;
        for (var i = 0; i < row.Words.Count; i++)
        {
            if (i > 0)
                column = Within(column + Math.Max(1, grid.Columns(row.Words[i].Box.Left - row.Words[i - 1].Box.Right)), column + 1, written);
            starts.Add(column);
            column += row.Words[i].Text.Length;
            written += row.Words[i].Text.Length + 1;
        }

        return new PlacedRow(row, starts, Compose(row, starts));
    }

    // Chords: each word goes to the character of the anchor row found at its x, or to the grid when there is no anchor.
    public static PlacedRow Mapped(TextRow row, PlacedRow? anchor, CharacterGrid grid)
    {
        var starts = new List<int>();
        var next = 0;
        var written = 0;
        foreach (var word in row.Words)
        {
            var column = Within(anchor?.ColumnAt(word.Box.Left, grid) ?? grid.ColumnOf(word.Box.Left), next, written);
            starts.Add(column);
            next = column + word.Text.Length + 1;
            written += word.Text.Length + 1;
        }

        return new PlacedRow(row, starts, Compose(row, starts));
    }

    // Column of the character found at x in this row, interpolating inside words and across gaps.
    public int ColumnAt(double x, CharacterGrid grid)
    {
        var words = Row.Words;
        if (x <= words[0].Box.Left)
            return IsNearStart(words[0], x) ? Starts[0] : Math.Max(0, Starts[0] - grid.Columns(words[0].Box.Left - x));

        for (var i = 0; i < words.Count; i++)
        {
            var box = words[i].Box;
            var length = words[i].Text.Length;
            if (x < box.Right)
                return Starts[i] + LetterAt(words[i], x);

            if (i + 1 < words.Count && x < words[i + 1].Box.Left)
                return IsNearStart(words[i + 1], x)
                    ? Starts[i + 1]
                    : Interpolate(x, box.Right, Starts[i] + length, words[i + 1].Box.Left, Starts[i + 1]);
        }

        return Starts[^1] + words[^1].Text.Length + grid.Columns(x - words[^1].Box.Right);
    }

    // A word without letter positions (OCR) takes a chord slightly before or after its start as sitting at the start.
    private static bool IsNearStart(PositionedWord word, double x) =>
        word.LetterLefts is null && Math.Abs(x - word.Box.Left) <= WordStartSnapShare * word.Box.Width / word.Text.Length;

    // Index of the letter under x: the last letter starting at or before x, or, when unknown, the letter start nearest to x by glyph widths.
    private static int LetterAt(PositionedWord word, double x)
    {
        var length = word.Text.Length;
        if (word.LetterLefts is not { } lefts || lefts.Count != length)
            return IsNearStart(word, x) ? 0 : NearestLetter(word, x);

        var index = 0;
        while (index + 1 < length && lefts[index + 1] <= x)
            index++;
        return index;
    }

    private static int NearestLetter(PositionedWord word, double x)
    {
        var target = (x - word.Box.Left) / word.Box.Width * word.Text.Sum(GlyphWidths.Of);
        var best = 0;
        var bestDistance = double.MaxValue;
        var start = 0.0;
        for (var i = 0; i < word.Text.Length; i++)
        {
            if (Math.Abs(start - target) < bestDistance)
            {
                best = i;
                bestDistance = Math.Abs(start - target);
            }

            start += GlyphWidths.Of(word.Text[i]);
        }

        return best;
    }

    private static int Round(double value) => CharacterGrid.SafeRound(value);

    // A start no earlier than the earliest allowed, leaving at most MaxLineColumns spaces beyond one after each earlier word.
    private static int Within(int column, int earliest, int written) =>
        Math.Max(earliest, Math.Min(column, PdfPageLayout.MaxLineColumns + written));

    private static int Interpolate(double x, double x0, int c0, double x1, int c1) =>
        c0 + Round((x - x0) / (x1 - x0) * (c1 - c0));

    private static string Compose(TextRow row, IReadOnlyList<int> starts)
    {
        var text = new StringBuilder();
        for (var i = 0; i < row.Words.Count; i++)
        {
            text.Append(' ', Math.Max(0, starts[i] - text.Length));
            text.Append(row.Words[i].Text);
        }

        return text.ToString();
    }
}
