using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Display;

/// <summary>Tells which notation a song's chords are written in and whether a minor marker contradicts the wanted style.</summary>
internal static class ChordNotationDetector
{
    /// <summary>The notation more chords read in (on a tie the first chord only one notation reads, else English), with a minor style that differs from <paramref name="wanted"/> exactly when a minor chord is written another way.</summary>
    public static NotationOptions Detect(IEnumerable<string> chords, MinorStyle wanted)
    {
        var list = chords.ToList();
        var notation = DetectNotation(list);
        var style = AnyMinorContradicts(list, wanted) ? Opposite(wanted) : wanted;
        return new NotationOptions(notation, style);
    }

    private static Notation DetectNotation(List<string> chords)
    {
        var italian = 0;
        var english = 0;
        Notation? first = null;
        foreach (var chord in chords)
        {
            var asItalian = ChordSymbolParser.TryParse(chord, Notation.Italian, out _);
            var asEnglish = ChordSymbolParser.TryParse(chord, Notation.English, out _);
            italian += asItalian ? 1 : 0;
            english += asEnglish ? 1 : 0;
            if (first is null && asItalian != asEnglish)
                first = asItalian ? Notation.Italian : Notation.English;
        }

        if (italian != english)
            return italian > english ? Notation.Italian : Notation.English;

        return first ?? Notation.English;
    }

    // A dash minor when an m is wanted, or any other marker when a dash is wanted.
    private static bool AnyMinorContradicts(List<string> chords, MinorStyle wanted)
    {
        foreach (var chord in chords)
        {
            if (!TryReadMinor(chord, out var token))
                continue;

            var isDash = token == "-";
            if (wanted == MinorStyle.LetterM ? isDash : !isDash)
                return true;
        }

        return false;
    }

    private static bool TryReadMinor(string chord, out string token)
    {
        token = string.Empty;
        foreach (var notation in new[] { Notation.English, Notation.Italian })
        {
            if (ChordSymbolParser.TryParse(chord, notation, out var symbol))
            {
                token = symbol.MinorToken;
                return symbol.IsMinor;
            }
        }

        return false;
    }

    private static MinorStyle Opposite(MinorStyle style) =>
        style == MinorStyle.LetterM ? MinorStyle.Dash : MinorStyle.LetterM;
}
