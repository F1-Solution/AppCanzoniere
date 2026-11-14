namespace AppCanzoniere.Application.Layout;

/// <summary>Lays out chordless text such as a comment or a section label as wrapped rows of one run each.</summary>
internal static class PlainTextLayouter
{
    /// <summary>Rows of <paramref name="text"/> from <paramref name="top"/>; run offsets count from <paramref name="offsetBase"/>, or are -1 when it is null.</summary>
    public static List<LayoutRow> Rows(string text, TextRole role, RunKind kind, int? offsetBase, double top, FlowContext ctx)
    {
        var width = ctx.WidthOf(text, role);
        var model = new LineModel(text, Array.Empty<LayoutChord>(), Array.Empty<int>(), role, false);
        var whole = new List<Seg> { new(0, text.Length, -1, width, 0) };
        var specs = width <= ctx.Available + SegmentMath.FitTolerance
            ? new List<RowSpec> { new(whole, false) }
            : RowWrapper.Wrap(model, whole, ctx);

        var metrics = ctx.Metrics(role);
        var rows = new List<LayoutRow>(specs.Count);
        var y = top;
        for (var i = 0; i < specs.Count; i++)
        {
            var seg = specs[i].Segments[0];
            var x = ctx.Gutter + (i > 0 ? ctx.ContinuationIndent : 0);
            var run = new LayoutRun(
                kind, text.Substring(seg.Start, seg.End - seg.Start), role,
                x, y, seg.LyricWidth, metrics.LineHeight,
                x, y + metrics.Ascent, offsetBase is { } start ? start + seg.Start : -1, false);
            rows.Add(new LayoutRow(i, y, metrics.LineHeight, Frozen.From(new[] { run }), specs[i].HardBreakAfter, ctx.Exceeds(x + seg.LyricWidth)));
            y += metrics.LineHeight + ctx.Options.LineSpacing;
        }

        return rows;
    }
}
