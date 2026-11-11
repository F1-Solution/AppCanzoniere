using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Services;

public class ChordTransposer : IChordTransposer
{
    public string TransposeChord(string chord, int steps, bool useFlats = false) =>
        TransposeChord(chord, steps, useFlats ? AccidentalPreference.Flats : AccidentalPreference.Sharps);

    public string TransposeKey(string key, int steps, bool useFlats = false) =>
        TransposeKey(key, steps, useFlats ? AccidentalPreference.Flats : AccidentalPreference.Sharps);

    public string TransposeChord(string chord, int steps, AccidentalPreference preference, string? key = null)
    {
        if (!ChordSymbolParser.TryParse(chord, Notation.English, out var symbol))
            return chord;

        var flats = ResolveFlats(preference, symbol, steps, key);
        return ChordNotation.Format(symbol.Transpose(steps, flats), NotationOptions.English, keepMinorToken: true);
    }

    public string TransposeKey(string key, int steps, AccidentalPreference preference)
    {
        if (!ChordSymbolParser.TryParse(key, Notation.English, out var symbol))
            return key;

        var flats = ResolveFlats(preference, symbol, steps, key);
        return ChordNotation.Format(symbol.Transpose(steps, flats), NotationOptions.English, keepMinorToken: true);
    }

    public int NormalizeSteps(int steps)
    {
        steps %= 12;
        return steps < 0 ? steps + 12 : steps;
    }

    private static bool ResolveFlats(AccidentalPreference preference, ChordSymbol chord, int steps, string? key)
    {
        if (preference != AccidentalPreference.Auto)
            return preference == AccidentalPreference.Flats;

        var reference = key is not null && ChordSymbolParser.TryParse(key, Notation.English, out var keySymbol)
            ? keySymbol
            : chord;
        return KeySignature.PrefersFlats(reference.Root.PitchClass + steps, reference.IsMinor);
    }
}
