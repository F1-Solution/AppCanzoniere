using System.Text;

namespace AppCanzoniere.Application.Import;

/// <summary>Writes chord lines as ChordPro: merged into the lyric below or alone (spec 5.3, 5.4).</summary>
internal static class ChordMerger
{
    internal sealed record MergeResult(string Line, IReadOnlyList<SheetWarning> Warnings);

    /// <summary>Merged line plus warnings found while placing the chords; the lyric text is never altered.</summary>
    public static MergeResult Merge(ContentUnit chords, ContentUnit lyric, int snapColumns)
    {
        var elements = TextElements.Split(lyric.Text[lyric.Start..]);
        var warnings = new List<SheetWarning>();
        var inline = new List<(int Offset, SheetToken Token)>();
        var tail = new List<SheetToken>();

        // Chords after a section label are placed relative to the text that follows the label.
        var chordColumns = TextElements.ColumnMap(chords.Text);
        var chordOrigin = chords.Indented ? TextElements.ColumnMap(lyric.Text)[lyric.Start] : chordColumns[chords.Start];
        foreach (var token in chords.Class.Tokens)
        {
            var column = Math.Max(0, chordColumns[Math.Min(token.Column, chords.Text.Length)] - chordOrigin);
            if (column >= elements.Length)
            {
                tail.Add(token);
                continue;
            }

            var offset = Snap(elements, column, snapColumns, warnings);
            if (offset > 0 && elements[offset - 1] != " " && elements[offset] != " ")
                warnings.Add(SheetWarning.ChordInsideWord);
            inline.Add((offset, token));
        }

        return new MergeResult(Build(elements, inline, tail), warnings.Distinct().ToList());
    }

    /// <summary>The chord line alone: chords in brackets, other tokens as written (annotations when no chord is present), whitespace collapsed.</summary>
    public static string Instrumental(ContentUnit chords)
    {
        var builder = new StringBuilder();
        var previousEnd = -1;
        var annotate = chords.Class.Tokens.All(t => t.Kind != TokenKind.Chord);
        foreach (var token in chords.Class.Tokens)
        {
            if (previousEnd >= 0 && token.Column > previousEnd)
                builder.Append(' ');
            builder.Append(token.Kind == TokenKind.Chord ? $"[{token.Chord}]" : annotate ? Render(token) : token.Text);
            previousEnd = token.Column + token.Text.Length;
        }

        return builder.ToString();
    }

    private static int Snap(string[] elements, int column, int snapColumns, List<SheetWarning> warnings)
    {
        if (elements[column] != " ")
            return column;

        for (var step = 1; step <= snapColumns && column + step < elements.Length; step++)
        {
            var candidate = column + step;
            if (elements[candidate] != " " && elements[candidate - 1] == " ")
            {
                warnings.Add(SheetWarning.SnappedColumn);
                return candidate;
            }
        }

        return column;
    }

    private static string Build(string[] elements, List<(int Offset, SheetToken Token)> inline, List<SheetToken> tail)
    {
        var builder = new StringBuilder();
        var position = 0;
        foreach (var (offset, token) in inline.OrderBy(i => i.Offset))
        {
            builder.AppendJoin(string.Empty, elements[position..offset]).Append(Render(token));
            position = offset;
        }

        builder.AppendJoin(string.Empty, elements[position..]);
        foreach (var token in tail)
            builder.Append(' ').Append(Render(token));
        return builder.ToString();
    }

    private static string Render(SheetToken token) => token.Kind switch
    {
        TokenKind.Chord => $"[{token.Chord}]",
        _ when token.Text.StartsWith("[*", StringComparison.Ordinal) => token.Text,
        _ => $"[*{token.Text}]"
    };
}
