using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppCanzoniere.MAUI.ViewModels;

[QueryProperty(nameof(SongId), "SongId")]
public partial class AiChordSuggestionViewModel : ObservableObject
{
    private readonly IChordSuggestionService _chordSuggestionService;
    private readonly ISongRepository _songRepository;

    [ObservableProperty]
    private Guid _songId;

    [ObservableProperty]
    private Song? _song;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _artist = string.Empty;

    [ObservableProperty]
    private string _key = string.Empty;

    [ObservableProperty]
    private string _lyrics = string.Empty;

    [ObservableProperty]
    private string _suggestedChordPro = string.Empty;

    [ObservableProperty]
    private string _suggestedKey = string.Empty;

    [ObservableProperty]
    private string _source = string.Empty;

    [ObservableProperty]
    private string _disclaimerText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasSuggestion;

    [ObservableProperty]
    private bool _showDisclaimer;

    [ObservableProperty]
    private bool _disclaimerAccepted;

    [ObservableProperty]
    private bool _serviceAvailable;

    public AiChordSuggestionViewModel(
        IChordSuggestionService chordSuggestionService,
        ISongRepository songRepository)
    {
        _chordSuggestionService = chordSuggestionService;
        _songRepository = songRepository;
    }

    partial void OnSongIdChanged(Guid value)
    {
        if (value != Guid.Empty)
        {
            LoadSongAsync().ConfigureAwait(false);
        }
    }

    [RelayCommand]
    private async Task LoadSongAsync()
    {
        if (SongId == Guid.Empty) return;

        Song = await _songRepository.GetByIdAsync(SongId);
        if (Song != null)
        {
            Title = Song.Title;
            Artist = Song.Artist ?? string.Empty;
            Key = Song.OriginalKey ?? string.Empty;
            Lyrics = ExtractLyricsFromChordPro(Song.ChordProContent ?? string.Empty);
        }
    }

    [RelayCommand]
    private async Task CheckServiceAvailabilityAsync()
    {
        ServiceAvailable = await _chordSuggestionService.IsAvailableAsync();
    }

    [RelayCommand]
    private async Task RequestSuggestionAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            await Shell.Current.DisplayAlert("Validation", "Song title is required", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Artist))
        {
            await Shell.Current.DisplayAlert("Validation", "Artist name is required", "OK");
            return;
        }

        IsLoading = true;
        HasSuggestion = false;
        ShowDisclaimer = false;

        try
        {
            var result = await _chordSuggestionService.GetChordSuggestionsWithContextAsync(
                Title,
                Artist,
                Lyrics,
                Key
            );

            if (result.Success)
            {
                SuggestedChordPro = result.ChordProContent ?? string.Empty;
                SuggestedKey = result.OriginalKey ?? string.Empty;
                Source = result.Source ?? "AI Service";
                DisclaimerText = result.DisclaimerText;
                HasSuggestion = true;
                ShowDisclaimer = true;
                DisclaimerAccepted = false;

                await Shell.Current.DisplayAlert(
                    "Success",
                    $"Chord suggestions received from {Source}",
                    "OK"
                );
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    result.ErrorMessage ?? "Failed to get suggestions",
                    "OK"
                );
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"An error occurred: {ex.Message}",
                "OK"
            );
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ApplySuggestionAsync()
    {
        if (!DisclaimerAccepted)
        {
            await Shell.Current.DisplayAlert(
                "Disclaimer Required",
                "Please read and accept the disclaimer before applying suggestions.",
                "OK"
            );
            return;
        }

        if (Song == null)
        {
            await Shell.Current.DisplayAlert("Error", "No song loaded", "OK");
            return;
        }

        bool confirm = await Shell.Current.DisplayAlert(
            "Apply Suggestion",
            "This will replace the current chord content. Continue?",
            "Yes",
            "No"
        );

        if (!confirm) return;

        try
        {
            Song.ChordProContent = SuggestedChordPro;
            if (!string.IsNullOrEmpty(SuggestedKey))
            {
                Song.OriginalKey = SuggestedKey;
            }

            await _songRepository.UpdateAsync(Song);

            await Shell.Current.DisplayAlert(
                "Success",
                "Chord suggestions applied to song",
                "OK"
            );

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"Failed to apply suggestions: {ex.Message}",
                "OK"
            );
        }
    }

    [RelayCommand]
    private void ToggleDisclaimer()
    {
        DisclaimerAccepted = !DisclaimerAccepted;
    }

    private string ExtractLyricsFromChordPro(string chordProContent)
    {
        if (string.IsNullOrEmpty(chordProContent))
            return string.Empty;

        var lines = chordProContent.Split('\n');
        var lyrics = new System.Text.StringBuilder();

        foreach (var line in lines)
        {
            // Skip directives
            if (line.TrimStart().StartsWith("{"))
                continue;

            // Remove chord markers
            var lyricsLine = System.Text.RegularExpressions.Regex.Replace(line, @"\[([^\]]+)\]", "");
            if (!string.IsNullOrWhiteSpace(lyricsLine))
            {
                lyrics.AppendLine(lyricsLine.Trim());
            }
        }

        return lyrics.ToString();
    }
}
