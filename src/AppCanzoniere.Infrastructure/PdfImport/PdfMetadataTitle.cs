using System.Xml;
using AppCanzoniere.Application.PdfImport;
using UglyToad.PdfPig;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Reads the title of a PDF from its information dictionary or, failing that, its XMP metadata; the file is untrusted, so any failure gives no title.</summary>
internal static class PdfMetadataTitle
{
    private const string DublinCore = "http://purl.org/dc/elements/1.1/";
    private const string Rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
    private const int MaxXmpBytes = 256 * 1024;

    public static string? Read(PdfDocument document)
    {
        try
        {
            return PdfDocumentTitle.Clean(document.Information?.Title) ?? FromXmp(document);
        }
        catch (Exception exception) when (exception is not OperationCanceledException and not OutOfMemoryException)
        {
            return null;
        }
    }

    private static string? FromXmp(PdfDocument document)
    {
        if (!document.TryGetXmpMetadata(out var xmp) || xmp is null)
            return null;

        var bytes = xmp.GetXmlMemory();
        if (bytes.Length == 0 || bytes.Length > MaxXmpBytes)
            return null;

        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = MaxXmpBytes };
        using var stream = new MemoryStream(bytes.ToArray(), writable: false);
        using var reader = XmlReader.Create(stream, settings);
        return PdfDocumentTitle.Clean(FirstTitleItem(reader));
    }

    // The first rdf:li found under dc:title.
    private static string? FirstTitleItem(XmlReader reader)
    {
        var depth = -1;
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "title" && reader.NamespaceURI == DublinCore)
                depth = reader.Depth;
            else if (depth >= 0 && reader.NodeType == XmlNodeType.Element && reader.LocalName == "li" && reader.NamespaceURI == Rdf)
                return reader.ReadElementContentAsString();
            else if (depth >= 0 && reader.NodeType == XmlNodeType.EndElement && reader.Depth == depth)
                return null;
        }

        return null;
    }
}
