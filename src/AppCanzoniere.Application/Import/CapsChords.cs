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

    private static readonly Regex ItalianRoot = new("^(?i:sol|do|re|mi|fa|la|si)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>True for a row that is a lyric in capitals: at least two real words of four or more letters.</summary>
    public static bool IsLyricRow(IReadOnlyList<SheetToken> tokens) =>
        tokens.Count(t => t.Kind == TokenKind.Word && !t.Bracketed && IsUpperWord(t.Text)) >= MinLongWords;

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
        foreach (var token in tokens)
        {
            if (token.Kind != TokenKind.Word || token.Bracketed || token.Parenthesised)
            {
                result.Add(token);
                continue;
            }

            if (token.Text == "m" && result.Count > 0 && Joined(result[^1], token, notation) is { } joined)
                result[^1] = joined;
            else if (Fix(token.Text, notation) is { } fixedName)
                result.Add(SheetTokenizer.Chord(token.Text, token.Column, fixedName, notation, false));
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

    // Zero, one and a lower-case l in the name stand for O, I or L; only the first characters are tried.
    private static string? Fix(string text, Notation notation)
    {
        if (text.Length is < 2 or > MaxFixLength || text.IndexOfAny(new[] { '0', '1', 'l' }) < 0)
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
