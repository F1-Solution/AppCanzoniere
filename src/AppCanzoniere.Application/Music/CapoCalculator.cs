namespace AppCanzoniere.Application.Music;

/// <summary>Relates the sounding chords of a song to the shapes played with a capo.</summary>
public static class CapoCalculator
{
    public const int MaxFret = 11;

    public static string ShapeChord(string soundingChord, int capo, AccidentalPreference preference = AccidentalPreference.Auto, string? soundingKey = null) => soundingChord;

    public static string SoundingChord(string shapeChord, int capo, AccidentalPreference preference = AccidentalPreference.Auto, string? shapeKey = null) => shapeChord;

    public static string ShapeKey(string soundingKey, int capo, AccidentalPreference preference = AccidentalPreference.Auto) => soundingKey;

    public static string SoundingKey(string shapeKey, int capo, AccidentalPreference preference = AccidentalPreference.Auto) => shapeKey;
}
