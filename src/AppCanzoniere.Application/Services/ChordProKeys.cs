using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Services;

/// <summary>Keys of a document under render options: the key of the chords as written, the capo, and the key the song sounds in.</summary>
public static class ChordProKeys
{
    /// <summary>Capo the rendered chords are played with: the requested fret, else the document's, else 0.</summary>
    public static int EffectiveCapo(ChordProDocument document, ChordProRenderOptions options)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(options);
        ChordTransformer.ValidateCapo(options);
        return options.Capo ?? document.CapoFret ?? 0;
    }

    /// <summary>Key of the rendered chords ("plays in"), or null when the document has no readable key.</summary>
    public static string? WrittenKey(ChordProDocument document, ChordProRenderOptions options) =>
        Transformer(document, options).WrittenKey();

    /// <summary>Key the song sounds in, in the output notation; the explicit accidental preference names it. Null without a readable key.</summary>
    public static string? SoundingKey(ChordProDocument document, ChordProRenderOptions options) =>
        Transformer(document, options).SoundingKey();

    private static ChordTransformer Transformer(ChordProDocument document, ChordProRenderOptions options)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(options);
        return new ChordTransformer(options, document.Key, document.CapoFret ?? 0, ChordsOf(document));
    }

    /// <summary>The bracketed chords of a document in reading order, tab blocks left out.</summary>
    internal static IEnumerable<string> ChordsOf(ChordProDocument document) =>
        document.Lines
            .Where(l => l.Type != ChordProLineType.Tab)
            .SelectMany(l => (l.Chords ?? new List<ChordPosition>()).OrderBy(c => c.Position).Select(c => c.Chord));
}
