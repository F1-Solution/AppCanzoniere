using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>Cleans the title found in a PDF's metadata and drops the ones that say nothing about the song.</summary>
public static partial class PdfDocumentTitle
{
    /// <summary>Longest title kept, in characters.</summary>
    public const int MaxLength = 100;

    private const int MinLetters = 2;

    [GeneratedRegex(@"^microsoft\s+(?:word|powerpoint|excel|publisher)\b", RegexOptions.IgnoreCase)]
    private static partial Regex OfficeProduct();

    [GeneratedRegex(@"\.(?:pdf|docx?|odt|rtf|txt|ps|indd|pages|jpe?g|png|tiff?|xlsx?|pptx?|mus|musx|sib|mscz|mscx|gp[x0-9]?|ptb|html?)$", RegexOptions.IgnoreCase)]
    private static partial Regex FileExtension();

    // A drive path, a UNC path, a rooted path, or a path of several segments ending in a file extension.
    [GeneratedRegex(@"^(?:[a-z]:[\\/]|\\\\|/[^\s/])|[\\/].*[\\/].*\.[a-z0-9]{2,5}$", RegexOptions.IgnoreCase)]
    private static partial Regex FilePath();

    [GeneratedRegex(@"^(?:untitled|senza\s+titolo|nuovo\s+documento|new\s+document|documento|document|titolo|title|scan|scansione|slide|presentazione)[\s_\-]*(?:\(?\d+\)?)?$", RegexOptions.IgnoreCase)]
    private static partial Regex Placeholder();

    /// <summary>The title without control characters and extra spaces, cut to the limit; null when it is empty or useless.</summary>
    public static string? Clean(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var text = Collapse(raw);
        return text.Count(char.IsLetter) < MinLetters || IsUseless(text) ? null : Cut(text);
    }

    private static bool IsUseless(string text) =>
        OfficeProduct().IsMatch(text)
        || FileExtension().IsMatch(text)
        || (!text.Contains(' ') && (text.Contains('_') || text.Count(c => c == '-') >= 2))
        || FilePath().IsMatch(text)
        || Placeholder().IsMatch(text);

    private static string Collapse(string raw)
    {
        var builder = new StringBuilder(Math.Min(raw.Length, MaxLength * 2));
        var space = false;
        foreach (var c in raw)
        {
            var category = char.GetUnicodeCategory(c);
            if (category is UnicodeCategory.Format or UnicodeCategory.PrivateUse or UnicodeCategory.OtherNotAssigned || (char.IsControl(c) && !char.IsWhiteSpace(c)))
                continue;
            if (char.IsWhiteSpace(c))
            {
                space = builder.Length > 0;
                continue;
            }

            if (space)
                builder.Append(' ');
            space = false;
            builder.Append(c);
            if (builder.Length > MaxLength * 2)
                break;
        }

        return builder.ToString();
    }

    private static string Cut(string text)
    {
        if (text.Length <= MaxLength)
            return text;

        var end = char.IsHighSurrogate(text[MaxLength - 1]) ? MaxLength - 1 : MaxLength;
        return text[..end].TrimEnd();
    }
}
