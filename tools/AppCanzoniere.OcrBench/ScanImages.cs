using AppCanzoniere.Application.PdfImport;
using UglyToad.PdfPig;

namespace AppCanzoniere.OcrBench;

/// <summary>A page without a text layer and the largest image found on it.</summary>
internal sealed record ScannedPage(int PageNumber, OcrPageImage? Image, string Note);

/// <summary>Finds the pages of a PDF that have no text layer and extracts their page image, within size limits.</summary>
internal static class ScanImages
{
    public const long MaxPixels = 40_000_000;
    public const int MaxImageBytes = 64 * 1024 * 1024;

    public static IReadOnlyList<ScannedPage> Extract(byte[] pdf)
    {
        var pages = new List<ScannedPage>();
        using var document = PdfDocument.Open(pdf);
        foreach (var page in document.GetPages())
        {
            if (page.Letters.Count > 0)
                continue;

            var best = page.GetImages()
                .Where(i => i.WidthInSamples > 0 && i.HeightInSamples > 0 && (long)i.WidthInSamples * i.HeightInSamples <= MaxPixels)
                .OrderByDescending(i => (long)i.WidthInSamples * i.HeightInSamples)
                .FirstOrDefault();
            pages.Add(best is null ? new ScannedPage(page.Number, null, "no usable image (none, or over 40 megapixels)") : Encode(page.Number, best));
        }

        return pages;
    }

    private static ScannedPage Encode(int number, UglyToad.PdfPig.Content.IPdfImage image)
    {
        var width = image.WidthInSamples;
        var height = image.HeightInSamples;
        if (image.TryGetPng(out var png) && png is { Length: > 0 and <= MaxImageBytes })
            return new ScannedPage(number, new OcrPageImage(png, width, height), $"{width}x{height} PNG");

        var raw = image.RawMemory;
        if (raw.Length is > 0 and <= MaxImageBytes && IsJpeg(raw.Span))
            return new ScannedPage(number, new OcrPageImage(raw.ToArray(), width, height), $"{width}x{height} JPEG");

        return new ScannedPage(number, null, "image could not be extracted");
    }

    private static bool IsJpeg(ReadOnlySpan<byte> bytes) => bytes.Length > 3 && bytes[0] == 0xFF && bytes[1] == 0xD8;
}
