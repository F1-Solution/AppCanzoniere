#if DEBUG
using Android.Graphics;
using Android.Util;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.MAUI.Ocr;

/// <summary>Debug-only check driven by the intent extra "selftest=ocr"; it logs one OCR OK or OCR FAIL line.</summary>
internal static class OcrSelfTest
{
    public const string LogTag = "AppCanzoniereSelfTest";
    public const string ExtraName = "selftest";

    private static readonly string[] Lines = ["Do Re Mi", "Alleluia canto nuovo"];

    public static void RunIfRequested(string? extra)
    {
        if (!string.Equals(extra, "ocr", StringComparison.OrdinalIgnoreCase))
            return;
        _ = Task.Run(RunAsync);
    }

    private static async Task RunAsync()
    {
        try
        {
            using var engine = new AndroidMlKitOcrEngine();
            Log.Info(LogTag, $"OCR INFO bundled={AndroidMlKitOcrEngine.IsBundledModel}");
            var result = await engine.RecognizeAsync(Draw()).ConfigureAwait(false);
            if (engine.LastFailure is { } failure)
            {
                Log.Info(LogTag, $"OCR FAIL {failure}");
                return;
            }

            var text = string.Join(" | ", result.Lines.Select(l => string.Join(' ', l.Words.Select(w => w.Text))));
            var ok = text.Contains("Alleluia", StringComparison.OrdinalIgnoreCase) && text.Contains("nuovo", StringComparison.OrdinalIgnoreCase);
            Log.Info(LogTag, ok ? $"OCR OK {text}" : $"OCR FAIL unexpected text '{text}'");
        }
        catch (Exception ex)
        {
            Log.Info(LogTag, $"OCR FAIL {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static OcrPageImage Draw()
    {
        const int width = 1200;
        const int height = 400;
        using var bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888!)!;
        using var canvas = new Canvas(bitmap);
        canvas.DrawColor(Color.White);
        using var paint = new Paint(PaintFlags.AntiAlias) { Color = Color.Black, TextSize = 80 };
        for (var i = 0; i < Lines.Length; i++)
            canvas.DrawText(Lines[i], 40, 130 + (i * 140), paint);

        using var stream = new MemoryStream();
        bitmap.Compress(Bitmap.CompressFormat.Png!, 100, stream);
        return new OcrPageImage(stream.ToArray(), width, height);
    }
}
#endif
