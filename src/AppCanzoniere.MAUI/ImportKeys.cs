namespace AppCanzoniere.MAUI;

/// <summary>Lets the import page claim the Windows shortcuts (Esc, Ctrl+S, Ctrl+O, Alt+arrows) while it is on screen.</summary>
internal static class ImportKeys
{
    /// <summary>Set by the import page while visible; returns true when the key was used.</summary>
    public static Func<string, bool, bool, bool>? Handler { get; set; }

    public static bool Handle(string key, bool ctrl, bool alt)
    {
        try
        {
            return Handler?.Invoke(key, ctrl, alt) ?? false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Import key handling failed: {ex}");
            return false;
        }
    }
}
