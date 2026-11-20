using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>Reads chord rows of songs printed in capitals: OCR repairs, sung note words and normal spelling.</summary>
internal static class CapsChords
{
    private const int MinLongWords = 2;
    private const int MinLongLetters = 4;
    private const int RootRegion = 4;
    private const int MaxFixLength = 8;
    private const int MinFlatRowTokens = 2;

    // Bar marks that OCR leaves stuck to a chord name.
    private static readonly char[] StrayMarks = { '/', '|', '\\' };

    private static readonly Regex ItalianRoot = new("^(?i:sol|do|re|mi|fa|la|si)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>True for a row that is a lyric in capitals: at least two real words of four or more letters.</summary>
    public static bool IsLyricRow(IReadOnlyList<SheetToken> tokens) =>
        tokens.Count(t => t.Kind == TokenKind.Word && !t.Bracketed && IsUpperWord(t.Text)) >= MinLongWords;

    /// <summary>True for a row with at least two words of three or more letters written in lower or mixed case.</summary>
    public static bool IsMixedCaseRow(IReadOnlyList<SheetToken> tokens) =>
        tokens.Count(t => t.Kind == TokenKind.Word && t.Text.Count(char.IsLetter) >= 3 && t.Text.Any(char.IsLower)) >= MinLongWords;

    private static bool IsUpperWord(string text)
    {
        var letters = text.Where(char.IsLetter).ToList();
        return letters.Count >= MinLongLetters && letters.All(char.IsUpper);
    }

    /// <summary>True when the row is only bare note names written with single spaces, the way a sung "DO RE MI" is.</summary>
    public static bool IsSungNoteRow(IReadOnlyList<SheetToken> tokens) =>
        tokens.Count >= 2 && tokens.All(t => t.Kind == TokenKind.Chord && t.Ambiguous && !t.Bracketed && !t.Parenthesised) &&
        tokens.Zip(tokens.Skip(1), (a, b) => b.Column - (a.Column + a.Text.Length)).All(gap => gap <= 1);

    /// <summary>True when the row has a word and no unambiguous chord, so its note names are sung words.</summary>
    public static bool IsNoteWordsWithText(IReadOnlyList<SheetToken> tokens) =>
        !tokens.Any(t => t.Bracketed) && tokens.Any(t => t.Kind == TokenKind.Word && !t.LowerChord && !t.Foreign && !t.Doubtful) &&
        !tokens.Any(t => t.Kind == TokenKind.Chord && !t.Ambiguous);

    /// <summary>The row with a split minor marker joined and OCR-confused chord names repaired, or null when nothing changed or a real word remains.</summary>
    public static IReadOnlyList<SheetToken>? Repair(IReadOnlyList<SheetToken> tokens, Notation notation)
    {
        var result = new List<SheetToken>(tokens.Count);
        var changed = false;
        var hasChord = tokens.Any(t => t.Kind == TokenKind.Chord && !t.Bracketed);
        foreach (var token in tokens)
        {
            if (token.Kind != TokenKind.Word || token.Bracketed || token.Parenthesised)
            {
                result.Add(token);
                continue;
            }

            var core = token.Text.Trim(StrayMarks);
            var cleaned = token with { Text = core, Column = token.Column + Math.Max(0, token.Text.IndexOf(core, StringComparison.Ordinal)) };
            if (core == "m" && result.Count > 0 && Joined(result[^1], cleaned, notation) is { } joined)
                result[^1] = joined;
            else if (core == "m" && core != token.Text)
            {
                // A minor marker whose chord OCR lost, left beside a bar mark, is dropped.
            }
            else if (Fix(core, notation, hasChord, tokens.Count) is { } fixedName)
                result.Add(SheetTokenizer.Chord(core, cleaned.Column, fixedName, notation, false));
            else
                return null;
            changed = true;
        }

        return changed ? result : null;
    }

    private static SheetToken? Joined(SheetToken chord, SheetToken marker, Notation notation)
    {
        var gap = marker.Column - (chord.Column + chord.Text.Length);
        if (chord.Kind != TokenKind.Chord || chord.Bracketed || chord.Parenthesised || gap is < 0 or > 1 ||
            !ChordSymbolParser.TryParse(chord.Chord, notation, out var symbol) || symbol.IsMinor || symbol.Suffix.Length > 0 || symbol.Bass is not null)
            return null;

        return chord with { Text = chord.Text + new string(' ', gap) + marker.Text, Chord = chord.Chord + "m", Ambiguous = false };
    }

    // An English "BB" is B flat and a bar read as a leading I or l is dropped, both only beside real chords; then digits and l are read as letters.
    private static string? Fix(string text, Notation notation, bool rowHasChord, int rowTokens)
    {
        if (text.Length < 2)
            return null;

        if (notation == Notation.English && rowHasChord && rowTokens >= MinFlatRowTokens && text[0] is >= 'A' and <= 'G' && text[1] == 'B' &&
            SheetTokenizer.TryChord(text[0] + "b" + text[2..], notation))
            return text[0] + "b" + text[2..];

        if (rowHasChord && text.Length > 2 && text[0] is 'I' or 'l' or '1' && SheetTokenizer.TryChord(text[1..], notation))
            return text[1..];

        return FixDigits(text, notation);
    }

    // Zero, one and a lower-case l in the name stand for O, I or L; only the first characters are tried.
    private static string? FixDigits(string text, Notation notation)
    {
        if (text.Length > MaxFixLength || text.IndexOfAny(new[] { '0', '1', 'l' }) < 0)
            return null;

        var candidates = new List<string> { string.Empty };
        for (var i = 0; i < text.Length; i++)
        {
            var options = i >= RootRegion ? new[] { text[i] } : text[i] switch
            {
                '0' => new[] { 'O' },
                '1' or 'l' => new[] { 'I', 'L' },
                var c => new[] { c }
            };
            candidates = candidates.SelectMany(prefix => options.Select(o => prefix + o)).ToList();
        }

        return candidates.FirstOrDefault(c => SheetTokenizer.TryChord(c, notation));
    }

    /// <summary>The chord tokens with their names in normal spelling (Sol, Lam, Do7, Re/Fa#).</summary>
    public static IReadOnlyList<SheetToken> Spell(IReadOnlyList<SheetToken> tokens, Notation notation) =>
        tokens.Select(t => t.Kind == TokenKind.Chord && !t.Bracketed ? t with { Chord = Spelled(t.Chord, notation) } : t).ToList();

    private static string Spelled(string chord, Notation notation)
    {
        var slash = chord.LastIndexOf('/');
        var spelled = slash > 0 && ChordSymbolParser.TryParseNote(chord[(slash + 1)..], notation, out _)
            ? SpelledPart(chord[..slash], notation) + "/" + SpelledPart(chord[(slash + 1)..], notation)
            : SpelledPart(chord, notation);
        return ChordWords.TryParseDefinite(spelled, notation, out _) ? spelled : chord;
    }

    private static string SpelledPart(string part, Notation notation)
    {
        var rootLength = notation == Notation.Italian ? ItalianRoot.Match(part) is { Success: true } m ? m.Length : 0 : 1;
        if (rootLength == 0 || part.Length < 2)
            return part;

        var rest = part[rootLength..];
        // An English "M7" after the root is a major seventh, so its capital stays.
        var keepCapital = notation == Notation.English && rest.Length > 1 && rest[0] == 'M' && char.IsDigit(rest[1]);
        var tail = keepCapital ? "M" + rest[1..].ToLowerInvariant() : rest.ToLowerInvariant();
        return char.ToUpperInvariant(part[0]) + part[1..rootLength].ToLowerInvariant() + tail;
    }
}
