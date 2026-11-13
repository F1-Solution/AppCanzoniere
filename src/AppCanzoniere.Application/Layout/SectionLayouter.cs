namespace AppCanzoniere.Application.Layout;

/// <summary>Lays out one planned section: optional label, then its lines stacked from the section top.</summary>
internal static class SectionLayouter
{
    public static LayoutSection Layout(SectionPlan plan, LayoutSource source, double top, FlowContext ctx, ref int lastNumber)
    {
        var y = top;
        IReadOnlyList<LayoutRun> labelRuns = Frozen.From(Array.Empty<LayoutRun>());
        if (plan.Label is { } label)
        {
            var rows = PlainTextLayouter.Rows(label, TextRole.SectionLabel, RunKind.SectionLabel, false, y, ctx);
            labelRuns = Frozen.From(rows.Select(r => r.Runs[0]).ToArray());
            y = rows[^1].Y + rows[^1].Height + ctx.Options.LabelSpacing;
        }

        if (plan.Kind == SectionKind.Tab)
        {
            var block = TabLayouter.Layout(source, plan.LineIndexes, y, ctx);
            return new LayoutSection(plan.Kind, plan.Label, labelRuns, top, block.Bottom - top, block.Scale, block.Clipped, false, false, block.Lines);
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

        return new LayoutSection(plan.Kind, plan.Label, labelRuns, top, y - top, 1, false, false, false, Frozen.From(lines));
    }

    private static LayoutLine LayoutOne(SectionKind kind, int index, LayoutSourceLine line, double y, FlowContext ctx, ref int lastNumber)
    {
        if (line.Type == Interfaces.ChordProLineType.Comment)
            return CommentLine(index, line, y, ctx);

        int? number = LineNumbering.IsNumbered(line) ? ++lastNumber : null;
        var role = kind == SectionKind.Chorus ? TextRole.ChorusLyric : TextRole.Lyric;
        return LyricLineLayouter.Layout(index, line, number, y, role, ctx);
    }

    private static LayoutLine CommentLine(int index, LayoutSourceLine line, double y, FlowContext ctx)
    {
        var rows = PlainTextLayouter.Rows(line.Text.Trim(), TextRole.Comment, RunKind.Comment, true, y, ctx);
        var height = rows[^1].Y + rows[^1].Height - y;
        return new LayoutLine(index, null, y, height, Frozen.From(rows.ToArray()), true);
    }
}
