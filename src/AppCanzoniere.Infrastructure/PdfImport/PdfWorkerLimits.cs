using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>The versioned form in which the host hands its extraction limits to the reader process.</summary>
public sealed record PdfWorkerLimits(
    [property: JsonPropertyName("v")] int Version,
    [property: JsonPropertyName("b")] long MaxBytes,
    [property: JsonPropertyName("pg")] int MaxPages,
    [property: JsonPropertyName("lp")] int MaxLettersPerPage,
    [property: JsonPropertyName("cp")] long MaxContentBytesPerPage,
    [property: JsonPropertyName("cd")] long MaxContentBytesPerDocument,
    [property: JsonPropertyName("ld")] long MaxLettersPerDocument,
    [property: JsonPropertyName("ds")] long MaxDecodedStreamBytes,
    [property: JsonPropertyName("dd")] long MaxDecodedBytesPerDocument,
    [property: JsonPropertyName("ms")] long MaxDurationMilliseconds)
{
    /// <summary>The only version this build writes and reads.</summary>
    public const int CurrentVersion = 1;

    /// <summary>Command-line switch that carries the limits.</summary>
    public const string Switch = "--limits";

    /// <summary>The limits as one command-line value.</summary>
    public static string Serialize(PdfExtractionLimits limits)
    {
        ArgumentNullException.ThrowIfNull(limits);
        return JsonSerializer.Serialize(
            new PdfWorkerLimits(
                CurrentVersion,
                limits.MaxBytes,
                limits.MaxPages,
                limits.MaxLettersPerPage,
                limits.MaxContentBytesPerPage,
                limits.MaxContentBytesPerDocument,
                limits.MaxLettersPerDocument,
                limits.MaxDecodedStreamBytes,
                limits.MaxDecodedBytesPerDocument,
                (long)limits.MaxDuration.TotalMilliseconds),
            PdfWorkerLimitsJsonContext.Default.PdfWorkerLimits);
    }

    /// <summary>Reads limits from a command-line value; null when it is malformed, of another version or out of range.</summary>
    public static PdfExtractionLimits? TryParse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > 1024)
            return null;

        try
        {
            var value = JsonSerializer.Deserialize(text, PdfWorkerLimitsJsonContext.Default.PdfWorkerLimits);
            return value is { Version: CurrentVersion } && value.InRange() ? value.ToLimits() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private bool InRange() =>
        MaxBytes > 0 && MaxPages > 0 && MaxLettersPerPage > 0 && MaxContentBytesPerPage > 0 && MaxContentBytesPerDocument > 0
        && MaxLettersPerDocument > 0 && MaxDecodedStreamBytes > 0 && MaxDecodedBytesPerDocument > 0
        && MaxDurationMilliseconds > 0 && MaxDurationMilliseconds <= (long)AppCanzoniere.Application.PdfImport.PdfTextImporter.MaxSupportedDuration.TotalMilliseconds;

    private PdfExtractionLimits ToLimits() => new()
    {
        MaxBytes = MaxBytes,
        MaxPages = MaxPages,
        MaxLettersPerPage = MaxLettersPerPage,
        MaxContentBytesPerPage = MaxContentBytesPerPage,
        MaxContentBytesPerDocument = MaxContentBytesPerDocument,
        MaxLettersPerDocument = MaxLettersPerDocument,
        MaxDecodedStreamBytes = MaxDecodedStreamBytes,
        MaxDecodedBytesPerDocument = MaxDecodedBytesPerDocument,
        MaxDuration = TimeSpan.FromMilliseconds(MaxDurationMilliseconds)
    };
}

[JsonSerializable(typeof(PdfWorkerLimits))]
internal sealed partial class PdfWorkerLimitsJsonContext : JsonSerializerContext;
