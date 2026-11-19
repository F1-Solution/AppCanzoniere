using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.MAUI.Ocr;

/// <summary>Renders pages of a scanned PDF to PNG images at about 200 dpi, within a pixel budget.</summary>
public sealed class AndroidPdfPageRenderer
{
    /// <summary>Renders one page (zero-based); returns null when the page does not exist or has no usable size.</summary>
    public Task<OcrPageImage?> RenderPageAsync(string path, int pageIndex, CancellationToken cancellationToken = default) =>
        Task.Run(() => Render(path, pageIndex, cancellationToken), cancellationToken);

    /// <summary>Number of pages of the file.</summary>
    public Task<int> CountPagesAsync(string path) => Task.Run(() =>
    {
        using var descriptor = ParcelFileDescriptor.Open(new Java.IO.File(path), ParcelFileMode.ReadOnly)!;
        using var renderer = new PdfRenderer(descriptor);
        return renderer.PageCount;
    });

    private static OcrPageImage? Render(string path, int pageIndex, CancellationToken cancellationToken)
    {
        using var descriptor = ParcelFileDescriptor.Open(new Java.IO.File(path), ParcelFileMode.ReadOnly)!;
        using var renderer = new PdfRenderer(descriptor);
        if (pageIndex < 0 || pageIndex >= renderer.PageCount)
            return null;

        using var page = renderer.OpenPage(pageIndex)!;
        if (OcrRenderPlan.ComputeOrNull(page.Width, page.Height) is not { } size)
            return null;

        cancellationToken.ThrowIfCancellationRequested();
        using var bitmap = Bitmap.CreateBitmap(size.Width, size.Height, Bitmap.Config.Argb8888!)!;
        bitmap.EraseColor(Android.Graphics.Color.White);
        page.Render(bitmap, null, null, PdfRenderMode.ForDisplay);

        using var stream = new MemoryStream();
        bitmap.Compress(Bitmap.CompressFormat.Png!, 100, stream);
        return new OcrPageImage(stream.ToArray(), size.Width, size.Height);
    }
}
