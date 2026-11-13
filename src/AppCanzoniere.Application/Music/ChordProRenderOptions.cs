namespace AppCanzoniere.Application.Music;

/// <summary>Immutable settings for rendering a ChordPro document: transposition, spelling, notation and capo.</summary>
public sealed record ChordProRenderOptions
{
    /// <summary>Semitones to transpose the sounding song; the written key and chords shift by this plus the capo change.</summary>
    public int TransposeSteps { get; init; }

    /// <summary>Spelling of altered notes at capo 0 and of the sounding key; with a capo the shapes are always spelled by their own key. Null keeps notes as written unless shifted, then sharps.</summary>
    public AccidentalPreference? Accidentals { get; init; }

    /// <summary>Notation the document's chords and key are written in.</summary>
    public NotationOptions InputNotation { get; init; } = NotationOptions.English;

    /// <summary>Notation to write chords and key in.</summary>
    public NotationOptions OutputNotation { get; init; } = NotationOptions.English;

    /// <summary>Capo fret 0-11 to show shapes for; null leaves the document's capo untouched.</summary>
    public int? Capo { get; init; }
}
