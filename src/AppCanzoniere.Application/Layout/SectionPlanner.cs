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
    public static List<SectionPlan> Plan(LayoutSource source, LayoutDisplayMode mode) =>
        new Builder(source.Lines, mode).Run();

    private static bool IsLabelTarget(ChordProLineType type) =>
        type is ChordProLineType.Verse or ChordProLineType.Chorus or ChordProLineType.Bridge or ChordProLineType.Tab;

    private static SectionKind KindOf(ChordProLineType type) => type switch
    {
        ChordProLineType.Chorus => SectionKind.Chorus,
        ChordProLineType.Bridge => SectionKind.Bridge,
        ChordProLineType.Tab => SectionKind.Tab,
        _ => SectionKind.Verse,
    };

    private sealed class Builder
    {
        private readonly IReadOnlyList<LayoutSourceLine> _lines;
        private readonly LayoutDisplayMode _mode;
        private readonly List<SectionPlan> _plans = new();
        private SectionPlan? _current;
        private int _pendingIndex = -1;
        private SectionKind? _pendingKind;

        public Builder(IReadOnlyList<LayoutSourceLine> lines, LayoutDisplayMode mode)
        {
            _lines = lines;
            _mode = mode;
        }

        public List<SectionPlan> Run()
        {
            for (var i = 0; i < _lines.Count; i++)
            {
                switch (_lines[i].Type)
                {
                    case ChordProLineType.Empty:
                        FlushPending();
                        _current = null;
                        break;
                    case ChordProLineType.Comment:
                        AddComment(i);
                        break;
                    case ChordProLineType.Title:
                        break;
                    default:
                        AddLine(i);
                        break;
                }
            }

            FlushPending();
            return _plans;
        }

        private string? PendingLabel => _pendingIndex >= 0 ? _lines[_pendingIndex].Text.Trim() : null;

        private void AddComment(int index)
        {
            FlushPending();
            if (_lines[index].Text.Trim().Length == 0)
            {
                _current = null;
            }
            else if (IsInsideExplicitSection(index))
            {
                _current!.LineIndexes.Add(index);
            }
            else if (index + 1 < _lines.Count && IsLabelTarget(_lines[index + 1].Type))
            {
                _pendingIndex = index;
                _current = null;
            }
            else
            {
                AddStandaloneComment(index);
            }
        }

        /// <summary>A comment written inside an open chorus or bridge, or between two lines of one, belongs to that section.</summary>
        private bool IsInsideExplicitSection(int index)
        {
            if (_current is not { Kind: SectionKind.Chorus or SectionKind.Bridge })
                return false;

            var line = _lines[index];
            var next = index + 1 < _lines.Count ? _lines[index + 1] : null;
            if (line.StartsSection || next is { StartsSection: true })
                return false;

            if (line.InOpenSection)
                return true;

            var type = _current.Kind == SectionKind.Chorus ? ChordProLineType.Chorus : ChordProLineType.Bridge;
            return index > 0 && next is not null && _lines[index - 1].Type == type && next.Type == type;
        }

        private void AddStandaloneComment(int index)
        {
            if (_current is not { Kind: SectionKind.Comment })
            {
                _current = new SectionPlan(SectionKind.Comment, null);
                _plans.Add(_current);
            }

            _current.LineIndexes.Add(index);
        }

        /// <summary>A label whose section never came is shown as a comment of its own.</summary>
        private void FlushPending()
        {
            if (_pendingIndex < 0)
                return;

            var index = _pendingIndex;
            _pendingIndex = -1;
            _pendingKind = null;
            AddStandaloneComment(index);
        }

        private void AddLine(int index)
        {
            var line = _lines[index];
            var kind = KindOf(line.Type);
            if (IsDroppedInstrumental(line))
            {
                if (_pendingIndex >= 0)
                    _pendingKind ??= kind;
                return;
            }

            if (_pendingKind is { } expected && expected != kind)
                FlushPending();

            var label = PendingLabel;
            _pendingIndex = -1;
            _pendingKind = null;
            if (_current is null || _current.Kind != kind || line.StartsTabBlock || line.StartsSection)
            {
                var tabLabel = kind == SectionKind.Tab && line.TabLabel.Trim().Length > 0 ? line.TabLabel.Trim() : null;
                _current = new SectionPlan(kind, label ?? tabLabel);
                _plans.Add(_current);
            }

            _current.LineIndexes.Add(index);
        }

        private bool IsDroppedInstrumental(LayoutSourceLine line) =>
            _mode == LayoutDisplayMode.LyricsOnly
            && line.Type is ChordProLineType.Verse or ChordProLineType.Chorus or ChordProLineType.Bridge
            && line.Chords.Count > 0 && LineModel.IsBlank(line.Text);
    }
}
