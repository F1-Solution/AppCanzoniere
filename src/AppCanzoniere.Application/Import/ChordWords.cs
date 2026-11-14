using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>Tells real chords from words that the shared chord parser also accepts (Dono, Remi, Ami).</summary>
internal static class ChordWords
{
    // Letter-only qualities that are real; "mi" is left out because it makes Remi, Domi and Ami words.
    private static readonly HashSet<string> RealQualities = new(StringComparer.OrdinalIgnoreCase)
    {
        string.Empty, "m", "min", "maj", "dim", "aug", "sus", "alt", "dom"
    };

    private const string EnglishMinorWord = "mi";

    private static readonly System.Text.RegularExpressions.Regex EnglishMiMinor =
        new("^[A-G][#b]?mi$", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    // Real chords that are also words: alone on a line they need document context.
    private static readonly HashSet<string> WordLike = new(StringComparer.OrdinalIgnoreCase)
    {
        "Dom", "Lab", "Sim", "Rem", "Mim", "Lam", "Solm", "Fam"
    };

    /// <summary>True when the text parses as a chord and its quality is not a bare letter run outside the real qualities.</summary>
    public static bool TryParseDefinite(string text, Notation notation, out ChordSymbol chord) =>
        TryParse(text, notation, out chord) && IsDefinite(chord, notation);

    /// <summary>True when the text parses as a chord but only through a letter-only suffix that is not a real quality.</summary>
    public static bool IsDoubtful(string text, Notation notation) =>
        TryParse(text, notation, out var chord) && !IsDefinite(chord, notation);

    /// <summary>True when the text parses as a chord of the notation and its parentheses are balanced.</summary>
    public static bool IsChord(string text, Notation notation) => TryParse(text, notation, out _);

    private static bool TryParse(string text, Notation notation, out ChordSymbol chord) =>
        ChordSymbolParser.TryParse(text, notation, out chord) && IsBalanced(text) && text.Length > 0 && !",;:.".Contains(text[^1]);

    private static bool IsBalanced(string text)
    {
        var depth = 0;
        foreach (var c in text)
        {
            depth += c == '(' ? 1 : c == ')' ? -1 : 0;
            if (depth < 0)
                return false;
        }

        return depth == 0;
    }

    public static bool IsWordLike(string chord) => WordLike.Contains(chord);

    /// <summary>True for an English root plus the bare "mi" quality (Ami, Emi): alone on a line it is a word, unless it has an accidental.</summary>
    public static bool IsMiMinor(string chord) => EnglishMiMinor.IsMatch(chord) && !HasAccidental(chord);

    private static bool HasAccidental(string chord) =>
        (ChordSymbolParser.TryParse(chord, Notation.Italian, out var italian) && italian.Root.Accidental != 0) ||
        (ChordSymbolParser.TryParse(chord, Notation.English, out var english) && english.Root.Accidental != 0);

    private static bool IsDefinite(ChordSymbol chord, Notation notation)
    {
        var quality = chord.MinorToken + chord.Suffix;
        return !quality.All(char.IsLetter) || RealQualities.Contains(quality) || (notation == Notation.English && quality == EnglishMinorWord);
    }
}
