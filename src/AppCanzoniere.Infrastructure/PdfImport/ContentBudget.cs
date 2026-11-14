using AppCanzoniere.Application.PdfImport;
using UglyToad.PdfPig;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Measures every page of a document before the first one is parsed, and refuses the document if one is too big.</summary>
internal static class ContentBudget
{
    // A page tree has fewer inner nodes than pages in practice; four nodes per allowed page is a wide margin.
    private const int TreeNodesPerPage = 4;

    /// <summary>A refusal, or null when every page fits; checkpoint stops the walk when time is up.</summary>
    public static PdfWordsResult? Check(PdfDocument document, PdfExtractionLimits limits, int pageCount, Func<PdfWordsResult?> checkpoint)
    {
        var pages = PageTree.Pages(document, limits.MaxPages * TreeNodesPerPage);
        if (pages is null)
            return TooLarge($"The page tree has more than {limits.MaxPages * TreeNodesPerPage} nodes, the limit.");
        if (pages.Count != pageCount)
            return PdfWordsResult.Failure(PdfExtractionStatus.Damaged, "The page tree of the file cannot be read consistently.");

        var cap = (int)Math.Min(limits.MaxContentBytesPerPage, Array.MaxLength);
        for (var i = 0; i < pages.Count; i++)
        {
            if (checkpoint() is { } stop)
                return stop;

            var content = PageContent.Decode(document, pages[i].Dictionary, cap);
            if (content.Status == DecodeStatus.OverCap)
                return TooLarge($"Page {i + 1} holds more than {limits.MaxContentBytesPerPage} bytes of content, the limit.");
            if (content.Status == DecodeStatus.Unreadable)
                return TooLarge($"Page {i + 1} holds content that cannot be measured before it is read.");
        }

        return null;
    }

    private static PdfWordsResult TooLarge(string reason) => PdfWordsResult.Failure(PdfExtractionStatus.TooLarge, reason);
}
