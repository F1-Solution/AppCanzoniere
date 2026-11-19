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
    [property: JsonPropertyName("m")] string? Title,
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
    public const int Version = 2;

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
        ArgumentNullException.ThrowIfNull(input);
        if (input is MemoryStream memory && memory.TryGetBuffer(out var buffer))
            return TryRead(buffer.AsSpan((int)memory.Position), limits);

        using var copy = new MemoryStream();
        input.CopyTo(copy);
        return TryRead(copy.GetBuffer().AsSpan(0, (int)copy.Length), limits);
    }

    /// <summary>Reads and validates a payload held in memory; the number of JSON values is bounded before any object is built.</summary>
    public static PdfWordsResult? TryRead(ReadOnlySpan<byte> json, PdfExtractionLimits limits)
    {
        try
        {
            if (!WithinCounts(json, limits))
                return null;

            var payload = JsonSerializer.Deserialize(json, PdfWorkerJsonContext.Default.PdfWorkerPayload);
            return payload is null ? null : PdfWorkerPayloadValidator.TryConvert(payload, limits);
        }
        catch (Exception exception) when (exception is JsonException or NotSupportedException or InvalidOperationException)
        {
            return null;
        }
    }

    // A word has at least one letter and a page at least a number, so these counts follow from the limits; a word takes at most 16 tokens, a letter box one more.
    private static bool WithinCounts(ReadOnlySpan<byte> json, PdfExtractionLimits limits)
    {
        var maxObjects = limits.MaxPages + limits.MaxLettersPerDocument + 1;
        var maxTokens = (16 * maxObjects) + limits.MaxLettersPerDocument + 64;
        long objects = 0;
        long tokens = 0;
        var reader = new Utf8JsonReader(json);
        while (reader.Read())
        {
            if (++tokens > maxTokens)
                return false;
            if (reader.TokenType == JsonTokenType.StartObject && ++objects > maxObjects)
                return false;
        }

        return true;
    }

    private static PdfWorkerPayload ToPayload(PdfWordsResult result) => new(
        Version,
        result.Status.ToString(),
        result.Reason,
        result.Detail,
        result.Title,
        result.Pages.Select(ToPage).ToList());

    private static PdfWorkerPage ToPage(PdfPageWords page) => new(
        page.PageNumber,
        page.Width,
        page.Height,
        page.HasImages,
        page.Words.Select(w => new PdfWorkerWord(w.Text, w.Box.Left, w.Box.Bottom, w.Box.Right, w.Box.Top, w.LetterLefts?.ToArray())).ToList());
}
