using AppCanzoniere.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppCanzoniere.Application.Services;

/// <summary>
/// Implementation of IChordSuggestionService using Perplexity API
/// </summary>
public class PerplexityChordSuggestionService : IChordSuggestionService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private const string ApiUrl = "https://api.perplexity.ai/chat/completions";

    public PerplexityChordSuggestionService(HttpClient httpClient, string? apiKey = null)
    {
        _httpClient = httpClient;
        _apiKey = apiKey ?? Environment.GetEnvironmentVariable("PERPLEXITY_API_KEY");
    }

    public async Task<ChordSuggestionResult> GetChordSuggestionsAsync(
        string title, 
        string artist, 
        CancellationToken cancellationToken = default)
    {
        return await GetChordSuggestionsWithContextAsync(title, artist, null, null, cancellationToken);
    }

    public async Task<ChordSuggestionResult> GetChordSuggestionsWithContextAsync(
        string title, 
        string artist, 
        string? lyrics = null, 
        string? key = null, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return new ChordSuggestionResult
            {
                Success = false,
                ErrorMessage = "API key not configured. Please set PERPLEXITY_API_KEY environment variable or provide key in settings.",
                Source = "Perplexity AI"
            };
        }

        try
        {
            var prompt = BuildPrompt(title, artist, lyrics, key);

            var request = new
            {
                model = "llama-3.1-sonar-small-128k-online",
                messages = new[]
                {
                    new { role = "system", content = "You are a music expert that provides chord progressions in ChordPro format." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.2,
                max_tokens = 1000
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsJsonAsync(ApiUrl, request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return new ChordSuggestionResult
                {
                    Success = false,
                    ErrorMessage = $"API error: {response.StatusCode} - {errorContent}",
                    Source = "Perplexity AI"
                };
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken);
            var content = jsonResponse?.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrEmpty(content))
            {
                return new ChordSuggestionResult
                {
                    Success = false,
                    ErrorMessage = "No content received from AI service",
                    Source = "Perplexity AI"
                };
            }

            // Extract ChordPro content and key
            var (chordPro, detectedKey) = ParseAIResponse(content);

            return new ChordSuggestionResult
            {
                Success = true,
                ChordProContent = chordPro,
                OriginalKey = detectedKey ?? key,
                Source = "Perplexity AI",
                RequestedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new ChordSuggestionResult
            {
                Success = false,
                ErrorMessage = $"Error calling AI service: {ex.Message}",
                Source = "Perplexity AI"
            };
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return false;

        try
        {
            // Simple ping to check if API is reachable
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            
            var testRequest = new
            {
                model = "llama-3.1-sonar-small-128k-online",
                messages = new[] { new { role = "user", content = "test" } },
                max_tokens = 1
            };

            var response = await _httpClient.PostAsJsonAsync(ApiUrl, testRequest, cancellationToken);
            return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.TooManyRequests;
        }
        catch
        {
            return false;
        }
    }

    private string BuildPrompt(string title, string artist, string? lyrics, string? key)
    {
        var promptBuilder = new System.Text.StringBuilder();
        promptBuilder.AppendLine($"Provide the chord progression for the song \"{title}\" by {artist}.");
        
        if (!string.IsNullOrEmpty(key))
        {
            promptBuilder.AppendLine($"The song is in the key of {key}.");
        }

        promptBuilder.AppendLine();
        promptBuilder.AppendLine("Format your response in ChordPro format with:");
        promptBuilder.AppendLine("- {title:Song Title}");
        promptBuilder.AppendLine("- {artist:Artist Name}");
        promptBuilder.AppendLine("- {key:Key}");
        promptBuilder.AppendLine("- Chords above lyrics in [ChordName] format");
        promptBuilder.AppendLine("- Use {start_of_chorus} and {end_of_chorus} for chorus sections");
        promptBuilder.AppendLine();

        if (!string.IsNullOrEmpty(lyrics))
        {
            promptBuilder.AppendLine("Lyrics reference:");
            promptBuilder.AppendLine(lyrics.Length > 500 ? lyrics.Substring(0, 500) + "..." : lyrics);
        }
        else
        {
            promptBuilder.AppendLine("Include the main sections: verse, chorus, bridge (if applicable).");
        }

        promptBuilder.AppendLine();
        promptBuilder.AppendLine("Provide ONLY the ChordPro formatted content, nothing else.");

        return promptBuilder.ToString();
    }

    private (string chordPro, string? key) ParseAIResponse(string response)
    {
        // Clean up response - remove markdown code blocks if present
        var cleaned = response.Trim();
        if (cleaned.StartsWith("```"))
        {
            var lines = cleaned.Split('\n');
            cleaned = string.Join('\n', lines.Skip(1).SkipLast(1));
        }

        // Try to extract key from {key:...} directive
        string? key = null;
        var keyMatch = System.Text.RegularExpressions.Regex.Match(cleaned, @"\{key:([^}]+)\}");
        if (keyMatch.Success)
        {
            key = keyMatch.Groups[1].Value.Trim();
        }

        return (cleaned.Trim(), key);
    }
}
