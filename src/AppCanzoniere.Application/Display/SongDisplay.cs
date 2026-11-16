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
