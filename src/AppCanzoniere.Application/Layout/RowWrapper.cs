using System.Globalization;

namespace AppCanzoniere.Application.Layout;

/// <summary>Greedy word wrapping of one lyric line into rows, keeping chords over their text.</summary>
internal sealed partial class RowWrapper
{
    private readonly LineModel _model;
    private readonly FlowContext _ctx;
    private readonly List<WrapPiece> _pieces;
    private readonly Dictionary<(int Start, int End), double> _trimmedWidths = new();

    private RowWrapper(in LineModel model, IReadOnlyList<Seg> segments, FlowContext ctx)
    {
        _model = model;
        _ctx = ctx;
        _pieces = WrapPieces.Build(model, segments, ctx);
    }

    /// <summary>Splits the line into rows that fit the available width, with a hard break inside over-wide words.</summary>
    public static List<RowSpec> Wrap(in LineModel model, IReadOnlyList<Seg> segments, FlowContext ctx) =>
        new RowWrapper(model, segments, ctx).Run();

    private double Gap => _ctx.Options.MinChordGap;

    private string Text => _model.Text;

    private List<RowSpec> Run()
    {
        var rows = new List<RowSpec>();
        var start = 0;
        var strip = false;
        while (start < _pieces.Count)
        {
            var available = rows.Count == 0 ? _ctx.Available : _ctx.Available - _ctx.ContinuationIndent;
            if (strip)
                StripLeading(start);

            var end = FindRowEnd(start, available, out var hard);
            if (!hard)
                TrimTrailing(end - 1);

            rows.Add(new RowSpec(ToSegments(start, end), hard && end < _pieces.Count));
            start = end;
            strip = !hard;
        }

        return rows;
    }

    private int FindRowEnd(int start, double available, out bool hard)
    {
        hard = false;
        var acc = new RowAccumulator(Gap);
        var lastFit = -1;
        for (var j = start; j < _pieces.Count; j++)
        {
            acc.Add(Resolved(j));
            if (!IsRowEnd(j))
                continue;

            if (acc.Width(TrimDelta(j)) > available + SegmentMath.FitTolerance)
                break;
            lastFit = j + 1;
        }

        if (lastFit > 0)
            return lastFit;

        hard = true;
        return HardBreak(start, available);
    }

    private bool IsRowEnd(int j) => j == _pieces.Count - 1 || _pieces[j + 1].Candidate;

    private void StripLeading(int start)
    {
        for (var k = start; k < _pieces.Count; k++)
        {
            var piece = _pieces[k];
            var s = piece.Start;
            while (s < piece.End && TextBreaks.IsBreakSpace(Text[s]))
                s++;
            if (s > piece.Start)
                _pieces[k] = piece with { Start = s, TextWidth = WidthOf(s, piece.End) };
            if (s < piece.End)
                break;
        }
    }

    private void TrimTrailing(int j)
    {
        var piece = _pieces[j];
        var end = TrimmedEnd(piece);
        if (end < piece.End)
            _pieces[j] = piece with { End = end, TextWidth = TrimmedWidth(piece.Start, end) };
    }

    private double TrimDelta(int j)
    {
        var piece = _pieces[j];
        var end = TrimmedEnd(piece);
        return end == piece.End ? 0 : piece.TextWidth - TrimmedWidth(piece.Start, end);
    }

    private int TrimmedEnd(WrapPiece piece)
    {
        var end = piece.End;
        while (end > piece.Start && TextBreaks.IsBreakSpace(Text[end - 1]))
            end--;
        return end;
    }

    private double TrimmedWidth(int start, int end)
    {
        if (end <= start)
            return 0;
        if (!_trimmedWidths.TryGetValue((start, end), out var width))
        {
            width = WidthOf(start, end);
            _trimmedWidths[(start, end)] = width;
        }

        return width;
    }

    private double WidthOf(int start, int end) =>
        end <= start ? 0 : _ctx.WidthOf(Text.Substring(start, end - start), _model.LyricRole);

    private List<Seg> ToSegments(int start, int end)
    {
        var segs = new List<Seg>();
        var k = start;
        while (k < end)
        {
            var first = _pieces[k++];
            var segEnd = first.End;
            var merged = false;
            while (k < end && _pieces[k].ChordIndex < 0)
            {
                segEnd = _pieces[k++].End;
                merged = true;
            }

            var lyric = merged ? WidthOf(first.Start, segEnd) : first.TextWidth;
            segs.Add(new Seg(first.Start, segEnd, first.ChordIndex, lyric, first.ChordWidth));
        }

        return segs;
    }
}
