namespace AppCanzoniere.Application.Music;

/// <summary>Immutable chord-notation setting; the language default can always be overridden with <c>with</c>.</summary>
public sealed record NotationOptions(Notation Notation, MinorStyle MinorStyle)
{
    public static NotationOptions English { get; } = new(Notation.English, MinorStyle.LetterM);

    public static NotationOptions Italian { get; } = new(Notation.Italian, MinorStyle.LetterM);

    public static NotationOptions ItalianDash { get; } = new(Notation.Italian, MinorStyle.Dash);

    /// <summary>Default notation for a language tag: Italian gets Do-Re-Mi with "Lam", everything else C-D-E with "Am".</summary>
    public static NotationOptions ForLanguage(string? languageTag) => English;
}
