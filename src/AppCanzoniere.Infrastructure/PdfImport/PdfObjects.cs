using UglyToad.PdfPig;
using UglyToad.PdfPig.Tokens;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Lenient lookups in the object graph of a PDF: a missing or broken object reads as absent, never throws.</summary>
internal static class PdfObjects
{
    /// <summary>The object a token stands for, following references; null when a reference leads nowhere.</summary>
    public static IToken? Resolve(PdfDocument document, IToken? token)
    {
        if (token is not IndirectReferenceToken reference)
            return token;
        try
        {
            return document.Structure.GetObject(reference.Data)?.Data;
        }
        catch (Exception exception) when (exception is not OutOfMemoryException and not DecodeLimitExceededException)
        {
            // A reference the parser cannot follow is treated as absent, as PdfPig's lenient mode does.
            return null;
        }
    }

    /// <summary>The entry of a dictionary as type T, following references.</summary>
    public static T? Get<T>(PdfDocument document, DictionaryToken dictionary, NameToken key)
        where T : class, IToken =>
        dictionary.TryGet(key, out var token) ? Resolve(document, token) as T : null;
}
