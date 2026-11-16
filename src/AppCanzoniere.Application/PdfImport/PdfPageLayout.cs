namespace AppCanzoniere.Application.PdfImport;

/// <summary>Aligned lines of one region of a page: a full-width stretch or one column.</summary>
public sealed record TextBlock(int ColumnIndex, int ColumnCount, IReadOnlyList<AlignedLine> Lines);

/// <summary>Text of one page in reading order; ColumnCount is 2 when any part of the page has two columns.</summary>
public sealed record PageText(int PageNumber, int ColumnCount, bool IsScanned, IReadOnlyList<TextBlock> Blocks)
{
    /// <summary>The blocks' lines, with a blank line between blocks.</summary>
    public string Text => string.Join("\n\n", Blocks.Select(b => string.Join("\n", b.Lines.Select(l => l.Text))));
}

/// <summary>Thrown when a page would lay out to more than PdfPageLayout.MaxPageCharacters characters.</summary>
public sealed class PageTextTooLargeException(int pageNumber)
    : Exception($"Page {pageNumber} lays out to more than {PdfPageLayout.MaxPageCharacters} characters, the limit.")
{
    public int PageNumber { get; } = pageNumber;
}

/// <summary>Turns the positioned words of a page into aligned text, column by column.</summary>
public static class PdfPageLayout
{
    /// <summary>Most spaces one line may hold besides one after each word (the longest corpus line has 141 characters).</summary>
    public const int MaxLineColumns = 1_000;

    /// <summary>Most characters one page may lay out to (the longest corpus page has 6,125).</summary>
    public const int MaxPageCharacters = 200_000;

    /// <summary>Lays out a page; throws PageTextTooLargeException when its text would pass MaxPageCharacters.</summary>
    public static PageText Lay(PdfPageWords page, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(page);
        cancellationToken.ThrowIfCancellationRequested();
        var words = page.Words.Where(w => !string.IsNullOrWhiteSpace(w.Text) && IsFinite(w.Box) && w.Box.Width > 0).ToList();
        if (words.Count == 0)
            return new PageText(page.PageNumber, 0, page.HasImages, Array.Empty<TextBlock>());

        var blocks = new List<TextBlock>();
        var budget = new TextBudget(MaxPageCharacters, () => new PageTextTooLargeException(page.PageNumber));
        foreach (var region in ColumnSplitter.Split(words, page.Width, cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            blocks.Add(new TextBlock(region.ColumnIndex, region.ColumnCount, AlignedTextRenderer.Render(region.Rows, budget)));
        }

        return new PageText(page.PageNumber, blocks.Max(b => b.ColumnCount), false, blocks);
    }

    private static bool IsFinite(WordBox box) =>
        double.IsFinite(box.Left) && double.IsFinite(box.Bottom) && double.IsFinite(box.Right) && double.IsFinite(box.Top);
}

/// <summary>Characters a layout may still produce; spending past the limit throws the given exception.</summary>
internal sealed class TextBudget(long limit, Func<Exception> overrun)
{
    public static readonly TextBudget Unlimited = new(long.MaxValue, () => new InvalidOperationException());

    private long _spent;

    public void Spend(int characters)
    {
        _spent += characters;
        if (_spent > limit)
            throw overrun();
    }
}
