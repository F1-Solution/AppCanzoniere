namespace AppCanzoniere.Application.Layout;

/// <summary>Per-flow constants and measurement helpers shared by the line layouters.</summary>
internal sealed class FlowContext
{
    /// <summary>Narrowest column a row keeps when the width is too small for the gutter or the indent.</summary>
    private const double MinColumn = 1;

    private readonly TextMetrics[] _metrics = new TextMetrics[(int)Enum.GetValues<TextRole>().Max() + 1];

    public FlowContext(LayoutOptions options, ITextMeasurer measurer, double gutter, bool showNumbers)
    {
        Options = options;
        Measurer = measurer;
        Gutter = gutter;
        ShowNumbers = showNumbers;
        foreach (var role in Enum.GetValues<TextRole>())
            _metrics[(int)role] = measurer.Measure(string.Empty, role);
    }

    public LayoutOptions Options { get; }

    public ITextMeasurer Measurer { get; }

    /// <summary>Width reserved on the left for line numbers; every X is shifted by it.</summary>
    public double Gutter { get; }

    public bool ShowNumbers { get; }

    /// <summary>Width of one page minus the gutter, at least one unit.</summary>
    public double Available => Math.Max(Options.Width - Gutter, MinColumn);

    /// <summary>The continuation indent, kept below the available width so a row always has a column.</summary>
    public double ContinuationIndent => Math.Min(Options.ContinuationIndent, Math.Max(0, Available - MinColumn));

    /// <summary>True when a right edge lies beyond the page width.</summary>
    public bool Exceeds(double right) => right > Options.Width + SegmentMath.FitTolerance;

    public bool LyricsOnly => Options.DisplayMode == LayoutDisplayMode.LyricsOnly;

    public double ChordPadX => Options.ChordStyle == ChordBoxStyle.Pill ? Options.PillPaddingX : 0;

    public double ChordPadY => Options.ChordStyle == ChordBoxStyle.Pill ? Options.PillPaddingY : 0;

    public double ChordRowHeight => Metrics(TextRole.Chord).LineHeight + 2 * ChordPadY;

    public TextMetrics Metrics(TextRole role) => _metrics[(int)role];

    /// <summary>Width of a string; the empty string is never sent to the measurer.</summary>
    public double WidthOf(string text, TextRole role) => text.Length == 0 ? 0 : Measurer.Measure(text, role).Width;
}
