using UglyToad.PdfPig;
using UglyToad.PdfPig.Filters;
using UglyToad.PdfPig.Tokens;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Decodes a stream through its filters as PdfPig does, one capped stage at a time.</summary>
internal static class FilterChain
{
    /// <summary>The stream's bytes after every filter, or why they cannot be had within cap bytes.</summary>
    public static Decoded Decode(PdfDocument document, StreamToken stream, int cap)
    {
        if (Names(document, stream.StreamDictionary) is not { } names)
            return Decoded.Unreadable;

        ReadOnlyMemory<byte> data = stream.Data;
        if (names.Count == 0)
            return data.Length > cap ? Decoded.OverCap : new Decoded(DecodeStatus.Decoded, data);

        var stage = new Decoded(DecodeStatus.Decoded, data);
        for (var i = 0; i < names.Count && stage.IsDecoded; i++)
            stage = Stage(names[i], stage.Bytes, stream.StreamDictionary, i, cap);
        return stage;
    }

    /// <summary>
    /// One filter stage: the bytes PdfPig's filter would give, OverCap when they or PdfPig's buffers would pass cap, and
    /// unreadable for the image filters PdfPig refuses, except CCITT, whose output size the file itself sets (OverCap).
    /// </summary>
    public static Decoded Stage(string name, ReadOnlyMemory<byte> input, DictionaryToken dictionary, int index, int cap)
    {
        var parameters = DecodeParameterResolver.GetFilterParameters(dictionary, index);
        return name switch
        {
            "FlateDecode" or "Fl" => Predicted(name, StreamDecoders.Flate(input, cap), input, dictionary, parameters, index, cap),
            "LZWDecode" or "LZW" => Predicted(name, LzwDecoder.Decode(input.Span, Int(parameters, NameToken.EarlyChange, 1) == 1, cap), input, dictionary, parameters, index, cap),
            "RunLengthDecode" or "RL" => StreamDecoders.RunLength(input.Span, cap),
            "ASCIIHexDecode" or "AHx" => StreamDecoders.AsciiHex(input.Span, cap),
            "ASCII85Decode" or "A85" => StreamDecoders.Ascii85(input.Span, cap),
            "CCITTFaxDecode" or "CCF" => Decoded.OverCap,
            _ => Decoded.Unreadable,
        };
    }

    // The filter names as PdfPig's lookup reads /Filter (or /F): a name, an array of names, or a reference to either.
    private static IReadOnlyList<string>? Names(PdfDocument document, DictionaryToken dictionary)
    {
        if (!dictionary.TryGet(NameToken.Filter, out var entry) && !dictionary.TryGet(NameToken.F, out entry))
            return Array.Empty<string>();
        if (entry is IndirectReferenceToken reference)
            entry = PdfObjects.Resolve(document, reference);
        return entry switch
        {
            NameToken name => new[] { name.Data },
            ArrayToken array when array.Data.All(t => t is NameToken) => array.Data.Select(t => ((NameToken)t).Data).ToArray(),
            _ => null,
        };
    }

    // PdfPig runs a predictor over Flate and LZW output; its two row buffers and padded last row come on top of the data.
    private static Decoded Predicted(string name, Decoded decoded, ReadOnlyMemory<byte> input, DictionaryToken dictionary, DictionaryToken parameters, int index, int cap)
    {
        var predictor = Int(parameters, NameToken.Predictor, -1);
        if (!decoded.IsDecoded || predictor <= 1)
            return decoded;

        var colors = Math.Min(Int(parameters, NameToken.Colors, 1), 32);
        var rowBytes = unchecked(((Int(parameters, NameToken.Columns, 1) * (colors * Int(parameters, NameToken.BitsPerComponent, 8))) + 7) / 8);
        if (rowBytes < 0 || decoded.Bytes.Length + (3L * rowBytes) > cap)
            return rowBytes < 0 ? Decoded.Unreadable : Decoded.OverCap;

        var filter = DefaultFilterProvider.Instance.GetNamedFilters(new[] { NameToken.Create(name) })[0];
        return new Decoded(DecodeStatus.Decoded, filter.Decode(input.ToArray(), dictionary, DefaultFilterProvider.Instance, index));
    }

    private static int Int(DictionaryToken dictionary, NameToken key, int fallback) =>
        dictionary.TryGet(key, out var token) && token is NumericToken number ? number.Int : fallback;
}
