using AppCanzoniere.Application.PdfImport;
using Windows.Data.Pdf;
using Windows.Storage.Streams;

namespace AppCanzoniere.WindowsOcr;

/// <summary>Renders PDF pages with the Windows PDF renderer at about 200 dpi, entirely on the device.</summary>
public sealed class WindowsPdfPageRenderer : IPageImageRenderer
{
    // Windows reports page sizes in device-independent pixels of 1/96 inch; PDF points are 1/72 inch.
    private const double PointsPerDip = 72.0 / 96.0;

    public async Task<IPageImageSession?> OpenAsync(Stream pdf, CancellationToken cancellationToken = default)
    {
        InMemoryRandomAccessStream? memory = null;
        try
        {
            memory = new InMemoryRandomAccessStream();
            await pdf.CopyToAsync(memory.AsStreamForWrite(), cancellationToken);
            await memory.FlushAsync().AsTask(cancellationToken);
            memory.Seek(0);
            var document = await PdfDocument.LoadFromStreamAsync(memory).AsTask(cancellationToken);
            if (document.IsPasswordProtected || document.PageCount == 0)
            {
                memory.Dispose();
                return null;
            }

            return new Session(document, memory);
        }
        catch (OperationCanceledException)
        {
            memory?.Dispose();
            throw;
        }
        catch (Exception)
        {
            memory?.Dispose();
            return null;
        }
    }

    private sealed class Session(PdfDocument document, InMemoryRandomAccessStream source) : IPageImageSession
    {
        public int PageCount => (int)document.PageCount;

        public async Task<OcrPageImage?> RenderPageAsync(int pageNumber, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1 || pageNumber > PageCount)
                return null;

            using var page = document.GetPage((uint)(pageNumber - 1));
            if (OcrRenderPlan.ComputeOrNull(page.Size.Width * PointsPerDip, page.Size.Height * PointsPerDip) is not { } size)
                return null;

            using var output = new InMemoryRandomAccessStream();
            var options = new PdfPageRenderOptions
            {
                DestinationWidth = (uint)size.Width,
                DestinationHeight = (uint)size.Height,
                BackgroundColor = global::Windows.UI.Color.FromArgb(255, 255, 255, 255)
            };
            await page.RenderToStreamAsync(output, options).AsTask(cancellationToken);
            output.Seek(0);
            using var bytes = new MemoryStream();
            await output.AsStreamForRead().CopyToAsync(bytes, cancellationToken);
            return new OcrPageImage(bytes.ToArray(), size.Width, size.Height);
        }

        public ValueTask DisposeAsync()
        {
            source.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
