namespace AppCanzoniere.Application.Display;

/// <summary>Maps the app-language picker of the settings page to the stored language and back.</summary>
public static class LanguagePicker
{
    /// <summary>Stored values in picker order; the empty one is "Automatic", the device language.</summary>
    public static IReadOnlyList<string> Codes { get; } = new[] { string.Empty, "en", "it" };

    /// <summary>Labels in picker order.</summary>
    public static IReadOnlyList<string> Labels { get; } = new[] { "Automatic (device language)", "English", "Italiano" };

    public static int ToIndex(string? stored) => throw new NotImplementedException();

    public static bool TryToCode(int index, out string code) => throw new NotImplementedException();
}
