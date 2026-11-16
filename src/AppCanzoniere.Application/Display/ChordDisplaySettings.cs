using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Display;

/// <summary>Which chord notation to write: the language default, or one the user picked.</summary>
public enum NotationChoice
{
    FollowLanguage,
    English,
    Italian
}

/// <summary>How the user wants chords written on the song screen; the default follows the language.</summary>
public sealed record ChordDisplaySettings(
    NotationChoice Notation = NotationChoice.FollowLanguage,
    MinorStyle MinorStyle = MinorStyle.LetterM,
    AccidentalPreference Accidentals = AccidentalPreference.Auto)
{
    public static ChordDisplaySettings Default { get; } = new();

    /// <summary>Notation to write for a language tag, honouring an explicit choice.</summary>
    public NotationOptions ToNotationOptions(string? languageTag) => Notation switch
    {
        NotationChoice.English => new NotationOptions(Music.Notation.English, MinorStyle),
        NotationChoice.Italian => new NotationOptions(Music.Notation.Italian, MinorStyle),
        _ => NotationOptions.ForLanguage(languageTag) with { MinorStyle = MinorStyle }
    };
}
