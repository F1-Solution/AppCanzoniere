namespace AppCanzoniere.Application.PdfImport;

/// <summary>Aligned lines of one region of a page: a full-width stretch or one column.</summary>
public sealed record TextBlock(int ColumnIndex, int ColumnCount, IReadOnlyList<AlignedLine> Lines);

/// <summary>Text of one page in reading order; ColumnCount is 2 when any part of the page has two columns.</summary>
public sealed record PageText(int PageNumber, int ColumnCount, bool IsScanned, IReadOnlyList<TextBlock> Blocks)
{
    /// <summary>The blocks' lines, with a blank line between blocks.</summary>
    public string Text => string.Join("\n\n", Blocks.Select(b => string.Join("\n", b.Lines.Select(l => l.Text))));
}

/// <summary>Turns the positioned words of a page into aligned text, column by column.</summary>
public static class PdfPageLayout
{
    public static PageText Lay(PdfPageWords page)
    {
        ArgumentNullException.ThrowIfNull(page);
        var words = page.Words.Where(w => !string.IsNullOrWhiteSpace(w.Text) && w.Box.Width > 0).ToList();
        if (words.Count == 0)
            return new PageText(page.PageNumber, 0, page.HasImages, Array.Empty<TextBlock>());

        var blocks = ColumnSplitter.Split(words, page.Width)
            .Select(r => new TextBlock(r.ColumnIndex, r.ColumnCount, AlignedTextRenderer.Render(r.Rows)))
            .ToList();
        return new PageText(page.PageNumber, blocks.Max(b => b.ColumnCount), false, blocks);
    }
}
