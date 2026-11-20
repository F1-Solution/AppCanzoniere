using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

/// <summary>Builds laid-out pages from plain multi-line strings, one string per column.</summary>
internal static class SyntheticPages
{
    private const double BodyHeight = 10;
    private const double HeadingHeight = 20;
    private const double SlightlyLargerHeight = 10.8;

    public static PageText Page(int number, string text) =>
        new(number, 1, false, new[] { Block(0, 1, text) });

    public static PageText TwoColumns(int number, string left, string right) =>
        new(number, 2, false, new[] { Block(0, 2, left), Block(1, 2, right) });

    public static PageText Blank(int number) => new(number, 0, true, Array.Empty<TextBlock>());

    // A line starting with '^' is set in a larger font and one starting with '~' only slightly larger, so every line of such a page carries a row height.
    private static TextBlock Block(int column, int count, string text)
    {
        var sized = text.Split('\n').Any(l => l.StartsWith('^') || l.StartsWith('~'));
        return new(column, count, text.Split('\n').Select(l => Line(l, sized)).ToList());
    }

    private static AlignedLine Line(string line, bool sized)
    {
        var tall = line.StartsWith('^');
        var slight = line.StartsWith('~');
        var content = (tall || slight ? line[1..] : line).TrimEnd();
        if (!sized || content.Trim().Length == 0)
            return new AlignedLine(content, null);

        var height = tall ? HeadingHeight : slight ? SlightlyLargerHeight : BodyHeight;
        var word = new PositionedWord(content.Trim(), new WordBox(0, 0, content.Trim().Length * 5, height));
        return new AlignedLine(content, new TextRow(new[] { word }));
    }
}
