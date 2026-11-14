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

/// <summary>Capped decoders in place of PdfPig's filters for every stream PdfPig unpacks while reading (content, forms, fonts, object streams).</summary>
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
                    // PdfPig's filter is never run, as it allocates by what the file claims: Flate gives back the packed bytes as PdfPig does on failure, the others fail as PdfPig's do.
                    return name is "FlateDecode" or "Fl" ? input : throw new UnreadableStreamException(name);
            }
        }
    }
}

/// <summary>Thrown inside PdfPig for a stream its filter cannot read either, in place of running that filter.</summary>
internal sealed class UnreadableStreamException(string filter)
    : InvalidOperationException($"The {filter} data of a stream cannot be read.");
