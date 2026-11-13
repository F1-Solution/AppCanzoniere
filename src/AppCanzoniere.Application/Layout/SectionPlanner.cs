using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Layout;

/// <summary>A section before layout: its kind, optional label and the source lines it holds.</summary>
internal sealed class SectionPlan
{
    public SectionPlan(SectionKind kind, string? label)
    {
        Kind = kind;
        Label = label;
    }

    public SectionKind Kind { get; }

    public string? Label { get; }

    public List<int> LineIndexes { get; } = new();
}

/// <summary>Groups source lines into sections and turns a comment before a section into its label.</summary>
internal static class SectionPlanner
{
    public static List<SectionPlan> Plan(LayoutSource source, LayoutDisplayMode mode)
    {
        var lines = source.Lines;
        var plans = new List<SectionPlan>();
        SectionPlan? current = null;
        string? pendingLabel = null;

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            switch (line.Type)
            {
                case ChordProLineType.Empty:
                    current = null;
                    break;
                case ChordProLineType.Comment:
                    (current, pendingLabel) = AddComment(plans, current, lines, i);
                    break;
                case ChordProLineType.Title:
                    break;
                default:
                    if (IsDroppedInstrumental(line, mode))
                    {
                        pendingLabel = null;
                        break;
                    }

                    current = AddBodyLine(plans, current, line, i, pendingLabel);
                    pendingLabel = null;
                    break;
            }
        }

        return plans;
    }

    private static (SectionPlan? Current, string? PendingLabel) AddComment(
        List<SectionPlan> plans, SectionPlan? current, IReadOnlyList<LayoutSourceLine> lines, int index)
    {
        var text = lines[index].Text.Trim();
        if (text.Length == 0)
            return (null, null);

        if (index + 1 < lines.Count && IsLabelTarget(lines[index + 1].Type))
            return (null, text);

        if (current is not { Kind: SectionKind.Comment })
        {
            current = new SectionPlan(SectionKind.Comment, null);
            plans.Add(current);
        }

        current.LineIndexes.Add(index);
        return (current, null);
    }

    private static SectionPlan AddBodyLine(
        List<SectionPlan> plans, SectionPlan? current, LayoutSourceLine line, int index, string? pendingLabel)
    {
        var kind = KindOf(line.Type);
        if (current is null || current.Kind != kind || line.StartsTabBlock)
        {
            var tabLabel = kind == SectionKind.Tab && line.TabLabel.Trim().Length > 0 ? line.TabLabel.Trim() : null;
            current = new SectionPlan(kind, pendingLabel ?? tabLabel);
            plans.Add(current);
        }

        current.LineIndexes.Add(index);
        return current;
    }

    private static bool IsDroppedInstrumental(LayoutSourceLine line, LayoutDisplayMode mode) =>
        mode == LayoutDisplayMode.LyricsOnly
        && line.Type is ChordProLineType.Verse or ChordProLineType.Chorus or ChordProLineType.Bridge
        && line.Chords.Count > 0 && LineModel.IsBlank(line.Text);

    private static bool IsLabelTarget(ChordProLineType type) =>
        type is ChordProLineType.Verse or ChordProLineType.Chorus or ChordProLineType.Bridge or ChordProLineType.Tab;

    private static SectionKind KindOf(ChordProLineType type) => type switch
    {
        ChordProLineType.Chorus => SectionKind.Chorus,
        ChordProLineType.Bridge => SectionKind.Bridge,
        ChordProLineType.Tab => SectionKind.Tab,
        _ => SectionKind.Verse,
    };
}
