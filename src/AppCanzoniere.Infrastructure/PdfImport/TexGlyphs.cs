using System.Text.RegularExpressions;
using UglyToad.PdfPig.Content;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Reads the flat, natural and sharp that TeX draws from its math-italic fonts, which PdfPig reports as brackets.</summary>
internal static partial class TexGlyphs
{
    // Computer Modern and Latin Modern math italic, in any size, with or without the six-letter subset prefix.
    [GeneratedRegex(@"^(?:[A-Z]{6}\+)?(?:CMMI|LMMI)B?\d+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex MathItalic();

    // Codes 0x5B, 0x5C and 0x5D of the font; the flat and sharp use the ASCII signs the chord detector reads.
    private static readonly Dictionary<char, string> MathItalicSigns = new()
    {
        ['['] = "b",
        ['\\'] = "♮",
        [']'] = "#",
    };

    /// <summary>The text of the letter, with a TeX accidental sign put back when the letter comes from a TeX math font.</summary>
    public static string TextOf(Letter letter) =>
        letter.Value.Length == 1 && MathItalicSigns.TryGetValue(letter.Value[0], out var sign) && IsMathItalic(letter.FontName)
            ? sign
            : letter.Value;

    private static bool IsMathItalic(string? fontName) => fontName is not null && MathItalic().IsMatch(fontName);
}
