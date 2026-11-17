using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.MAUI.Controls;

/// <summary>Font, size and colour of one text role.</summary>
public sealed record RoleStyle(string Font, float Size, Color Color);

/// <summary>The reader's look, read from the Leggio.* resource tokens so a new theme needs no change to the control.</summary>
public sealed class LeggioStyle
{
    private readonly Dictionary<TextRole, RoleStyle> _roles;

    private LeggioStyle(Color background, Dictionary<TextRole, RoleStyle> roles)
    {
        Background = background;
        _roles = roles;
    }

    public Color Background { get; }

    public RoleStyle For(TextRole role) => _roles[role];

    /// <summary>Reads the tokens; a missing or invalid token falls back to a built-in value.</summary>
    public static LeggioStyle Load()
    {
        var lyric = Role("Lyric", "OpenSansRegular", 18, "#1F1F1F");
        var chord = Role("Chord", "OpenSansSemibold", 16, "#512BD4");
        var annotation = Role("Annotation", "OpenSansRegular", 13, "#6E6E6E");
        var label = Role("Label", "OpenSansSemibold", 14, "#2B0B98");
        var comment = Role("Comment", "OpenSansRegular", 14, "#6E6E6E");
        return new LeggioStyle(Token("Leggio.Background", Color.FromArgb("#FFFFFF")), new Dictionary<TextRole, RoleStyle>
        {
            [TextRole.Lyric] = lyric,
            [TextRole.ChorusLyric] = lyric,
            [TextRole.Chord] = chord,
            [TextRole.Annotation] = annotation,
            [TextRole.SectionLabel] = label,
            [TextRole.Comment] = comment,
            [TextRole.Tab] = lyric with { Font = "JetBrainsMono", Size = 14 },
            [TextRole.LineNumber] = comment
        });
    }

    private static RoleStyle Role(string name, string font, float size, string color) => new(
        Token($"Leggio.{name}.Font", font),
        (float)Token($"Leggio.{name}.Size", (double)size),
        Token($"Leggio.{name}.Color", Color.FromArgb(color)));

    private static T Token<T>(string key, T fallback)
    {
        try
        {
            var resources = Microsoft.Maui.Controls.Application.Current?.Resources;
            if (resources is not null && resources.TryGetValue(key, out var value) && value is T typed)
                return typed;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Reading token {key} failed: {ex}");
        }

        return fallback;
    }
}
