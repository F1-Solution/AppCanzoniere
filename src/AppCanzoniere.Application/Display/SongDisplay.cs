namespace AppCanzoniere.Application.Display;

/// <summary>What the song screen needs to render one song.</summary>
/// <param name="ChordPro">The song as stored.</param>
/// <param name="TransposeSteps">Semitones the user moved the song.</param>
/// <param name="LanguageTag">Language that picks the default notation; null is English.</param>
public sealed record SongDisplayRequest(
    string ChordPro,
    int TransposeSteps,
    ChordDisplaySettings Settings,
    string? LanguageTag);

/// <summary>What the song screen shows: the text with chords above the lyrics, the sounding key (empty when the song has none) and the capo fret.</summary>
public sealed record SongDisplay(string Text, string Key, int Capo);
