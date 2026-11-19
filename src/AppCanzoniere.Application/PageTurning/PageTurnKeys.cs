namespace AppCanzoniere.Application.PageTurning;

public static class PageTurnKeys
{
    /// <summary>Maps a platform-neutral key name to a direction; Ctrl or Alt combinations and unrelated keys give None.</summary>
    public static PageTurnDirection Map(string? key, bool ctrl, bool alt)
    {
        if (ctrl || alt)
            return PageTurnDirection.None;

        return key switch
        {
            "PageDown" or "ArrowDown" or "ArrowRight" or "Space" or "VolumeDown" => PageTurnDirection.Forward,
            "PageUp" or "ArrowUp" or "ArrowLeft" or "VolumeUp" => PageTurnDirection.Back,
            _ => PageTurnDirection.None
        };
    }
}
