namespace AppCanzoniere.MAUI;

/// <summary>Swaps the provisional Token colors between the light and the dark values when the system theme changes.</summary>
internal static class TokenPalette
{
    private static readonly Dictionary<string, string> Dark = new()
    {
        ["Token.bg"] = "#16120E",
        ["Token.surface"] = "#1F1A15",
        ["Token.surface2"] = "#2A241D",
        ["Token.lyricsBg"] = "#16120E",
        ["Token.text"] = "#EFE6D6",
        ["Token.text2"] = "#B9AD98",
        ["Token.accent"] = "#EC8B76",
        ["Token.onAccent"] = "#1A0E0A",
        ["Token.accentSoft"] = "#3D211A",
        ["Token.onAccentSoft"] = "#F7C2B4",
        ["Token.border"] = "#3B3229",
        ["Token.danger"] = "#FF9C8A",
        ["Token.focus"] = "#EFE6D6"
    };

    private static readonly Dictionary<string, string> Light = new()
    {
        ["Token.bg"] = "#F6F1E7",
        ["Token.surface"] = "#FFFCF5",
        ["Token.surface2"] = "#EFE8DA",
        ["Token.lyricsBg"] = "#FFFCF5",
        ["Token.text"] = "#1E1A15",
        ["Token.text2"] = "#5B5246",
        ["Token.accent"] = "#8C1E14",
        ["Token.onAccent"] = "#FFFFFF",
        ["Token.accentSoft"] = "#F2DED6",
        ["Token.onAccentSoft"] = "#6A160E",
        ["Token.border"] = "#D9CEBB",
        ["Token.danger"] = "#8C1E14",
        ["Token.focus"] = "#1E1A15"
    };

    /// <summary>Writes the values of the theme into the application resources; never throws.</summary>
    public static void Apply(AppTheme theme)
    {
        try
        {
            var resources = Microsoft.Maui.Controls.Application.Current?.Resources;
            if (resources is null)
                return;

            foreach (var (key, value) in theme == AppTheme.Dark ? Dark : Light)
                resources[key] = Color.FromArgb(value);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Applying the token palette failed: {ex}");
        }
    }
}
