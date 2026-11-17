using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.Application.Display;

/// <summary>A song with notation, accidentals, transposition and capo applied, ready to be laid out at any width.</summary>
public sealed class PreparedSong
{
    private readonly LayoutSource _source;

    internal PreparedSong(LayoutSource source, string key, int capo)
    {
        _source = source;
        Key = key;
        Capo = capo;
    }

    /// <summary>The sounding key; empty when the song has none.</summary>
    public string Key { get; }

    public int Capo { get; }

    /// <summary>Lays the song out for a width in device-independent units; a width that is not above 0 and finite throws <see cref="ArgumentOutOfRangeException"/>.</summary>
    public SongDrawing LayoutAt(double width, ITextMeasurer measurer)
    {
        ArgumentNullException.ThrowIfNull(measurer);
        var flow = SongLayoutEngine.Flow(_source, new LayoutOptions { Width = width }, measurer);
        var labels = flow.Sections.SelectMany(s => s.LabelRuns).Select(Draw).ToList();
        var rows = flow.Sections
            .SelectMany(s => s.Lines)
            .SelectMany(l => l.Rows)
            .Select(r => new DrawnRow(r.Y, r.Height, r.Overflow, r.Runs.Select(Draw).ToList()))
            .ToList();
        return new SongDrawing(flow.Width, flow.Height, rows, labels);
    }

    /// <summary>The song as monospace text, chords on a row above their lyrics.</summary>
    public string ToMonospaceText() =>
        LayoutTextProjector.Project(SongLayoutEngine.Flow(_source, LayoutTextProjector.Options, new MonospaceMeasurer()));

    private static DrawnRun Draw(LayoutRun run) =>
        new(run.Text, run.Role, run.Kind, run.TextX, run.BaselineY, run.Width);
}
