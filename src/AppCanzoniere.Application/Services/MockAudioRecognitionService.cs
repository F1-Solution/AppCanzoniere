using AppCanzoniere.Application.DTOs;
using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Services;

public class MockAudioRecognitionService : IAudioRecognitionService
{
    public string ServiceName => "Mock Audio Recognition (Demo)";

    public Task<AudioRecognitionResult> RecognizeFromFileAsync(
        string audioFilePath,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetMockResult());
    }

    public Task<AudioRecognitionResult> RecognizeFromBytesAsync(
        byte[] audioBytes,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetMockResult());
    }

    public Task<AudioRecognitionResult> RecognizeFromStreamAsync(
        Stream audioStream,
        int durationSeconds = 10,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetMockResult());
    }

    public Task<bool> IsAvailableAsync()
    {
        return Task.FromResult(true);
    }

    private AudioRecognitionResult GetMockResult()
    {
        // Return sample recognition result
        var mockResults = new[]
        {
            new AudioRecognitionResult
            {
                Success = true,
                Title = "Amazing Grace",
                Artist = "Various Artists",
                Album = "Traditional Hymns",
                ReleaseDate = "1772",
                Score = 95.5f,
                DurationMs = 245000,
                Genres = new List<string> { "Hymn", "Traditional", "Religious" },
                ServiceName = ServiceName,
                SpotifyId = "mock-spotify-id-1",
                YouTubeId = "mock-youtube-id-1"
            },
            new AudioRecognitionResult
            {
                Success = true,
                Title = "How Great Thou Art",
                Artist = "Carl Boberg",
                Album = "Christian Classics",
                ReleaseDate = "1885",
                Score = 92.3f,
                DurationMs = 268000,
                Genres = new List<string> { "Hymn", "Gospel" },
                ServiceName = ServiceName,
                SpotifyId = "mock-spotify-id-2",
                YouTubeId = "mock-youtube-id-2"
            },
            new AudioRecognitionResult
            {
                Success = true,
                Title = "Blessed Assurance",
                Artist = "Fanny Crosby",
                Album = "Gospel Favorites",
                ReleaseDate = "1873",
                Score = 88.7f,
                DurationMs = 192000,
                Genres = new List<string> { "Gospel", "Traditional" },
                ServiceName = ServiceName,
                SpotifyId = "mock-spotify-id-3",
                YouTubeId = "mock-youtube-id-3"
            }
        };

        // Return random mock result
        var random = new Random();
        return mockResults[random.Next(mockResults.Length)];
    }
}
