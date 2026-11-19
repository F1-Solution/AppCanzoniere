namespace AppCanzoniere.Application.PdfImport;

/// <summary>The page words an OCR result adapts to, and how many recognised words were dropped.</summary>
public sealed record OcrWordsResult(PdfPageWords Page, int RejectedWords, bool TooManyWords, string? FailureReason = null)
{
    public bool IsFailure => FailureReason is not null;
}

/// <summary>Turns OCR words into the positioned words of the PDF path, so the same layout reads a scan.</summary>
public static class OcrWordAdapter
{
    /// <summary>Most words one page may carry (a dense songbook page has under 1,000).</summary>
    public const int MaxWordsPerPage = 20_000;

    /// <summary>Longest word text kept; longer texts are rejected as noise.</summary>
    public const int MaxWordLength = 200;

    /// <summary>Most pixels per page side accepted.</summary>
    public const int MaxSidePixels = 20_000;

    /// <summary>Width of the page the pixels are scaled to, as an A4 sheet in points.</summary>
    public const double PageWidthPoints = 595;

    /// <summary>Scales the pixel boxes to points, flips y to point up and drops words with unusable boxes or text.</summary>
    public static OcrWordsResult ToPageWords(OcrPageResult result, int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return Empty(pageNumber, 0, tooMany: false) with { FailureReason = result.FailureReason };

        if (result.Width is <= 0 or > MaxSidePixels || result.Height is <= 0 or > MaxSidePixels)
            return Empty(pageNumber, CountWords(result), tooMany: false);

        var scale = PageWidthPoints / result.Width;
        var pageHeight = result.Height * scale;
        var words = new List<PositionedWord>();
        var rejected = 0;
        foreach (var word in result.Lines.SelectMany(l => l.Words))
        {
            if (words.Count + rejected >= MaxWordsPerPage)
                return Empty(pageNumber, CountWords(result), tooMany: true);
            if (Convert(word, result.Width, result.Height, scale, pageHeight) is { } positioned)
                words.Add(positioned);
            else
                rejected++;
        }

        return new OcrWordsResult(new PdfPageWords(pageNumber, PageWidthPoints, pageHeight, words, HasImages: true), rejected, false);
    }

    private static PositionedWord? Convert(OcrWord word, int width, int height, double scale, double pageHeight)
    {
        var text = word.Text?.Trim();
        if (string.IsNullOrEmpty(text) || text.Length > MaxWordLength)
            return null;
        if (!double.IsFinite(word.Left) || !double.IsFinite(word.Top) || !double.IsFinite(word.Width) || !double.IsFinite(word.Height))
            return null;

        var left = Math.Clamp(word.Left, 0, width);
        var right = Math.Clamp(word.Left + word.Width, 0, width);
        var top = Math.Clamp(word.Top, 0, height);
        var bottom = Math.Clamp(word.Top + word.Height, 0, height);
        if (right - left <= 0 || bottom - top <= 0)
            return null;

        return new PositionedWord(text, new WordBox(left * scale, pageHeight - (bottom * scale), right * scale, pageHeight - (top * scale)));
    }

    private static OcrWordsResult Empty(int pageNumber, int rejected, bool tooMany) =>
        new(new PdfPageWords(pageNumber, PageWidthPoints, 0, Array.Empty<PositionedWord>(), HasImages: true), rejected, tooMany);

    private static int CountWords(OcrPageResult result) => result.Lines.Sum(l => l.Words.Count);
}
