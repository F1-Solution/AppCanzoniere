using System.Globalization;

namespace AppCanzoniere.Application.Layout;

/// <summary>Turns the segments of one row into positioned runs.</summary>
internal static class RowRenderer
{
    public static LayoutRow Build(in LineModel model, RowSpec spec, int rowIndex, double top, int? number, FlowContext ctx)
    {
        var options = ctx.Options;
        var segs = spec.Segments;
        var hasChords = segs.Any(s => s.ChordIndex >= 0);
        var lyric = ctx.Metrics(model.LyricRole);
        var chordHeight = hasChords ? ctx.ChordRowHeight : 0;
        var lyricTop = top + (hasChords ? chordHeight + options.ChordLyricSpacing : 0);
        var height = model.Instrumental ? chordHeight : lyricTop - top + lyric.LineHeight;
        var x = ctx.Gutter + (rowIndex > 0 ? options.ContinuationIndent : 0);

        var runs = new List<LayoutRun>(segs.Count * 2 + 1);
        if (number is { } n && rowIndex == 0 && ctx.ShowNumbers)
            runs.Add(NumberRun(n, lyricTop + lyric.Ascent, ctx));

        for (var i = 0; i < segs.Count; i++)
        {
            var last = i == segs.Count - 1;
            var advance = SegmentMath.Advance(segs[i], last, options.MinChordGap);
            AddChord(runs, model, segs[i], x, top, ctx);
            AddLyric(runs, model, segs[i], last ? null : segs[i + 1], advance, x, lyricTop, ctx);
            x += advance;
        }

        var right = runs.Where(r => r.Kind != RunKind.LineNumber).Select(r => r.X + r.Width).DefaultIfEmpty(0).Max();
        var overflow = ctx.Exceeds(right);
        return new LayoutRow(rowIndex, top, height, Frozen.From(runs), spec.HardBreakAfter, overflow);
    }

    private static void AddChord(List<LayoutRun> runs, in LineModel model, in Seg seg, double x, double top, FlowContext ctx)
    {
        if (seg.ChordIndex < 0)
            return;

        var chord = model.Chords[seg.ChordIndex];
        var annotation = LineModel.IsAnnotation(chord.Text);
        var role = annotation ? TextRole.Annotation : TextRole.Chord;
        runs.Add(new LayoutRun(
            annotation ? RunKind.Annotation : RunKind.Chord,
            annotation ? chord.Text[1..] : chord.Text,
            role,
            x, top, seg.ChordWidth, ctx.ChordRowHeight,
            x + ctx.ChordPadX, top + ctx.ChordPadY + ctx.Metrics(role).Ascent,
            chord.Position, false));
    }

    private static void AddLyric(List<LayoutRun> runs, in LineModel model, in Seg seg, Seg? next, double advance, double x, double lyricTop, FlowContext ctx)
    {
        if (model.Instrumental || seg.End <= seg.Start)
            return;

        var text = model.Text.Substring(seg.Start, seg.End - seg.Start);
        var splitsAWord = advance > seg.LyricWidth && next is { } following
            && following.End > following.Start
            && char.IsLetter(text[^1]) && char.IsLetter(model.Text[following.Start]);
        var metrics = ctx.Metrics(model.LyricRole);
        runs.Add(new LayoutRun(
            RunKind.Lyric, text, model.LyricRole,
            x, lyricTop, seg.LyricWidth, metrics.LineHeight,
            x, lyricTop + metrics.Ascent,
            seg.Start, splitsAWord));
    }

    private static LayoutRun NumberRun(int number, double baseline, FlowContext ctx)
    {
        var text = number.ToString(CultureInfo.InvariantCulture);
        var metrics = ctx.Metrics(TextRole.LineNumber);
        var width = ctx.WidthOf(text, TextRole.LineNumber);
        var x = ctx.Gutter - ctx.Options.GutterGap - width;
        return new LayoutRun(RunKind.LineNumber, text, TextRole.LineNumber, x, baseline - metrics.Ascent, width, metrics.LineHeight, x, baseline, -1, false);
    }
}
