using System.Text.RegularExpressions;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Services;

public class ChordTransposer : IChordTransposer
{
    // Chromatic scale with sharps
    private static readonly string[] NotesSharp = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    // Chromatic scale with flats
    private static readonly string[] NotesFlat = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

    // Regex to match chord components: root note, optional sharp/flat, quality, bass note
    private static readonly Regex ChordRegex = new Regex(
        @"^([A-G])(b|#)?(.*?)(/([A-G])(b|#)?)?$",
        RegexOptions.Compiled
    );

    public string TransposeChord(string chord, int steps, bool useFlats = false)
    {
        if (string.IsNullOrWhiteSpace(chord))
            return chord;

        var match = ChordRegex.Match(chord.Trim());
        if (!match.Success)
            return chord; // Return original if can't parse

        var root = match.Groups[1].Value;
        var accidental = match.Groups[2].Value;
        var quality = match.Groups[3].Value;
        var hasBassnote = match.Groups[4].Success;
        var bassNote = hasBassnote ? match.Groups[5].Value : string.Empty;
        var bassAccidental = hasBassnote ? match.Groups[6].Value : string.Empty;

        // Transpose root note
        var transposedRoot = TransposeNote(root + accidental, steps, useFlats);

        // Transpose bass note if present
        var transposedBass = hasBassnote
            ? TransposeNote(bassNote + bassAccidental, steps, useFlats)
            : string.Empty;

        // Build transposed chord
        var result = transposedRoot + quality;
        if (hasBassnote)
            result += "/" + transposedBass;

        return result;
    }

    public string TransposeKey(string key, int steps, bool useFlats = false)
    {
        if (string.IsNullOrWhiteSpace(key))
            return key;

        // Handle minor keys (e.g., "Am", "Dm")
        var isMinor = key.EndsWith("m", StringComparison.OrdinalIgnoreCase);
        var keyNote = isMinor ? key.Substring(0, key.Length - 1) : key;

        var transposedNote = TransposeNote(keyNote, steps, useFlats);

        return isMinor ? transposedNote + "m" : transposedNote;
    }

    public string TransposeChord(string chord, int steps, AccidentalPreference preference, string? key = null) =>
        TransposeChord(chord, steps, preference == AccidentalPreference.Flats);

    public string TransposeKey(string key, int steps, AccidentalPreference preference) =>
        TransposeKey(key, steps, preference == AccidentalPreference.Flats);

    public int NormalizeSteps(int steps)
    {
        steps = steps % 12;
        if (steps < 0)
            steps += 12;
        return steps;
    }

    private string TransposeNote(string note, int steps, bool useFlats)
    {
        if (string.IsNullOrWhiteSpace(note))
            return note;

        var notes = useFlats ? NotesFlat : NotesSharp;

        // Find current position
        var currentIndex = FindNoteIndex(note, NotesSharp);
        if (currentIndex == -1)
            currentIndex = FindNoteIndex(note, NotesFlat);

        if (currentIndex == -1)
            return note; // Can't find note, return original

        // Calculate new position
        var normalizedSteps = NormalizeSteps(steps);
        var newIndex = (currentIndex + normalizedSteps) % 12;

        return notes[newIndex];
    }

    private int FindNoteIndex(string note, string[] noteArray)
    {
        for (int i = 0; i < noteArray.Length; i++)
        {
            if (noteArray[i].Equals(note, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }
}
