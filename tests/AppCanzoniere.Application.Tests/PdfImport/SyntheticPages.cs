using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

/// <summary>Builds laid-out pages from plain multi-line strings, one string per column.</summary>
internal static class SyntheticPages
{
    public static PageText Page(int number, string text) =>
        new(number, 1, false, new[] { Block(0, 1, text) });

    public static PageText TwoColumns(int number, string left, string right) =>
        new(number, 2, false, new[] { Block(0, 2, left), Block(1, 2, right) });

    public static PageText Blank(int number) => new(number, 0, true, Array.Empty<TextBlock>());

    private static TextBlock Block(int column, int count, string text) =>
        new(column, count, text.Split('\n').Select(l => new AlignedLine(l.TrimEnd(), null)).ToList());
}
