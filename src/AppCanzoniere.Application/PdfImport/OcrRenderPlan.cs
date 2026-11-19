namespace AppCanzoniere.Application.PdfImport;

/// <summary>The pixel size a scanned page is rendered at before recognition; Capped tells the size was reduced.</summary>
public sealed record OcrRenderSize(int Width, int Height, bool Capped);

/// <summary>Chooses the render size of a page (about 200 dpi, bounded against out-of-memory) and scales boxes between sizes.</summary>
public static class OcrRenderPlan
{
    /// <summary>Target resolution in dots per inch.</summary>
    public const double Dpi = 200;

    /// <summary>Most pixels on one side of the rendered page.</summary>
    public const int MaxSidePixels = 4096;

    /// <summary>Most pixels on a whole page (about 48 MB as ARGB_8888).</summary>
    public const long MaxPixels = 12_000_000;

    /// <summary>Size for a page of the given size in points (1/72 inch); null when the size is unusable.</summary>
    public static OcrRenderSize? ComputeOrNull(double pageWidthPoints, double pageHeightPoints)
    {
        if (!double.IsFinite(pageWidthPoints) || !double.IsFinite(pageHeightPoints) || pageWidthPoints <= 0 || pageHeightPoints <= 0)
            return null;

        var scale = Dpi / 72.0;
        var width = pageWidthPoints * scale;
        var height = pageHeightPoints * scale;
        var shrink = Math.Min(1.0, Math.Min(MaxSidePixels / Math.Max(width, height), Math.Sqrt(MaxPixels / (width * height))));
        var capped = shrink < 1.0;
        var w = (int)Math.Max(1, Math.Floor(width * shrink + 0.5));
        var h = (int)Math.Max(1, Math.Floor(height * shrink + 0.5));
        while (w > MaxSidePixels || h > MaxSidePixels || (long)w * h > MaxPixels)
        {
            w = Math.Max(1, w - 1);
            h = Math.Max(1, h - 1);
        }

        return new OcrRenderSize(w, h, capped);
    }

    /// <summary>Scales a word box by a factor, for example from a reduced bitmap back to the rendered page.</summary>
    public static OcrWord ScaleWord(OcrWord word, double factor) =>
        new(word.Text, word.Left * factor, word.Top * factor, word.Width * factor, word.Height * factor);
}
