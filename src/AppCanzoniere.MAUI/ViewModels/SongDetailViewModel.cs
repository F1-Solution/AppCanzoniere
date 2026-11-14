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
    private readonly ISongDisplayService _songDisplay;
    private readonly IChordDisplaySettingsStore _displaySettings;
    private string _errorMessage = string.Empty;
    private string _capoText = string.Empty;

    [ObservableProperty]
    private Song? _song;

    [ObservableProperty]
    private string _displayedContent = string.Empty;

    [ObservableProperty]
    private int _currentTranspose;

    [ObservableProperty]
    private string _currentKey = string.Empty;

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
        ISongDisplayService songDisplay,
        IChordDisplaySettingsStore displaySettings)
    {
        _songRepository = songRepository;
        _songDisplay = songDisplay;
        _displaySettings = displaySettings;
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

    // First display and every transpose step both come through here.
    private void UpdateDisplayedContent()
    {
        if (Song == null) return;

        var chordPro = Song.ChordProContent;
        if (string.IsNullOrEmpty(chordPro))
        {
            CurrentKey = Song.OriginalKey;
            CapoText = string.Empty;
            ErrorMessage = string.Empty;
            DisplayedContent = Song.MusicXmlContent ?? Song.JsonContent ?? "No content available";
            return;
        }

        try
        {
            var request = new SongDisplayRequest(
                chordPro, CurrentTranspose, _displaySettings.Load(), CultureInfo.CurrentUICulture.Name, Song.OriginalKey);
            var shown = _songDisplay.Render(request);
            DisplayedContent = shown.Text;
            CurrentKey = string.IsNullOrEmpty(shown.Key) ? Song.OriginalKey : shown.Key;
            CapoText = shown.Capo > 0 ? $"Capo {shown.Capo}" : string.Empty;
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Song display failed: {ex}");
            DisplayedContent = chordPro;
            CurrentKey = Song.OriginalKey;
            CapoText = string.Empty;
            ErrorMessage = "The chords could not be laid out, so the song is shown as stored.";
        }
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
