using AppCanzoniere.Application.DTOs;

namespace AppCanzoniere.Application.Interfaces;

public interface IAudioRecognitionService
{
    /// <summary>
    /// Recognize a track from audio file
    /// </summary>
    Task<AudioRecognitionResult> RecognizeFromFileAsync(
        string audioFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recognize a track from audio bytes
    /// </summary>
    Task<AudioRecognitionResult> RecognizeFromBytesAsync(
        byte[] audioBytes,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recognize a track from audio stream (for recording)
    /// </summary>
    Task<AudioRecognitionResult> RecognizeFromStreamAsync(
        Stream audioStream,
        int durationSeconds = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the service is available and configured
    /// </summary>
    Task<bool> IsAvailableAsync();

    /// <summary>
    /// Get the service name
    /// </summary>
    string ServiceName { get; }
}
