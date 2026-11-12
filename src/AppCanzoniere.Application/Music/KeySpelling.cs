namespace AppCanzoniere.Application.Music;

/// <summary>Spells notes by their degree in a key: diatonic notes by the key signature, chromatic ones by fixed accidentals.</summary>
public static class KeySpelling
{
    private static readonly HashSet<int> MajorScale = new() { 0, 2, 4, 5, 7, 9, 11 };
    private static readonly HashSet<int> MinorScale = new() { 0, 2, 3, 5, 7, 8, 10 };
    private static readonly HashSet<int> MajorFlatDegrees = new() { 1, 3, 8, 10 };

    /// <summary>Spells a pitch class in the key with the given tonic: b2 b3 b6 b7 in major, b2 in minor are flats; the others sharps.</summary>
    public static Note Spell(int pitchClass, int tonic, bool minor)
    {
        var interval = Note.Mod12(pitchClass - tonic);
        return Note.FromPitchClass(pitchClass, UsesFlats(interval, tonic, minor));
    }

    /// <summary>Shifts a chord by <paramref name="steps"/> and spells root and bass in the key of <paramref name="tonic"/>.</summary>
    public static ChordSymbol Transpose(ChordSymbol chord, int steps, int tonic, bool minor)
    {
        var rootPitch = chord.Root.PitchClass + Note.Mod12(steps);
        var root = IsDiminished(chord) ? SpellDiminishedRoot(rootPitch, tonic, minor) : Spell(rootPitch, tonic, minor);
        Note? bass = chord.Bass is { } source ? SpellBass(source.PitchClass + Note.Mod12(steps), root, tonic, minor) : null;
        return chord with { Root = root, Bass = bass };
    }

    // Passing diminished chords on chromatic degrees are written with sharps.
    private static Note SpellDiminishedRoot(int pitchClass, int tonic, bool minor) =>
        (minor ? MinorScale : MajorScale).Contains(Note.Mod12(pitchClass - tonic))
            ? Spell(pitchClass, tonic, minor)
            : Note.FromPitchClass(pitchClass, false);

    private static bool IsDiminished(ChordSymbol chord)
    {
        var suffix = chord.Suffix;
        return suffix.StartsWith("dim", StringComparison.OrdinalIgnoreCase) ||
               suffix.StartsWith('°') || ChordSymbolParser.HasBareDiminishedMarker(suffix) || suffix.StartsWith('ø') ||
               (chord.IsMinor && (suffix.Contains("b5", StringComparison.Ordinal) || suffix.Contains("♭5", StringComparison.Ordinal)));
    }

    private static bool UsesFlats(int interval, int tonic, bool minor)
    {
        if ((minor ? MinorScale : MajorScale).Contains(interval))
            return KeySignature.PrefersFlats(tonic, minor);

        return minor ? interval == 1 : MajorFlatDegrees.Contains(interval);
    }

    private static Note SpellBass(int pitchClass, Note root, int tonic, bool minor)
    {
        var diatonic = (minor ? MinorScale : MajorScale).Contains(Note.Mod12(pitchClass - tonic));
        var below = !diatonic && Note.Mod12(root.PitchClass - pitchClass) == 1;
        var interval = Note.Mod12(pitchClass - root.PitchClass);
        if ((interval == 4 || below) && OnLetter(root, below ? 6 : 2, pitchClass) is { } note)
            return note;

        return Spell(pitchClass, tonic, minor);
    }

    // The note on the letter <paramref name="letters"/> above the root, or null when it needs a double or awkward accidental.
    private static Note? OnLetter(Note root, int letters, int pitchClass)
    {
        var letter = Note.LetterAbove(root.Letter, letters);
        var accidental = Note.Mod12(pitchClass - Note.LetterPitch(letter) + 6) - 6;
        var note = new Note(letter, accidental);
        return accidental is < -1 or > 1 || IsAwkward(note) ? null : note;
    }

    private static bool IsAwkward(Note note) =>
        (note.Accidental > 0 && note.Letter is 'E' or 'B') || (note.Accidental < 0 && note.Letter is 'F' or 'C');
}
