namespace AppCanzoniere.Application.PdfImport;

/// <summary>The encoded image of one scanned page (PNG, JPEG or BMP) with its size in pixels.</summary>
public sealed record OcrPageImage(ReadOnlyMemory<byte> Content, int Width, int Height);

/// <summary>A recognised word; the box is in image pixels with the origin at the top left and y pointing down.</summary>
public sealed record OcrWord(string Text, double Left, double Top, double Width, double Height);

/// <summary>A recognised line of words as the engine groups them.</summary>
public sealed record OcrLine(IReadOnlyList<OcrWord> Words);

/// <summary>Whether the engine read the page; a failed page is not a blank page.</summary>
public enum OcrPageStatus
{
    Read,
    Failed
}

/// <summary>What an engine recognised on one page image; Width and Height are the pixels the coordinates refer to.</summary>
public sealed record OcrPageResult(
    int Width,
    int Height,
    IReadOnlyList<OcrLine> Lines,
    string? Language = null,
    OcrPageStatus Status = OcrPageStatus.Read,
    string? FailureReason = null)
{
    public bool IsFailure => Status == OcrPageStatus.Failed;

    /// <summary>A page the engine could not read, with the reason.</summary>
    public static OcrPageResult Failure(string reason, string? language = null) =>
        new(0, 0, Array.Empty<OcrLine>(), language, OcrPageStatus.Failed, string.IsNullOrWhiteSpace(reason) ? "Unknown failure" : reason);
}

/// <summary>Reads text from a page image on the device; implementations never call a cloud service.</summary>
public interface IOcrEngine
{
    /// <summary>Recognises one page; throws only OperationCanceledException when the token is cancelled.</summary>
    Task<OcrPageResult> RecognizeAsync(OcrPageImage image, CancellationToken cancellationToken = default);
}
