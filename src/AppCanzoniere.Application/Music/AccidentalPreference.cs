namespace AppCanzoniere.Application.Music;

/// <summary>How altered notes are spelled after transposing.</summary>
public enum AccidentalPreference
{
    Sharps,
    Flats,

    /// <summary>Chosen from the target key signature (F major gives Bb, not A#).</summary>
    Auto
}
