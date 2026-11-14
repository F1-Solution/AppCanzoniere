using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Layout;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Display;

/// <summary>Turns stored ChordPro into the text the song screen shows.</summary>
public sealed class SongDisplayService : ISongDisplayService
{
    private static readonly char[] UnsafeInKey = { '{', '}', '\r', '\n' };

    private readonly IChordProParser _parser;
    private readonly ITextMeasurer _measurer = new MonospaceMeasurer();

    public SongDisplayService(IChordProParser parser)
    {
        ArgumentNullException.ThrowIfNull(parser);
        _parser = parser;
    }

    public SongDisplay Render(SongDisplayRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.ChordPro is null)
            throw new ArgumentException("The song text is missing.", nameof(request));

        var settings = request.Settings ?? ChordDisplaySettings.Default;
        var document = ParseWithKey(request.ChordPro, request.FallbackKey);
        var options = new ChordProRenderOptions
        {
            TransposeSteps = request.TransposeSteps,
            Accidentals = settings.Accidentals,
            InputNotation = ChordNotationDetector.Detect(ChordProKeys.ChordsOf(document)),
            OutputNotation = settings.ToNotationOptions(request.LanguageTag)
        };

        var shown = _parser.Parse(_parser.Render(document, options));
        var flow = SongLayoutEngine.Flow(LayoutSource.From(shown), LayoutTextProjector.Options, _measurer);
        return new SongDisplay(
            LayoutTextProjector.Project(flow),
            ChordProKeys.SoundingKey(document, options) ?? string.Empty,
            ChordProKeys.EffectiveCapo(document, options));
    }

    // A song without a key of its own borrows the stored one, so key and chords move together.
    private ChordProDocument ParseWithKey(string chordPro, string? fallbackKey)
    {
        var document = _parser.Parse(chordPro);
        var fallback = fallbackKey?.Trim();
        if (!string.IsNullOrWhiteSpace(document.Key) || string.IsNullOrEmpty(fallback) || fallback.IndexOfAny(UnsafeInKey) >= 0)
            return document;

        return _parser.Parse($"{{key: {fallback}}}\n{chordPro}");
    }
}
