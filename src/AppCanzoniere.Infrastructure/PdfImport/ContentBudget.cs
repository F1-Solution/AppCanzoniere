using AppCanzoniere.Application.PdfImport;
using UglyToad.PdfPig;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Costs every page of a document before the first one is parsed, and refuses the document if a page or the whole is too big.</summary>
internal static class ContentBudget
{
    // A page tree has fewer inner nodes than pages in practice; four nodes per allowed page is a wide margin.
    private const int TreeNodesPerPage = 4;

    // Corpus pages draw from one resource dictionary; the cost walk scans the whole stack for every name it looks up.
    private const int MaxInheritedResources = 32;

    /// <summary>A refusal, or null when every page fits; checkpoint stops the walk when time is up.</summary>
    public static PdfWordsResult? Check(PdfDocument document, PdfExtractionLimits limits, int pageCount, Func<PdfWordsResult?> checkpoint)
    {
        var pages = PageTree.Pages(document, limits.MaxPages * TreeNodesPerPage);
        if (pages is null)
            return TooLarge($"The page tree has more than {limits.MaxPages * TreeNodesPerPage} nodes, the limit.");
        if (pages.Count != pageCount)
            return PdfWordsResult.Failure(PdfExtractionStatus.Damaged, "The page tree of the file cannot be read consistently.");
        if (pages.Any(p => p.Resources.Depth > MaxInheritedResources))
            return TooLarge($"A page draws from more than {MaxInheritedResources} resource dictionaries, the limit.");

        var costs = new DrawingCost(document, limits.MaxContentBytesPerPage, limits.MaxLettersPerPage);
        var total = new Cost(0, 0);
        for (var i = 0; i < pages.Count; i++)
        {
            if (checkpoint() is { } stop)
                return stop;

            var cost = costs.Page(pages[i]);
            if (costs.TooManyNamedCalls)
                return TooLarge($"The document calls drawings by name more than {DrawingCost.MaxNamedCalls} times, the limit.");
            if (PageRefusal(cost, i + 1, limits) is { } refusal)
                return refusal;

            total = total.Plus(cost);
            if (DocumentRefusal(total, limits) is { } tooMuch)
                return tooMuch;
        }

        return null;
    }

    private static PdfWordsResult? PageRefusal(Cost cost, int number, PdfExtractionLimits limits)
    {
        if (cost.Unmeasurable)
            return TooLarge($"Page {number} holds content that cannot be measured before it is read.");
        if (cost.Bytes > limits.MaxContentBytesPerPage)
            return TooLarge($"Page {number} holds more than {limits.MaxContentBytesPerPage} bytes of content, the limit.");
        return cost.Letters > limits.MaxLettersPerPage
            ? TooLarge($"Page {number} would draw more than {limits.MaxLettersPerPage} characters, the limit.")
            : null;
    }

    private static PdfWordsResult? DocumentRefusal(Cost total, PdfExtractionLimits limits)
    {
        if (total.Bytes > limits.MaxContentBytesPerDocument)
            return TooLarge($"The document holds more than {limits.MaxContentBytesPerDocument} bytes of content, the limit.");
        return total.Letters > limits.MaxLettersPerDocument
            ? TooLarge($"The document would draw more than {limits.MaxLettersPerDocument} characters, the limit.")
            : null;
    }

    private static PdfWordsResult TooLarge(string reason) => PdfWordsResult.Failure(PdfExtractionStatus.TooLarge, reason);
}
