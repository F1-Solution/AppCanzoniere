namespace AppCanzoniere.Application.PageTurning;

public sealed class HardwareKeySource : IHardwareKeySource
{
    public event EventHandler<PageTurnDirection>? PageTurnRequested;

    public bool IsListening => PageTurnRequested is not null;

    public bool OnKey(string key, bool ctrl, bool alt)
    {
        var handler = PageTurnRequested;
        if (handler is null)
            return false;

        var direction = PageTurnKeys.Map(key, ctrl, alt);
        if (direction == PageTurnDirection.None)
            return false;

        handler(this, direction);
        return true;
    }
}
