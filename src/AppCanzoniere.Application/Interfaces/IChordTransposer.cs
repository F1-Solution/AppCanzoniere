using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Interfaces;

public interface IChordTransposer
{
    /// <summary>
    /// Transpose a chord by the specified number of semitones
    /// </summary>
    /// <param name="chord">Original chord (e.g., "C", "Dm", "G7")</param>
    /// <param name="steps">Number of semitones to transpose (positive = up, negative = down)</param>
    /// <param name="useFlats">Prefer flats over sharps in the result</param>
    /// <returns>Transposed chord</returns>
    string TransposeChord(string chord, int steps, bool useFlats = false);

    /// <summary>
    /// Get the key signature after transposing
    /// </summary>
    string TransposeKey(string key, int steps, bool useFlats = false);

    /// <summary>
    /// Normalize steps to be within 0-11 range
    /// </summary>
    int NormalizeSteps(int steps);

    /// <summary>Transposes a chord; Auto spells it in the key reached by the shift from <paramref name="sourceKey"/>, the key the chord is currently in (null: the chord is its own key).</summary>
    string TransposeChord(string chord, int steps, AccidentalPreference preference, string? sourceKey = null);

    /// <summary>Transposes a key with the given accidental spelling.</summary>
    string TransposeKey(string key, int steps, AccidentalPreference preference);
}
