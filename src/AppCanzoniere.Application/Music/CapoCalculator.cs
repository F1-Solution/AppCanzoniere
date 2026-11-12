using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Music;

/// <summary>Relates the sounding chords of a song to the shapes played with a capo; frets outside 0-11 are rejected.</summary>
public static class CapoCalculator
{
    public const int MaxFret = 11;

    private static readonly ChordTransposer Transposer = new();

    /// <summary>Chord shape to play for a sounding chord; <paramref name="soundingKey"/> steers the Auto spelling.</summary>
    public static string ShapeChord(string soundingChord, int capo, AccidentalPreference preference = AccidentalPreference.Auto, string? soundingKey = null) =>
        Shift(soundingChord, -ValidFret(capo), capo, preference, soundingKey, isKey: false);

    /// <summary>Chord that sounds when a shape is played with the capo.</summary>
    public static string SoundingChord(string shapeChord, int capo, AccidentalPreference preference = AccidentalPreference.Auto, string? shapeKey = null) =>
        Shift(shapeChord, ValidFret(capo), capo, preference, shapeKey, isKey: false);

    /// <summary>Key of the shapes to play for a sounding key.</summary>
    public static string ShapeKey(string soundingKey, int capo, AccidentalPreference preference = AccidentalPreference.Auto) =>
        Shift(soundingKey, -ValidFret(capo), capo, preference, null, isKey: true);

    /// <summary>Sounding key when shapes in <paramref name="shapeKey"/> are played with the capo.</summary>
    public static string SoundingKey(string shapeKey, int capo, AccidentalPreference preference = AccidentalPreference.Auto) =>
        Shift(shapeKey, ValidFret(capo), capo, preference, null, isKey: true);

    private static string Shift(string text, int steps, int capo, AccidentalPreference preference, string? key, bool isKey)
    {
        if (capo == 0)
            return text;

        return isKey
            ? Transposer.TransposeKey(text, steps, preference)
            : Transposer.TransposeChord(text, steps, preference, key);
    }

    private static int ValidFret(int capo) =>
        capo is >= 0 and <= MaxFret
            ? capo
            : throw new ArgumentOutOfRangeException(nameof(capo), capo, $"Capo fret must be between 0 and {MaxFret}.");
}
