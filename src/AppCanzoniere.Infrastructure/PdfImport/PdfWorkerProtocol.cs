using System.Text.Json;
using System.Text.Json.Serialization;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>The serialised form a PDF reader process writes to stdout; the version changes with any incompatible change.</summary>
public sealed record PdfWorkerPayload(
    [property: JsonPropertyName("v")] int Version,
    [property: JsonPropertyName("s")] string Status,
    [property: JsonPropertyName("r")] string Reason,
    [property: JsonPropertyName("d")] string? Detail,
    [property: JsonPropertyName("p")] List<PdfWorkerPage> Pages);

/// <summary>One page of the payload.</summary>
public sealed record PdfWorkerPage(
    [property: JsonPropertyName("n")] int Number,
    [property: JsonPropertyName("w")] double Width,
    [property: JsonPropertyName("h")] double Height,
    [property: JsonPropertyName("i")] bool HasImages,
    [property: JsonPropertyName("x")] List<PdfWorkerWord> Words);

/// <summary>One word of the payload: text, box and the optional left x of each character.</summary>
public sealed record PdfWorkerWord(
    [property: JsonPropertyName("t")] string Text,
    [property: JsonPropertyName("l")] double Left,
    [property: JsonPropertyName("b")] double Bottom,
    [property: JsonPropertyName("r")] double Right,
    [property: JsonPropertyName("u")] double Top,
    [property: JsonPropertyName("c")] double[]? LetterLefts);

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(PdfWorkerPayload))]
internal sealed partial class PdfWorkerJsonContext : JsonSerializerContext;

/// <summary>Converts a result to and from the payload; reading never trusts the bytes it is given.</summary>
public static class PdfWorkerProtocol
{
    /// <summary>The only payload version this build writes and reads.</summary>
    public const int Version = 1;

    /// <summary>Writes the result as one JSON document.</summary>
    public static void Write(Stream output, PdfWordsResult result)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(result);
        JsonSerializer.Serialize(output, ToPayload(result), PdfWorkerJsonContext.Default.PdfWorkerPayload);
    }

    /// <summary>Reads and validates a payload against the limits; null when it is malformed or breaks a limit.</summary>
    public static PdfWordsResult? TryRead(Stream input, PdfExtractionLimits limits)
    {
        try
        {
            var payload = JsonSerializer.Deserialize(input, PdfWorkerJsonContext.Default.PdfWorkerPayload);
            return payload is null ? null : PdfWorkerPayloadValidator.TryConvert(payload, limits);
        }
        catch (Exception exception) when (exception is JsonException or NotSupportedException or InvalidOperationException)
        {
            return null;
        }
    }

    private static PdfWorkerPayload ToPayload(PdfWordsResult result) => new(
        Version,
        result.Status.ToString(),
        result.Reason,
        result.Detail,
        result.Pages.Select(ToPage).ToList());

    private static PdfWorkerPage ToPage(PdfPageWords page) => new(
        page.PageNumber,
        page.Width,
        page.Height,
        page.HasImages,
        page.Words.Select(w => new PdfWorkerWord(w.Text, w.Box.Left, w.Box.Bottom, w.Box.Right, w.Box.Top, w.LetterLefts?.ToArray())).ToList());
}
