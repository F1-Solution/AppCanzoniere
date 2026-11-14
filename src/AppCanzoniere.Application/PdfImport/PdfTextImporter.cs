namespace AppCanzoniere.Application.PdfImport;

/// <summary>Aligned text of every page of a PDF, or the typed failure that prevented it.</summary>
public sealed record PdfTextResult(PdfExtractionStatus Status, string Reason, IReadOnlyList<PageText> Pages)
{
    public bool IsSuccess => Status == PdfExtractionStatus.Success;
}

/// <summary>Reads a PDF on the device and returns its text page by page and column by column.</summary>
public sealed class PdfTextImporter
{
    private readonly IPdfWordExtractor _extractor;

    public PdfTextImporter(IPdfWordExtractor extractor) =>
        _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));

    /// <summary>Extracts and lays out every page; failures come back typed, cancellation throws.</summary>
    public async Task<PdfTextResult> ImportAsync(Stream pdf, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdf);
        var words = await _extractor.ExtractAsync(pdf, cancellationToken).ConfigureAwait(false);
        var pages = new List<PageText>(words.Pages.Count);
        foreach (var page in words.Pages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            pages.Add(PdfPageLayout.Lay(page));
        }

        return new PdfTextResult(words.Status, words.Reason, pages);
    }
}
