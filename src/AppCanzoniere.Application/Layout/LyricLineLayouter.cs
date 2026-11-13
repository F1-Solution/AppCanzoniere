namespace AppCanzoniere.Application.Layout;

/// <summary>Lays out one lyric line, with its chords, as one or more rows.</summary>
internal static class LyricLineLayouter
{
    public static LayoutLine Layout(int sourceIndex, LayoutSourceLine line, int? number, double top, TextRole lyricRole, FlowContext ctx)
    {
        var chords = ctx.LyricsOnly ? Array.Empty<LayoutChord>() : line.Chords;
        var instrumental = chords.Count > 0 && LineModel.IsBlank(line.Text);
        var model = new LineModel(line.Text, chords, lyricRole, instrumental);
        var segments = BuildSegments(model, ctx);
        var specs = SplitIntoRows(model, segments, ctx);

        var rows = new List<LayoutRow>(specs.Count);
        var y = top;
        for (var i = 0; i < specs.Count; i++)
        {
            var row = RowRenderer.Build(model, specs[i], i, y, number, ctx);
            rows.Add(row);
            y = row.Y + row.Height + ctx.Options.LineSpacing;
        }

        var last = rows[^1];
        return new LayoutLine(sourceIndex, number, top, last.Y + last.Height - top, Frozen.From(rows));
    }

    private static List<RowSpec> SplitIntoRows(in LineModel model, List<Seg> segments, FlowContext ctx)
    {
        var fits = SegmentMath.RowWidth(segments, ctx.Options.MinChordGap) <= ctx.Available + SegmentMath.FitTolerance;
        if (fits)
            return new List<RowSpec> { new(segments, false) };

        return model.Instrumental ? InstrumentalWrapper.Wrap(segments, ctx) : RowWrapper.Wrap(model, segments, ctx);
    }

    private static List<Seg> BuildSegments(in LineModel model, FlowContext ctx)
    {
        var text = model.Text;
        var chords = model.Chords;
        var segs = new List<Seg>(chords.Count + 1);
        var firstPosition = chords.Count > 0 ? chords[0].Position : text.Length;
        if (firstPosition > 0 || chords.Count == 0)
            segs.Add(new Seg(0, firstPosition, -1, LyricWidth(text, 0, firstPosition, model.LyricRole, ctx), 0));

        for (var i = 0; i < chords.Count; i++)
        {
            var start = chords[i].Position;
            var end = i + 1 < chords.Count ? chords[i + 1].Position : text.Length;
            segs.Add(new Seg(start, end, i, LyricWidth(text, start, end, model.LyricRole, ctx), ChordWidth(chords[i], ctx)));
        }

        return segs;
    }

    private static double LyricWidth(string text, int start, int end, TextRole role, FlowContext ctx) =>
        ctx.WidthOf(text.Substring(start, end - start), role);

    private static double ChordWidth(LayoutChord chord, FlowContext ctx)
    {
        var annotation = LineModel.IsAnnotation(chord.Text);
        var shown = annotation ? chord.Text[1..] : chord.Text;
        return ctx.WidthOf(shown, annotation ? TextRole.Annotation : TextRole.Chord) + 2 * ctx.ChordPadX;
    }
}
