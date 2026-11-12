using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Services;

public class ChordTransposer : IChordTransposer
{
    public string TransposeChord(string chord, int steps, bool useFlats = false) =>
        TransposeChord(chord, steps, useFlats ? AccidentalPreference.Flats : AccidentalPreference.Sharps);

    public string TransposeKey(string key, int steps, bool useFlats = false) =>
        TransposeKey(key, steps, useFlats ? AccidentalPreference.Flats : AccidentalPreference.Sharps);

    public string TransposeChord(string chord, int steps, AccidentalPreference preference, string? sourceKey = null) =>
        Transpose(chord, steps, preference, sourceKey);

    public string TransposeKey(string key, int steps, AccidentalPreference preference) =>
        Transpose(key, steps, preference, key);

    public int NormalizeSteps(int steps) => Note.Mod12(steps);

    private static string Transpose(string text, int steps, AccidentalPreference preference, string? key)
    {
        if (!ChordSymbolParser.TryParse(text, Notation.English, out var symbol))
            return text;

        var shifted = preference == AccidentalPreference.Auto
            ? SpellAuto(symbol, steps, key)
            : symbol.Transpose(steps, preference == AccidentalPreference.Flats);
        return ChordNotation.Format(shifted, NotationOptions.English, keepMinorToken: true);
    }

    private static ChordSymbol SpellAuto(ChordSymbol symbol, int steps, string? key)
    {
        var reference = key is not null && ChordSymbolParser.TryParse(key, Notation.English, out var keySymbol)
            ? keySymbol
            : symbol;
        return KeySpelling.Transpose(symbol, steps, reference.Root.PitchClass + Note.Mod12(steps), reference.IsMinor);
    }
}
