namespace AppCanzoniere.Domain.Entities;

public class UserSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PreferredInstrument { get; set; } = "Guitar";
    public string PreferredLanguage { get; set; } = string.Empty; // empty: use the device language
    public string ChordNotation { get; set; } = "Anglo"; // "Anglo" (C,D,E) vs "Italian" (DO,RE,MI)
    public bool ShowSharpInsteadOfFlat { get; set; } = true;
    public string ThemeMode { get; set; } = "System"; // "Light", "Dark", "System"
    public bool EnableCloudSync { get; set; }
    public Enums.CloudProvider CloudProvider { get; set; } = Enums.CloudProvider.Local;
    public bool EnableAIFeatures { get; set; } = true;
}
