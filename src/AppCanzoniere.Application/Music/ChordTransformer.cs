namespace AppCanzoniere.Application.Music;

/// <summary>Applies render options (transposition, spelling, capo, notation) to single chords and to the key.</summary>
public sealed class ChordTransformer
{
    private readonly ChordProRenderOptions _options;
    private readonly string? _documentKeyText;
    private readonly KeyValue? _documentKey;
    private readonly int _transpose;
    private readonly int _documentCapo;
    private readonly int _shift;
    private readonly AccidentalPreference? _writtenPreference;
    private readonly (int Tonic, bool Minor)? _reference;

    /// <param name="options">Render settings; a capo outside 0-11 is rejected.</param>
    /// <param name="documentKey">Key of the chords as written, in either notation or in words; steers Auto spelling.</param>
    /// <param name="documentCapo">Capo the document's chords were written for.</param>
    /// <param name="documentChords">Chords of the song in order; without a usable key the first one stands in for it.</param>
    public ChordTransformer(ChordProRenderOptions options, string? documentKey, int documentCapo, IEnumerable<string>? documentChords = null)
    {
        if (options.Capo is { } requested && requested is < 0 or > CapoCalculator.MaxFret)
            throw new ArgumentOutOfRangeException(nameof(options), options.Capo, $"Capo fret must be between 0 and {CapoCalculator.MaxFret}.");

        _options = options;
        _documentKeyText = documentKey;
        _documentCapo = documentCapo;
        _transpose = Note.Mod12(options.TransposeSteps);
        var capo = options.Capo ?? documentCapo;

        // Chords and key always move together: the capo change is taken out of the transposition.
        _shift = Note.Mod12(_transpose + documentCapo - capo);

        // With a capo the written chords are shapes, spelled by their own key whatever the preference.
        _writtenPreference = capo > 0 ? AccidentalPreference.Auto : options.Accidentals;
        if (KeyValue.TryParse(documentKey, options.InputNotation.Notation, out var key))
        {
            _documentKey = key;
            _reference = (Note.Mod12(key.Symbol.Root.PitchClass + _shift), key.Symbol.IsMinor);
        }
        else if (FirstChord(documentChords, options.InputNotation.Notation) is { } first)
            _reference = (Note.Mod12(first.Root.PitchClass + _shift), first.IsMinor);
    }

    private static ChordSymbol? FirstChord(IEnumerable<string>? chords, Notation notation)
    {
        foreach (var text in chords ?? Enumerable.Empty<string>())
        {
            if (ChordSymbolParser.TryParse(text, notation, out var symbol))
                return symbol;
        }

        return null;
    }

    private bool Conversion => _options.InputNotation != _options.OutputNotation;

    /// <summary>Chord as it is written in the output: shifted by transposition and capo; non-chords are returned unchanged.</summary>
    public string Chord(string chord)
    {
        if (!ChordSymbolParser.TryParse(chord, _options.InputNotation.Notation, out var symbol))
            return chord;

        var reference = _reference ?? (Note.Mod12(symbol.Root.PitchClass + _shift), symbol.IsMinor);
        var untouched = IsUntouched(_shift, _writtenPreference);
        if (untouched && !Conversion)
            return chord;

        var shifted = Shifted(symbol, _shift, reference, _writtenPreference);
        return ChordNotation.Format(shifted, _options.OutputNotation, keepMinorToken: !Conversion);
    }

    /// <summary>Key as it is written in the output: it moves with the chords; text that is not a key is returned unchanged.</summary>
    public string Key(string key) =>
        KeyValue.TryParse(key, _options.InputNotation.Notation, out var value) ? WriteKey(key, value, _shift, _writtenPreference) : key;

    /// <summary>Key of the chords as written in the output, or null when the document has no readable key.</summary>
    public string? WrittenKey() =>
        _documentKey is null ? null : WriteKey(_documentKeyText!.Trim(), _documentKey, _shift, _writtenPreference);

    /// <summary>Key the song sounds in, named by the explicit preference (Auto when none); null without a readable key.</summary>
    public string? SoundingKey() =>
        _documentKey is null
            ? null
            : WriteKey(_documentKeyText!.Trim(), _documentKey, Note.Mod12(_transpose + _documentCapo), _options.Accidentals ?? AccidentalPreference.Auto);

    private string WriteKey(string original, KeyValue key, int steps, AccidentalPreference? preference)
    {
        if (IsUntouched(steps, preference) && !Conversion)
            return original;

        var reference = (Note.Mod12(key.Symbol.Root.PitchClass + steps), key.Symbol.IsMinor);
        return key.Write(Shifted(key.Symbol, steps, reference, preference), _options.OutputNotation, Conversion);
    }

    private static bool IsUntouched(int steps, AccidentalPreference? preference) =>
        steps == 0 && (preference is null or AccidentalPreference.Auto);

    private static ChordSymbol Shifted(ChordSymbol symbol, int steps, (int Tonic, bool Minor) reference, AccidentalPreference? preference)
    {
        if (IsUntouched(steps, preference))
            return symbol;

        return preference == AccidentalPreference.Auto
            ? KeySpelling.Transpose(symbol, steps, reference.Tonic, reference.Minor)
            : symbol.Transpose(steps, preference == AccidentalPreference.Flats);
    }
}
