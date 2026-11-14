using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>Tokens read from a bracketed group such as <c>[Do]</c> or <c>[*N.C.]</c>, and the text length they cover.</summary>
internal sealed record TokenGroup(int Length, IReadOnlyList<SheetToken> Tokens);

/// <summary>Finds bracketed or parenthesised groups that are chords or annotations on a line, so they never read as words.</summary>
internal static class TokenGroups
{
    private static readonly Regex Candidates = new(@"\[[^\[\]]*\]|\([^()\[\]]*\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex Parts = new(@"\S+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static Dictionary<int, TokenGroup> Find(string text, IReadOnlyDictionary<int, Match> marks, Notation notation)
    {
        var found = new Dictionary<int, TokenGroup>();
        foreach (Match m in Candidates.Matches(text))
        {
            if (!marks.ContainsKey(m.Index) && EndsAtBoundary(text, m) && Read(m, notation) is { } tokens)
                found[m.Index] = new TokenGroup(m.Length, tokens);
        }

        return found;
    }

    // A group glued to letters belongs to a word, not to the chord line.
    private static bool EndsAtBoundary(string text, Match m)
    {
        var end = m.Index + m.Length;
        return end >= text.Length || text[end] is ' ' or '-' or '–';
    }

    private static IReadOnlyList<SheetToken>? Read(Match m, Notation notation) =>
        m.Value[0] == '(' ? ReadParenthesised(m, notation) : ReadBracketed(m, notation);

    // A single unspaced "(Do)" stays with the plain token path; spaced or multi-chord groups are split here.
    private static IReadOnlyList<SheetToken>? ReadParenthesised(Match m, Notation notation)
    {
        var inner = m.Value[1..^1];
        if (!inner.Any(char.IsWhiteSpace))
            return null;

        // A separator after a chord, as in "(Si, Re)", is dropped.
        var parts = Parts.Matches(inner).Select(p => (Text: p.Value.TrimEnd(',', ';'), p.Index)).ToList();
        if (parts.Count == 0 || parts.Any(p => !SheetTokenizer.TryChord(p.Text, notation)))
            return null;

        return parts.Select(p => SheetTokenizer.Chord(p.Text, m.Index + 1 + p.Index, p.Text, notation, true)).ToList();
    }

    private static IReadOnlyList<SheetToken>? ReadBracketed(Match m, Notation notation)
    {
        var inner = m.Value[1..^1];
        if (BracketedText.IsAnnotation(inner))
            return new[] { new SheetToken(m.Value, m.Index, TokenKind.Neutral) };

        var trimmed = inner.Trim();
        if (trimmed.Length == 0 || trimmed.Any(char.IsWhiteSpace) || !ChordWords.TryParseDefinite(trimmed, notation, out _))
            return null;

        var column = m.Index + 1 + (inner.Length - inner.TrimStart().Length);
        var spaced = trimmed.Length != inner.Length;
        return new[] { SheetTokenizer.Chord(spaced ? trimmed : m.Value, spaced ? column : m.Index, trimmed, notation, false) };
    }
}
