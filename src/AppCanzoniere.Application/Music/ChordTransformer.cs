namespace AppCanzoniere.Application.Music;

/// <summary>Applies render options (transposition, spelling, capo, notation) to single chords and to the key.</summary>
public sealed class ChordTransformer
{
    private readonly ChordProRenderOptions _options;
    private readonly int _transpose;
    private readonly int _shapeShift;
    private readonly (int Tonic, bool Minor)? _shapeKey;

    /// <param name="options">Render settings; a capo outside 0-11 is rejected.</param>
    /// <param name="documentKey">Sounding key of the document, written in the input notation; steers Auto spelling.</param>
    /// <param name="documentCapo">Capo the document's chords were written for.</param>
    public ChordTransformer(ChordProRenderOptions options, string? documentKey, int documentCapo)
    {
        _options = options;
        _transpose = Note.Mod12(options.TransposeSteps);
        var capo = options.Capo ?? documentCapo;
        if (options.Capo is { } requested && requested is < 0 or > CapoCalculator.MaxFret)
            throw new ArgumentOutOfRangeException(nameof(options), requested, $"Capo fret must be between 0 and {CapoCalculator.MaxFret}.");

        _shapeShift = options.Capo is null ? _transpose : Note.Mod12(_transpose + documentCapo - capo);
        if (ChordSymbolParser.TryParse(documentKey, options.InputNotation.Notation, out var key))
            _shapeKey = (Note.Mod12(key.Root.PitchClass + _transpose - capo), key.IsMinor);
    }

    /// <summary>Chord as it is written in the output: shifted by transposition and capo; non-chords are returned unchanged.</summary>
    public string Chord(string chord)
    {
        if (!ChordSymbolParser.TryParse(chord, _options.InputNotation.Notation, out var symbol))
            return chord;

        var reference = _shapeKey ?? (symbol.Root.PitchClass + _shapeShift, symbol.IsMinor);
        return Write(chord, symbol, _shapeShift, reference);
    }

    /// <summary>Sounding key after transposition (the capo does not change it).</summary>
    public string Key(string key)
    {
        if (!ChordSymbolParser.TryParse(key, _options.InputNotation.Notation, out var symbol))
            return key;

        return Write(key, symbol, _transpose, (symbol.Root.PitchClass + _transpose, symbol.IsMinor));
    }

    private string Write(string original, ChordSymbol symbol, int steps, (int Tonic, bool Minor) reference)
    {
        var untouched = steps == 0 && _options.Accidentals is null or AccidentalPreference.Auto;
        if (untouched && _options.InputNotation == _options.OutputNotation)
            return original;

        var shifted = untouched ? symbol : Shift(symbol, steps, reference);
        return ChordNotation.Format(shifted, _options.OutputNotation, keepMinorToken: _options.InputNotation == _options.OutputNotation);
    }

    private ChordSymbol Shift(ChordSymbol symbol, int steps, (int Tonic, bool Minor) reference) =>
        _options.Accidentals == AccidentalPreference.Auto
            ? KeySpelling.Transpose(symbol, steps, reference.Tonic, reference.Minor)
            : symbol.Transpose(steps, _options.Accidentals == AccidentalPreference.Flats);
}
