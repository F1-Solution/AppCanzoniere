namespace AppCanzoniere.Application.Music;

/// <summary>A spelled note: a letter A-G and an accidental of -1 (flat), 0 or +1 (sharp).</summary>
public readonly record struct Note(char Letter, int Accidental)
{
    private const string Letters = "CDEFGAB";
    private static readonly string[] SharpNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
    private static readonly string[] FlatNames = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

    public int PitchClass => Mod12(LetterPitch(Letter) + Accidental);

    /// <summary>Spells a pitch class (any integer) using sharps or flats.</summary>
    public static Note FromPitchClass(int pitchClass, bool flats)
    {
        var name = (flats ? FlatNames : SharpNames)[Mod12(pitchClass)];
        return new Note(name[0], name.Length == 1 ? 0 : name[1] == '#' ? 1 : -1);
    }

    public Note Transpose(int steps, bool flats) => FromPitchClass(PitchClass + Mod12(steps), flats);

    /// <summary>Reduces any integer, including the extremes, to 0-11.</summary>
    internal static int Mod12(int value) => (int)(((value % 12L) + 12) % 12);

    /// <summary>The letter <paramref name="steps"/> letters above, wrapping after B.</summary>
    internal static char LetterAbove(char letter, int steps) => Letters[(Letters.IndexOf(letter) + steps) % 7];

    internal static int LetterPitch(char letter) => letter switch
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
