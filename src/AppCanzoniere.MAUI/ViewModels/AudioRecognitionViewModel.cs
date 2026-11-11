using AppCanzoniere.Application.DTOs;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppCanzoniere.MAUI.ViewModels;

public partial class AudioRecognitionViewModel : ObservableObject
{
    private readonly IAudioRecognitionService _audioRecognitionService;
    private readonly ISongRepository _songRepository;

    [ObservableProperty]
    private string _serviceName = string.Empty;

    [ObservableProperty]
    private bool _isServiceAvailable;

    [ObservableProperty]
    private bool _isRecording;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private int _recordingDuration = 10;

    [ObservableProperty]
    private string _selectedFilePath = string.Empty;

    [ObservableProperty]
    private AudioRecognitionResult? _recognitionResult;

    [ObservableProperty]
    private bool _hasResult;

    [ObservableProperty]
    private string? _errorMessage;

    public List<int> AvailableDurations { get; } = new() { 5, 10, 15, 20 };

    public AudioRecognitionViewModel(
        IAudioRecognitionService audioRecognitionService,
        ISongRepository songRepository)
    {
        _audioRecognitionService = audioRecognitionService;
        _songRepository = songRepository;

        ServiceName = _audioRecognitionService.ServiceName;
    }

    public async Task InitializeAsync()
    {
        IsServiceAvailable = await _audioRecognitionService.IsAvailableAsync();
    }

    [RelayCommand]
    private async Task SelectAudioFileAsync()
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Select audio file to identify",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "audio/*" } },
                    { DevicePlatform.iOS, new[] { "public.audio" } },
                    { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".m4a", ".flac" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.audio" } }
                })
            };

            var result = await FilePicker.PickAsync(options);

            if (result != null)
            {
                SelectedFilePath = result.FullPath;

                // Auto-process after selection
                await RecognizeFromFileAsync();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to select file: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task RecognizeFromFileAsync()
    {
        if (string.IsNullOrEmpty(SelectedFilePath))
        {
            await Shell.Current.DisplayAlert("Error", "Please select an audio file first", "OK");
            return;
        }

        if (!IsServiceAvailable)
        {
            await Shell.Current.DisplayAlert(
                "Service Unavailable",
                $"{ServiceName} is not configured. Check SETUP_INSTRUCTIONS.md for configuration steps.",
                "OK"
            );
            return;
        }

        IsProcessing = true;
        ErrorMessage = null;
        HasResult = false;

        try
        {
            var result = await _audioRecognitionService.RecognizeFromFileAsync(SelectedFilePath);

            if (result.Success)
            {
                RecognitionResult = result;
                HasResult = true;
                ErrorMessage = null;

                await Shell.Current.DisplayAlert(
                    "Track Identified!",
                    $"Found: {result.Title} by {result.Artist}\nScore: {result.Score:F1}%",
                    "OK"
                );
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
                HasResult = false;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Recognition failed: {ex.Message}";
            HasResult = false;
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private async Task StartRecordingAsync()
    {
        if (!IsServiceAvailable)
        {
            await Shell.Current.DisplayAlert(
                "Service Unavailable",
                $"{ServiceName} is not configured.",
                "OK"
            );
            return;
        }

        // TODO: Implement actual audio recording from microphone
        // This requires platform-specific permissions and audio capture
        await Shell.Current.DisplayAlert(
            "Not Implemented",
            "Audio recording from microphone is not yet implemented. Please use the file selection option instead.",
            "OK"
        );

        /*
        Future implementation:
        
        IsRecording = true;
        ErrorMessage = null;

        try
        {
            // Request microphone permissions
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("Permission Denied", "Microphone access is required", "OK");
                return;
            }

            // Start recording using platform-specific audio capture
            // Record for RecordingDuration seconds
            // Convert to appropriate format (WAV, 16kHz, mono recommended)
            
            using var audioStream = new MemoryStream();
            // ... record audio to stream ...
            
            IsRecording = false;
            IsProcessing = true;
            
            var result = await _audioRecognitionService.RecognizeFromStreamAsync(
                audioStream, 
                RecordingDuration
            );
            
            if (result.Success)
            {
                RecognitionResult = result;
                HasResult = true;
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Recording failed: {ex.Message}";
        }
        finally
        {
            IsRecording = false;
            IsProcessing = false;
        }
        */
    }

    [RelayCommand]
    private async Task CreateSongFromResultAsync()
    {
        if (RecognitionResult == null || !HasResult)
        {
            await Shell.Current.DisplayAlert("Error", "No recognition result available", "OK");
            return;
        }

        try
        {
            // Check if song already exists
            var existingSongs = await _songRepository.SearchAsync(RecognitionResult.Title ?? "");
            var duplicate = existingSongs.FirstOrDefault(s =>
                s.Title == RecognitionResult.Title && s.Artist == RecognitionResult.Artist);

            if (duplicate != null)
            {
                var overwrite = await Shell.Current.DisplayAlert(
                    "Song Exists",
                    $"A song with title '{RecognitionResult.Title}' by '{RecognitionResult.Artist}' already exists. Open existing song?",
                    "Yes",
                    "No"
                );

                if (overwrite)
                {
                    await Shell.Current.GoToAsync("songdetail", new Dictionary<string, object>
                    {
                        { "Song", duplicate }
                    });
                }
                return;
            }

            var song = new Song
            {
                Title = RecognitionResult.Title ?? "Unknown Title",
                Artist = RecognitionResult.Artist ?? "Unknown Artist",
                PrimaryFormat = ContentFormat.ChordPro,
                ChordProContent = $"{{title: {RecognitionResult.Title}}}\n{{artist: {RecognitionResult.Artist}}}\n\n",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add YouTube link if available
            if (!string.IsNullOrEmpty(RecognitionResult.YouTubeId))
            {
                song.YouTubeLinks.Add(new Domain.Entities.YouTubeLink
                {
                    VideoId = RecognitionResult.YouTubeId,
                    Title = "Official Video",
                    AddedAt = DateTime.UtcNow
                });
            }

            await _songRepository.AddAsync(song);

            await Shell.Current.DisplayAlert(
                "Success",
                $"Song '{song.Title}' created from audio recognition",
                "OK"
            );

            // Navigate to song detail
            await Shell.Current.GoToAsync("songdetail", new Dictionary<string, object>
            {
                { "Song", song }
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to create song: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private void ClearResults()
    {
        SelectedFilePath = string.Empty;
        RecognitionResult = null;
        HasResult = false;
        ErrorMessage = null;
    }
}
