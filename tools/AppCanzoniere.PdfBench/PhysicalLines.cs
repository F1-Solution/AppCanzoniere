using System.Text.RegularExpressions;

namespace AppCanzoniere.PdfBench;

/// <summary>Maps every converter line to the physical (newline-terminated) line it belongs to, as the corpus tests do.</summary>
internal static partial class PhysicalLines
{
    private const char NextLine = (char)0x85;
    private const char LineSeparator = (char)0x2028;
    private const char ParagraphSeparator = (char)0x2029;

    [GeneratedRegex(@"[\u0085\u2028\u2029]+(?=\r\n|\r|\n|\z)")]
    private static partial Regex TrailingSeparators();

    public static IReadOnlyList<int> Map(string rawText)
    {
        var text = TrailingSeparators().Replace(rawText, string.Empty);
        var map = new List<int> { 1 };
        var physical = 1;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
                continue;

            if (c is '\n' or '\r')
                physical++;
            else if (c is not NextLine and not LineSeparator and not ParagraphSeparator)
                continue;

            map.Add(physical);
        }

        return map;
    }
}
