using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.Music;

/// <summary>Strict parser for chord symbols in either notation; anything that is not a chord is rejected.</summary>
public static class ChordSymbolParser
{
    private const string Accidentals = "[#b♯♭]";

    private static readonly Regex EnglishRoot = new($"^([A-Ga-g])({Accidentals}?)(.*)$", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex ItalianRoot = new($"^((?i:sol|do|re|mi|fa|la|si))({Accidentals}?)(.*)$", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex MinorMarker = new("^(?:-|(?i:min)(?![a-zA-Z])|(?i:mi)(?![a-zA-Z])|m(?!(?i:aj)|(?i:a)[0-9]))", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex ValidSuffix = new(
        "^(?:maj|ma(?=[0-9])|add|aug|alt|dim|dom|sus|no|M|[0-9]|[#b+°ø(),.△Δ♯♭-]|/(?=[0-9]))*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool TryParse(string? text, Notation notation, out ChordSymbol chord)
    {
        chord = null!;
        var trimmed = text?.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return false;

        var slash = trimmed.LastIndexOf('/');
        if (slash > 0 && TryParseNote(trimmed[(slash + 1)..], notation, out var bass) &&
            TryParseBody(trimmed[..slash], notation, bass, out chord))
            return true;

        return TryParseBody(trimmed, notation, null, out chord);
    }

    public static bool TryParseNote(string text, Notation notation, out Note note)
    {
        note = default;
        var match = RootRegex(notation).Match(text);
        if (!match.Success || match.Groups[3].Length > 0)
            return false;

        note = ToNote(match, notation);
        return true;
    }

    private static bool TryParseBody(string body, Notation notation, Note? bass, out ChordSymbol chord)
    {
        chord = null!;
        var match = RootRegex(notation).Match(body);
        if (!match.Success)
            return false;

        var rest = match.Groups[3].Value;
        var minor = MinorMarker.Match(rest);
        var suffix = minor.Success ? rest[minor.Length..] : rest;
        if (!ValidSuffix.IsMatch(suffix))
            return false;

        chord = new ChordSymbol(ToNote(match, notation), minor.Success, minor.Value, suffix, bass);
        return true;
    }

    private static Regex RootRegex(Notation notation) => notation == Notation.Italian ? ItalianRoot : EnglishRoot;

    private static Note ToNote(Match match, Notation notation)
    {
        var letter = notation == Notation.Italian
            ? ItalianLetter(match.Groups[1].Value)
            : char.ToUpperInvariant(match.Groups[1].Value[0]);
        var accidental = match.Groups[2].Value switch
        {
            "#" or "♯" => 1,
            "b" or "♭" => -1,
            _ => 0
        };
        return new Note(letter, accidental);
    }

    private static char ItalianLetter(string name) => name.ToLowerInvariant() switch
    {
        "do" => 'C',
        "re" => 'D',
        "mi" => 'E',
        "fa" => 'F',
        "sol" => 'G',
        "la" => 'A',
        _ => 'B'
    };
}
