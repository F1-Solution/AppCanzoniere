using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.MAUI.Ocr;

/// <summary>Renders pages of a scanned PDF at about 200 dpi, within a pixel budget; renders run one at a time.</summary>
public sealed class AndroidPdfPageRenderer
{
    // PdfRenderer allows one open page at a time and is not thread safe.
    private static readonly object RenderLock = new();

    /// <summary>Renders one page (zero-based) to a PNG image; returns null when the page does not exist or has no usable size.</summary>
    public Task<OcrPageImage?> RenderPageAsync(string path, int pageIndex, CancellationToken cancellationToken = default) =>
        Task.Run(() =>
        {
            var bitmap = Render(path, pageIndex, cancellationToken);
            if (bitmap is null)
                return null;

            try
            {
                using var stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png!, 100, stream);
                return new OcrPageImage(stream.ToArray(), bitmap.Width, bitmap.Height);
            }
            finally
            {
                bitmap.Recycle();
                bitmap.Dispose();
            }
        }, cancellationToken);

    /// <summary>Renders one page (zero-based) to a bitmap the caller owns and must recycle; null when the page does not exist or has no usable size.</summary>
    public Task<Bitmap?> RenderBitmapAsync(string path, int pageIndex, CancellationToken cancellationToken = default) =>
        Task.Run(() => Render(path, pageIndex, cancellationToken), cancellationToken);

    /// <summary>Number of pages of the file.</summary>
    public Task<int> CountPagesAsync(string path) => Task.Run(() =>
    {
        lock (RenderLock)
        {
            ParcelFileDescriptor? descriptor = null;
            PdfRenderer? renderer = null;
            try
            {
                descriptor = ParcelFileDescriptor.Open(new Java.IO.File(path), ParcelFileMode.ReadOnly)!;
                renderer = new PdfRenderer(descriptor);
                return renderer.PageCount;
            }
            finally
            {
                renderer?.Close();
                renderer?.Dispose();
                descriptor?.Close();
                descriptor?.Dispose();
            }
        }
    });

    private static Bitmap? Render(string path, int pageIndex, CancellationToken cancellationToken)
    {
        lock (RenderLock)
        {
            ParcelFileDescriptor? descriptor = null;
            PdfRenderer? renderer = null;
            PdfRenderer.Page? page = null;
            Bitmap? bitmap = null;
            try
            {
                descriptor = ParcelFileDescriptor.Open(new Java.IO.File(path), ParcelFileMode.ReadOnly)!;
                renderer = new PdfRenderer(descriptor);
                if (pageIndex < 0 || pageIndex >= renderer.PageCount)
                    return null;

                page = renderer.OpenPage(pageIndex)!;
                if (OcrRenderPlan.ComputeOrNull(page.Width, page.Height) is not { } size)
                    return null;

                cancellationToken.ThrowIfCancellationRequested();
                bitmap = Bitmap.CreateBitmap(size.Width, size.Height, Bitmap.Config.Argb8888!)!;
                bitmap.EraseColor(Android.Graphics.Color.White);
                page.Render(bitmap, null, null, PdfRenderMode.ForDisplay);
                var result = bitmap;
                bitmap = null;
                return result;
            }
            finally
            {
                bitmap?.Recycle();
                bitmap?.Dispose();
                page?.Close();
                page?.Dispose();
                renderer?.Close();
                renderer?.Dispose();
                descriptor?.Close();
                descriptor?.Dispose();
            }
        }
    }
}
