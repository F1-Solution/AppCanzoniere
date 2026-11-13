using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

internal enum UnitKind
{
    Chord,
    Lyric,
    Inline
}

/// <summary>Chord-or-lyric decision of one line (spec 5.1); Pending single bare notes wait for document context.</summary>
internal sealed record LineClassification(
    bool IsChord,
    bool Pending,
    double Confidence,
    IReadOnlyList<SheetWarning> Warnings,
    IReadOnlyList<SheetToken> Tokens,
    int Unambiguous,
    int Ambiguous,
    int Words)
{
    public int ChordCount => Unambiguous + Ambiguous;

    /// <summary>True when the line can decide the notation of the document.</summary>
    public bool CountsForDetection => IsChord && !Pending && ChordCount > 0;
}

/// <summary>The text of a line that carries lyrics or chords: a whole line or the rest after a section label.</summary>
internal sealed record ContentUnit(UnitKind Kind, string Text, int Start, bool Indented, LineClassification Class)
{
    /// <summary>Text written when the unit is not merged.</summary>
    public string OutputText => Indented ? Text : Text[Start..];
}

internal static class LineClassifier
{
    private const double SureChord = 1.0;
    private const double MixedChord = 0.9;
    private const double BareNamesChord = 0.8;
    private const double LikelyChord = 0.6;
    private const double UnsureSingle = 0.5;
    private const double CertainLyric = 1.0;
    private const double LikelyLyric = 0.8;
    private const int StrayWordMinChords = 4;

    public static ContentUnit BuildUnit(string text, int start, bool indented, Notation notation, LineContext context = default)
    {
        var classification = Classify(SheetTokenizer.Tokenize(text, start, notation), notation, context);
        if (classification.IsChord)
            return new ContentUnit(UnitKind.Chord, text, start, indented, SafeTokens(classification));

        if (InlineChords.TryConvert(text, start, notation, out var converted, out var unambiguous, out var ambiguous))
        {
            var safe = TextSafety.ReplaceKeepingChords(converted);
            var inline = new LineClassification(false, false, CertainLyric, Replaced(safe != converted), Array.Empty<SheetToken>(), unambiguous, ambiguous, 0);
            return new ContentUnit(UnitKind.Inline, safe, start, indented, inline);
        }

        var lyric = TextSafety.Replace(text);
        var warned = classification with { Warnings = classification.Warnings.Concat(Replaced(lyric != text)).ToList() };
        return new ContentUnit(UnitKind.Lyric, lyric, start, indented, warned);
    }

    private static SheetWarning[] Replaced(bool changed) => changed ? new[] { SheetWarning.BracketsReplaced } : Array.Empty<SheetWarning>();

    private static LineClassification SafeTokens(LineClassification classification)
    {
        var tokens = classification.Tokens.Select(t => t.Kind == TokenKind.Chord ? t : t with { Text = TextSafety.Replace(t.Text) }).ToList();
        var changed = tokens.Zip(classification.Tokens).Any(p => p.First.Text != p.Second.Text);
        return changed
            ? classification with { Tokens = tokens, Warnings = classification.Warnings.Concat(Replaced(true)).ToList() }
            : classification;
    }

    public static LineClassification Classify(IReadOnlyList<SheetToken> tokens, Notation notation, LineContext context = default)
    {
        if (LooseChords.Promote(tokens, notation, context) is { } promoted && ClassifyTokens(promoted) is { IsChord: true } chordLine)
        {
            return chordLine with
            {
                Confidence = Math.Min(chordLine.Confidence, LikelyChord),
                Warnings = chordLine.Warnings.Append(SheetWarning.AmbiguousNoteWords).Distinct().ToList()
            };
        }

        return ClassifyTokens(tokens);
    }

    private static LineClassification ClassifyTokens(IReadOnlyList<SheetToken> tokens)
    {
        var u = tokens.Count(t => t.Kind == TokenKind.Chord && !t.Ambiguous);
        var a = tokens.Count(t => t.Kind == TokenKind.Chord && t.Ambiguous);
        var w = tokens.Count(t => t.Kind == TokenKind.Word);
        var neutral = tokens.Count(t => t.Kind == TokenKind.Neutral);
        LineClassification Make(bool chord, double confidence, params SheetWarning[] warnings) =>
            new(chord, false, confidence, warnings, tokens, u, a, w);

        if (u + a == 0)
        {
            var noChord = neutral >= 1 && tokens.Any(SheetTokenizer.IsNoChord);
            return noChord ? Make(true, LikelyChord) : Make(false, CertainLyric);
        }

        if (w == 0)
            return ClassifyChordsOnly(tokens, u, a, w);

        if (w == 1 && u + a >= StrayWordMinChords && !tokens.Any(t => t.HasInlineChord))
            return Make(true, LikelyChord, SheetWarning.StrayWordOnChordLine);

        var ratio = (double)(u + a) / (u + a + w);
        return Make(false, ratio < 0.5 ? LikelyLyric : LikelyChord, SheetWarning.AmbiguousNoteWords);
    }

    private static LineClassification ClassifyChordsOnly(IReadOnlyList<SheetToken> tokens, int u, int a, int w)
    {
        LineClassification Make(bool chord, bool pending, double confidence, params SheetWarning[] warnings) =>
            new(chord, pending, confidence, warnings, tokens, u, a, w);

        var wordLikeAlone = tokens.Count == 1 && tokens[0].Kind == TokenKind.Chord && ChordWords.IsWordLike(tokens[0].Chord);
        if (u >= 1 && !wordLikeAlone)
            return Make(true, false, a > 0 ? MixedChord : SureChord);

        if (a == 1 || wordLikeAlone)
            return tokens.Any(SheetTokenizer.IsNoChord)
                ? Make(true, false, LikelyChord) // A no-chord mark beside a note name is chord-line evidence by itself.
                : Make(false, true, UnsureSingle, SheetWarning.AmbiguousSingleToken);

        var chords = tokens.Where(t => t.Kind == TokenKind.Chord).Select(t => t.Chord).ToList();
        var sameName = chords.All(c => string.Equals(c, chords[0], StringComparison.OrdinalIgnoreCase));
        return sameName
            ? Make(false, false, LikelyChord, SheetWarning.AmbiguousNoteWords)
            : Make(true, false, BareNamesChord);
    }

    /// <summary>Turns a pending single bare note into a chord line once the document has other chord lines.</summary>
    public static ContentUnit ResolvePending(ContentUnit unit) =>
        unit with { Kind = UnitKind.Chord, Class = unit.Class with { IsChord = true, Pending = false } };
}

/// <summary>Chords already written inside a lyric: bracketed, or in parentheses before a letter (spec 5.5).</summary>
internal static class InlineChords
{
    private static readonly Regex Parenthesised = new(@"\(([^()\s]+)\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool TryConvert(string text, int start, Notation notation, out string converted, out int unambiguous, out int ambiguous)
    {
        converted = text;
        unambiguous = ambiguous = 0;
        var rest = text[start..];

        var bracketed = BracketedText.Matches(rest).Select(m => m.Groups[1].Value).Where(BracketedText.IsKept).ToList();
        if (bracketed.Count > 0)
        {
            Count(bracketed.Where(c => !BracketedText.IsAnnotation(c) && ChordWords.IsChord(c, notation)), notation, ref unambiguous, ref ambiguous);
            return true;
        }

        var chords = Parenthesised.Matches(rest).Where(m => SheetTokenizer.TryChord(m.Groups[1].Value, notation)).ToList();
        var followedByLetter = chords.Any(m => m.Index + m.Length < rest.Length && char.IsLetter(rest[m.Index + m.Length]));
        if (chords.Count == 0 || !(followedByLetter || chords.Count >= 2))
            return false;

        var characters = rest.ToCharArray();
        foreach (var m in chords)
        {
            characters[m.Index] = '[';
            characters[m.Index + m.Length - 1] = ']';
        }

        converted = text[..start] + new string(characters);
        Count(chords.Select(m => m.Groups[1].Value), notation, ref unambiguous, ref ambiguous);
        return true;
    }

    private static void Count(IEnumerable<string> chords, Notation notation, ref int unambiguous, ref int ambiguous)
    {
        foreach (var chord in chords)
        {
            if (SheetTokenizer.IsAmbiguous(chord, notation))
                ambiguous++;
            else
                unambiguous++;
        }
    }
}
