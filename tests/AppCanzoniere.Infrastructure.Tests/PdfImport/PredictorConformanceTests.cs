using AppCanzoniere.Infrastructure.PdfImport;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Filters;
using UglyToad.PdfPig.Tokens;
using static AppCanzoniere.Infrastructure.Tests.PdfImport.CraftedPdf;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

/// <summary>
/// The capped decoders must give the bytes PdfPig's own filters give, predictor included, since PdfPig parses what they
/// return. PdfPig's filters are the oracle here, on small inputs where they are safe to run.
/// </summary>
public class PredictorConformanceTests
{
    private const int Cap = 1024 * 1024;

    public static TheoryData<int, int, int, int> Parameters()
    {
        var data = new TheoryData<int, int, int, int>();
        var combinations =
            from predictor in new[] { 2, 10, 11, 12, 13, 14, 15 }
            from colors in new[] { 1, 3 }
            from bits in new[] { 1, 2, 4, 8, 16 }
            from columns in new[] { 1, 5, 17 }
            select (predictor, colors, bits, columns);
        foreach (var (predictor, colors, bits, columns) in combinations)
            data.Add(predictor, colors, bits, columns);
        return data;
    }

    [Theory]
    [MemberData(nameof(Parameters))]
    public void Flate_GivesPdfPigsBytes_ForEveryPredictor(int predictor, int colors, int bits, int columns)
    {
        var (dictionary, raw) = Sample(predictor, colors, bits, columns);
        var packed = Zlib(raw);

        var expected = new FlateFilter().Decode(packed, dictionary, DefaultFilterProvider.Instance, 0).ToArray();
        var actual = FilterChain.Stage("FlateDecode", packed, dictionary, 0, Cap);

        Assert.Equal(DecodeStatus.Decoded, actual.Status);
        Assert.Equal(expected, actual.Bytes.ToArray());
    }

    [Theory]
    [MemberData(nameof(Parameters))]
    public void Lzw_GivesPdfPigsBytes_ForEveryPredictor(int predictor, int colors, int bits, int columns)
    {
        var (dictionary, raw) = Sample(predictor, colors, bits, columns);
        var packed = Lzw(raw);

        var expected = new LzwFilter().Decode(packed, dictionary, DefaultFilterProvider.Instance, 0).ToArray();
        var actual = FilterChain.Stage("LZWDecode", packed, dictionary, 0, Cap);

        Assert.Equal(DecodeStatus.Decoded, actual.Status);
        Assert.Equal(expected, actual.Bytes.ToArray());
    }

    // Pseudo-random rows whose per-row predictor bytes cover every PNG type and a few invalid ones, with a short last row.
    private static (DictionaryToken Dictionary, byte[] Raw) Sample(int predictor, int colors, int bits, int columns)
    {
        var parameters = new DictionaryToken(new Dictionary<NameToken, IToken>
        {
            [NameToken.Predictor] = new NumericToken(predictor),
            [NameToken.Colors] = new NumericToken(colors),
            [NameToken.BitsPerComponent] = new NumericToken(bits),
            [NameToken.Columns] = new NumericToken(columns),
        });
        var dictionary = new DictionaryToken(new Dictionary<NameToken, IToken> { [NameToken.DecodeParms] = parameters });
        var random = new Random((predictor * 1000) + (colors * 100) + (bits * 10) + columns);
        var rowBytes = ((columns * colors * bits) + 7) / 8;
        var raw = new byte[((rowBytes + 1) * 9) + 3];
        random.NextBytes(raw);
        for (var row = 0; predictor >= 10 && row * (rowBytes + 1) < raw.Length; row++)
            raw[row * (rowBytes + 1)] = (byte)(row % 6);
        return (dictionary, raw);
    }
}
