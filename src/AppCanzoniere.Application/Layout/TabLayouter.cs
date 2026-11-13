namespace AppCanzoniere.Application.Layout;

/// <summary>The lines of a tab block with the scale they share.</summary>
internal sealed record TabBlock(IReadOnlyList<LayoutLine> Lines, double Scale, bool Clipped, double Bottom);

/// <summary>Lays out a tab block: verbatim monospace lines, never wrapped, scaled together to fit the width.</summary>
internal static class TabLayouter
{
    public static TabBlock Layout(LayoutSource source, IReadOnlyList<int> indexes, double top, FlowContext ctx)
    {
        var widths = new double[indexes.Count];
        var widest = 0.0;
        for (var i = 0; i < widths.Length; i++)
        {
            widths[i] = ctx.WidthOf(source.Lines[indexes[i]].Text, TextRole.Tab);
            widest = Math.Max(widest, widths[i]);
        }

        var (scale, clipped) = Scale(widest, ctx);
        var metrics = ctx.Metrics(TextRole.Tab);
        var lines = new List<LayoutLine>(widths.Length);
        var y = top;
        for (var i = 0; i < widths.Length; i++)
        {
            var height = metrics.LineHeight * scale;
            var run = new LayoutRun(
                RunKind.Tab, source.Lines[indexes[i]].Text, TextRole.Tab,
                ctx.Gutter, y, widths[i] * scale, height,
                ctx.Gutter, y + metrics.Ascent * scale, 0, false);
            var row = new LayoutRow(0, y, height, Frozen.From(new[] { run }), false);
            lines.Add(new LayoutLine(indexes[i], null, y, height, Frozen.From(new[] { row })));
            y += height;
        }

        return new TabBlock(Frozen.From(lines), scale, clipped, y);
    }

    private static (double Scale, bool Clipped) Scale(double widest, FlowContext ctx)
    {
        var scale = widest <= 0 ? 1 : Math.Min(1, ctx.Available / widest);
        return scale < ctx.Options.MinTabScale ? (ctx.Options.MinTabScale, true) : (scale, false);
    }
}
