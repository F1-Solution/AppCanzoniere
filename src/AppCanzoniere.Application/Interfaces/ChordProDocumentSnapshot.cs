namespace AppCanzoniere.Application.Interfaces;

/// <summary>Copy of a parsed document's typed content, used to tell whether a caller has edited it since.</summary>
internal sealed class ChordProDocumentSnapshot
{
    private readonly string?[] _fields;
    private readonly LineCopy[] _lines;

    private ChordProDocumentSnapshot(string?[] fields, LineCopy[] lines)
    {
        _fields = fields;
        _lines = lines;
    }

    public static ChordProDocumentSnapshot Of(ChordProDocument document) =>
        new(Fields(document), (document.Lines ?? new List<ChordProLine>()).Select(LineCopy.Of).ToArray());

    /// <summary>True when the document still holds exactly the copied content.</summary>
    public bool Matches(ChordProDocument document) =>
        LinesMatch(document) && Fields(document).AsSpan().SequenceEqual(_fields);

    /// <summary>The header values that differ from the copy, in field order.</summary>
    public IReadOnlyList<HeaderEdit> HeaderEdits(ChordProDocument document)
    {
        var now = Fields(document);
        var edits = new List<HeaderEdit>();
        for (var i = 0; i < now.Length; i++)
        {
            if (now[i] != _fields[i])
                edits.Add(new HeaderEdit((HeaderField)i, now[i] ?? string.Empty));
        }

        return edits;
    }

    /// <summary>True when every line of the document is as copied, whatever the header values.</summary>
    public bool LinesMatch(ChordProDocument document)
    {
        if (document.Lines is not { } lines || lines.Count != _lines.Length)
            return false;

        for (var i = 0; i < _lines.Length; i++)
        {
            if (!_lines[i].Matches(lines[i]))
                return false;
        }

        return true;
    }

    private static string?[] Fields(ChordProDocument d) => new[] { d.Title, d.Artist, d.Key, d.Capo, d.Tempo, d.Time };

    private sealed record LineCopy(ChordProLineType Type, string? Text, bool StartsTabBlock, string? TabLabel, bool StartsSection, bool InOpenSection, (string? Chord, int Position)[] Chords)
    {
        public static LineCopy Of(ChordProLine line) =>
            new(line.Type, line.Text, line.StartsTabBlock, line.TabLabel, line.StartsSection, line.InOpenSection, (line.Chords ?? new List<ChordPosition>()).Select(c => ((string?)c.Chord, c.Position)).ToArray());

        public bool Matches(ChordProLine line)
        {
            var chords = line.Chords ?? new List<ChordPosition>();
            if (line.Type != Type || line.Text != Text || line.StartsTabBlock != StartsTabBlock || line.TabLabel != TabLabel || line.StartsSection != StartsSection || line.InOpenSection != InOpenSection || chords.Count != Chords.Length)
                return false;

            for (var i = 0; i < Chords.Length; i++)
            {
                if (chords[i].Chord != Chords[i].Chord || chords[i].Position != Chords[i].Position)
                    return false;
            }

            return true;
        }
    }
}

/// <summary>A typed header value of a document, in the order the snapshot copies them.</summary>
internal enum HeaderField
{
    Title,
    Artist,
    Key,
    Capo,
    Tempo,
    Time
}

/// <summary>A header value changed since the document was parsed.</summary>
internal sealed record HeaderEdit(HeaderField Field, string Value);
