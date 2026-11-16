using System.Text;
using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.Application.Display;

/// <summary>Draws a laid-out song as monospace text: chords on a row above their lyrics, one blank line between sections.</summary>
internal static class LayoutTextProjector
{
    /// <summary>Columns wide enough that ordinary lines never wrap, which would break the chord alignment.</summary>
    public const double Columns = 2000;

    public static LayoutOptions Options { get; } = new()
    {
        Width = Columns,
        LineSpacing = 0,
        SectionSpacing = 0,
        ChordLyricSpacing = 0,
        MinChordGap = 1,
        PillPaddingX = 0,
        PillPaddingY = 0,
        ContinuationIndent = 0,
        LabelSpacing = 0
    };

    public static string Project(FlowLayout flow)
    {
        ArgumentNullException.ThrowIfNull(flow);
        var lines = new List<string>();
        foreach (var section in flow.Sections)
        {
            if (lines.Count > 0)
                lines.Add(string.Empty);

            foreach (var label in section.LabelRuns)
                lines.Add(label.Text);

            foreach (var row in section.Lines.SelectMany(l => l.Rows))
                AddRow(lines, row);
        }

        return string.Join('\n', lines);
    }

    private static void AddRow(List<string> lines, LayoutRow row)
    {
        var chords = Draw(row.Runs.Where(r => r.Kind is RunKind.Chord or RunKind.Annotation));
        if (chords.Length > 0)
            lines.Add(chords);

        var text = Draw(row.Runs.Where(r => r.Kind is not (RunKind.Chord or RunKind.Annotation or RunKind.LineNumber)));
        if (text.Length > 0)
            lines.Add(text);
    }

    // A run that would overlap the previous one is pushed right by one space.
    private static string Draw(IEnumerable<LayoutRun> runs)
    {
        var sb = new StringBuilder();
        foreach (var run in runs.OrderBy(r => r.X))
        {
            var column = (int)Math.Round(run.X);
            if (sb.Length > 0 && column <= sb.Length)
                column = sb.Length + (run.Kind is RunKind.Chord or RunKind.Annotation ? 1 : 0);

            sb.Append(' ', Math.Max(0, column - sb.Length)).Append(run.Text);
        }

        return sb.ToString().TrimEnd();
    }
}
