using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>What the rest of the document says about a line: the lyric below it and whether the document writes chords in lower case.</summary>
internal readonly record struct LineContext(string? Below, bool LowerCaseStyle, bool OtherNotationAsText = false, bool AfterLabel = false);

/// <summary>Promotes lower-case and other-notation chord words when the rest of the line is made of chords.</summary>
internal static class LooseChords
{
    private const int MinForeignAnchors = 3;
    private const int MinDoubtfulAnchors = 2;
    private const int MinWideGap = 3;
    public const int MinStyleLines = 3;

    /// <summary>The tokens with loose chords turned into chords, or null when the line does not qualify.</summary>
    public static IReadOnlyList<SheetToken>? Promote(IReadOnlyList<SheetToken> tokens, Notation notation, LineContext context)
    {
        // A bracketed chord of the other notation is explicit evidence, so it counts as a chord, not as a loose word.
        var realChords = tokens.Count(t => t.Kind == TokenKind.Chord);
        var plainChords = tokens.Count(t => t.Kind == TokenKind.Chord && !t.Bracketed);
        var explicitForeign = plainChords > 0 ? tokens.Where(IsExplicitForeign).ToList() : new List<SheetToken>();
        var loose = tokens.Where(t => (t.LowerChord || t.Foreign || t.Doubtful) && !explicitForeign.Contains(t)).ToList();
        var hasRealWords = tokens.Any(t => t.Kind == TokenKind.Word && !t.LowerChord && !t.Foreign && !t.Doubtful);
        var strict = realChords + explicitForeign.Count;
        if (loose.Count + explicitForeign.Count == 0 || hasRealWords || strict + loose.Count < 2)
            return null;

        var lowers = loose.Where(t => t.LowerChord).ToList();
        var foreign = loose.Count(t => t.Foreign);
        var doubtful = loose.Count(t => t.Doubtful);
        if ((lowers.Count > 0 && !LowerCaseLooksLikeChords(tokens, lowers, notation, context)) ||
            (foreign > 0 && strict < MinForeignAnchors) || (doubtful > 0 && strict < MinDoubtfulAnchors))
            return null;

        return tokens.Select(t => t.LowerChord || t.Foreign || t.Doubtful ? ToChord(t, notation) : t).ToList();
    }

    /// <summary>After a section label, two or more different lower-case note names alone are chords: the label is the evidence.</summary>
    public static IReadOnlyList<SheetToken>? PromoteAfterLabel(IReadOnlyList<SheetToken> tokens, Notation notation)
    {
        var lowers = tokens.Where(t => t.LowerChord && !t.Parenthesised).ToList();
        var onlyNotes = tokens.All(t => t.Kind == TokenKind.Chord || t.LowerChord && !t.Parenthesised);
        return lowers.Count == 0 || !onlyNotes || DistinctNames(tokens, lowers) < 2
            ? null
            : tokens.Select(t => t.LowerChord ? ToChord(t, notation) : t).ToList();
    }

    private static bool IsExplicitForeign(SheetToken t) => t.Foreign && t.Bracketed;

    /// <summary>True when the line shows lower-case chord evidence by itself: a chord feature or wide gaps; used to see the document's style.</summary>
    public static bool IsStyleEvidence(IReadOnlyList<SheetToken> tokens, Notation notation)
    {
        var lowers = tokens.Where(t => t.LowerChord).ToList();
        var others = tokens.Count(t => t.Kind == TokenKind.Word && !t.LowerChord && !t.Foreign && !t.Doubtful);
        return lowers.Count > 0 && others == 0 && DistinctNames(tokens, lowers) >= 2 &&
               (lowers.Any(t => HasChordFeature(t, notation)) || HasWideGap(tokens));
    }

    private static bool LowerCaseLooksLikeChords(IReadOnlyList<SheetToken> tokens, List<SheetToken> lowers, Notation notation, LineContext context)
    {
        var distinct = DistinctNames(tokens, lowers);
        if (distinct < 2)
            return false;

        var styled = context.LowerCaseStyle && HasWideGap(tokens);
        return styled || lowers.Any(t => HasChordFeature(t, notation)) || HasAlignedWideGap(tokens, context.Below, notation);
    }

    private static int DistinctNames(IReadOnlyList<SheetToken> tokens, List<SheetToken> lowers) =>
        tokens.Where(t => t.Kind == TokenKind.Chord).Select(t => t.Chord).Concat(lowers.Select(t => t.Text))
            .Select(n => n.ToLowerInvariant()).Distinct().Count();

    private static bool HasWideGap(IReadOnlyList<SheetToken> tokens) =>
        tokens.Zip(tokens.Skip(1), (a, b) => b.Column - (a.Column + a.Text.Length)).Any(gap => gap >= MinWideGap);

    // An accidental, a minor marker, a digit suffix or a slash bass: things a word never has.
    private static bool HasChordFeature(SheetToken token, Notation notation)
    {
        var text = token.Parenthesised ? token.Text[1..^1] : token.Text;
        return ChordWords.TryParseDefinite(text, notation, out var chord) &&
               (chord.Root.Accidental != 0 || chord.IsMinor || chord.Suffix.Length > 0 || chord.Bass is not null);
    }

    // Wide gaps count only when the chords sit over word starts of the lyric line directly below.
    private static bool HasAlignedWideGap(IReadOnlyList<SheetToken> tokens, string? below, Notation notation)
    {
        // A merged lyric, which the line above never saw as its lyric, holds chords of the notation; other brackets may be written back as text.
        if (!HasWideGap(tokens) || string.IsNullOrWhiteSpace(below) || BracketedText.Matches(below).Any(m => BracketedText.IsInlineChord(m.Groups[1].Value, notation)))
            return false;

        var lyric = below.TrimStart();
        var columns = tokens.Select(t => t.Column - tokens[0].Column).ToList();
        return columns[^1] < lyric.Length && columns.Count(c => NearWordStart(lyric, c)) * 2 >= columns.Count;
    }

    private static bool NearWordStart(string text, int column)
    {
        for (var c = Math.Max(0, column - 1); c <= Math.Min(text.Length - 1, column + 1); c++)
        {
            if (text[c] != ' ' && (c == 0 || text[c - 1] == ' '))
                return true;
        }

        return false;
    }

    private static SheetToken ToChord(SheetToken token, Notation notation) =>
        token with
        {
            Kind = TokenKind.Chord,
            Parenthesised = token.Parenthesised && !token.Bracketed,
            Chord = token.Parenthesised ? token.Text[1..^1] : token.Text,
            Ambiguous = token.LowerChord && SheetTokenizer.IsAmbiguous(token.Parenthesised ? token.Text[1..^1] : token.Text, notation),
            LowerChord = false,
            Foreign = false,
            Doubtful = false
        };
}
