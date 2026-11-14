namespace AppCanzoniere.Application.PdfImport;

/// <summary>A run of rows read as one block: a full-width stretch (column 0 of 1) or one column of a two-column band.</summary>
public sealed record LayoutRegion(int ColumnIndex, int ColumnCount, IReadOnlyList<TextRow> Rows);

/// <summary>Splits a page into full-width stretches and two-column bands, in reading order.</summary>
public static class ColumnSplitter
{
    public static IReadOnlyList<LayoutRegion> Split(IReadOnlyList<PositionedWord> words, double pageWidth) => Array.Empty<LayoutRegion>();
}
