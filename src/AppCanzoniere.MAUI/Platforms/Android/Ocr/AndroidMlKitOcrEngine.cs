using Android.Gms.Extensions;
using Android.Graphics;
using AppCanzoniere.Application.PdfImport;
using Xamarin.Google.MLKit.Vision.Common;
using Xamarin.Google.MLKit.Vision.Text;
using Xamarin.Google.MLKit.Vision.Text.Latin;
using MlText = Xamarin.Google.MLKit.Vision.Text.Text;

namespace AppCanzoniere.MAUI.Ocr;

/// <summary>Recognises page images with the bundled Latin model of ML Kit, entirely on the device.</summary>
public sealed class AndroidMlKitOcrEngine : IOcrEngine, IDisposable
{
    private readonly Lazy<ITextRecognizer> _recognizer = new(() => TextRecognition.GetClient(TextRecognizerOptions.DefaultOptions));

    /// <summary>True when the recogniser carries its model in the app (no Play Services download).</summary>
    public static bool IsBundledModel => TextRecognizerOptions.DefaultOptions.IsThickClient;

    /// <summary>Recognises an encoded page image; callers must await it and never block on the UI thread, because ML Kit completes there.</summary>
    public async Task<OcrPageResult> RecognizeAsync(OcrPageImage image, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var decoded = Decode(image.Content.ToArray());
            try
            {
                return await RecognizeCoreAsync(decoded, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                decoded.Recycle();
                decoded.Dispose();
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return OcrPageResult.Failure(ex.Message);
        }
    }

    /// <summary>Recognises a bitmap without encoding it; the caller keeps ownership and recycles it. Do not block on the result on the UI thread.</summary>
    public async Task<OcrPageResult> RecognizeBitmapAsync(Bitmap bitmap, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await RecognizeCoreAsync(bitmap, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return OcrPageResult.Failure(ex.Message);
        }
    }

    public void Dispose()
    {
        if (_recognizer.IsValueCreated)
            _recognizer.Value.Close();
    }

    // Reads the bounds first, then decodes at a power-of-two sample size within the render caps.
    private static Bitmap Decode(byte[] content)
    {
        var bounds = new BitmapFactory.Options { InJustDecodeBounds = true };
        BitmapFactory.DecodeByteArray(content, 0, content.Length, bounds);
        if (bounds.OutWidth <= 0 || bounds.OutHeight <= 0)
            throw new InvalidOperationException("The page image cannot be decoded.");

        var options = new BitmapFactory.Options { InSampleSize = OcrRenderPlan.SampleSizeFor(bounds.OutWidth, bounds.OutHeight) };
        return BitmapFactory.DecodeByteArray(content, 0, content.Length, options)
            ?? throw new InvalidOperationException("The page image cannot be decoded.");
    }

    private async Task<OcrPageResult> RecognizeCoreAsync(Bitmap source, CancellationToken cancellationToken)
    {
        var reduced = OcrRenderPlan.ComputeOrNull(source.Width * 72.0 / OcrRenderPlan.Dpi, source.Height * 72.0 / OcrRenderPlan.Dpi);
        var shrink = reduced is { Capped: true };
        var scaled = shrink ? Bitmap.CreateScaledBitmap(source, reduced!.Width, reduced.Height, true) : null;
        try
        {
            var bitmap = scaled ?? source;
            using var input = InputImage.FromBitmap(bitmap, 0);
            var raw = await _recognizer.Value.Process(input).AsAsync<MlText>().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            // The task completes on the main thread, so the mapping moves to the thread pool.
            var width = bitmap.Width;
            var height = bitmap.Height;
            var originalWidth = source.Width;
            var originalHeight = source.Height;
            return await Task.Run(() =>
            {
                var result = MlKitTextMapper.Map(width, height, ToRawLines(raw));
                return shrink ? ScaleBack(result, (double)originalWidth / width, originalWidth, originalHeight) : result;
            }, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (scaled is not null)
            {
                scaled.Recycle();
                scaled.Dispose();
            }
        }
    }

    private static IEnumerable<RawTextLine> ToRawLines(MlText text)
    {
        foreach (var block in text.TextBlocks ?? [])
        {
            foreach (var line in block.Lines ?? [])
            {
                var elements = (line.Elements ?? []).Select(e => ToRaw(e.Text, e.BoundingBox)).ToList();
                var box = line.BoundingBox;
                yield return new RawTextLine(line.Text, elements, box?.Left ?? 0, box?.Top ?? 0, box?.Right ?? 0, box?.Bottom ?? 0);
            }
        }
    }

    private static RawTextElement ToRaw(string? text, Android.Graphics.Rect? box) =>
        new(text, box?.Left ?? 0, box?.Top ?? 0, box?.Right ?? 0, box?.Bottom ?? 0);

    private static OcrPageResult ScaleBack(OcrPageResult result, double factor, int width, int height) =>
        new(width, height, result.Lines.Select(l => new OcrLine(l.Words.Select(w => OcrRenderPlan.ScaleWord(w, factor)).ToList())).ToList());
}
