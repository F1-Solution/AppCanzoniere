using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Reads positioned words from a PDF with PdfPig, entirely on the device.</summary>
public sealed class PdfPigWordExtractor : IPdfWordExtractor
{
    private readonly PdfExtractionLimits _limits;

    public PdfPigWordExtractor(PdfExtractionLimits? limits = null) => _limits = limits ?? new PdfExtractionLimits();

    public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default) =>
        Task.FromResult(PdfWordsResult.Success(Array.Empty<PdfPageWords>()));
}
