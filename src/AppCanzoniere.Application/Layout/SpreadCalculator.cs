namespace AppCanzoniere.Application.Layout;

/// <summary>Maps page turns to the pair of pages shown side by side.</summary>
public static class SpreadCalculator
{
    /// <summary>Number of page turns for a page count (at least 1).</summary>
    public static int TurnCount(int pageCount, SpreadMode mode)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageCount, 1);
        return mode == SpreadMode.Full ? (pageCount + 1) / 2 : Math.Max(1, pageCount - 1);
    }

    /// <summary>Pages shown on the left and right at a turn; Right is null past the last page.</summary>
    public static (int Left, int? Right) At(int turn, int pageCount, SpreadMode mode)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(turn);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(turn, TurnCount(pageCount, mode));
        if (mode == SpreadMode.Full)
            return (2 * turn, 2 * turn + 1 < pageCount ? 2 * turn + 1 : null);

        var next = turn + 1;
        var (left, right) = next % 2 == 0 ? (next, turn) : (turn, next);
        return (left, right < pageCount ? right : null);
    }
}
