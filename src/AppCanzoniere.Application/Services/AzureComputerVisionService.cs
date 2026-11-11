using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AppCanzoniere.Application.DTOs;
using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Services;

public class AzureComputerVisionService : IOcrService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string? _endpoint;

    public string ServiceName => "Azure Computer Vision";

    public AzureComputerVisionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("AZURE_COMPUTER_VISION_KEY");
        _endpoint = Environment.GetEnvironmentVariable("AZURE_COMPUTER_VISION_ENDPOINT");

        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
        }
    }

    public async Task<OcrResult> ExtractTextFromImageAsync(
        string imagePath,
        string language = "ita",
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(imagePath))
        {
            return new OcrResult
            {
                Success = false,
                ErrorMessage = $"Image file not found: {imagePath}"
            };
        }

        var imageBytes = await File.ReadAllBytesAsync(imagePath, cancellationToken);
        return await ExtractTextFromImageAsync(imageBytes, language, cancellationToken);
    }

    public async Task<OcrResult> ExtractTextFromImageAsync(
        byte[] imageBytes,
        string language = "ita",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_endpoint))
        {
            return new OcrResult
            {
                Success = false,
                ErrorMessage = "Azure Computer Vision not configured. Set AZURE_COMPUTER_VISION_KEY and AZURE_COMPUTER_VISION_ENDPOINT environment variables."
            };
        }

        try
        {
            // Step 1: Submit image for OCR (Read API)
            var readUrl = $"{_endpoint.TrimEnd('/')}/vision/v3.2/read/analyze?language={MapLanguage(language)}";

            using var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var submitResponse = await _httpClient.PostAsync(readUrl, imageContent, cancellationToken);

            if (!submitResponse.IsSuccessStatusCode)
            {
                var errorContent = await submitResponse.Content.ReadAsStringAsync(cancellationToken);
                return new OcrResult
                {
                    Success = false,
                    ErrorMessage = $"Azure API error: {submitResponse.StatusCode} - {errorContent}"
                };
            }

            // Get operation location from response headers
            if (!submitResponse.Headers.TryGetValues("Operation-Location", out var operationLocations))
            {
                return new OcrResult
                {
                    Success = false,
                    ErrorMessage = "No Operation-Location header in Azure response"
                };
            }

            var operationUrl = operationLocations.First();

            // Step 2: Poll for results
            JsonDocument? result = null;
            int maxAttempts = 10;
            int attempt = 0;

            while (attempt < maxAttempts)
            {
                await Task.Delay(1000, cancellationToken); // Wait 1 second between polls

                var resultResponse = await _httpClient.GetAsync(operationUrl, cancellationToken);
                var resultContent = await resultResponse.Content.ReadAsStringAsync(cancellationToken);
                result = JsonDocument.Parse(resultContent);

                var status = result.RootElement.GetProperty("status").GetString();

                if (status == "succeeded")
                {
                    break;
                }
                else if (status == "failed")
                {
                    return new OcrResult
                    {
                        Success = false,
                        ErrorMessage = "Azure OCR processing failed"
                    };
                }

                attempt++;
            }

            if (result == null || attempt >= maxAttempts)
            {
                return new OcrResult
                {
                    Success = false,
                    ErrorMessage = "Azure OCR processing timed out"
                };
            }

            // Step 3: Parse results
            var analyzeResult = result.RootElement.GetProperty("analyzeResult");
            var readResults = analyzeResult.GetProperty("readResults");

            var allText = new StringBuilder();
            var lines = new List<OcrLine>();
            int lineNumber = 0;

            foreach (var page in readResults.EnumerateArray())
            {
                if (page.TryGetProperty("lines", out var pageLines))
                {
                    foreach (var line in pageLines.EnumerateArray())
                    {
                        var text = line.GetProperty("text").GetString() ?? string.Empty;

                        // Azure doesn't provide per-line confidence, use default
                        var confidence = 0.9f;

                        allText.AppendLine(text);
                        lines.Add(new OcrLine
                        {
                            Text = text,
                            Confidence = confidence,
                            LineNumber = ++lineNumber
                        });
                    }
                }
            }

            return new OcrResult
            {
                Success = true,
                Text = allText.ToString().Trim(),
                Confidence = 0.9f,
                Language = language,
                Lines = lines
            };
        }
        catch (Exception ex)
        {
            return new OcrResult
            {
                Success = false,
                ErrorMessage = $"Azure Computer Vision error: {ex.Message}",
                Language = language
            };
        }
    }

    public async Task<OcrResult> ExtractTextFromPdfAsync(
        string pdfPath,
        string language = "ita",
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        // Azure Computer Vision can handle PDFs directly with Read API
        // TODO: Implement PDF support
        return new OcrResult
        {
            Success = false,
            ErrorMessage = "PDF OCR via Azure not yet implemented. Convert to images first.",
            Language = language
        };
    }

    public async Task<bool> IsAvailableAsync()
    {
        if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_endpoint))
        {
            return false;
        }

        try
        {
            // Test connection with a minimal request
            var testUrl = $"{_endpoint.TrimEnd('/')}/vision/v3.2/";
            var response = await _httpClient.GetAsync(testUrl);
            return response.StatusCode != System.Net.HttpStatusCode.Unauthorized;
        }
        catch
        {
            return false;
        }
    }

    private string MapLanguage(string language)
    {
        // Map language codes to Azure supported codes
        return language.ToLower() switch
        {
            "ita" or "it" => "it",
            "eng" or "en" => "en",
            "fra" or "fr" => "fr",
            "deu" or "de" => "de",
            "spa" or "es" => "es",
            _ => "en"
        };
    }
}
