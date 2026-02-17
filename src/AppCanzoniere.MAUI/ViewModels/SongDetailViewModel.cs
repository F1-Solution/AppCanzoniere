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
    private readonly IChordProParser _chordProParser;
    private readonly IChordTransposer _chordTransposer;

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

    public SongDetailViewModel(
        ISongRepository songRepository,
        IChordProParser chordProParser,
        IChordTransposer chordTransposer)
    {
        _songRepository = songRepository;
        _chordProParser = chordProParser;
        _chordTransposer = chordTransposer;
    }

    partial void OnSongChanged(Song? value)
    {
        if (value != null)
        {
            CurrentKey = value.OriginalKey;
            DisplayedContent = value.ChordProContent ?? value.MusicXmlContent ?? value.JsonContent ?? "No content available";
            CurrentTranspose = 0;
        }
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

    private void UpdateDisplayedContent()
    {
        if (Song == null) return;

        // Update current key with transposition
        if (!string.IsNullOrEmpty(Song.OriginalKey))
        {
            CurrentKey = _chordTransposer.TransposeKey(Song.OriginalKey, CurrentTranspose);
        }

        // Transpose ChordPro content if available
        if (!string.IsNullOrEmpty(Song.ChordProContent))
        {
            var document = _chordProParser.Parse(Song.ChordProContent);
            DisplayedContent = _chordProParser.Render(document, CurrentTranspose);
        }
        else
        {
            DisplayedContent = Song.MusicXmlContent ?? Song.JsonContent ?? "No content available";
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
