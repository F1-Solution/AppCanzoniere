namespace AppCanzoniere.Application.PdfImport;

/// <summary>Draws pages of a scanned PDF as images for OCR on the device; implementations never call a cloud service.</summary>
public interface IPageImageRenderer
{
    /// <summary>Opens the PDF for rendering; returns null when the platform cannot open it. Throws only OperationCanceledException when cancelled.</summary>
    Task<IPageImageSession?> OpenAsync(Stream pdf, CancellationToken cancellationToken = default);
}

/// <summary>An open PDF whose pages can be rendered one at a time; dispose it to release the file.</summary>
public interface IPageImageSession : IAsyncDisposable
{
    int PageCount { get; }

    /// <summary>Renders a page (1-based) at the size chosen by OcrRenderPlan; null when the page is missing or has no usable size.</summary>
    Task<OcrPageImage?> RenderPageAsync(int pageNumber, CancellationToken cancellationToken = default);
}

/// <summary>An OCR engine for a device that has none: every page comes back as a failed page with the reason.</summary>
public sealed class UnavailableOcrEngine(string reason) : IOcrEngine
{
    public Task<OcrPageResult> RecognizeAsync(OcrPageImage image, CancellationToken cancellationToken = default) =>
        Task.FromResult(OcrPageResult.Failure(reason));
}
