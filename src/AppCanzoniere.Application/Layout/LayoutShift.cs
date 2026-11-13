namespace AppCanzoniere.Application.Layout;

/// <summary>Copies layout elements moved vertically, or cut to a range of rows; the originals are never touched.</summary>
internal static class LayoutShift
{
    public static LayoutRun Run(LayoutRun run, double dy) =>
        run with { Y = run.Y + dy, BaselineY = run.BaselineY + dy };

    public static LayoutRow Row(LayoutRow row, double dy)
    {
        var runs = new LayoutRun[row.Runs.Count];
        for (var i = 0; i < runs.Length; i++)
            runs[i] = Run(row.Runs[i], dy);
        return row with { Y = row.Y + dy, Runs = Frozen.From(runs) };
    }

    public static LayoutLine Line(LayoutLine line, double dy)
    {
        var rows = new LayoutRow[line.Rows.Count];
        for (var i = 0; i < rows.Length; i++)
            rows[i] = Row(line.Rows[i], dy);
        return line with { Y = line.Y + dy, Rows = Frozen.From(rows) };
    }

    /// <summary>The rows from <paramref name="from"/> up to but not including <paramref name="to"/>; only the one holding row 0 keeps the number.</summary>
    public static LayoutLine Slice(LayoutLine line, int from, int to)
    {
        var rows = new LayoutRow[to - from];
        for (var i = 0; i < rows.Length; i++)
            rows[i] = line.Rows[from + i];
        var top = rows[0].Y;
        var last = rows[^1];
        return line with { Number = from == 0 ? line.Number : null, Y = top, Height = last.Y + last.Height - top, Rows = Frozen.From(rows) };
    }
}
