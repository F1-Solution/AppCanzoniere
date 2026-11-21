namespace AppCanzoniere.Application.PdfImport;

/// <summary>Stands in when the PDF reader cannot be started; every read fails with the given reason.</summary>
public sealed class UnavailablePdfWordExtractor(string reason) : IPdfWordExtractor
{
    public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default) =>
        Task.FromResult(PdfWordsResult.Failure(PdfExtractionStatus.Unreadable, reason));
}
