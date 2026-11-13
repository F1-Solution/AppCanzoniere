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

        var name = match.Groups["note"].Value;
        var notation = name.Length == 1 ? Notation.English : Notation.Italian;
        var accidental = match.Groups["symbol"].Success ? match.Groups["symbol"].Value : match.Groups["word"].Value;
        var root = ReadNote(name, accidental, notation);
        var quality = match.Groups["quality"].Value;
        var symbol = new ChordSymbol(root, quality.StartsWith("min", StringComparison.OrdinalIgnoreCase), string.Empty, string.Empty, null);
        key = new KeyValue(symbol, notation, new KeyWords(match.Groups["symbol"].Success, match.Groups["word"].Success ? match.Groups["word"].Value : null, quality));
        return true;
    }

    private static Note ReadNote(string name, string accidental, Notation notation)
    {
        var parsed = ChordSymbolParser.TryParseNote(name + (accidental.Length == 1 ? accidental : string.Empty), notation, out var note);
        var sign = accidental.ToLowerInvariant() switch
        {
            "diesis" or "sharp" => 1,
            "bemolle" or "flat" => -1,
            _ => note.Accidental
        };
        return parsed ? new Note(note.Letter, sign) : default;
    }

    /// <summary>Writes the key with a new root; the style it was written in is kept unless a conversion asks for the output notation.</summary>
    public string Write(ChordSymbol shifted, NotationOptions output, bool conversion)
    {
        var notation = conversion ? output.Notation : Notation;
        if (Words is null)
            return ChordNotation.Format(shifted, new NotationOptions(notation, output.MinorStyle), keepMinorToken: !conversion);

        var root = shifted.Root;
        var text = ChordNotation.FormatNote(new Note(root.Letter, 0), notation);
        if (root.Accidental != 0)
            text += Words.SymbolAccidental ? (root.Accidental > 0 ? "#" : "b") : " " + AccidentalWord(root.Accidental, notation);

        return text + " " + Quality(Notation, notation);
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

        return char.IsUpper(template[0]) ? char.ToUpperInvariant(word[0]) + word[1..] : word;
    }
}

/// <summary>How a worded key was written: symbol or word accidental, and the quality word as typed.</summary>
internal sealed record KeyWords(bool SymbolAccidental, string? AccidentalWord, string Quality);
