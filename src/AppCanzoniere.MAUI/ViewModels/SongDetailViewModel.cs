using System.Diagnostics;
using System.Globalization;
using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppCanzoniere.MAUI.ViewModels;

[QueryProperty(nameof(Song), "Song")]
public partial class SongDetailViewModel : ObservableObject
{
    private readonly ISongRepository _songRepository;
    private readonly SongScreenPresenter _presenter;
    private readonly IChordDisplaySettingsStore _displaySettings;
    private readonly IUserSettingsRepository _userSettings;
    private string _errorMessage = string.Empty;
    private string _capoText = string.Empty;
    private string? _appLanguage;

    [ObservableProperty]
    private Song? _song;

    [ObservableProperty]
    private string _displayedContent = string.Empty;

    /// <summary>The song text one string per row, so no label ever has to wrap or truncate.</summary>
    [ObservableProperty]
    private IReadOnlyList<string> _displayedLines = Array.Empty<string>();

    [ObservableProperty]
    private int _currentTranspose;

    private string _currentKey = string.Empty;

    /// <summary>The sounding key; empty when the song file has none.</summary>
    public string CurrentKey
    {
        get => _currentKey;
        private set
        {
            if (SetProperty(ref _currentKey, value))
                OnPropertyChanged(nameof(HasKey));
        }
    }

    public bool HasKey => _currentKey.Length > 0;

    [ObservableProperty]
    private bool _isEditMode;

    /// <summary>Why the chords could not be shown; empty when the song rendered.</summary>
    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
                OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => _errorMessage.Length > 0;

    /// <summary>"Capo 2" when the song is played with a capo; empty otherwise.</summary>
    public string CapoText
    {
        get => _capoText;
        private set
        {
            if (SetProperty(ref _capoText, value))
                OnPropertyChanged(nameof(HasCapo));
        }
    }

    public bool HasCapo => _capoText.Length > 0;

    public SongDetailViewModel(
        ISongRepository songRepository,
        SongScreenPresenter presenter,
        IChordDisplaySettingsStore displaySettings,
        IUserSettingsRepository userSettings)
    {
        _songRepository = songRepository;
        _presenter = presenter;
        _displaySettings = displaySettings;
        _userSettings = userSettings;
    }

    partial void OnSongChanged(Song? value)
    {
        if (value == null) return;

        CurrentTranspose = 0;
        UpdateDisplayedContent();
    }

    [RelayCommand]
    private void InitializeNewSong()
    {
        if (Song == null)
        {
            Song = new Song();
            IsEditMode = true;
        }
    }

    [RelayCommand]
    private void TransposeUp()
    {
        CurrentTranspose++;
        UpdateDisplayedContent();
    }

    [RelayCommand]
    private void TransposeDown()
    {
        CurrentTranspose--;
        UpdateDisplayedContent();
    }

    [RelayCommand]
    private void ResetTranspose()
    {
        CurrentTranspose = 0;
        UpdateDisplayedContent();
    }

    /// <summary>Shows the song again with the current settings and language, keeping the transposition.</summary>
    public async Task RefreshDisplayAsync()
    {
        try
        {
            var stored = await _userSettings.GetAsync();
            _appLanguage = stored?.PreferredLanguage;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Reading the app language failed: {ex}");
        }

        UpdateDisplayedContent();
    }

    // First display, every transpose step and every refresh come through here.
    private void UpdateDisplayedContent()
    {
        if (Song == null) return;

        var screen = _presenter.Present(new SongScreenRequest(
            Song.ChordProContent,
            Song.MusicXmlContent ?? Song.JsonContent,
            CurrentTranspose,
            _displaySettings.Load(),
            _appLanguage,
            CultureInfo.CurrentUICulture.Name));
        DisplayedContent = screen.Text;
        DisplayedLines = screen.Text.Split('\n').Select(l => l.Length == 0 ? " " : l).ToArray();
        CurrentKey = screen.Key;
        CapoText = screen.Capo;
        ErrorMessage = screen.Error;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Song == null) return;

        try
        {
            if (Song.Id == Guid.Empty)
            {
                await _songRepository.AddAsync(Song);
            }
            else
            {
                await _songRepository.UpdateAsync(Song);
            }

            await Shell.Current.DisplayAlertAsync("Success", "Song saved successfully", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to save song: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Song == null || Song.Id == Guid.Empty) return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Confirm Delete",
            $"Are you sure you want to delete '{Song.Title}'?",
            "Yes",
            "No");

        if (!confirm) return;

        try
        {
            await _songRepository.DeleteAsync(Song.Id);
            await Shell.Current.DisplayAlertAsync("Success", "Song deleted successfully", "OK");
            await Shell.Current.GoToAsync("../..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete song: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private void ToggleEditMode()
    {
        IsEditMode = !IsEditMode;
    }

    [RelayCommand]
    private async Task NavigateToMetadataAsync()
    {
        if (Song == null || Song.Id == Guid.Empty) return;

        await Shell.Current.GoToAsync("songmetadata", new Dictionary<string, object>
        {
            { "SongId", Song.Id }
        });
    }

    [RelayCommand]
    private async Task NavigateToAiSuggestionsAsync()
    {
        if (Song == null || Song.Id == Guid.Empty) return;

        await Shell.Current.GoToAsync("aichords", new Dictionary<string, object>
        {
            { "SongId", Song.Id }
        });
    }
}
