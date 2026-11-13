using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Layout;

/// <summary>A chord or annotation text and its character offset in the line text.</summary>
public readonly record struct LayoutChord(string Text, int Position);

/// <summary>One immutable source line as the layout reads it.</summary>
public sealed record LayoutSourceLine(
    ChordProLineType Type, string Text, IReadOnlyList<LayoutChord> Chords, bool StartsTabBlock, string TabLabel);

/// <summary>An immutable snapshot of a parsed document, so later edits cannot change a layout in progress.</summary>
public sealed record LayoutSource(IReadOnlyList<LayoutSourceLine> Lines)
{
    /// <summary>Immutable snapshot of a parsed document; Title lines are dropped, chords sorted and clamped.</summary>
    public static LayoutSource From(ChordProDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var lines = new List<LayoutSourceLine>(document.Lines.Count);
        foreach (var line in document.Lines)
        {
            if (line.Type != ChordProLineType.Title)
                lines.Add(Copy(line));
        }

        return new LayoutSource(Frozen.From(lines));
    }

    private static LayoutSourceLine Copy(ChordProLine line)
    {
        var text = line.Text ?? string.Empty;
        var chords = (line.Chords ?? new List<ChordPosition>())
            .Select(c => new LayoutChord(c.Chord ?? string.Empty, Math.Clamp(c.Position, 0, text.Length)))
            .OrderBy(c => c.Position)
            .ToArray();
        return new LayoutSourceLine(line.Type, text, Frozen.From(chords), line.StartsTabBlock, line.TabLabel ?? string.Empty);
    }
}
