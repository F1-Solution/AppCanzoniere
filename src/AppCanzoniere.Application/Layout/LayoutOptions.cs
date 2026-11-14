namespace AppCanzoniere.Application.Layout;

/// <summary>Immutable layout settings; invalid values throw <see cref="ArgumentOutOfRangeException"/> as they are set.</summary>
public sealed record LayoutOptions
{
    private readonly double _width = 1;
    private readonly double? _pageHeight;
    private readonly double _lineSpacing = 4;
    private readonly double _sectionSpacing = 16;
    private readonly double _chordLyricSpacing;
    private readonly double _minChordGap = 8;
    private readonly double _pillPaddingX = 4;
    private readonly double _pillPaddingY = 2;
    private readonly double _continuationIndent = 20;
    private readonly double _gutterGap = 8;
    private readonly double _labelSpacing = 4;
    private readonly double _minTabScale = 0.5;
    private readonly int _minLinesAtBreak = 2;
    private readonly int _keepWholeMaxLines = 6;

    /// <summary>Available width of one page or column, in layout units; above 0.</summary>
    public required double Width { get => _width; init => _width = Positive(value, nameof(Width)); }

    /// <summary>Page height; null is scroll mode, one tall page.</summary>
    public double? PageHeight
    {
        get => _pageHeight;
        init => _pageHeight = value is { } height ? Positive(height, nameof(PageHeight)) : null;
    }

    public LayoutDisplayMode DisplayMode { get; init; } = LayoutDisplayMode.ChordsAndLyrics;

    public ChordBoxStyle ChordStyle { get; init; } = ChordBoxStyle.Plain;

    /// <summary>Show line numbers in a left gutter.</summary>
    public bool LineNumbers { get; init; }

    /// <summary>Gap between lines and between continuation rows.</summary>
    public double LineSpacing { get => _lineSpacing; init => _lineSpacing = NonNegative(value, nameof(LineSpacing)); }

    /// <summary>Gap between sections.</summary>
    public double SectionSpacing { get => _sectionSpacing; init => _sectionSpacing = NonNegative(value, nameof(SectionSpacing)); }

    /// <summary>Gap between the chord row and the lyric row.</summary>
    public double ChordLyricSpacing { get => _chordLyricSpacing; init => _chordLyricSpacing = NonNegative(value, nameof(ChordLyricSpacing)); }

    /// <summary>Minimum horizontal gap between a chord box and the next one.</summary>
    public double MinChordGap { get => _minChordGap; init => _minChordGap = NonNegative(value, nameof(MinChordGap)); }

    public double PillPaddingX { get => _pillPaddingX; init => _pillPaddingX = NonNegative(value, nameof(PillPaddingX)); }

    public double PillPaddingY { get => _pillPaddingY; init => _pillPaddingY = NonNegative(value, nameof(PillPaddingY)); }

    /// <summary>X offset of wrapped continuation rows.</summary>
    public double ContinuationIndent { get => _continuationIndent; init => _continuationIndent = NonNegative(value, nameof(ContinuationIndent)); }

    /// <summary>Gap between line numbers and content.</summary>
    public double GutterGap { get => _gutterGap; init => _gutterGap = NonNegative(value, nameof(GutterGap)); }

    /// <summary>Gap between a section label and the section's first line.</summary>
    public double LabelSpacing { get => _labelSpacing; init => _labelSpacing = NonNegative(value, nameof(LabelSpacing)); }

    /// <summary>Smallest scale applied to a tab block before clipping; in (0, 1].</summary>
    public double MinTabScale
    {
        get => _minTabScale;
        init => _minTabScale = value is > 0 and <= 1 ? value : throw OutOfRange(nameof(MinTabScale), value);
    }

    /// <summary>Widow and orphan control: minimum lines of a section left at a page bottom or carried to the next top.</summary>
    public int MinLinesAtBreak
    {
        get => _minLinesAtBreak;
        init => _minLinesAtBreak = value >= 1 ? value : throw OutOfRange(nameof(MinLinesAtBreak), value);
    }

    /// <summary>A chorus with at most this many lines is never split if it fits on one page.</summary>
    public int KeepWholeMaxLines
    {
        get => _keepWholeMaxLines;
        init => _keepWholeMaxLines = value >= 0 ? value : throw OutOfRange(nameof(KeepWholeMaxLines), value);
    }

    private static double Positive(double value, string name) =>
        value > 0 && double.IsFinite(value) ? value : throw OutOfRange(name, value);

    private static double NonNegative(double value, string name) =>
        value >= 0 && double.IsFinite(value) ? value : throw OutOfRange(name, value);

    private static ArgumentOutOfRangeException OutOfRange(string name, object value) =>
        new(name, value, $"{name} is out of range.");
}
