namespace AppCanzoniere.Application.Music;

/// <summary>A parsed chord: root, minor flag with the token as written, remaining suffix and optional bass.</summary>
public sealed record ChordSymbol(Note Root, bool IsMinor, string MinorToken, string Suffix, Note? Bass)
{
    /// <summary>Returns a copy shifted by <paramref name="steps"/> semitones, spelled with sharps or flats.</summary>
    public ChordSymbol Transpose(int steps, bool flats) =>
        this with { Root = Root.Transpose(steps, flats), Bass = Bass?.Transpose(steps, flats) };
}
