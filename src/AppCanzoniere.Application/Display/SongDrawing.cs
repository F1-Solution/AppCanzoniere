using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.Application.Display;

/// <summary>One string to draw with its left edge at X and its baseline at BaselineY, in device-independent units.</summary>
public sealed record DrawnRun(string Text, TextRole Role, RunKind Kind, double X, double BaselineY, double Width);

/// <summary>A visual row; Overflow is true when it is wider than the layout width (an unbreakable word).</summary>
public sealed record DrawnRow(double Y, double Height, bool Overflow, IReadOnlyList<DrawnRun> Runs);

/// <summary>A song laid out for one width, positioned from the top-left corner and ready to draw.</summary>
public sealed record SongDrawing(double Width, double Height, IReadOnlyList<DrawnRow> Rows, IReadOnlyList<DrawnRun> LabelRuns)
{
    /// <summary>Every run to draw: section labels first, then the rows in reading order.</summary>
    public IEnumerable<DrawnRun> Runs => LabelRuns.Concat(Rows.SelectMany(r => r.Runs));
}
