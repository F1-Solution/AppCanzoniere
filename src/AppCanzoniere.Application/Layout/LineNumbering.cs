using System.Globalization;

namespace AppCanzoniere.Application.Layout;

/// <summary>Which lines carry a number and how wide the number gutter is.</summary>
internal static class LineNumbering
{
    /// <summary>A verse, chorus or bridge line with text; instrumental lines are never numbered.</summary>
    public static bool IsNumbered(LayoutSourceLine line) =>
        !LineModel.IsBlank(line.Text) && line.Type is not (Interfaces.ChordProLineType.Tab or Interfaces.ChordProLineType.Comment);

    public static int Count(LayoutSource source, IReadOnlyList<SectionPlan> plans)
    {
        var count = 0;
        foreach (var plan in plans)
        {
            if (plan.Kind is SectionKind.Tab or SectionKind.Comment)
                continue;

            foreach (var index in plan.LineIndexes)
            {
                if (!LineModel.IsBlank(source.Lines[index].Text))
                    count++;
            }
        }

        return count;
    }

    public static double GutterWidth(int numberedCount, LayoutOptions options, ITextMeasurer measurer)
    {
        if (!options.LineNumbers || numberedCount == 0)
            return 0;

        var widest = numberedCount.ToString(CultureInfo.InvariantCulture);
        return measurer.Measure(widest, TextRole.LineNumber).Width + options.GutterGap;
    }
}
