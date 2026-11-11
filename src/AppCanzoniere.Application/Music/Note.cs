namespace AppCanzoniere.Application.Music;

/// <summary>A spelled note: a letter A-G and an accidental of -1 (flat), 0 or +1 (sharp).</summary>
public readonly record struct Note(char Letter, int Accidental)
{
    private static readonly string[] SharpNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
    private static readonly string[] FlatNames = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

    public int PitchClass => (LetterPitch(Letter) + Accidental + 12) % 12;

    /// <summary>Spells a pitch class (any integer) using sharps or flats.</summary>
    public static Note FromPitchClass(int pitchClass, bool flats)
    {
        var index = ((pitchClass % 12) + 12) % 12;
        var name = (flats ? FlatNames : SharpNames)[index];
        return new Note(name[0], name.Length == 1 ? 0 : name[1] == '#' ? 1 : -1);
    }

    public Note Transpose(int steps, bool flats) => FromPitchClass(PitchClass + steps, flats);

    private static int LetterPitch(char letter) => letter switch
    {
        'C' => 0,
        'D' => 2,
        'E' => 4,
        'F' => 5,
        'G' => 7,
        'A' => 9,
        _ => 11
    };
}
