namespace AppCanzoniere.Application.Music;

/// <summary>Spelling convention of a key: whether its signature uses flats.</summary>
public static class KeySignature
{
    private static readonly HashSet<int> FlatMajorTonics = new() { 1, 3, 5, 8, 10 };
    private static readonly HashSet<int> FlatMinorTonics = new() { 0, 2, 3, 5, 7, 10 };

    /// <summary>True for Db Eb F Ab Bb major and C D Eb F G Bb minor; the ambiguous F#/Gb major is spelled with sharps.</summary>
    public static bool PrefersFlats(int tonicPitchClass, bool minor)
    {
        var tonic = ((tonicPitchClass % 12) + 12) % 12;
        return (minor ? FlatMinorTonics : FlatMajorTonics).Contains(tonic);
    }
}
