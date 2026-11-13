namespace AppCanzoniere.Application.Layout;

/// <summary>Lays out one planned section: optional label, then its lines stacked from the section top.</summary>
internal static class SectionLayouter
{
    public static LayoutSection Layout(SectionPlan plan, LayoutSource source, double top, FlowContext ctx, ref int lastNumber)
    {
        var y = top;
        LayoutRun? labelRun = null;
        if (plan.Label is { } label)
        {
            labelRun = LabelRun(label, y, ctx);
            y = labelRun.Y + labelRun.Height + ctx.Options.LabelSpacing;
        }

        if (plan.Kind == SectionKind.Tab)
        {
            var block = TabLayouter.Layout(source, plan.LineIndexes, y, ctx);
            return new LayoutSection(plan.Kind, plan.Label, labelRun, top, block.Bottom - top, block.Scale, block.Clipped, false, false, block.Lines);
        }

        var lines = new List<LayoutLine>(plan.LineIndexes.Count);
        foreach (var index in plan.LineIndexes)
        {
            if (lines.Count > 0)
                y += ctx.Options.LineSpacing;

            var line = LayoutOne(plan.Kind, index, source.Lines[index], y, ctx, ref lastNumber);
            lines.Add(line);
            y = line.Y + line.Height;
        }

        return new LayoutSection(plan.Kind, plan.Label, labelRun, top, y - top, 1, false, false, false, Frozen.From(lines));
    }

    private static LayoutLine LayoutOne(SectionKind kind, int index, LayoutSourceLine line, double y, FlowContext ctx, ref int lastNumber)
    {
        if (kind == SectionKind.Comment)
            return CommentLine(index, line, y, ctx);

        int? number = LineNumbering.IsNumbered(line) ? ++lastNumber : null;
        var role = kind == SectionKind.Chorus ? TextRole.ChorusLyric : TextRole.Lyric;
        return LyricLineLayouter.Layout(index, line, number, y, role, ctx);
    }

    private static LayoutRun LabelRun(string label, double y, FlowContext ctx)
    {
        var metrics = ctx.Metrics(TextRole.SectionLabel);
        return new LayoutRun(
            RunKind.SectionLabel, label, TextRole.SectionLabel,
            ctx.Gutter, y, ctx.WidthOf(label, TextRole.SectionLabel), metrics.LineHeight,
            ctx.Gutter, y + metrics.Ascent, -1, false);
    }

    private static LayoutLine CommentLine(int index, LayoutSourceLine line, double y, FlowContext ctx)
    {
        var text = line.Text.Trim();
        var metrics = ctx.Metrics(TextRole.Comment);
        var run = new LayoutRun(
            RunKind.Comment, text, TextRole.Comment,
            ctx.Gutter, y, ctx.WidthOf(text, TextRole.Comment), metrics.LineHeight,
            ctx.Gutter, y + metrics.Ascent, 0, false);
        var row = new LayoutRow(0, y, metrics.LineHeight, Frozen.From(new[] { run }), false);
        return new LayoutLine(index, null, y, metrics.LineHeight, Frozen.From(new[] { row }));
    }
}
