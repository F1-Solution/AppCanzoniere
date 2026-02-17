using AppCanzoniere.Application.DTOs;

namespace AppCanzoniere.Application.Interfaces;

public interface IOcrService
{
    /// <summary>
    /// Extract text from an image file
    /// </summary>
    Task<OcrResult> ExtractTextFromImageAsync(
        string imagePath, 
        string language = "ita", 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extract text from image bytes
    /// </summary>
    Task<OcrResult> ExtractTextFromImageAsync(
        byte[] imageBytes, 
        string language = "ita", 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extract text from a PDF file
    /// </summary>
    Task<OcrResult> ExtractTextFromPdfAsync(
        string pdfPath, 
        string language = "ita", 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the service is available and configured
    /// </summary>
    Task<bool> IsAvailableAsync();

    /// <summary>
    /// Get the service name
    /// </summary>
    string ServiceName { get; }
}
