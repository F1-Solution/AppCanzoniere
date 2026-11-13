using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Layout;

/// <summary>A chord or annotation text and its character offset in the line text.</summary>
public readonly record struct LayoutChord(string Text, int Position);

/// <summary>One immutable source line as the layout reads it; chords are copied, sorted by position and clamped to the text.</summary>
public sealed record LayoutSourceLine
{
    public LayoutSourceLine(ChordProLineType type, string text, IReadOnlyList<LayoutChord> chords, bool startsTabBlock, string tabLabel, int documentIndex = -1, bool startsSection = false, bool inOpenSection = false)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(chords);
        ArgumentNullException.ThrowIfNull(tabLabel);
        if (documentIndex < -1)
            throw new ArgumentException("A document index is not negative; -1 leaves it to the position.", nameof(documentIndex));

        Type = type;
        Text = text;
        var (sorted, ordinals) = Normalise(chords, text.Length);
        Chords = Frozen.From(sorted);
        ChordOrdinals = Frozen.From(ordinals);
        StartsTabBlock = startsTabBlock;
        TabLabel = tabLabel;
        DocumentIndex = documentIndex;
        StartsSection = startsSection;
        InOpenSection = inOpenSection;
    }

    public ChordProLineType Type { get; }

    public string Text { get; }

    public IReadOnlyList<LayoutChord> Chords { get; }

    public bool StartsTabBlock { get; }

    public string TabLabel { get; }

    /// <summary>Index of the line in the parsed document's lines, titles included; the position in the source when hand-built.</summary>
    public int DocumentIndex { get; init; }

    /// <summary>For each chord of <see cref="Chords"/>, its index in the list the caller gave.</summary>
    internal IReadOnlyList<int> ChordOrdinals { get; }

    /// <summary>True on the first line of an explicit chorus or bridge; a comment before it labels it instead of joining the previous section.</summary>
    public bool StartsSection { get; }

    /// <summary>True on a comment written inside an open chorus or bridge, even as its last line.</summary>
    public bool InOpenSection { get; }

    internal LayoutSourceLine WithDocumentIndex(int index) => this with { DocumentIndex = index };

    private static (LayoutChord[] Sorted, int[] Ordinals) Normalise(IReadOnlyList<LayoutChord> chords, int textLength)
    {
        var copy = new LayoutChord[chords.Count];
        for (var i = 0; i < copy.Length; i++)
        {
            var chord = chords[i];
            if (chord.Text is null)
                throw new ArgumentException($"The chord at {i} has no text.", nameof(chords));

            copy[i] = chord with { Position = Math.Clamp(chord.Position, 0, textLength) };
        }

        var order = Enumerable.Range(0, copy.Length).OrderBy(i => copy[i].Position).ToArray();
        return (order.Select(i => copy[i]).ToArray(), order);
    }
}

/// <summary>An immutable snapshot of a parsed document, so later edits cannot change a layout in progress.</summary>
public sealed record LayoutSource
{
    public LayoutSource(IReadOnlyList<LayoutSourceLine> lines)
    {
        var copy = Frozen.Copy(lines, nameof(lines));
        var indexed = new LayoutSourceLine[copy.Count];
        var seen = new HashSet<int>();
        for (var i = 0; i < indexed.Length; i++)
        {
            indexed[i] = copy[i].DocumentIndex >= 0 ? copy[i] : copy[i].WithDocumentIndex(i);
            if (!seen.Add(indexed[i].DocumentIndex))
                throw new ArgumentException($"The document index {indexed[i].DocumentIndex} is used by more than one line.", nameof(lines));
        }

        Lines = Frozen.From(indexed);
    }

    public IReadOnlyList<LayoutSourceLine> Lines { get; }

    /// <summary>Immutable snapshot of a parsed document; Title lines are dropped, chords sorted and clamped.</summary>
    public static LayoutSource From(ChordProDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var lines = new List<LayoutSourceLine>(document.Lines.Count);
        for (var i = 0; i < document.Lines.Count; i++)
        {
            if (document.Lines[i].Type != ChordProLineType.Title)
                lines.Add(Copy(document.Lines[i], i));
        }

        return new LayoutSource(lines);
    }

    private static LayoutSourceLine Copy(ChordProLine line, int documentIndex)
    {
        var chords = (line.Chords ?? new List<ChordPosition>())
            .Select(c => new LayoutChord(c.Chord ?? string.Empty, c.Position))
            .ToArray();
        return new LayoutSourceLine(line.Type, line.Text ?? string.Empty, chords, line.StartsTabBlock, line.TabLabel ?? string.Empty, documentIndex, line.StartsSection, line.InOpenSection);
    }
}
