using AppCanzoniere.Application.Layout;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests.Layout;

/// <summary>Helpers shared by the layout tests: build sources, run layouts and find runs.</summary>
internal static class LayoutKit
{
    private static readonly ChordProParser Parser = new();

    public static LayoutSource Source(string chordPro) => LayoutSource.From(Parser.Parse(chordPro));

    public static LayoutOptions Options(double width = 400, double? pageHeight = null) =>
        new() { Width = width, PageHeight = pageHeight };

    public static FlowLayout Flow(string chordPro, LayoutOptions? options = null, ITextMeasurer? measurer = null) =>
        SongLayoutEngine.Flow(Source(chordPro), options ?? Options(), measurer ?? new FixedAdvanceMeasurer());

    public static SongLayout Layout(string chordPro, LayoutOptions options, ITextMeasurer? measurer = null) =>
        SongLayoutEngine.Layout(Source(chordPro), options, measurer ?? new FixedAdvanceMeasurer());

    public static LayoutSection OnlySection(FlowLayout flow)
    {
        Assert.Single(flow.Sections);
        return flow.Sections[0];
    }

    public static LayoutRow OnlyRow(LayoutLine line)
    {
        Assert.Single(line.Rows);
        return line.Rows[0];
    }

    public static IReadOnlyList<LayoutRun> Of(LayoutRow row, RunKind kind) => row.Runs.Where(r => r.Kind == kind).ToArray();

    public static LayoutRun Single(LayoutRow row, RunKind kind)
    {
        var runs = Of(row, kind);
        Assert.Single(runs);
        return runs[0];
    }

    public static LayoutRun Run(LayoutRow row, RunKind kind, string text)
    {
        var runs = row.Runs.Where(r => r.Kind == kind && r.Text == text).ToArray();
        Assert.Single(runs);
        return runs[0];
    }

    public static void AssertBox(LayoutRun run, double x, double width, double? y = null, double? height = null, double? baseline = null)
    {
        Assert.Equal(x, run.X, 9);
        Assert.Equal(width, run.Width, 9);
        if (y is { } top) Assert.Equal(top, run.Y, 9);
        if (height is { } h) Assert.Equal(h, run.Height, 9);
        if (baseline is { } b) Assert.Equal(b, run.BaselineY, 9);
    }

    /// <summary>A song of the given lines, one per source line.</summary>
    public static string Lines(params string[] lines) => string.Join('\n', lines);
}
