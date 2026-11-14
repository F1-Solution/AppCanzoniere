using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Display;

/// <summary>Maps the three chord pickers of the settings page to settings and back.</summary>
public static class ChordDisplayPickers
{
    private static readonly NotationChoice[] Notations = { NotationChoice.FollowLanguage, NotationChoice.English, NotationChoice.Italian };
    private static readonly MinorStyle[] Minors = { MinorStyle.LetterM, MinorStyle.Dash };
    private static readonly AccidentalPreference[] Accidentals = { AccidentalPreference.Auto, AccidentalPreference.Sharps, AccidentalPreference.Flats };

    /// <summary>False when an index is -1 (a picker being reset) or past its list: that is not a choice.</summary>
    public static bool TryToSettings(int notation, int minor, int accidentals, out ChordDisplaySettings settings)
    {
        settings = ChordDisplaySettings.Default;
        if (!InRange(notation, Notations.Length) || !InRange(minor, Minors.Length) || !InRange(accidentals, Accidentals.Length))
            return false;

        settings = new ChordDisplaySettings(Notations[notation], Minors[minor], Accidentals[accidentals]);
        return true;
    }

    public static (int Notation, int Minor, int Accidentals) ToIndexes(ChordDisplaySettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return (Array.IndexOf(Notations, settings.Notation), Array.IndexOf(Minors, settings.MinorStyle), Array.IndexOf(Accidentals, settings.Accidentals));
    }

    private static bool InRange(int index, int length) => index >= 0 && index < length;
}
