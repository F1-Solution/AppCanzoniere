using AppCanzoniere.Application.PdfImport;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;
using AppLine = AppCanzoniere.Application.PdfImport.OcrLine;
using AppWord = AppCanzoniere.Application.PdfImport.OcrWord;

namespace AppCanzoniere.WindowsOcr;

/// <summary>Recognises page images with the OCR engine of Windows, entirely on the device.</summary>
public sealed class WindowsOcrEngine : IOcrEngine
{
    private readonly OcrEngine _engine;

    private WindowsOcrEngine(OcrEngine engine, string language)
    {
        _engine = engine;
        Language = language;
    }

    public string Language { get; }

    public static string InstalledLanguages() =>
        string.Join(", ", OcrEngine.AvailableRecognizerLanguages.Select(l => l.LanguageTag));

    /// <summary>Italian if installed, else English, else the user profile languages; null when none is available.</summary>
    public static WindowsOcrEngine? Create()
    {
        foreach (var tag in new[] { "it", "en" })
        {
            var language = new Language(tag);
            if (OcrEngine.IsLanguageSupported(language) && OcrEngine.TryCreateFromLanguage(language) is { } engine)
                return new WindowsOcrEngine(engine, engine.RecognizerLanguage.LanguageTag);
        }

        return OcrEngine.TryCreateFromUserProfileLanguages() is { } profile
            ? new WindowsOcrEngine(profile, profile.RecognizerLanguage.LanguageTag + " (user profile)")
            : null;
    }

    public async Task<OcrPageResult> RecognizeAsync(OcrPageImage image, CancellationToken cancellationToken = default)
    {
        try
        {
            return await RecognizeCoreAsync(image, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return OcrPageResult.Failure(ex.Message, Language);
        }
    }

    private async Task<OcrPageResult> RecognizeCoreAsync(OcrPageImage image, CancellationToken cancellationToken)
    {
        using var stream = new InMemoryRandomAccessStream();
        using (var writer = new DataWriter(stream.GetOutputStreamAt(0)))
        {
            writer.WriteBytes(image.Content.ToArray());
            await writer.StoreAsync().AsTask(cancellationToken);
            writer.DetachStream();
        }

        stream.Seek(0);
        var decoder = await BitmapDecoder.CreateAsync(stream).AsTask(cancellationToken);
        var scale = Scale(decoder.PixelWidth, decoder.PixelHeight);
        var width = (uint)Math.Max(1, Math.Round(decoder.PixelWidth * scale));
        var height = (uint)Math.Max(1, Math.Round(decoder.PixelHeight * scale));
        var transform = new BitmapTransform { ScaledWidth = width, ScaledHeight = height, InterpolationMode = BitmapInterpolationMode.Fant };
        using var bitmap = await decoder.GetSoftwareBitmapAsync(
            BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage).AsTask(cancellationToken);
        var result = await _engine.RecognizeAsync(bitmap).AsTask(cancellationToken);
        var lines = result.Lines
            .Select(l => new AppLine(l.Words.Select(w => new AppWord(w.Text, w.BoundingRect.X, w.BoundingRect.Y, w.BoundingRect.Width, w.BoundingRect.Height)).ToList()))
            .ToList();
        return new OcrPageResult((int)width, (int)height, lines, Language);
    }

    // Shrinks an image that is too large for the engine, and enlarges a small one to about 2,000 pixels on its long side.
    private static double Scale(uint width, uint height)
    {
        var longSide = Math.Max(width, height);
        var max = OcrEngine.MaxImageDimension;
        if (longSide > max)
            return (double)max / longSide;
        return longSide < 2000 ? Math.Min(2.0, 2000.0 / longSide) : 1.0;
    }
}
