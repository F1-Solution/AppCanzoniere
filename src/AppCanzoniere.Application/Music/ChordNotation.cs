using System.Text;
using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.Music;

/// <summary>Reads chords in one notation and writes them in another.</summary>
public static class ChordNotation
{
    private static readonly Regex AlteredTensionStart = new("^[#b♯♭][0-9]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>Converts a chord symbol between notations; text that is not a chord is returned unchanged.</summary>
    public static string Convert(string chord, NotationOptions from, NotationOptions to) =>
        ChordSymbolParser.TryParse(chord, from.Notation, out var symbol) ? Format(symbol, to) : chord;

    /// <summary>Writes a chord in the given notation; with <paramref name="keepMinorToken"/> the minor marker is written as typed.</summary>
    public static string Format(ChordSymbol chord, NotationOptions options, bool keepMinorToken = false)
    {
        var builder = new StringBuilder(FormatNote(chord.Root, options.Notation));
        if (chord.IsMinor)
            builder.Append(keepMinorToken ? chord.MinorToken : options.MinorStyle == MinorStyle.Dash ? "-" : "m");

        builder.Append(FormatSuffix(chord, options.Notation));
        if (chord.Bass is { } bass)
            builder.Append('/').Append(FormatNote(bass, options.Notation));

        return builder.ToString();
    }

    private static string FormatSuffix(ChordSymbol chord, Notation notation)
    {
        if (ChordSymbolParser.HasBareDiminishedMarker(chord.Suffix) && UsesDegreeSign(chord.Root, notation))
            return "°" + chord.Suffix[1..];

        // "Cb5" would read as C-flat, so an altered tension after a natural root is parenthesised.
        var tension = AlteredTensionStart.Match(chord.Suffix);
        return tension.Success && chord.Root.Accidental == 0 && !chord.IsMinor
            ? "(" + tension.Value + ")" + chord.Suffix[tension.Length..]
            : chord.Suffix;
    }

    // Italian "Do" and English "D" followed by "o" would read back as the wrong chord, so they take the degree sign.
    private static bool UsesDegreeSign(Note root, Notation notation) =>
        notation == Notation.Italian || (root.Letter == 'D' && root.Accidental == 0);

    public static string FormatNote(Note note, Notation notation)
    {
        var name = notation == Notation.Italian ? ItalianName(note.Letter) : note.Letter.ToString();
        return name + (note.Accidental > 0 ? "#" : note.Accidental < 0 ? "b" : string.Empty);
    }

    private static string ItalianName(char letter) => letter switch
    {
        'C' => "Do",
        'D' => "Re",
        'E' => "Mi",
        'F' => "Fa",
        'G' => "Sol",
        'A' => "La",
        _ => "Si"
    };
}
