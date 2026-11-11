namespace AppCanzoniere.Application.Music;

/// <summary>Note-naming system used to read or write chords.</summary>
public enum Notation
{
    /// <summary>Letters C D E F G A B.</summary>
    English,

    /// <summary>Solfège Do Re Mi Fa Sol La Si.</summary>
    Italian
}

/// <summary>How a minor chord is marked.</summary>
public enum MinorStyle
{
    /// <summary>Trailing "m", as in "Am" or "Lam".</summary>
    LetterM,

    /// <summary>Trailing hyphen, as in "A-" or "La-".</summary>
    Dash
}
