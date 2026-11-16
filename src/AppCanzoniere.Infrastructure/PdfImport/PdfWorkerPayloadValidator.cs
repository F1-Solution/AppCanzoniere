using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Checks a payload from another process against the limits before any of it is used.</summary>
internal static class PdfWorkerPayloadValidator
{
    private const int MaxTextLength = 4000;

    // The PDF format limits a page side to 14,400 units; word boxes may overhang a little.
    private const double MaxPageSide = 14400;
    private const double MaxCoordinate = 1_000_000;

    public static PdfWordsResult? TryConvert(PdfWorkerPayload payload, PdfExtractionLimits limits)
    {
        if (payload.Version != PdfWorkerProtocol.Version
            || !Enum.TryParse(payload.Status, out PdfExtractionStatus status)
            || !Enum.IsDefined(status)
            || payload.Reason is null || payload.Reason.Length > MaxTextLength
            || (payload.Detail is { } detail && detail.Length > MaxTextLength)
            || payload.Pages is null || payload.Pages.Count > limits.MaxPages)
            return null;

        var pages = new List<PdfPageWords>(payload.Pages.Count);
        long letters = 0;
        foreach (var page in payload.Pages)
        {
            if (!TryPage(page, pages.Count + 1, limits, ref letters, out var converted))
                return null;
            pages.Add(converted);
        }

        return StatusMatchesPages(status, pages)
            ? new PdfWordsResult(status, payload.Reason, pages) { Detail = payload.Detail }
            : null;
    }

    private static bool StatusMatchesPages(PdfExtractionStatus status, List<PdfPageWords> pages) => status switch
    {
        PdfExtractionStatus.Success => pages.Any(p => p.HasText),
        PdfExtractionStatus.NoTextLayer => pages.Count > 0 && pages.All(p => !p.HasText),
        _ => pages.Count == 0
    };

    private static bool TryPage(PdfWorkerPage? page, int expectedNumber, PdfExtractionLimits limits, ref long letters, out PdfPageWords converted)
    {
        converted = null!;
        if (page?.Words is null || page.Number != expectedNumber
            || !InRange(page.Width, 0, MaxPageSide) || !InRange(page.Height, 0, MaxPageSide)
            || page.Words.Count > limits.MaxLettersPerPage)
            return false;

        var words = new List<PositionedWord>(page.Words.Count);
        long onPage = 0;
        foreach (var word in page.Words)
        {
            if (!TryWord(word, out var positioned))
                return false;
            onPage += positioned.Text.Length;
            letters += positioned.Text.Length;
            if (onPage > limits.MaxLettersPerPage || letters > limits.MaxLettersPerDocument)
                return false;
            words.Add(positioned);
        }

        converted = new PdfPageWords(page.Number, page.Width, page.Height, words, page.HasImages);
        return true;
    }

    private static bool TryWord(PdfWorkerWord? word, out PositionedWord positioned)
    {
        positioned = null!;
        if (word?.Text is null || word.Text.Length > MaxTextLength
            || !InRange(word.Left, -MaxCoordinate, MaxCoordinate) || !InRange(word.Right, -MaxCoordinate, MaxCoordinate)
            || !InRange(word.Bottom, -MaxCoordinate, MaxCoordinate) || !InRange(word.Top, -MaxCoordinate, MaxCoordinate)
            || word.Right < word.Left || word.Top < word.Bottom)
            return false;

        if (word.LetterLefts is { } lefts && (lefts.Length > word.Text.Length || lefts.Any(x => !InRange(x, -MaxCoordinate, MaxCoordinate))))
            return false;

        positioned = new PositionedWord(word.Text, new WordBox(word.Left, word.Bottom, word.Right, word.Top), word.LetterLefts);
        return true;
    }

    private static bool InRange(double value, double min, double max) => double.IsFinite(value) && value >= min && value <= max;
}
