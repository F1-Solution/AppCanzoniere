namespace AppCanzoniere.Application.Music;

/// <summary>Spelling convention of a key: whether its signature uses flats.</summary>
public static class KeySignature
{
    public static bool PrefersFlats(int tonicPitchClass, bool minor) => false;
}
