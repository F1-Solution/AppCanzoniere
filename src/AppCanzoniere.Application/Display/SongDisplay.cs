using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Display;

/// <summary>What the song screen needs to render one song.</summary>
/// <param name="ChordPro">The song as stored.</param>
/// <param name="TransposeSteps">Semitones the user moved the song.</param>
/// <param name="LanguageTag">Language that picks the default notation; null is English.</param>
/// <param name="FallbackKey">Key to use when the song file has none.</param>
public sealed record SongDisplayRequest(
    string ChordPro,
    int TransposeSteps,
    ChordDisplaySettings Settings,
    string? LanguageTag,
    string? FallbackKey = null);

/// <summary>What the song screen shows: the text with chords above the lyrics, the sounding key and the capo fret.</summary>
public sealed record SongDisplay(string Text, string Key, int Capo);
