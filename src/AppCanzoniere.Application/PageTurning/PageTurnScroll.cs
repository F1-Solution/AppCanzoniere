namespace AppCanzoniere.Application.PageTurning;

public static class PageTurnScroll
{
    /// <summary>New scroll offset after a page turn: one viewport minus one row, clamped to the scrollable range; invalid inputs give 0.</summary>
    public static double NextOffset(double viewportHeight, double contentHeight, double offset, double rowHeight, PageTurnDirection direction)
    {
        if (!IsValid(viewportHeight) || !IsValid(contentHeight) || !IsValid(offset) || !IsValid(rowHeight))
            return 0;

        var max = Math.Max(0, contentHeight - viewportHeight);
        var step = viewportHeight > rowHeight ? viewportHeight - rowHeight : viewportHeight;
        var current = Math.Min(offset, max);
        var target = direction switch
        {
            PageTurnDirection.Forward => current + step,
            PageTurnDirection.Back => current - step,
            _ => current
        };
        return Math.Clamp(target, 0, max);
    }

    private static bool IsValid(double value) => double.IsFinite(value) && value >= 0;
}
