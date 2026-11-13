using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.Music;

/// <summary>A <c>{key}</c> value as written: compact ("Am", "Lam") or in words ("La minore", "B flat major").</summary>
internal sealed record KeyValue(ChordSymbol Symbol, Notation Notation, KeyWords? Words)
{
    private static readonly Regex Spaces = new(@"\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex Worded = new(
        @"^(?<note>sol|do|re|mi|fa|la|si|[a-g])(?<symbol>[#b♯♭])?(?: (?<word>diesis|bemolle|sharp|flat))? (?<quality>minore|minor|min\.?|maggiore|major|magg\.?|maj\.?)$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    // Quality word (lower case) to its Italian and English forms.
    private static readonly Dictionary<string, (string Italian, string English)> Qualities = new()
    {
        ["minore"] = ("minore", "minor"),
        ["minor"] = ("minore", "minor"),
        ["min"] = ("min", "min"),
        ["min."] = ("min.", "min."),
        ["maggiore"] = ("maggiore", "major"),
        ["major"] = ("maggiore", "major"),
        ["magg"] = ("magg.", "maj"),
        ["magg."] = ("magg.", "maj"),
        ["maj"] = ("magg.", "maj"),
        ["maj."] = ("magg.", "maj")
    };

    /// <summary>Reads a key in either notation, compact or in words; the document's input notation is tried first.</summary>
    public static bool TryParse(string? text, Notation preferred, out KeyValue key)
    {
        key = null!;
        var trimmed = text is null ? string.Empty : Spaces.Replace(text.Trim(), " ");
        if (trimmed.Length == 0)
            return false;

        var other = preferred == Notation.English ? Notation.Italian : Notation.English;
        foreach (var notation in new[] { preferred, other })
        {
            if (ChordSymbolParser.TryParse(trimmed, notation, out var chord))
            {
                key = new KeyValue(chord, notation, null);
                return true;
            }
        }

        return TryParseWords(trimmed, out key);
    }

    private static bool TryParseWords(string text, out KeyValue key)
    {
        key = null!;
        var match = Worded.Match(text);
        if (!match.Success || (match.Groups["symbol"].Success && match.Groups["word"].Success))
            return false;

        var allUpper = !text.Any(char.IsLower);
        var symbol = match.Groups["symbol"].Success ? match.Groups["symbol"].Value : null;
        if (symbol == "B" && !allUpper)
            return false; // An upper-case B is a flat sign only when the whole key is upper case.

        var name = match.Groups["note"].Value;
        var notation = name.Length == 1 ? Notation.English : Notation.Italian;
        if (!ChordSymbolParser.TryParseNote(name, notation, out var natural))
            return false;

        var word = match.Groups["word"].Success ? match.Groups["word"].Value : null;
        var root = new Note(natural.Letter, AccidentalSign(symbol ?? word));
        var quality = match.Groups["quality"].Value;
        var chord = new ChordSymbol(root, quality.StartsWith("min", StringComparison.OrdinalIgnoreCase), string.Empty, string.Empty, null);
        key = new KeyValue(chord, notation, new KeyWords(symbol, word, quality, name, allUpper));
        return true;
    }

    private static int AccidentalSign(string? accidental) => accidental?.ToLowerInvariant() switch
    {
        "#" or "♯" or "diesis" or "sharp" => 1,
        "b" or "♭" or "bemolle" or "flat" => -1,
        _ => 0
    };

    /// <summary>Writes the key with a new root; the style it was written in is kept unless a conversion asks for the output notation.</summary>
    public string Write(ChordSymbol shifted, NotationOptions output, bool conversion)
    {
        var notation = conversion ? output.Notation : Notation;
        if (Words is null)
            return ChordNotation.Format(shifted, new NotationOptions(notation, output.MinorStyle), keepMinorToken: !conversion);

        var root = shifted.Root;
        var name = ChordNotation.FormatNote(new Note(root.Letter, 0), notation);
        var text = Words.AllUpper ? name.ToUpperInvariant() : MatchCase(Words.NoteName, name);
        if (root.Accidental != 0)
            text += Words.Symbol is not null ? AccidentalSymbol(root.Accidental) : " " + AccidentalWord(root.Accidental, notation);

        return text + " " + Quality(Notation, notation);
    }

    // The written sign decides the style: Unicode stays Unicode, and the flat letter follows the case of an all-upper key.
    private string AccidentalSymbol(int accidental)
    {
        var written = Words!.Symbol![0];
        if (written is '♭' or '♯')
            return accidental > 0 ? "♯" : "♭";

        return accidental > 0 ? "#" : Words.AllUpper ? "B" : "b";
    }

    private string AccidentalWord(int accidental, Notation notation)
    {
        var word = notation == Notation.Italian
            ? (accidental > 0 ? "diesis" : "bemolle")
            : (accidental > 0 ? "sharp" : "flat");
        return MatchCase(Words!.AccidentalWord ?? Words.Quality, word);
    }

    private string Quality(Notation from, Notation to)
    {
        var written = Words!.Quality;
        if (from == to || !Qualities.TryGetValue(written.ToLowerInvariant(), out var forms))
            return written;

        return MatchCase(written, to == Notation.Italian ? forms.Italian : forms.English);
    }

    // All upper case stays upper case, a leading capital stays a leading capital, anything else is lower case.
    private static string MatchCase(string template, string word)
    {
        if (template.Length > 1 && template.All(c => !char.IsLetter(c) || char.IsUpper(c)))
            return word.ToUpperInvariant();

        return char.IsUpper(template[0]) ? char.ToUpperInvariant(word[0]) + word[1..] : word.ToLowerInvariant();
    }
}

/// <summary>How a worded key was written: its accidental sign or word, quality word and note name as typed.</summary>
internal sealed record KeyWords(string? Symbol, string? AccidentalWord, string Quality, string NoteName, bool AllUpper);
