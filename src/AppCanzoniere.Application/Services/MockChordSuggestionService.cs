using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Services;

/// <summary>
/// Mock implementation for testing without API key
/// </summary>
public class MockChordSuggestionService : IChordSuggestionService
{
    public Task<ChordSuggestionResult> GetChordSuggestionsAsync(
        string title, 
        string artist, 
        CancellationToken cancellationToken = default)
    {
        return GetChordSuggestionsWithContextAsync(title, artist, null, null, cancellationToken);
    }

    public Task<ChordSuggestionResult> GetChordSuggestionsWithContextAsync(
        string title, 
        string artist, 
        string? lyrics = null, 
        string? key = null, 
        CancellationToken cancellationToken = default)
    {
        // Generate mock ChordPro content
        var mockKey = key ?? "G";
        var chordPro = $@"{{title:{title}}}
{{artist:{artist}}}
{{key:{mockKey}}}

[{mockKey}]Verse line [D]one
[Em]Verse line [C]two

{{start_of_chorus}}
[{mockKey}]Chorus [D]line
[Em]Chorus [C]line
{{end_of_chorus}}

[{mockKey}]Bridge [D]section
[Em]Final [C]line";

        var result = new ChordSuggestionResult
        {
            Success = true,
            ChordProContent = chordPro,
            OriginalKey = mockKey,
            Source = "Mock Service (Demo Mode)",
            RequestedAt = DateTime.UtcNow,
            DisclaimerText = "DEMO MODE: This is mock data for demonstration purposes only. " +
                           "Configure a real AI service API key to get actual chord suggestions."
        };

        return Task.FromResult(result);
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}
