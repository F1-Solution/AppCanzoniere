namespace AppCanzoniere.Application.PdfImport;

/// <summary>Aligned lines of one region of a page: a full-width stretch or one column.</summary>
public sealed record TextBlock(int ColumnIndex, int ColumnCount, IReadOnlyList<AlignedLine> Lines);

/// <summary>Text of one page in reading order; ColumnCount is 2 when any part of the page has two columns.</summary>
public sealed record PageText(int PageNumber, int ColumnCount, bool IsScanned, IReadOnlyList<TextBlock> Blocks)
{
    /// <summary>The blocks' lines, with a blank line between blocks.</summary>
    public string Text => string.Empty;
}

/// <summary>Turns the positioned words of a page into aligned text, column by column.</summary>
public static class PdfPageLayout
{
    public static PageText Lay(PdfPageWords page) => new(page.PageNumber, 0, false, Array.Empty<TextBlock>());
}
