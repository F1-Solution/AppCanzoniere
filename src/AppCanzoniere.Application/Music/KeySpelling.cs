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
        var root = Spell(chord.Root.PitchClass + Note.Mod12(steps), tonic, minor);
        Note? bass = chord.Bass is { } source ? SpellBass(source.PitchClass + Note.Mod12(steps), root, tonic, minor) : null;
        return chord with { Root = root, Bass = bass };
    }

    private static bool UsesFlats(int interval, int tonic, bool minor)
    {
        if ((minor ? MinorScale : MajorScale).Contains(interval))
            return KeySignature.PrefersFlats(tonic, minor);

        return minor ? interval == 1 : MajorFlatDegrees.Contains(interval);
    }

    private static Note SpellBass(int pitchClass, Note root, int tonic, bool minor) =>
        Note.Mod12(pitchClass - root.PitchClass) == 4 && MajorThird(root, pitchClass) is { } third
            ? third
            : Spell(pitchClass, tonic, minor);

    private static Note? MajorThird(Note root, int pitchClass)
    {
        var letter = Note.LetterAbove(root.Letter, 2);
        var accidental = Note.Mod12(pitchClass - Note.LetterPitch(letter) + 6) - 6;
        var third = new Note(letter, accidental);
        return accidental is < -1 or > 1 || IsAwkward(third) ? null : third;
    }

    private static bool IsAwkward(Note note) =>
        (note.Accidental > 0 && note.Letter is 'E' or 'B') || (note.Accidental < 0 && note.Letter is 'F' or 'C');
}
