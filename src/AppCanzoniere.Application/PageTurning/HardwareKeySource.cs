namespace AppCanzoniere.Application.PageTurning;

public sealed class HardwareKeySource : IHardwareKeySource
{
    public event EventHandler<PageTurnDirection>? PageTurnRequested;

    public bool IsListening => PageTurnRequested is not null;

    public bool OnKey(string key, bool ctrl, bool alt, KeyPhase phase = KeyPhase.Press)
    {
        var handler = PageTurnRequested;
        if (handler is null)
            return false;

        var direction = PageTurnKeys.Map(key, ctrl, alt);
        if (direction == PageTurnDirection.None)
            return false;

        // Only the first press turns; a held key or its release is swallowed so nothing else reacts to half a key.
        if (phase == KeyPhase.Press)
            handler(this, direction);
        return true;
    }
}
