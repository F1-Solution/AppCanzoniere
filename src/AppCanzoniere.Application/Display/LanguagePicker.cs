namespace AppCanzoniere.Application.Display;

/// <summary>Maps the app-language picker of the settings page to the stored language and back.</summary>
public static class LanguagePicker
{
    /// <summary>Stored values in picker order; the empty one is "Automatic", the device language.</summary>
    public static IReadOnlyList<string> Codes { get; } = new[] { string.Empty, "en", "it" };

    /// <summary>Labels in picker order.</summary>
    public static IReadOnlyList<string> Labels { get; } = new[] { "Automatic (device language)", "English", "Italiano" };

    /// <summary>Entry of a stored language; blank or unknown languages show as Automatic.</summary>
    public static int ToIndex(string? stored)
    {
        var index = stored is null ? -1 : IndexOf(stored.Trim());
        return Math.Max(0, index);
    }

    /// <summary>False when the index is -1 (a picker being reset) or past the list: that is not a choice.</summary>
    public static bool TryToCode(int index, out string code)
    {
        var valid = index >= 0 && index < Codes.Count;
        code = valid ? Codes[index] : string.Empty;
        return valid;
    }

    private static int IndexOf(string code)
    {
        for (var i = 0; i < Codes.Count; i++)
        {
            if (string.Equals(Codes[i], code, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return -1;
    }
}
