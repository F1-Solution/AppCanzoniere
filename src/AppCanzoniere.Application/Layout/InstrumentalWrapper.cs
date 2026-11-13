namespace AppCanzoniere.Application.Layout;

/// <summary>Wraps a chords-only line into rows, each chord or annotation being an unbreakable atom.</summary>
internal static class InstrumentalWrapper
{
    public static List<RowSpec> Wrap(IReadOnlyList<Seg> segments, FlowContext ctx)
    {
        var atoms = segments.Where(s => s.ChordIndex >= 0).ToList();
        var rows = new List<RowSpec>();
        var start = 0;
        while (start < atoms.Count)
        {
            var available = rows.Count == 0 ? ctx.Available : ctx.Available - ctx.Options.ContinuationIndent;
            var end = RowEnd(atoms, start, available, ctx.Options.MinChordGap);
            var row = atoms.GetRange(start, end - start);
            row[^1] = row[^1] with { LyricWidth = 0 };
            rows.Add(new RowSpec(row, false));
            start = end;
        }

        return rows;
    }

    /// <summary>End of the row starting at <paramref name="start"/>; a chord that cannot fit still takes a row alone.</summary>
    private static int RowEnd(List<Seg> atoms, int start, double available, double gap)
    {
        var used = 0.0;
        for (var k = start; k < atoms.Count; k++)
        {
            if (used + atoms[k].ChordWidth > available + SegmentMath.FitTolerance)
                return k == start ? k + 1 : k;

            used += SegmentMath.Advance(atoms[k], false, gap);
        }

        return atoms.Count;
    }
}
