using System.IO.Compression;
using System.Runtime.InteropServices;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Tokens;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Measures the content streams of a page from its dictionary, before PdfPig parses the page.</summary>
internal static class PageContentSize
{
    private const int UnpackBufferBytes = 81920;

    /// <summary>The page dictionaries of the document by page number, walking the page tree in reading order.</summary>
    public static IReadOnlyDictionary<int, DictionaryToken> PageDictionaries(PdfDocument document, int maxPages)
    {
        var pages = new Dictionary<int, DictionaryToken>();
        if (!document.Structure.Catalog.CatalogDictionary.TryGet(NameToken.Pages, out var root))
            return pages;

        var pending = new Stack<IToken>();
        var visited = new HashSet<IndirectReference>();
        pending.Push(root);
        while (pending.Count > 0 && pages.Count < maxPages)
        {
            var token = pending.Pop();
            if (token is IndirectReferenceToken reference && !visited.Add(reference.Data))
                continue;
            if (Resolve(document, token) is not DictionaryToken node)
                continue;

            if (node.TryGet(NameToken.Kids, out var kids) && Resolve(document, kids) is ArrayToken children)
            {
                for (var i = children.Data.Count - 1; i >= 0; i--)
                    pending.Push(children.Data[i]);
            }
            else
            {
                pages[pages.Count + 1] = node;
            }
        }

        return pages;
    }

    /// <summary>True when the page's content unpacks to more than limit bytes; a stream in another filter counts as its packed size.</summary>
    public static bool Exceeds(PdfDocument document, DictionaryToken page, long limit)
    {
        long total = 0;
        foreach (var stream in Streams(document, page))
        {
            total += Unpacked(stream, limit - total);
            if (total > limit)
                return true;
        }

        return false;
    }

    private static IEnumerable<StreamToken> Streams(PdfDocument document, DictionaryToken page)
    {
        if (!page.TryGet(NameToken.Contents, out var contents))
            yield break;

        var resolved = Resolve(document, contents);
        var parts = resolved is ArrayToken array ? array.Data : new[] { resolved };
        foreach (var part in parts)
        {
            if (Resolve(document, part) is StreamToken stream)
                yield return stream;
        }
    }

    private static IToken Resolve(PdfDocument document, IToken token) =>
        token is IndirectReferenceToken reference ? document.Structure.GetObject(reference.Data).Data : token;

    // Bytes the stream unpacks to, counting no further than just past cap, so a compression bomb is never unpacked whole.
    private static long Unpacked(StreamToken stream, long cap)
    {
        if (!stream.StreamDictionary.TryGet(NameToken.Filter, out var filter))
            return stream.Data.Length;
        if (!IsFlate(filter))
            return stream.Data.Length;

        var packed = MemoryMarshal.TryGetArray((ReadOnlyMemory<byte>)stream.Data, out var segment)
            ? new MemoryStream(segment.Array!, segment.Offset, segment.Count, writable: false)
            : new MemoryStream(stream.Data.ToArray(), writable: false);
        try
        {
            using var unpacked = new ZLibStream(packed, CompressionMode.Decompress);
            var buffer = new byte[UnpackBufferBytes];
            long total = 0;
            int read;
            while (total <= cap && (read = unpacked.Read(buffer)) > 0)
                total += read;
            return total;
        }
        catch (InvalidDataException)
        {
            // A stream zlib rejects is left to PdfPig's lenient decoder and counted at its packed size.
            return stream.Data.Length;
        }
    }

    private static bool IsFlate(IToken filter) => filter switch
    {
        NameToken name => name.Data is "FlateDecode" or "Fl",
        ArrayToken { Data.Count: 1 } array => IsFlate(array.Data[0]),
        _ => false,
    };
}
