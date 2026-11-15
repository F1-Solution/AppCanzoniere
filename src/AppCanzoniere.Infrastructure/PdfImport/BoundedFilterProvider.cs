using System.Runtime.InteropServices;
using UglyToad.PdfPig.Filters;
using UglyToad.PdfPig.Tokens;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Thrown inside PdfPig when unpacking would pass a decode limit; the extractor turns it into a refusal.</summary>
internal sealed class DecodeLimitExceededException(string message) : Exception(message);

/// <summary>Thrown inside PdfPig for a stream its filter cannot read either, in place of running that filter.</summary>
internal sealed class UnreadableStreamException(string filter)
    : InvalidOperationException($"The {filter} data of a stream cannot be read.");

/// <summary>Which decode limit a document passed, if any.</summary>
internal enum DecodeRefusal
{
    None,
    Stream,
    Document,
}

/// <summary>Capped decoders in place of PdfPig's filters for every stream PdfPig unpacks while reading (content, forms, fonts, object streams).</summary>
internal sealed class BoundedFilterProvider : BaseFilterProvider
{
    private readonly Tally _tally;

    public BoundedFilterProvider(long maxStreamBytes, long maxDocumentBytes)
        : this(new Tally(maxDocumentBytes), Cap(maxStreamBytes))
    {
    }

    private BoundedFilterProvider(Tally tally, int cap)
        : base(Wrap(cap, tally)) => _tally = tally;

    /// <summary>The limit passed, even if PdfPig caught the refusal and carried on.</summary>
    public DecodeRefusal Refusal => _tally.Refusal;

    private static int Cap(long bytes) => (int)Math.Clamp(bytes, 1, Array.MaxLength);

    private static Dictionary<string, IFilter> Wrap(int cap, Tally tally)
    {
        var inner = DefaultFilterProvider.Instance;
        var wrapped = new Dictionary<IFilter, IFilter>();
        var names = new Dictionary<string, IFilter>();
        foreach (var name in FilterNames)
        {
            var filter = inner.GetNamedFilters(new[] { NameToken.Create(name) })[0];
            if (!wrapped.TryGetValue(filter, out var bounded))
                wrapped[filter] = bounded = new BoundedFilter(name, filter.IsSupported, cap, tally);
            names[name] = bounded;
        }

        return names;
    }

    private static readonly string[] FilterNames =
    {
        "ASCII85Decode", "A85", "ASCIIHexDecode", "AHx", "CCITTFaxDecode", "CCF", "DCTDecode", "DCT", "FlateDecode", "Fl",
        "JBIG2Decode", "JPXDecode", "RunLengthDecode", "RL", "LZWDecode", "LZW",
    };

    // The bytes unpacked so far in one document, every unpack of a stream counted, and the first limit passed.
    private sealed class Tally(long budget)
    {
        private long _total;
        private int _refusal;

        public DecodeRefusal Refusal => (DecodeRefusal)Volatile.Read(ref _refusal);

        // Once a limit is passed nothing more is unpacked, so work PdfPig repeats after catching the refusal stays cheap.
        public void ThrowIfRefused()
        {
            if (Refusal != DecodeRefusal.None)
                throw new DecodeLimitExceededException("A decode limit of the document has already been passed.");
        }

        public Memory<byte> Count(Memory<byte> bytes)
        {
            if (Interlocked.Add(ref _total, bytes.Length) <= budget)
                return bytes;
            Refuse(DecodeRefusal.Document);
            throw new DecodeLimitExceededException($"The document unpacks to more than {budget} bytes.");
        }

        public void Refuse(DecodeRefusal refusal) => Interlocked.CompareExchange(ref _refusal, (int)refusal, (int)DecodeRefusal.None);
    }

    private sealed class BoundedFilter(string name, bool isSupported, int cap, Tally tally) : IFilter
    {
        public bool IsSupported => isSupported;

        public Memory<byte> Decode(Memory<byte> input, DictionaryToken streamDictionary, IFilterProvider filterProvider, int filterIndex)
        {
            tally.ThrowIfRefused();
            var decoded = FilterChain.Stage(name, input, streamDictionary, filterIndex, cap);
            switch (decoded.Status)
            {
                case DecodeStatus.Decoded:
                    return tally.Count(MemoryMarshal.AsMemory(decoded.Bytes));
                case DecodeStatus.OverCap:
                    tally.Refuse(DecodeRefusal.Stream);
                    throw new DecodeLimitExceededException($"A stream unpacks to more than {cap} bytes.");
                default:
                    // PdfPig's filter is never run, as it allocates by what the file claims: Flate gives back the packed bytes as PdfPig does on failure, the others fail as PdfPig's do.
                    return name is "FlateDecode" or "Fl" ? tally.Count(input) : throw new UnreadableStreamException(name);
            }
        }
    }
}
