namespace AppCanzoniere.Application.Layout;

/// <summary>One string to draw at (TextX, BaselineY) inside its box.</summary>
/// <param name="ChordIndex">The chord run's ordinal in its source line's chords, else -1.</param>
public sealed record LayoutRun(
    RunKind Kind, string Text, TextRole Role,
    double X, double Y, double Width, double Height,
    double TextX, double BaselineY,
    int SourceOffset,
    bool HyphenAfter,
    int ChordIndex = -1);

/// <summary>A visual row of one source line.</summary>
/// <param name="Overflow">True when the row is wider than the width.</param>
public sealed record LayoutRow(int Index, double Y, double Height, IReadOnlyList<LayoutRun> Runs, bool HardBreakAfter, bool Overflow = false)
{
    private readonly IReadOnlyList<LayoutRun> _runs = Frozen.Copy(Runs, nameof(Runs));

    public IReadOnlyList<LayoutRun> Runs { get => _runs; init => _runs = Frozen.Copy(value, nameof(Runs)); }
}

/// <summary>A source line laid out in one or more rows.</summary>
/// <param name="SourceLineIndex">The line's index in the parsed document, titles included.</param>
/// <param name="Number">Set only on the fragment holding row 0.</param>
/// <param name="IsComment">True for a comment inside a section.</param>
public sealed record LayoutLine(int SourceLineIndex, int? Number, double Y, double Height, IReadOnlyList<LayoutRow> Rows, bool IsComment = false)
{
    private readonly IReadOnlyList<LayoutRow> _rows = Frozen.Copy(Rows, nameof(Rows));

    public IReadOnlyList<LayoutRow> Rows { get => _rows; init => _rows = Frozen.Copy(value, nameof(Rows)); }
}

/// <summary>A section or the part of it that falls on one page; LabelRuns holds one run per row of a wrapped label.</summary>
public sealed record LayoutSection(
    SectionKind Kind, string? Label, IReadOnlyList<LayoutRun> LabelRuns,
    double Y, double Height,
    double TabScale, bool TabClipped,
    bool ContinuedFromPrevious, bool ContinuesOnNext,
    IReadOnlyList<LayoutLine> Lines)
{
    private readonly IReadOnlyList<LayoutRun> _labelRuns = Frozen.Copy(LabelRuns, nameof(LabelRuns));
    private readonly IReadOnlyList<LayoutLine> _lines = Frozen.Copy(Lines, nameof(Lines));

    public IReadOnlyList<LayoutRun> LabelRuns { get => _labelRuns; init => _labelRuns = Frozen.Copy(value, nameof(LabelRuns)); }

    public IReadOnlyList<LayoutLine> Lines { get => _lines; init => _lines = Frozen.Copy(value, nameof(Lines)); }

    /// <summary>The first row of the label, or null when the section has none.</summary>
    public LayoutRun? LabelRun => LabelRuns.Count > 0 ? LabelRuns[0] : null;
}

/// <summary>Result of the flow phase: every section stacked from Y 0, independent of page height.</summary>
public sealed record FlowLayout(double Width, double GutterWidth, double Height, int NumberedLineCount, IReadOnlyList<LayoutSection> Sections)
{
    private readonly IReadOnlyList<LayoutSection> _sections = Frozen.Copy(Sections, nameof(Sections));

    public IReadOnlyList<LayoutSection> Sections { get => _sections; init => _sections = Frozen.Copy(value, nameof(Sections)); }
}

/// <summary>One page of a paginated song; Overflow is true when something taller than the page was placed anyway.</summary>
public sealed record LayoutPage(int Index, double Height, bool Overflow, IReadOnlyList<LayoutSection> Sections)
{
    private readonly IReadOnlyList<LayoutSection> _sections = Frozen.Copy(Sections, nameof(Sections));

    public IReadOnlyList<LayoutSection> Sections { get => _sections; init => _sections = Frozen.Copy(value, nameof(Sections)); }
}

/// <summary>A song laid out in pages.</summary>
public sealed record SongLayout(IReadOnlyList<LayoutPage> Pages, double GutterWidth, int NumberedLineCount)
{
    private readonly IReadOnlyList<LayoutPage> _pages = Frozen.Copy(Pages, nameof(Pages));

    public IReadOnlyList<LayoutPage> Pages { get => _pages; init => _pages = Frozen.Copy(value, nameof(Pages)); }
}
