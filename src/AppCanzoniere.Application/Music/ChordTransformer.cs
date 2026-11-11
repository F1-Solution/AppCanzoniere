namespace AppCanzoniere.Application.Music;

/// <summary>Applies render options (transposition, spelling, capo, notation) to single chords and to the key.</summary>
public sealed class ChordTransformer
{
    private readonly ChordProRenderOptions _options;
    private readonly ChordSymbol? _key;
    private readonly int _shapeShift;

    /// <param name="options">Render settings; a capo outside 0-11 is rejected.</param>
    /// <param name="documentKey">Sounding key of the document, written in the input notation; steers Auto spelling.</param>
    /// <param name="documentCapo">Capo the document's chords were written for.</param>
    public ChordTransformer(ChordProRenderOptions options, string? documentKey, int documentCapo)
    {
        _options = options;
        _shapeShift = options.TransposeSteps;
        if (options.Capo is { } capo)
        {
            if (capo is < 0 or > CapoCalculator.MaxFret)
                throw new ArgumentOutOfRangeException(nameof(options), capo, $"Capo fret must be between 0 and {CapoCalculator.MaxFret}.");

            _shapeShift += documentCapo - capo;
        }

        _key = ChordSymbolParser.TryParse(documentKey, options.InputNotation.Notation, out var key) ? key : null;
    }

    /// <summary>Chord as it is written in the output: shifted by transposition and capo; non-chords are returned unchanged.</summary>
    public string Chord(string chord)
    {
        if (!ChordSymbolParser.TryParse(chord, _options.InputNotation.Notation, out var symbol))
            return chord;

        var flats = ResolveFlats(symbol, _shapeShift, _key);
        return Write(chord, symbol, _shapeShift, flats);
    }

    /// <summary>Sounding key after transposition (the capo does not change it).</summary>
    public string Key(string key)
    {
        if (!ChordSymbolParser.TryParse(key, _options.InputNotation.Notation, out var symbol))
            return key;

        var flats = ResolveFlats(symbol, _options.TransposeSteps, symbol);
        return Write(key, symbol, _options.TransposeSteps, flats);
    }

    private string Write(string original, ChordSymbol symbol, int steps, bool flats)
    {
        var untouched = steps == 0 && _options.Accidentals is null;
        if (untouched && _options.InputNotation == _options.OutputNotation)
            return original;

        var shifted = untouched ? symbol : symbol.Transpose(steps, flats);
        return ChordNotation.Format(shifted, _options.OutputNotation, keepMinorToken: _options.InputNotation == _options.OutputNotation);
    }

    private bool ResolveFlats(ChordSymbol chord, int steps, ChordSymbol? key)
    {
        if (_options.Accidentals != AccidentalPreference.Auto)
            return _options.Accidentals == AccidentalPreference.Flats;

        var reference = key ?? chord;
        return KeySignature.PrefersFlats(reference.Root.PitchClass + steps, reference.IsMinor);
    }
}
