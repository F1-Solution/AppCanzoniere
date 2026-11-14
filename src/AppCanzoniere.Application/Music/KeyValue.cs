using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.Music;

/// <summary>A <c>{key}</c> value as written: compact ("Am", "Lam") or in words ("La minore", "B flat major").</summary>
internal sealed record KeyValue(ChordSymbol Symbol, Notation Notation, KeyWords? Words, bool AllCaps = false)
{
    private static readonly Regex Spaces = new(@"\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    // In an all-caps compact key a B after the note is the flat sign and an M is the minor marker.
    private static readonly Regex AllCapsCompact = new(@"^(?<note>SOL|DO|RE|MI|FA|LA|SI|[A-G])(?<flat>B)?(?<minor>M)?$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex Worded = new(
        @"^(?<note>sol|do|re|mi|fa|la|si|[a-g])(?<symbol>[#b♯♭])?(?: (?<detached>[#b♯♭])(?= ))?(?: (?<word>diesis|bemolle|sharp|flat))?(?: (?<quality>minore|minor|min\.?|maggiore|major|magg\.?|maj\.?))?$",
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

        if (TryParseAllCaps(trimmed, out key))
            return true;

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

    private static bool TryParseAllCaps(string text, out KeyValue key)
    {
        key = null!;
        var match = text.Length > 1 ? AllCapsCompact.Match(text) : Match.Empty;
        if (!match.Success)
            return false;

        var name = match.Groups["note"].Value;
        var notation = name.Length == 1 ? Notation.English : Notation.Italian;
        var written = name[0] + name[1..].ToLowerInvariant() + (match.Groups["flat"].Success ? "b" : string.Empty) + (match.Groups["minor"].Success ? "m" : string.Empty);
        if (!ChordSymbolParser.TryParse(written, notation, out var chord))
            return false;

        key = new KeyValue(chord, notation, null, AllCaps: true);
        return true;
    }

    private static bool TryParseWords(string text, out KeyValue key)
    {
        if (TryParseWorded(text, string.Empty, out key))
            return true;

        // A full stop after the whole key is kept as written.
        return text.EndsWith('.') && TryParseWorded(text[..^1], ".", out key);
    }

    private static bool TryParseWorded(string text, string trailing, out KeyValue key)
    {
        key = null!;
        var match = Worded.Match(text);
        if (!match.Success)
            return false;

        var attached = match.Groups["symbol"].Success;
        var detached = match.Groups["detached"].Success;
        var hasWord = match.Groups["word"].Success;
        var hasQuality = match.Groups["quality"].Success;
        if ((attached || detached) && hasWord || attached && detached || detached && !hasQuality || !hasQuality && !hasWord)
            return false;

        var allUpper = !text.Any(char.IsLower);
        var symbol = attached ? match.Groups["symbol"].Value : detached ? match.Groups["detached"].Value : null;
        if (symbol == "B" && !allUpper)
            return false; // An upper-case B is a flat sign only when the whole key is upper case.

        var name = match.Groups["note"].Value;
        var notation = name.Length == 1 ? Notation.English : Notation.Italian;
        if (!ChordSymbolParser.TryParseNote(name, notation, out var natural))
            return false;

        var word = match.Groups["word"].Success ? match.Groups["word"].Value : null;
        var root = new Note(natural.Letter, AccidentalSign(symbol ?? word));
        var quality = hasQuality ? match.Groups["quality"].Value : null;
        var chord = new ChordSymbol(root, quality?.StartsWith("min", StringComparison.OrdinalIgnoreCase) == true, string.Empty, string.Empty, null);
        key = new KeyValue(chord, notation, new KeyWords(symbol, word, quality, name, allUpper, detached, trailing));
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
        {
            var compact = ChordNotation.Format(shifted, new NotationOptions(notation, output.MinorStyle), keepMinorToken: !conversion);
            return AllCaps ? compact.ToUpperInvariant() : compact;
        }

        var root = shifted.Root;
        var name = ChordNotation.FormatNote(new Note(root.Letter, 0), notation);
        var text = Words.AllUpper ? name.ToUpperInvariant() : MatchCase(Words.NoteName, name);
        if (root.Accidental != 0)
            text += Words.Symbol is not null ? (Words.Detached ? " " : string.Empty) + AccidentalSymbol(root.Accidental) : " " + AccidentalWord(root.Accidental, notation);

        return text + (Words.Quality is null ? string.Empty : " " + Quality(Notation, notation)) + Words.Trailing;
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
        return MatchCase(Words!.AccidentalWord ?? Words.Quality!, word);
    }

    private string Quality(Notation from, Notation to)
    {
        var written = Words!.Quality!;
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

/// <summary>How a worded key was written: its accidental sign or word, quality word, note name and closing full stop as typed.</summary>
internal sealed record KeyWords(string? Symbol, string? AccidentalWord, string? Quality, string NoteName, bool AllUpper, bool Detached, string Trailing);
