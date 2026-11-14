using System.Globalization;

namespace AppCanzoniere.Application.Layout;

internal sealed partial class RowWrapper
{
    private int HardBreak(int start, double available)
    {
        var acc = new RowAccumulator(Gap);
        for (var j = start; j < _pieces.Count; j++)
        {
            var before = acc;
            acc.Add(Resolved(j));
            if (acc.Width(0) > available + SegmentMath.FitTolerance)
                return BreakInside(start, j, before, available);
        }

        return _pieces.Count;
    }

    /// <summary>A cut-off remainder carries an estimated width; it is measured exactly once it could fit a row.</summary>
    private WrapPiece Resolved(int j)
    {
        var piece = _pieces[j];
        if (!piece.Approximate || piece.TextWidth > _ctx.Available + SegmentMath.FitTolerance)
            return piece;

        piece = piece with { TextWidth = WidthOf(piece.Start, piece.End), Approximate = false };
        _pieces[j] = piece;
        return piece;
    }

    private int BreakInside(int start, int j, RowAccumulator before, double available)
    {
        var piece = _pieces[j];
        if (piece.End <= piece.Start)
            return j == start ? j + 1 : j;

        var fit = FittingPrefix(piece, before, available);
        if (fit.Whole)
            return j + 1;

        if (fit.End == piece.Start)
        {
            if (j != start)
                return j;

            fit = FirstElement(piece);
        }

        SplitPiece(j, fit.End, fit.Width);
        return j + 1;
    }

    private (bool Whole, int End, double Width) FirstElement(WrapPiece piece)
    {
        var end = piece.Start + StringInfo.GetNextTextElementLength(Text.AsSpan(piece.Start, piece.End - piece.Start));
        return (false, end, WidthOf(piece.Start, end));
    }

    /// <summary>The longest prefix of the piece that fits, found by probing near the average width and widening a bracket.</summary>
    private (bool Whole, int End, double Width) FittingPrefix(WrapPiece piece, RowAccumulator before, double available)
    {
        var probe = new ElementProbe(Text, piece.Start, piece.End);
        var best = (End: piece.Start, Width: 0.0);

        bool Fits(int count)
        {
            var end = probe.EndOf(count);
            var width = WidthOf(piece.Start, end);
            var trial = before;
            trial.Add(piece with { End = end, TextWidth = width, Approximate = false });
            var fits = trial.Width(0) <= available + SegmentMath.FitTolerance;
            if (fits && end > best.End)
                best = (end, width);
            return fits;
        }

        var chars = piece.End - piece.Start;
        var room = available - before.Width(0);
        var guess = piece.TextWidth > 0 ? (int)Math.Clamp(room / (piece.TextWidth / chars), 1, chars) : 1;
        probe.Ensure(guess);
        var (low, high) = Bracket(probe, Math.Min(guess, probe.Count), Fits);
        if (high < 0)
            return (true, piece.End, 0);

        while (high - low > 1)
        {
            var mid = low + (high - low) / 2;
            if (Fits(mid))
                low = mid;
            else
                high = mid;
        }

        return (false, best.End, best.Width);
    }

    /// <summary>A pair of element counts where the lower fits (or is 0) and the higher does not; high is -1 when the whole piece fits.</summary>
    private static (int Low, int High) Bracket(ElementProbe probe, int guess, Func<int, bool> fits)
    {
        var step = 1;
        if (fits(guess))
        {
            var low = guess;
            while (true)
            {
                var next = Math.Min(low + step, probe.Ensure(low + step));
                if (next == low)
                    return (low, -1);

                if (!fits(next))
                    return (low, next);

                (low, step) = (next, step * 2);
            }
        }

        var high = guess;
        while (high - step > 0)
        {
            var next = high - step;
            if (fits(next))
                return (next, high);

            (high, step) = (next, step * 2);
        }

        return (0, high);
    }

    private void SplitPiece(int j, int cut, double prefixWidth)
    {
        var piece = _pieces[j];
        if (cut >= piece.End)
            return;

        _pieces[j] = piece with { End = cut, TextWidth = prefixWidth, Approximate = false };
        var rest = Math.Max(0, piece.TextWidth - prefixWidth);
        _pieces.Insert(j + 1, new WrapPiece(cut, piece.End, -1, rest, 0, false, true));
    }
}

/// <summary>Walks the text elements of a piece forward on demand, so a probe never scans the whole remainder.</summary>
internal sealed class ElementProbe
{
    private readonly string _text;
    private readonly int _end;
    private readonly List<int> _ends = new();
    private int _offset;

    public ElementProbe(string text, int start, int end)
    {
        _text = text;
        _end = end;
        _offset = start;
    }

    public int Count => _ends.Count;

    /// <summary>Reads elements until <paramref name="count"/> are known or the piece ends; returns how many are known.</summary>
    public int Ensure(int count)
    {
        while (_ends.Count < count && _offset < _end)
        {
            _offset += StringInfo.GetNextTextElementLength(_text.AsSpan(_offset, _end - _offset));
            _ends.Add(_offset);
        }

        return _ends.Count;
    }

    public int EndOf(int count) => _ends[count - 1];
}
