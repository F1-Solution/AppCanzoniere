using System.Text;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>Average character width of a region, used to turn x distances into character counts.</summary>
internal sealed record CharacterGrid(double Left, double CharWidth)
{
    // Lower bound of the character width, in points, so degenerate boxes never divide by zero.
    private const double MinCharWidthPoints = 0.5;

    public static CharacterGrid For(IReadOnlyList<TextRow> rows)
    {
        var words = rows.SelectMany(r => r.Words).ToList();
        var chars = words.Sum(w => w.Text.Length);
        var width = words.Sum(w => w.Box.Width);
        return new CharacterGrid(words.Min(w => w.Box.Left), Math.Max(MinCharWidthPoints, chars == 0 ? 0 : width / chars));
    }

    public int Columns(double distance) => (int)Math.Round(distance / CharWidth, MidpointRounding.AwayFromZero);

    public int ColumnOf(double x) => Math.Max(0, Columns(x - Left));
}

/// <summary>A rendered row and where each of its words starts, so that other rows can map x positions onto it.</summary>
internal sealed record PlacedRow(TextRow Row, IReadOnlyList<int> Starts, string Text)
{
    // Lyrics: words keep their own text and are separated by as many spaces as their gap holds, at least one.
    public static PlacedRow Natural(TextRow row, CharacterGrid grid)
    {
        var starts = new List<int>();
        var column = grid.ColumnOf(row.Left);
        for (var i = 0; i < row.Words.Count; i++)
        {
            if (i > 0)
                column += Math.Max(1, grid.Columns(row.Words[i].Box.Left - row.Words[i - 1].Box.Right));
            starts.Add(column);
            column += row.Words[i].Text.Length;
        }

        return new PlacedRow(row, starts, Compose(row, starts));
    }

    // Chords: each word goes to the character of the anchor row found at its x, or to the grid when there is no anchor.
    public static PlacedRow Mapped(TextRow row, PlacedRow? anchor, CharacterGrid grid)
    {
        var starts = new List<int>();
        var next = 0;
        foreach (var word in row.Words)
        {
            var column = Math.Max(next, anchor?.ColumnAt(word.Box.Left, grid) ?? grid.ColumnOf(word.Box.Left));
            starts.Add(column);
            next = column + word.Text.Length + 1;
        }

        return new PlacedRow(row, starts, Compose(row, starts));
    }

    // Column of the character found at x in this row, interpolating inside words and across gaps.
    public int ColumnAt(double x, CharacterGrid grid)
    {
        var words = Row.Words;
        if (x <= words[0].Box.Left)
            return Math.Max(0, Starts[0] - grid.Columns(words[0].Box.Left - x));

        for (var i = 0; i < words.Count; i++)
        {
            var box = words[i].Box;
            var length = words[i].Text.Length;
            if (x < box.Right)
                return Starts[i] + Math.Min(length - 1, Round((x - box.Left) / (box.Width / length)));

            if (i + 1 < words.Count && x < words[i + 1].Box.Left)
                return Interpolate(x, box.Right, Starts[i] + length, words[i + 1].Box.Left, Starts[i + 1]);
        }

        return Starts[^1] + words[^1].Text.Length + grid.Columns(x - words[^1].Box.Right);
    }

    private static int Round(double value) => (int)Math.Round(value, MidpointRounding.AwayFromZero);

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
