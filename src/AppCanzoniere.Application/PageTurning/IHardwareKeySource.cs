namespace AppCanzoniere.Application.PageTurning;

/// <summary>Receives hardware keys from the platform and turns them into page-turn requests while a screen listens.</summary>
public interface IHardwareKeySource
{
    /// <summary>Raised on a page-turn key while at least one screen is subscribed.</summary>
    event EventHandler<PageTurnDirection>? PageTurnRequested;

    /// <summary>True while at least one screen is subscribed.</summary>
    bool IsListening { get; }

    /// <summary>Called by the platform for each key phase; returns true when a listening screen maps the key and it must be consumed. Only the first press turns the page.</summary>
    bool OnKey(string key, bool ctrl, bool alt, KeyPhase phase = KeyPhase.Press);
}
