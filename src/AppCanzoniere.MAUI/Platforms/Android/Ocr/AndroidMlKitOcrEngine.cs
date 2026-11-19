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

    /// <summary>Why the last page could not be read; null when it was read.</summary>
    public string? LastFailure { get; private set; }

    /// <summary>True when the recogniser carries its model in the app (no Play Services download).</summary>
    public static bool IsBundledModel => TextRecognizerOptions.DefaultOptions.IsThickClient;

    public async Task<OcrPageResult> RecognizeAsync(OcrPageImage image, CancellationToken cancellationToken = default)
    {
        LastFailure = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await RecognizeCoreAsync(image, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            LastFailure = ex.Message;
            return new OcrPageResult(0, 0, Array.Empty<OcrLine>());
        }
    }

    public void Dispose()
    {
        if (_recognizer.IsValueCreated)
            _recognizer.Value.Dispose();
    }

    private async Task<OcrPageResult> RecognizeCoreAsync(OcrPageImage image, CancellationToken cancellationToken)
    {
        using var decoded = BitmapFactory.DecodeByteArray(image.Content.ToArray(), 0, image.Content.Length)
            ?? throw new InvalidOperationException("The page image cannot be decoded.");
        var originalWidth = decoded.Width;
        var originalHeight = decoded.Height;
        var reduced = OcrRenderPlan.ComputeOrNull(originalWidth * 72.0 / OcrRenderPlan.Dpi, originalHeight * 72.0 / OcrRenderPlan.Dpi);
        var shrink = reduced is { Capped: true };
        using var scaled = shrink ? Bitmap.CreateScaledBitmap(decoded, reduced!.Width, reduced.Height, true) : null;
        var bitmap = scaled ?? decoded;

        using var input = InputImage.FromBitmap(bitmap, 0);
        var raw = await _recognizer.Value.Process(input).AsAsync<MlText>().ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        var result = MlKitTextMapper.Map(bitmap.Width, bitmap.Height, ToRawLines(raw));
        return shrink ? ScaleBack(result, (double)originalWidth / bitmap.Width, originalWidth, originalHeight) : result;
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
