namespace AppCanzoniere.Application.Layout;

/// <summary>Pure, deterministic song layout: flow for a width, then pagination for a page height.</summary>
public static class SongLayoutEngine
{
    /// <summary>Lays out sections, lines, rows and runs for the given width; Y is relative to the flow top.</summary>
    public static FlowLayout Flow(LayoutSource source, LayoutOptions options, ITextMeasurer measurer) =>
        new(options.Width, 0, 0, 0, Frozen.From(new List<LayoutSection>()));

    /// <summary>Splits a flow into pages of options.PageHeight; one page of the flow height when it is null.</summary>
    public static SongLayout Paginate(FlowLayout flow, LayoutOptions options) =>
        new(Frozen.From(new List<LayoutPage>()), flow.GutterWidth, flow.NumberedLineCount);

    /// <summary>Flow then Paginate.</summary>
    public static SongLayout Layout(LayoutSource source, LayoutOptions options, ITextMeasurer measurer) =>
        Paginate(Flow(source, options, measurer), options);
}
