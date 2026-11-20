namespace AppCanzoniere.Application.PdfImport;

/// <summary>Hands back words that are already read, so the text importer lays out pages that came partly from OCR.</summary>
internal sealed class PrecomputedWordExtractor(PdfWordsResult result) : IPdfWordExtractor
{
    public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default) => Task.FromResult(result);
}
