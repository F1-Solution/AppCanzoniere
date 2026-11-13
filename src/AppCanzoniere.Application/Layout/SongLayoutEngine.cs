namespace AppCanzoniere.Application.Layout;

/// <summary>Pure, deterministic song layout: flow for a width, then pagination for a page height.</summary>
public static class SongLayoutEngine
{
    /// <summary>Lays out sections, lines, rows and runs for the given width; Y is relative to the flow top.</summary>
    public static FlowLayout Flow(LayoutSource source, LayoutOptions options, ITextMeasurer measurer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(measurer);

        var plans = SectionPlanner.Plan(source, options.DisplayMode);
        var numbered = LineNumbering.Count(source, plans);
        var gutter = LineNumbering.GutterWidth(numbered, options, measurer);
        var ctx = new FlowContext(options, measurer, gutter, gutter > 0);
        var sections = new List<LayoutSection>(plans.Count);
        var y = 0.0;
        var lastNumber = 0;
        foreach (var plan in plans)
        {
            var top = sections.Count == 0 ? 0 : y + options.SectionSpacing;
            var section = SectionLayouter.Layout(plan, source, top, ctx, ref lastNumber);
            sections.Add(section);
            y = section.Y + section.Height;
        }

        return new FlowLayout(options.Width, gutter, y, numbered, Frozen.From(sections));
    }

    /// <summary>Splits a flow into pages of options.PageHeight; one page of the flow height when it is null.</summary>
    public static SongLayout Paginate(FlowLayout flow, LayoutOptions options)
    {
        ArgumentNullException.ThrowIfNull(flow);
        ArgumentNullException.ThrowIfNull(options);
        return Paginator.Paginate(flow, options);
    }

    /// <summary>Flow then Paginate.</summary>
    public static SongLayout Layout(LayoutSource source, LayoutOptions options, ITextMeasurer measurer) =>
        Paginate(Flow(source, options, measurer), options);
}
