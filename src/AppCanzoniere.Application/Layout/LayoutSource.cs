using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Layout;

/// <summary>A chord or annotation text and its character offset in the line text.</summary>
public readonly record struct LayoutChord(string Text, int Position);

/// <summary>One immutable source line as the layout reads it; chords are copied, sorted by position and clamped to the text.</summary>
public sealed record LayoutSourceLine
{
    public LayoutSourceLine(ChordProLineType type, string text, IReadOnlyList<LayoutChord> chords, bool startsTabBlock, string tabLabel)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(chords);
        ArgumentNullException.ThrowIfNull(tabLabel);
        Type = type;
        Text = text;
        Chords = Frozen.From(Normalise(chords, text.Length));
        StartsTabBlock = startsTabBlock;
        TabLabel = tabLabel;
    }

    public ChordProLineType Type { get; }

    public string Text { get; }

    public IReadOnlyList<LayoutChord> Chords { get; }

    public bool StartsTabBlock { get; }

    public string TabLabel { get; }

    private static LayoutChord[] Normalise(IReadOnlyList<LayoutChord> chords, int textLength)
    {
        var copy = new LayoutChord[chords.Count];
        for (var i = 0; i < copy.Length; i++)
        {
            var chord = chords[i];
            if (chord.Text is null)
                throw new ArgumentException($"The chord at {i} has no text.", nameof(chords));

            copy[i] = chord with { Position = Math.Clamp(chord.Position, 0, textLength) };
        }

        return copy.OrderBy(c => c.Position).ToArray();
    }
}

/// <summary>An immutable snapshot of a parsed document, so later edits cannot change a layout in progress.</summary>
public sealed record LayoutSource
{
    public LayoutSource(IReadOnlyList<LayoutSourceLine> lines)
    {
        Lines = Frozen.Copy(lines, nameof(lines));
    }

    public IReadOnlyList<LayoutSourceLine> Lines { get; }

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

        return new LayoutSource(lines);
    }

    private static LayoutSourceLine Copy(ChordProLine line)
    {
        var chords = (line.Chords ?? new List<ChordPosition>())
            .Select(c => new LayoutChord(c.Chord ?? string.Empty, c.Position))
            .ToArray();
        return new LayoutSourceLine(line.Type, line.Text ?? string.Empty, chords, line.StartsTabBlock, line.TabLabel ?? string.Empty);
    }
}
