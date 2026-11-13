namespace AppCanzoniere.Application.Layout;

/// <summary>Per-flow constants and measurement helpers shared by the line layouters.</summary>
internal sealed class FlowContext
{
    private const int RoleCount = 8;

    private readonly TextMetrics[] _metrics = new TextMetrics[RoleCount];

    public FlowContext(LayoutOptions options, ITextMeasurer measurer, double gutter, bool showNumbers)
    {
        Options = options;
        Measurer = measurer;
        Gutter = gutter;
        ShowNumbers = showNumbers;
        for (var role = 0; role < RoleCount; role++)
            _metrics[role] = measurer.Measure(string.Empty, (TextRole)role);
    }

    public LayoutOptions Options { get; }

    public ITextMeasurer Measurer { get; }

    /// <summary>Width reserved on the left for line numbers; every X is shifted by it.</summary>
    public double Gutter { get; }

    public bool ShowNumbers { get; }

    /// <summary>Width of one page minus the gutter.</summary>
    public double Available => Options.Width - Gutter;

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
