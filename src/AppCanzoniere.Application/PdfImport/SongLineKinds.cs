using System.Text.RegularExpressions;
using AppCanzoniere.Application.Import;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>One column of one page with the converter's verdict on which lines carry chords.</summary>
internal sealed record Region(int Page, IReadOnlyList<string> Lines, IReadOnlyList<bool> IsChord)
{
    public static Region Of(int page, IReadOnlyList<string> lines) => new(page, lines, ChordFlags(lines));

    /// <summary>True when a chord row lies above the line at index i.</summary>
    public bool HasChordAbove(int i) => IsChord.Take(i).Any(c => c);

    // The converter's own diagnostics tell which lines are chord lines; they map one to one to the input lines.
    private static IReadOnlyList<bool> ChordFlags(IReadOnlyList<string> lines)
    {
        var none = new bool[lines.Count];
        if (lines.Count == 0)
            return none;

        var result = ChordSheetConverter.Convert(string.Join('\n', lines));
        return result.Lines.Count == lines.Count ? result.Lines.Select(l => l.CarriesChords).ToList() : none;
    }
}

/// <summary>Pattern checks on single lines used to find titles, index entries and noise.</summary>
internal static partial class SongLineKinds
{
    [GeneratedRegex(@"^(?<n>[0-9]{1,3})(?:\s*[.)\-–]\s*|\s+)(?<rest>\p{L}.*)$")]
    private static partial Regex NumberedTitle();

    [GeneratedRegex(@"^[-–\s]*(?:pag\.?\s*)?[0-9]{1,4}[-–\s]*$", RegexOptions.IgnoreCase)]
    private static partial Regex PageNumberOnly();

    [GeneratedRegex(@"^(?:https?://|www\.)\S+$", RegexOptions.IgnoreCase)]
    private static partial Regex Link();

    [GeneratedRegex(@"(?:\.{2,}|\s{2,}|\spag\.?\s*)[0-9]{1,4}(?:\s*[-–])?$|\s[0-9]{1,4}(?:\s*[-–])?$", RegexOptions.IgnoreCase)]
    private static partial Regex EndsWithPageNumber();

    [GeneratedRegex(@"^[0-9]{1,3}\s*[.)\-–]?\s+\S")]
    private static partial Regex StartsWithNumber();

    public static bool IsBlank(string line) => string.IsNullOrWhiteSpace(line);

    /// <summary>True for lines that are only a page number or a link, which never belong to the song text.</summary>
    public static bool IsNoise(string line)
    {
        var t = line.Trim();
        return PageNumberOnly().IsMatch(t) || Link().IsMatch(t);
    }

    public static bool IsNumbered(string line) => StartsWithNumber().IsMatch(line.Trim());

    public static bool EndsWithNumber(string line) => EndsWithPageNumber().IsMatch(line.Trim());

    /// <summary>Splits "12. TITLE" into its number and text; the number is null and the text the trimmed line when there is none.</summary>
    public static (int? Number, string Text) StripNumber(string line)
    {
        var t = line.Trim();
        var m = NumberedTitle().Match(t);
        return m.Success && int.TryParse(m.Groups["n"].Value, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var number)
            ? (number, m.Groups["rest"].Value.Trim())
            : (null, t);
    }

    public static int Indent(string line) => line.Length - line.TrimStart().Length;

    /// <summary>True when the text has at least minLetters letters and every letter is upper case.</summary>
    public static bool IsUpperCase(string text, int minLetters)
    {
        var letters = 0;
        foreach (var c in text)
        {
            if (!char.IsLetter(c))
                continue;
            if (!char.IsUpper(c))
                return false;
            letters++;
        }

        return letters >= minLetters;
    }

    /// <summary>True when every letter of the text is I, V or X, as in a row of finger positions.</summary>
    public static bool IsRomanNumerals(string text) =>
        text.Any(char.IsLetter) && text.Where(char.IsLetter).All(c => "IVXivx".Contains(c));

    public static int WordCount(string text) => text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    /// <summary>True for Rit., Coro, Intro: and friends, which label a part of a song.</summary>
    public static bool IsSectionLabel(string text) =>
        !char.IsDigit(text.TrimStart().FirstOrDefault()) && SectionLabelParser.TryParse(text) is not null;
}
