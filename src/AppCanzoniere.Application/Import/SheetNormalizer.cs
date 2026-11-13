using System.Text;

namespace AppCanzoniere.Application.Import;

/// <summary>One input line as pasted and after normalisation.</summary>
internal sealed record NormalizedLine(string Raw, string Text, bool TabExpanded);

/// <summary>Splits pasted text into lines and normalises tabs, invisible characters and trailing spaces.</summary>
internal static class SheetNormalizer
{
    private const char ByteOrderMark = '﻿';

    public static IReadOnlyList<string> SplitLines(string text)
    {
        var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        return lines.Length > 1 && lines[^1].Length == 0
            ? lines[..^1] // A terminating newline ends the last line, it does not start another.
            : lines;
    }

    public static IReadOnlyList<NormalizedLine> Normalize(string text, int tabWidth)
    {
        var withoutBom = text.StartsWith(ByteOrderMark) ? text[1..] : text;
        return SplitLines(withoutBom).Select(raw => NormalizeLine(raw, Math.Max(1, tabWidth))).ToList();
    }

    private static NormalizedLine NormalizeLine(string raw, int tabWidth)
    {
        var builder = new StringBuilder(raw.Length);
        var tabExpanded = false;
        foreach (var c in raw)
        {
            if (c == '\t')
            {
                tabExpanded = true;
                builder.Append(' ', tabWidth - builder.Length % tabWidth);
            }
            else if (c is not ('​' or ByteOrderMark))
                builder.Append(IsSpaceLike(c) ? ' ' : c);
        }

        return new NormalizedLine(raw, builder.ToString().TrimEnd(), tabExpanded);
    }

    private static bool IsSpaceLike(char c) =>
        c is ' ' or ' ' or ' ' or (>= ' ' and <= ' ');
}
