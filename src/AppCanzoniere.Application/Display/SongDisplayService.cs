using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Layout;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Display;

/// <summary>Turns stored ChordPro into the text the song screen shows.</summary>
public sealed class SongDisplayService : ISongDisplayService
{
    // Pitch classes whose key can be named with a sharp or a flat.
    private static readonly HashSet<int> NaturalTonics = new() { 0, 2, 4, 5, 7, 9, 11 };

    private readonly IChordProParser _parser;

    public SongDisplayService(IChordProParser parser)
    {
        ArgumentNullException.ThrowIfNull(parser);
        _parser = parser;
    }

    public PreparedSong Prepare(SongDisplayRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.ChordPro is null)
            throw new ArgumentException("The song text is missing.", nameof(request));

        var settings = request.Settings ?? ChordDisplaySettings.Default;
        var output = settings.ToNotationOptions(request.LanguageTag);
        var source = SongSourcePreparer.Prepare(request.ChordPro, ChorusWord(request.LanguageTag));
        var document = _parser.Parse(source);
        var chords = ChordProKeys.ChordsOf(document).ToList();
        var baseOptions = new ChordProRenderOptions
        {
            TransposeSteps = request.TransposeSteps,
            Accidentals = AccidentalPreference.Auto,
            InputNotation = ChordNotationDetector.Detect(chords, output.MinorStyle),
            OutputNotation = output,
            ReadBothNotations = true
        };

        var shownOptions = baseOptions with { Accidentals = ChordSpelling(document, chords, baseOptions, settings.Accidentals) };
        var shown = _parser.Parse(_parser.Render(document, shownOptions));
        var keyOptions = baseOptions with { Accidentals = settings.Accidentals };
        return new PreparedSong(
            LayoutSource.From(shown),
            ChordProKeys.SoundingKey(document, keyOptions) ?? string.Empty,
            ChordProKeys.EffectiveCapo(document, shownOptions));
    }

    private static string ChorusWord(string? languageTag) =>
        NotationOptions.ForLanguage(languageTag).Notation == Notation.Italian ? "Rit." : "Chorus";

    // DEC 16: the preference only decides between the two names of an ambiguous key; stored chords stay as written.
    private static AccidentalPreference ChordSpelling(ChordProDocument document, List<string> chords, ChordProRenderOptions options, AccidentalPreference wanted)
    {
        if (wanted == AccidentalPreference.Auto || Note.Mod12(options.TransposeSteps) == 0)
            return AccidentalPreference.Auto;

        var reference = new ChordTransformer(options, document.Key, document.CapoFret ?? 0, chords).ReferenceKey;
        return reference is { } key && !NaturalTonics.Contains(key.Tonic) ? wanted : AccidentalPreference.Auto;
    }
}
