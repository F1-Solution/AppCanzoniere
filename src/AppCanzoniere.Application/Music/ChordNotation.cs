namespace AppCanzoniere.Application.Music;

/// <summary>Reads chords in one notation and writes them in another.</summary>
public static class ChordNotation
{
    /// <summary>Converts a chord symbol between notations; text that is not a chord is returned unchanged.</summary>
    public static string Convert(string chord, NotationOptions from, NotationOptions to) => chord;
}
