using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using AppCanzoniere.Application.PdfImport;
using OperationCanceledException = System.OperationCanceledException;
using Path = System.IO.Path;

namespace AppCanzoniere.MAUI.Ocr;

/// <summary>Renders pages of a scanned PDF at about 200 dpi, within a pixel budget; renders run one at a time.</summary>
public sealed class AndroidPdfPageRenderer : IPageImageRenderer
{
    // PdfRenderer allows one open page at a time and is not thread safe.
    private static readonly object RenderLock = new();

    /// <summary>Copies the PDF to a private cache file, because PdfRenderer needs a file descriptor; the file is deleted when the session is disposed.</summary>
    public async Task<IPageImageSession?> OpenAsync(Stream pdf, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, $"import-{Guid.NewGuid():N}.pdf");
        try
        {
            await using (var file = File.Create(path))
                await pdf.CopyToAsync(file, cancellationToken).ConfigureAwait(false);

            var count = await CountPagesAsync(path).ConfigureAwait(false);
            return count > 0 ? new Session(this, path, count) : Discard(path);
        }
        catch (OperationCanceledException)
        {
            Delete(path);
            throw;
        }
        catch (Exception)
        {
            return Discard(path);
        }
    }

    private static IPageImageSession? Discard(string path)
    {
        Delete(path);
        return null;
    }

    private static void Delete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (IOException)
        {
            // The cache folder is cleaned by the system; a leftover file is harmless.
        }
    }

    private sealed class Session(AndroidPdfPageRenderer owner, string path, int pageCount) : IPageImageSession
    {
        public int PageCount => pageCount;

        public Task<OcrPageImage?> RenderPageAsync(int pageNumber, CancellationToken cancellationToken = default) =>
            owner.RenderPageAsync(path, pageNumber - 1, cancellationToken);

        public ValueTask DisposeAsync()
        {
            Delete(path);
            return ValueTask.CompletedTask;
        }
    }

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
