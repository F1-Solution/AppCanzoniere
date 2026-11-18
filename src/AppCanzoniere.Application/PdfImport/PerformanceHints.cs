using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>Recognises the instrument, capo and transposition remarks that sheets print beside a title and that are never titles or lyrics.</summary>
internal static partial class PerformanceHints
{
    [GeneratedRegex(@"^\(?\s*(?:accordi\s+per\b|per\s+(?:tastiera|chitarra)\b|capotasto\b|trasp\w*\b|tono\b)", RegexOptions.IgnoreCase)]
    private static partial Regex Leading();

    [GeneratedRegex(@"^\(?\s*(?:tastiera|chitarra)\b.*(?:capotasto|trasp|tono|[+\-]\s?\d|\b[A-G][#b]?-?\)?\s*$)", RegexOptions.IgnoreCase)]
    private static partial Regex Instrument();

    [GeneratedRegex(@"^(?<t>.*?\S)\s*(?:\btono\s+reale\b[^()]*)?(?<n>\((?:in\s+(?:do|re|mi|fa|sol|la|si|[A-G])\b[^()]*|[^()]*(?:capotasto|tastiera|chitarra|trasp|tono)[^()]*)\))\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex TrailingNote();

    [GeneratedRegex(@"^bis\b\s*(?<rest>.*)$", RegexOptions.IgnoreCase)]
    private static partial Regex Bis();

    /// <summary>True for a line that only says which instrument, capo position or transposition a sheet is for.</summary>
    public static bool IsHint(string line)
    {
        var t = SongLineKinds.StripNumber(line).Text;
        return Leading().IsMatch(t) || Instrument().IsMatch(t);
    }

    /// <summary>Splits a title from a trailing parenthesised performance note; the note is null when there is none.</summary>
    public static (string Title, string? Note) SplitNote(string text)
    {
        var m = TrailingNote().Match(text.Trim());
        return m.Success ? (m.Groups["t"].Value.Trim(), m.Groups["n"].Value) : (text, null);
    }

    /// <summary>True for "24 BIS ..." under song 24: the same song again for another instrument.</summary>
    public static bool IsBisOfLast(string line, int lastNumber)
    {
        var (number, text) = SongLineKinds.StripNumber(line);
        return number is not null && number == lastNumber && lastNumber > 0 && Bis().IsMatch(text);
    }
}
