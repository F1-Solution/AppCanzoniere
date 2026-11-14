namespace AppCanzoniere.Application.Display;

/// <summary>Maps the three chord pickers of the settings page to settings and back.</summary>
public static class ChordDisplayPickers
{
    public static bool TryToSettings(int notation, int minor, int accidentals, out ChordDisplaySettings settings) =>
        throw new NotImplementedException();

    public static (int Notation, int Minor, int Accidentals) ToIndexes(ChordDisplaySettings settings) =>
        throw new NotImplementedException();
}
