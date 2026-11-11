using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AppCanzoniere.Application.DTOs;
using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Services;

public class ACRCloudAudioRecognitionService : IAudioRecognitionService
{
    private readonly HttpClient _httpClient;
    private readonly string? _accessKey;
    private readonly string? _accessSecret;
    private readonly string? _host;

    public string ServiceName => "ACRCloud Audio Recognition";

    public ACRCloudAudioRecognitionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _accessKey = Environment.GetEnvironmentVariable("ACRCLOUD_ACCESS_KEY");
        _accessSecret = Environment.GetEnvironmentVariable("ACRCLOUD_ACCESS_SECRET");
        _host = Environment.GetEnvironmentVariable("ACRCLOUD_HOST") ?? "identify-eu-west-1.acrcloud.com";
    }

    public async Task<AudioRecognitionResult> RecognizeFromFileAsync(
        string audioFilePath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(audioFilePath))
        {
            return new AudioRecognitionResult
            {
                Success = false,
                ErrorMessage = $"Audio file not found: {audioFilePath}",
                ServiceName = ServiceName
            };
        }

        var audioBytes = await File.ReadAllBytesAsync(audioFilePath, cancellationToken);
        return await RecognizeFromBytesAsync(audioBytes, cancellationToken);
    }

    public async Task<AudioRecognitionResult> RecognizeFromBytesAsync(
        byte[] audioBytes,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_accessKey) || string.IsNullOrEmpty(_accessSecret))
        {
            return new AudioRecognitionResult
            {
                Success = false,
                ErrorMessage = "ACRCloud not configured. Set ACRCLOUD_ACCESS_KEY and ACRCLOUD_ACCESS_SECRET environment variables.",
                ServiceName = ServiceName
            };
        }

        try
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var signature = GenerateSignature(timestamp);

            var url = $"https://{_host}/v1/identify";

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(_accessKey), "access_key");
            content.Add(new StringContent(timestamp), "timestamp");
            content.Add(new StringContent(signature), "signature");
            content.Add(new StringContent("audio"), "data_type");
            content.Add(new StringContent("fingerprint"), "signature_version");

            // Add audio sample
            var audioContent = new ByteArrayContent(audioBytes);
            content.Add(audioContent, "sample", "audio.wav");

            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new AudioRecognitionResult
                {
                    Success = false,
                    ErrorMessage = $"ACRCloud API error: {response.StatusCode} - {responseContent}",
                    ServiceName = ServiceName
                };
            }

            return ParseACRCloudResponse(responseContent);
        }
        catch (Exception ex)
        {
            return new AudioRecognitionResult
            {
                Success = false,
                ErrorMessage = $"ACRCloud recognition error: {ex.Message}",
                ServiceName = ServiceName
            };
        }
    }

    public async Task<AudioRecognitionResult> RecognizeFromStreamAsync(
        Stream audioStream,
        int durationSeconds = 10,
        CancellationToken cancellationToken = default)
    {
        // Read stream to bytes (limit to durationSeconds * sample rate)
        using var memoryStream = new MemoryStream();
        await audioStream.CopyToAsync(memoryStream, cancellationToken);
        var audioBytes = memoryStream.ToArray();

        return await RecognizeFromBytesAsync(audioBytes, cancellationToken);
    }

    public async Task<bool> IsAvailableAsync()
    {
        if (string.IsNullOrEmpty(_accessKey) || string.IsNullOrEmpty(_accessSecret))
        {
            return false;
        }

        try
        {
            // Test with minimal request
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var signature = GenerateSignature(timestamp);
            var url = $"https://{_host}/v1/identify";

            // Don't actually call, just verify configuration
            return !string.IsNullOrEmpty(_host);
        }
        catch
        {
            return false;
        }
    }

    private string GenerateSignature(string timestamp)
    {
        // ACRCloud signature: HMAC-SHA1 of HTTP method + URI path + access_key + data_type + signature_version + timestamp
        var stringToSign = $"POST\n/v1/identify\n{_accessKey}\naudio\nv1\n{timestamp}";

        using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_accessSecret!));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        return Convert.ToBase64String(hash);
    }

    private AudioRecognitionResult ParseACRCloudResponse(string jsonResponse)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;

            var status = root.GetProperty("status");
            var statusCode = status.GetProperty("code").GetInt32();

            if (statusCode != 0)
            {
                var statusMsg = status.GetProperty("msg").GetString();
                return new AudioRecognitionResult
                {
                    Success = false,
                    ErrorMessage = $"ACRCloud status {statusCode}: {statusMsg}",
                    ServiceName = ServiceName
                };
            }

            var metadata = root.GetProperty("metadata");
            var music = metadata.GetProperty("music")[0];

            var result = new AudioRecognitionResult
            {
                Success = true,
                ServiceName = ServiceName,
                Title = music.TryGetProperty("title", out var title) ? title.GetString() : null,
                DurationMs = music.TryGetProperty("duration_ms", out var duration) ? duration.GetInt32() : 0,
                Score = music.TryGetProperty("score", out var score) ? score.GetSingle() : 0f
            };

            // Artists
            if (music.TryGetProperty("artists", out var artists) && artists.GetArrayLength() > 0)
            {
                var artist = artists[0];
                result.Artist = artist.TryGetProperty("name", out var artistName) ? artistName.GetString() : null;
            }

            // Album
            if (music.TryGetProperty("album", out var album))
            {
                result.Album = album.TryGetProperty("name", out var albumName) ? albumName.GetString() : null;
            }

            // Release date
            if (music.TryGetProperty("release_date", out var releaseDate))
            {
                result.ReleaseDate = releaseDate.GetString();
            }

            // External IDs
            if (music.TryGetProperty("external_metadata", out var externalMetadata))
            {
                if (externalMetadata.TryGetProperty("spotify", out var spotify) && spotify.GetArrayLength() > 0)
                {
                    var spotifyTrack = spotify[0];
                    result.SpotifyId = spotifyTrack.TryGetProperty("track", out var track)
                        ? track.TryGetProperty("id", out var id) ? id.GetString() : null
                        : null;
                }

                if (externalMetadata.TryGetProperty("youtube", out var youtube) && youtube.GetArrayLength() > 0)
                {
                    var youtubeVideo = youtube[0];
                    result.YouTubeId = youtubeVideo.TryGetProperty("vid", out var vid) ? vid.GetString() : null;
                }
            }

            // Genres
            if (music.TryGetProperty("genres", out var genres))
            {
                foreach (var genre in genres.EnumerateArray())
                {
                    if (genre.TryGetProperty("name", out var genreName))
                    {
                        var name = genreName.GetString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            result.Genres.Add(name);
                        }
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            return new AudioRecognitionResult
            {
                Success = false,
                ErrorMessage = $"Failed to parse ACRCloud response: {ex.Message}",
                ServiceName = ServiceName
            };
        }
    }
}
