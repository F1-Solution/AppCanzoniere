using AppCanzoniere.Application.DTOs;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppCanzoniere.MAUI.ViewModels;

public partial class OcrImportViewModel : ObservableObject
{
    private readonly IOcrService _ocrService;
    private readonly ISongRepository _songRepository;
    private readonly IChordProParser _chordProParser;

    [ObservableProperty]
    private string _serviceName = string.Empty;

    [ObservableProperty]
    private bool _isServiceAvailable;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string _selectedFilePath = string.Empty;

    [ObservableProperty]
    private string _extractedText = string.Empty;

    [ObservableProperty]
    private float _confidence;

    [ObservableProperty]
    private string _selectedLanguage = "ita";

    [ObservableProperty]
    private bool _hasResult;

    [ObservableProperty]
    private string? _errorMessage;

    public List<string> AvailableLanguages { get; } = new()
    {
        "ita", "eng", "fra", "deu", "spa"
    };

    public OcrImportViewModel(
        IOcrService ocrService,
        ISongRepository songRepository,
        IChordProParser chordProParser)
    {
        _ocrService = ocrService;
        _songRepository = songRepository;
        _chordProParser = chordProParser;
        
        ServiceName = _ocrService.ServiceName;
    }

    public async Task InitializeAsync()
    {
        IsServiceAvailable = await _ocrService.IsAvailableAsync();
    }

    [RelayCommand]
    private async Task SelectFileAsync()
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Select image or PDF to import",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "image/*", "application/pdf" } },
                    { DevicePlatform.iOS, new[] { "public.image", "com.adobe.pdf" } },
                    { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".pdf" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.image", "com.adobe.pdf" } }
                })
            };

            var result = await FilePicker.PickAsync(options);
            
            if (result != null)
            {
                SelectedFilePath = result.FullPath;
                
                // Auto-process after selection
                await ProcessFileAsync();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to select file: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ProcessFileAsync()
    {
        if (string.IsNullOrEmpty(SelectedFilePath))
        {
            await Shell.Current.DisplayAlert("Error", "Please select a file first", "OK");
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
            OcrResult ocrResult;

            if (SelectedFilePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                ocrResult = await _ocrService.ExtractTextFromPdfAsync(
                    SelectedFilePath, 
                    SelectedLanguage
                );
            }
            else
            {
                ocrResult = await _ocrService.ExtractTextFromImageAsync(
                    SelectedFilePath, 
                    SelectedLanguage
                );
            }

            if (ocrResult.Success)
            {
                ExtractedText = ocrResult.Text;
                Confidence = ocrResult.Confidence;
                HasResult = true;
                ErrorMessage = null;
            }
            else
            {
                ErrorMessage = ocrResult.ErrorMessage;
                HasResult = false;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"OCR processing failed: {ex.Message}";
            HasResult = false;
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private async Task CreateSongAsync()
    {
        if (!HasResult || string.IsNullOrEmpty(ExtractedText))
        {
            await Shell.Current.DisplayAlert("Error", "No extracted text available", "OK");
            return;
        }

        try
        {
            // Parse extracted text to get metadata
            var parsedData = _chordProParser.Parse(ExtractedText);

            var song = new Song
            {
                Title = parsedData.Title ?? "Untitled (OCR Import)",
                Artist = parsedData.Artist ?? "Unknown",
                OriginalKey = parsedData.Key,
                ChordProContent = ExtractedText,
                PrimaryFormat = ContentFormat.ChordPro,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _songRepository.AddAsync(song);

            await Shell.Current.DisplayAlert(
                "Success", 
                $"Song '{song.Title}' created successfully from OCR", 
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
        ExtractedText = string.Empty;
        Confidence = 0f;
        HasResult = false;
        ErrorMessage = null;
    }
}
