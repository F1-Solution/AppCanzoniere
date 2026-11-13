namespace AppCanzoniere.Application.Layout;

/// <summary>One string to draw. X/Y is the top-left of its box; the painter draws Text at (TextX, BaselineY).</summary>
public sealed record LayoutRun(
    RunKind Kind, string Text, TextRole Role,
    double X, double Y, double Width, double Height,
    double TextX, double BaselineY,
    int SourceOffset,
    bool HyphenAfter);

/// <summary>A visual row of one source line; Index 0 is the first row, above 0 are wrapped continuation rows.</summary>
public sealed record LayoutRow(int Index, double Y, double Height, IReadOnlyList<LayoutRun> Runs, bool HardBreakAfter);

/// <summary>A source line laid out in one or more rows; Number is set only on the fragment holding row 0.</summary>
public sealed record LayoutLine(int SourceLineIndex, int? Number, double Y, double Height, IReadOnlyList<LayoutRow> Rows);

/// <summary>A section or the part of it that falls on one page.</summary>
public sealed record LayoutSection(
    SectionKind Kind, string? Label, LayoutRun? LabelRun,
    double Y, double Height,
    double TabScale, bool TabClipped,
    bool ContinuedFromPrevious, bool ContinuesOnNext,
    IReadOnlyList<LayoutLine> Lines);

/// <summary>Result of the flow phase: every section stacked from Y 0, independent of page height.</summary>
public sealed record FlowLayout(double Width, double GutterWidth, double Height, int NumberedLineCount, IReadOnlyList<LayoutSection> Sections);

/// <summary>One page of a paginated song; Overflow is true when something taller than the page was placed anyway.</summary>
public sealed record LayoutPage(int Index, double Height, bool Overflow, IReadOnlyList<LayoutSection> Sections);

/// <summary>A song laid out in pages.</summary>
public sealed record SongLayout(IReadOnlyList<LayoutPage> Pages, double GutterWidth, int NumberedLineCount);

internal static class Frozen
{
    public static IReadOnlyList<T> From<T>(List<T> items) => Array.AsReadOnly(items.ToArray());

    public static IReadOnlyList<T> From<T>(T[] items) => Array.AsReadOnly(items);
}
