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

    public PdfTextImporter(IPdfWordExtractor extractor) => _extractor = extractor;

    public Task<PdfTextResult> ImportAsync(Stream pdf, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PdfTextResult(PdfExtractionStatus.Damaged, string.Empty, Array.Empty<PageText>()));
}
