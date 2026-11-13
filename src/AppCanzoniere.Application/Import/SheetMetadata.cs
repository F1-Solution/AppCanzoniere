using System.Globalization;
using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

internal enum MetaKind
{
    Title,
    Artist,
    Lyricist,
    Composer,
    Key,
    Capo,
    Tempo,
    Time,
    Comment
}

/// <summary>A header line turned into a directive; Unreadable ones are written as comments.</summary>
internal sealed record MetadataItem(MetaKind Kind, string Value, bool Unreadable);

/// <summary>Reads Titolo:, Tonalità:, Capo and similar header lines (spec 7).</summary>
internal static class MetadataParser
{
    private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
    private const int MaxCapo = 11;

    private static readonly (MetaKind Kind, Regex Pattern)[] Simple =
    {
        (MetaKind.Title, new Regex(@"^(?:titolo|title)\s*:\s*(?<v>.+)$", Options)),
        (MetaKind.Artist, new Regex(@"^(?:testo e musica|parole e musica|t\. e m\.|words and music)\s*:?\s*(?:di\s+)?(?<v>.+)$", Options)),
        (MetaKind.Artist, new Regex(@"^(?:autore|autori|artista|author|artist)\s*:\s*(?<v>.+)$", Options)),
        (MetaKind.Lyricist, new Regex(@"^(?:testo|parole|lyrics|words)\s*:\s*(?<v>.+)$", Options)),
        (MetaKind.Composer, new Regex(@"^(?:musica|music)\s*:\s*(?<v>.+)$", Options))
    };

    private static readonly Regex KeyLine = new(@"^(?:tonalit[àa]|tono|key)(?![\p{L}])\s*(?<colon>:)?\s*(?<v>.+)$", Options);
    private static readonly Regex KeyMode = new(@"^(?<x>\S+)\s+(?:(?<major>maggiore|major)|(?<minor>minore|minor))$", Options);
    private static readonly Regex CapoLine = new(@"^(?:capotasto|capo)\s*(?:al\s*)?:?\s*(?<v>\d{1,2}|[ivx]{1,4})\s*[°º]?\s*(?:tasto)?\s*\.?$", Options);
    private static readonly Regex TempoLine = new(@"^(?<k>tempo|time|bpm|metronomo)(?![\p{L}])\s*(?<colon>:)?\s*(?<v>.+)$", Options);
    private static readonly Regex TimeSignature = new(@"^\d+/\d+$", Options);
    private static readonly Regex BeatsPerMinute = new(@"^(?:♩\s*=\s*)?(?<n>\d{2,3})(?:\s*bpm)?$", Options);

    public static MetadataItem? TryParse(string trimmed)
    {
        foreach (var (kind, pattern) in Simple)
        {
            var match = pattern.Match(trimmed);
            if (match.Success)
                return new MetadataItem(kind, match.Groups["v"].Value.Trim(), false);
        }

        return ParseKey(trimmed) ?? ParseCapo(trimmed) ?? ParseTempo(trimmed);
    }

    private static MetadataItem? ParseKey(string trimmed)
    {
        var match = KeyLine.Match(trimmed);
        if (!match.Success)
            return null;

        var key = ReadKey(match.Groups["v"].Value.Trim());
        if (key is not null)
            return new MetadataItem(MetaKind.Key, key, false);

        return match.Groups["colon"].Success ? Unreadable(trimmed) : null;
    }

    // A key may be written in either notation; only "X minore" gets the minor marker appended.
    private static string? ReadKey(string value)
    {
        if (IsChord(value))
            return value;

        var mode = KeyMode.Match(value);
        if (!mode.Success || !IsChord(mode.Groups["x"].Value))
            return null;

        var x = mode.Groups["x"].Value;
        return mode.Groups["minor"].Success && !IsMinor(x) ? x + "m" : x;
    }

    private static MetadataItem? ParseCapo(string trimmed)
    {
        var match = CapoLine.Match(trimmed);
        if (!match.Success)
            return null;

        var value = match.Groups["v"].Value;
        var fret = char.IsDigit(value[0]) ? int.Parse(value, CultureInfo.InvariantCulture) : FromRoman(value);
        return fret is >= 1 and <= MaxCapo
            ? new MetadataItem(MetaKind.Capo, fret.ToString(CultureInfo.InvariantCulture), false)
            : Unreadable(trimmed);
    }

    private static MetadataItem? ParseTempo(string trimmed)
    {
        var match = TempoLine.Match(trimmed);
        if (!match.Success)
            return null;

        var value = match.Groups["v"].Value.Trim();
        if (TimeSignature.IsMatch(value))
            return new MetadataItem(MetaKind.Time, value, false);

        var tempo = BeatsPerMinute.Match(value);
        if (tempo.Success)
            return new MetadataItem(MetaKind.Tempo, tempo.Groups["n"].Value, false);

        return match.Groups["colon"].Success ? Unreadable(trimmed) : null;
    }

    private static MetadataItem Unreadable(string trimmed) => new(MetaKind.Comment, trimmed, true);

    private static bool IsChord(string text) =>
        ChordSymbolParser.TryParse(text, Notation.Italian, out _) || ChordSymbolParser.TryParse(text, Notation.English, out _);

    private static bool IsMinor(string text) =>
        (ChordSymbolParser.TryParse(text, Notation.Italian, out var italian) && italian.IsMinor) ||
        (ChordSymbolParser.TryParse(text, Notation.English, out var english) && english.IsMinor);

    private static int FromRoman(string numeral)
    {
        var total = 0;
        for (var i = 0; i < numeral.Length; i++)
        {
            var value = RomanValue(numeral[i]);
            total += i + 1 < numeral.Length && value < RomanValue(numeral[i + 1]) ? -value : value;
        }

        return total;
    }

    private static int RomanValue(char c) => char.ToUpperInvariant(c) switch { 'I' => 1, 'V' => 5, _ => 10 };
}
