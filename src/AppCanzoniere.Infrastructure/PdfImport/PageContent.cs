using UglyToad.PdfPig;
using UglyToad.PdfPig.Tokens;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>The unpacked content of a page as PdfPig assembles it, decoded before PdfPig parses the page.</summary>
internal static class PageContent
{
    private const byte PartSeparator = (byte)'\n';

    /// <summary>The page's content streams, unpacked and joined by a newline, or why they cannot be had within cap bytes.</summary>
    public static Decoded Decode(PdfDocument document, DictionaryToken page, int cap)
    {
        if (!page.TryGet(NameToken.Contents, out var entry))
            return new Decoded(DecodeStatus.Decoded, ReadOnlyMemory<byte>.Empty);

        var contents = PdfObjects.Resolve(document, entry);
        if (contents is not ArrayToken parts)
            return contents is StreamToken single ? FilterChain.Decode(document, single, cap) : Decoded.Unreadable;

        var output = new BoundedOutput(cap);
        for (var i = 0; i < parts.Data.Count; i++)
        {
            // PdfPig's lenient mode skips a part that is not a reference.
            if (parts.Data[i] is not IndirectReferenceToken reference)
                continue;
            if (PdfObjects.Resolve(document, reference) is not StreamToken stream)
                return Decoded.Unreadable;

            var part = FilterChain.Decode(document, stream, cap - output.Count);
            if (!part.IsDecoded)
                return part;
            if (!output.Write(part.Bytes.Span) || (i < parts.Data.Count - 1 && !output.Write(PartSeparator)))
                return Decoded.OverCap;
        }

        return Decoded.Of(output);
    }
}
