namespace AppCanzoniere.Application.Layout;

/// <summary>Splits a flow into pages in one pass over sections and lines; never measures.</summary>
internal static class Paginator
{
    private const double Tolerance = 1e-9;

    public static SongLayout Paginate(FlowLayout flow, LayoutOptions options)
    {
        if (options.PageHeight is not { } height)
        {
            var single = new LayoutPage(0, flow.Height, false, flow.Sections);
            return new SongLayout(Frozen.From(new[] { single }), flow.GutterWidth, flow.NumberedLineCount);
        }

        var pages = new PageState(height);
        foreach (var section in flow.Sections)
            PlaceSection(pages, new SectionCursor(section), options, height);

        return new SongLayout(pages.Finish(), flow.GutterWidth, flow.NumberedLineCount);
    }

    private static void PlaceSection(PageState pages, SectionCursor cursor, LayoutOptions options, double height)
    {
        while (true)
        {
            var gap = pages.Empty ? 0 : options.SectionSpacing;
            var count = cursor.Lines.Count;
            var whole = cursor.HeightOf(count);
            if (pages.Y + gap + whole <= height + Tolerance)
            {
                pages.Add(cursor.Fragment(count, pages.Y + gap));
                return;
            }

            if (!pages.Empty && KeepWhole(cursor, options) && whole <= height + Tolerance)
            {
                pages.NewPage();
                continue;
            }

            if (!TryPlacePart(pages, cursor, options, height, gap))
                continue;

            if (cursor.Lines.Count == 0)
                return;
            pages.NewPage();
        }
    }

    /// <summary>Places a leading part of the section; false means a new page was opened and the section must be retried.</summary>
    private static bool TryPlacePart(PageState pages, SectionCursor cursor, LayoutOptions options, double height, double gap)
    {
        var fit = cursor.CountFitting(height - pages.Y - gap);
        if (fit == 0 || (!pages.Empty && fit < options.MinLinesAtBreak))
        {
            if (!pages.Empty)
            {
                pages.NewPage();
                return false;
            }

            PlaceOversizedLine(pages, cursor, height);
            return true;
        }

        var take = AdjustForWidow(fit, cursor.Lines.Count, pages.Empty, options);
        if (take < 0)
        {
            pages.NewPage();
            return false;
        }

        pages.Add(cursor.Fragment(take, pages.Y + gap));
        cursor.Advance(take);
        return true;
    }

    private static bool KeepWhole(SectionCursor cursor, LayoutOptions options)
    {
        var lines = cursor.Lines.Count;
        return (cursor.Template.Kind == SectionKind.Chorus && lines <= options.KeepWholeMaxLines)
            || lines < 2 * options.MinLinesAtBreak;
    }

    /// <summary>Avoids leaving fewer than the minimum lines for the next page; -1 means the section moves to a new page.</summary>
    private static int AdjustForWidow(int fit, int lines, bool pageEmpty, LayoutOptions options)
    {
        var rest = lines - fit;
        if (rest <= 0 || rest >= options.MinLinesAtBreak)
            return fit;

        var reduced = lines - options.MinLinesAtBreak;
        if (reduced >= (pageEmpty ? 1 : options.MinLinesAtBreak))
            return reduced;
        return pageEmpty ? fit : -1;
    }

    /// <summary>The first line does not fit an empty page: place as many of its rows as fit, at least one.</summary>
    private static void PlaceOversizedLine(PageState pages, SectionCursor cursor, double height)
    {
        var line = cursor.Lines[0];
        var rows = line.Rows;
        var take = 1;
        while (take < rows.Count && rows[take].Y + rows[take].Height - cursor.Top <= height + Tolerance)
            take++;

        if (take < rows.Count)
        {
            cursor.Lines[0] = LayoutShift.Slice(line, 0, take);
            cursor.Lines.Insert(1, LayoutShift.Slice(line, take, rows.Count));
        }

        pages.Add(cursor.Fragment(1, pages.Y));
        cursor.Advance(1);
    }

    private sealed class PageState
    {
        private readonly double _height;
        private readonly List<LayoutPage> _pages = new();
        private List<LayoutSection> _sections = new();
        private bool _overflow;

        public PageState(double height)
        {
            _height = height;
        }

        public double Y { get; private set; }

        public bool Empty => _sections.Count == 0;

        public void Add(LayoutSection section)
        {
            _sections.Add(section);
            Y = section.Y + section.Height;
            _overflow |= Y > _height + Tolerance;
        }

        public void NewPage()
        {
            if (Empty)
                return;

            _pages.Add(new LayoutPage(_pages.Count, Y, _overflow, Frozen.From(_sections)));
            _sections = new List<LayoutSection>();
            Y = 0;
            _overflow = false;
        }

        public IReadOnlyList<LayoutPage> Finish()
        {
            if (!Empty)
                NewPage();
            else if (_pages.Count == 0)
                _pages.Add(new LayoutPage(0, 0, false, Frozen.From(new List<LayoutSection>())));
            return Frozen.From(_pages);
        }
    }
}

/// <summary>The part of a section still to be placed: the remaining lines, and whether its label is still attached.</summary>
internal sealed class SectionCursor
{
    public SectionCursor(LayoutSection section)
    {
        Template = section;
        Lines = new List<LayoutLine>(section.Lines);
        Labeled = section.LabelRun is not null;
    }

    public LayoutSection Template { get; }

    public List<LayoutLine> Lines { get; }

    public bool Labeled { get; private set; }

    public bool Continued { get; private set; }

    /// <summary>Flow Y where the remaining part starts: the section top while the label is attached.</summary>
    public double Top => Labeled ? Template.Y : Lines[0].Y;

    public double HeightOf(int count) => Lines[count - 1].Y + Lines[count - 1].Height - Top;

    public int CountFitting(double space)
    {
        var count = 0;
        while (count < Lines.Count && HeightOf(count + 1) <= space + 1e-9)
            count++;
        return count;
    }

    /// <summary>The first <paramref name="count"/> lines as a section fragment whose top sits at <paramref name="pageY"/>.</summary>
    public LayoutSection Fragment(int count, double pageY)
    {
        var dy = pageY - Top;
        var lines = new LayoutLine[count];
        for (var i = 0; i < count; i++)
            lines[i] = LayoutShift.Line(Lines[i], dy);

        return Template with
        {
            Label = Labeled ? Template.Label : null,
            LabelRun = Labeled ? LayoutShift.Run(Template.LabelRun!, dy) : null,
            Y = pageY,
            Height = HeightOf(count),
            ContinuedFromPrevious = Continued,
            ContinuesOnNext = count < Lines.Count,
            Lines = Frozen.From(lines),
        };
    }

    public void Advance(int count)
    {
        Lines.RemoveRange(0, count);
        Labeled = false;
        Continued = true;
    }
}
