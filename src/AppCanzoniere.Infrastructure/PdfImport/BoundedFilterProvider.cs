using System.Runtime.InteropServices;
using UglyToad.PdfPig.Filters;
using UglyToad.PdfPig.Tokens;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Thrown inside PdfPig when a stream would unpack past the decode limit; the extractor turns it into a refusal.</summary>
internal sealed class DecodeLimitExceededException(long limit)
    : Exception($"A stream unpacks to more than {limit} bytes.")
{
    public long Limit { get; } = limit;
}

/// <summary>
/// PdfPig's own filters behind a cap: every stream PdfPig unpacks while reading (content, forms, fonts, object streams)
/// is first decoded within the cap, so no single stream can unpack to more than it.
/// </summary>
internal sealed class BoundedFilterProvider : BaseFilterProvider
{
    public BoundedFilterProvider(long maxDecodedBytes)
        : base(Wrap(Cap(maxDecodedBytes)))
    {
    }

    /// <summary>True once a stream has been refused, even if PdfPig caught the refusal and carried on.</summary>
    public bool Tripped => Filters.Any(f => f.Tripped);

    private IEnumerable<BoundedFilter> Filters => FilterInstances.Values.OfType<BoundedFilter>();

    private static int Cap(long bytes) => (int)Math.Clamp(bytes, 1, Array.MaxLength);

    private static Dictionary<string, IFilter> Wrap(int cap)
    {
        var inner = DefaultFilterProvider.Instance;
        var wrapped = new Dictionary<IFilter, IFilter>();
        var names = new Dictionary<string, IFilter>();
        foreach (var name in FilterNames)
        {
            var filter = inner.GetNamedFilters(new[] { NameToken.Create(name) })[0];
            if (!wrapped.TryGetValue(filter, out var bounded))
                wrapped[filter] = bounded = new BoundedFilter(name, filter, cap);
            names[name] = bounded;
        }

        return names;
    }

    private static readonly string[] FilterNames =
    {
        "ASCII85Decode", "A85", "ASCIIHexDecode", "AHx", "CCITTFaxDecode", "CCF", "DCTDecode", "DCT", "FlateDecode", "Fl",
        "JBIG2Decode", "JPXDecode", "RunLengthDecode", "RL", "LZWDecode", "LZW",
    };

    private sealed class BoundedFilter(string name, IFilter inner, int cap) : IFilter
    {
        private int _tripped;

        public bool IsSupported => inner.IsSupported;

        public bool Tripped => Volatile.Read(ref _tripped) != 0;

        public Memory<byte> Decode(Memory<byte> input, DictionaryToken streamDictionary, IFilterProvider filterProvider, int filterIndex)
        {
            var decoded = FilterChain.Stage(name, input, streamDictionary, filterIndex, cap);
            switch (decoded.Status)
            {
                case DecodeStatus.Decoded:
                    return MemoryMarshal.AsMemory(decoded.Bytes);
                case DecodeStatus.OverCap:
                    Volatile.Write(ref _tripped, 1);
                    throw new DecodeLimitExceededException(cap);
                default:
                    // Where the capped decoder fails PdfPig fails too, or falls back to the packed bytes: both stay small.
                    return inner.Decode(input, streamDictionary, filterProvider, filterIndex);
            }
        }
    }
}
