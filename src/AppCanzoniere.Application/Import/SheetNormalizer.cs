using System.Globalization;
using System.Text;

namespace AppCanzoniere.Application.Import;

/// <summary>One input line as pasted and after normalisation.</summary>
internal sealed record NormalizedLine(string Raw, string Text, bool TabExpanded);

/// <summary>Splits pasted text into lines and normalises Unicode form, tabs, invisible characters and trailing spaces.</summary>
internal static class SheetNormalizer
{
    // Code points are written as numbers, so no invisible character appears literally in the source.
    private const char ByteOrderMark = (char)0xFEFF;
    private const char ZeroWidthSpace = (char)0x200B;
    private const char NoBreakSpace = (char)0x00A0;
    private const char EnSpace = (char)0x2000;
    private const char HairSpace = (char)0x200A;
    private const char FigureSpace = (char)0x2007;
    private const char NarrowNoBreakSpace = (char)0x202F;

    private static readonly string[] LineBreaks =
    {
        "\r\n", "\r", "\n", ((char)NextLine).ToString(), ((char)LineSeparator).ToString(), ((char)ParagraphSeparator).ToString()
    };

    private const int NextLine = 0x0085;
    private const int LineSeparator = 0x2028;
    private const int ParagraphSeparator = 0x2029;

    public static IReadOnlyList<string> SplitLines(string text)
    {
        var lines = text.Split(LineBreaks, StringSplitOptions.None);
        return lines.Length > 1 && lines[^1].Length == 0
            ? lines[..^1] // A terminating newline ends the last line, it does not start another.
            : lines;
    }

    public static IReadOnlyList<NormalizedLine> Normalize(string text, int tabWidth)
    {
        var withoutBom = text.StartsWith(ByteOrderMark) ? text[1..] : text;
        var pasted = SplitLines(withoutBom);
        var composed = SplitLines(Compose(withoutBom));
        var sameLines = pasted.Count == composed.Count;
        return composed.Select((line, i) => NormalizeLine(sameLines ? pasted[i] : line, line, tabWidth)).ToList();
    }

    // Unpaired surrogates cannot be normalised; such text is left as it is.
    private static string Compose(string text)
    {
        try
        {
            return text.Normalize(NormalizationForm.FormC);
        }
        catch (ArgumentException)
        {
            return text;
        }
    }

    private static NormalizedLine NormalizeLine(string raw, string composed, int tabWidth)
    {
        var cleaned = new StringBuilder(composed.Length);
        foreach (var c in composed)
        {
            if (c is not (ZeroWidthSpace or ByteOrderMark) && (c == '\t' || !char.IsControl(c)))
                cleaned.Append(IsSpaceLike(c) ? ' ' : c);
        }

        var text = cleaned.ToString();
        var tabExpanded = text.Contains('\t');
        return new NormalizedLine(raw, (tabExpanded ? ExpandTabs(text, tabWidth) : text).TrimEnd(), tabExpanded);
    }

    // Tab stops count text elements, like every other column of the converter.
    private static string ExpandTabs(string text, int tabWidth)
    {
        var builder = new StringBuilder(text.Length);
        var column = 0;
        var elements = StringInfo.GetTextElementEnumerator(text);
        while (elements.MoveNext())
        {
            var element = (string)elements.Current;
            if (element[0] != '\t')
            {
                builder.Append(element);
                column++;
                continue;
            }

            var width = tabWidth - column % tabWidth;
            builder.Append(' ', width).Append(element, 1, element.Length - 1);
            column += width;
        }

        return builder.ToString();
    }

    private static bool IsSpaceLike(char c) =>
        c is NoBreakSpace or FigureSpace or NarrowNoBreakSpace or (>= EnSpace and <= HairSpace);
}
