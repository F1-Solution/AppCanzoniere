using System.Text;

namespace AppCanzoniere.Application.Import;

/// <summary>Writes chord lines as ChordPro: merged into the lyric below or alone (spec 5.3, 5.4).</summary>
internal static class ChordMerger
{
    internal sealed record MergeResult(string Line, IReadOnlyList<SheetWarning> Warnings);

    /// <summary>Merged line plus warnings found while placing the chords; the lyric text is never altered.</summary>
    public static MergeResult Merge(ContentUnit chords, ContentUnit lyric, int snapColumns)
    {
        var text = lyric.Text[lyric.Start..];
        var warnings = new List<SheetWarning>();
        var inline = new List<(int Offset, SheetToken Token)>();
        var tail = new List<SheetToken>();

        // Chords after a section label are placed relative to the text that follows the label.
        var chordOrigin = chords.Indented ? lyric.Start : chords.Start;
        foreach (var token in chords.Class.Tokens)
        {
            var column = Math.Max(0, token.Column - chordOrigin);
            if (column >= text.Length)
            {
                tail.Add(token);
                continue;
            }

            var offset = BeforeBracket(text, Snap(text, column, snapColumns, warnings));
            if (offset > 0 && text[offset - 1] != ' ' && text[offset] != ' ')
                warnings.Add(SheetWarning.ChordInsideWord);
            inline.Add((offset, token));
        }

        return new MergeResult(Build(text, inline, tail), warnings.Distinct().ToList());
    }

    /// <summary>The chord line alone: chords in brackets, other tokens as written, whitespace collapsed.</summary>
    public static string Instrumental(ContentUnit chords)
    {
        var builder = new StringBuilder();
        var previousEnd = -1;
        foreach (var token in chords.Class.Tokens)
        {
            if (previousEnd >= 0 && token.Column > previousEnd)
                builder.Append(' ');
            builder.Append(token.Kind == TokenKind.Chord ? $"[{token.Chord}]" : token.Text);
            previousEnd = token.Column + token.Text.Length;
        }

        return builder.ToString();
    }

    // A chord never goes inside an existing [chord]: it is placed right before its opening bracket.
    private static int BeforeBracket(string text, int offset)
    {
        if (offset == 0)
            return offset;

        var open = text.LastIndexOf('[', offset - 1);
        return open >= 0 && open > text.LastIndexOf(']', offset - 1) ? open : offset;
    }

    private static int Snap(string text, int column, int snapColumns, List<SheetWarning> warnings)
    {
        if (text[column] != ' ')
            return column;

        for (var step = 1; step <= snapColumns && column + step < text.Length; step++)
        {
            var candidate = column + step;
            if (text[candidate] != ' ' && text[candidate - 1] == ' ')
            {
                warnings.Add(SheetWarning.SnappedColumn);
                return candidate;
            }
        }

        return column;
    }

    private static string Build(string text, List<(int Offset, SheetToken Token)> inline, List<SheetToken> tail)
    {
        var builder = new StringBuilder();
        var position = 0;
        foreach (var (offset, token) in inline.OrderBy(i => i.Offset))
        {
            builder.Append(text, position, offset - position).Append(Render(token));
            position = offset;
        }

        builder.Append(text, position, text.Length - position);
        foreach (var token in tail)
            builder.Append(' ').Append(Render(token));
        return builder.ToString();
    }

    private static string Render(SheetToken token) => token.Kind switch
    {
        TokenKind.Chord => $"[{token.Chord}]",
        _ => $"[*{token.Text}]"
    };
}
