using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>Promotes lower-case and other-notation chord words when the rest of the line is made of chords.</summary>
internal static class LooseChords
{
    private const int MinForeignAnchors = 3;
    private const int MinDoubtfulAnchors = 2;
    private const int MinWideGap = 3;
    private const int MinWideIndent = 6;

    /// <summary>The tokens with loose chords turned into chords, or null when the line does not qualify.</summary>
    public static IReadOnlyList<SheetToken>? Promote(IReadOnlyList<SheetToken> tokens, Notation notation)
    {
        var loose = tokens.Where(t => t.LowerChord || t.Foreign || t.Doubtful).ToList();
        var hasRealWords = tokens.Any(t => t.Kind == TokenKind.Word && !t.LowerChord && !t.Foreign && !t.Doubtful);
        var strict = tokens.Count(t => t.Kind == TokenKind.Chord);
        if (loose.Count == 0 || hasRealWords || strict + loose.Count < 2)
            return null;

        var lowers = loose.Where(t => t.LowerChord).ToList();
        var foreign = loose.Count(t => t.Foreign);
        var doubtful = loose.Count(t => t.Doubtful);
        if ((lowers.Count > 0 && !LowerCaseLooksLikeChords(tokens, lowers, notation)) ||
            (foreign > 0 && strict < MinForeignAnchors) || (doubtful > 0 && strict < MinDoubtfulAnchors))
            return null;

        return tokens.Select(t => t.LowerChord || t.Foreign || t.Doubtful ? ToChord(t, notation) : t).ToList();
    }

    private static bool LowerCaseLooksLikeChords(IReadOnlyList<SheetToken> tokens, List<SheetToken> lowers, Notation notation)
    {
        var names = tokens.Where(t => t.Kind == TokenKind.Chord).Select(t => t.Chord).Concat(lowers.Select(t => t.Text));
        var distinct = names.Select(n => n.ToLowerInvariant()).Distinct().Count() >= 2;
        var decorated = lowers.Any(t => !IsBareName(t.Text, notation));
        return distinct && (decorated || HasWideGap(tokens));
    }

    private static bool IsBareName(string text, Notation notation) =>
        notation == Notation.Italian ? SheetTokenizer.IsAmbiguous(text, notation) : text.Length == 1;

    private static bool HasWideGap(IReadOnlyList<SheetToken> tokens) =>
        tokens[0].Column >= MinWideIndent ||
        tokens.Zip(tokens.Skip(1), (a, b) => b.Column - (a.Column + a.Text.Length)).Any(gap => gap >= MinWideGap);

    private static SheetToken ToChord(SheetToken token, Notation notation) =>
        token with
        {
            Kind = TokenKind.Chord,
            Chord = token.Parenthesised ? token.Text[1..^1] : token.Text,
            Ambiguous = token.LowerChord && SheetTokenizer.IsAmbiguous(token.Parenthesised ? token.Text[1..^1] : token.Text, notation),
            LowerChord = false,
            Foreign = false,
            Doubtful = false
        };
}
