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

    // Real chords that are also words: alone on a line they need document context.
    private static readonly HashSet<string> WordLike = new(StringComparer.OrdinalIgnoreCase)
    {
        "Dom", "Lab", "Sim", "Rem", "Mim", "Lam", "Solm", "Fam"
    };

    /// <summary>True when the text parses as a chord and its quality is not a bare letter run outside the real qualities.</summary>
    public static bool TryParseDefinite(string text, Notation notation, out ChordSymbol chord) =>
        ChordSymbolParser.TryParse(text, notation, out chord) && IsDefinite(chord);

    /// <summary>True when the text parses as a chord but only through a letter-only suffix that is not a real quality.</summary>
    public static bool IsDoubtful(string text, Notation notation) =>
        ChordSymbolParser.TryParse(text, notation, out var chord) && !IsDefinite(chord);

    public static bool IsWordLike(string chord) => WordLike.Contains(chord);

    private static bool IsDefinite(ChordSymbol chord)
    {
        var quality = chord.MinorToken + chord.Suffix;
        return !quality.All(char.IsLetter) || RealQualities.Contains(quality);
    }
}
