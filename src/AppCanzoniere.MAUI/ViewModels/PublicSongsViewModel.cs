using AppCanzoniere.Application.DTOs.Backend;
using AppCanzoniere.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppCanzoniere.MAUI.ViewModels;

public partial class PublicSongsViewModel : ObservableObject
{
    private readonly IBackendSongService _backendSongService;
    private readonly IAuthService _authService;

    public PublicSongsViewModel(IBackendSongService backendSongService, IAuthService authService)
    {
        _backendSongService = backendSongService;
        _authService = authService;
    }

    [ObservableProperty]
    private ObservableCollection<PublicSongDto> _songs = new();

    [ObservableProperty]
    private PublicSongDto? _selectedSong;

    [ObservableProperty]
    private bool _isBusy = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _isAuthenticated = false;

    private int _currentPage = 1;
    private const int PageSize = 20;

    [RelayCommand]
    private async Task LoadSongsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = "Loading public songs...";

        try
        {
            var songs = await _backendSongService.GetPublicSongsAsync(_currentPage, PageSize);
            
            Songs.Clear();
            foreach (var song in songs)
            {
                Songs.Add(song);
            }

            StatusMessage = $"Loaded {Songs.Count} songs";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SearchSongsAsync()
    {
        if (IsBusy) return;
        
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            await LoadSongsAsync();
            return;
        }

        IsBusy = true;
        StatusMessage = $"Searching for '{SearchQuery}'...";

        try
        {
            var songs = await _backendSongService.SearchSongsAsync(SearchQuery);
            
            Songs.Clear();
            foreach (var song in songs)
            {
                Songs.Add(song);
            }

            StatusMessage = $"Found {Songs.Count} songs";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SelectSongAsync(PublicSongDto? song)
    {
        if (song == null) return;

        SelectedSong = song;
        
        // Navigate to song detail or show details
        await Shell.Current.DisplayAlert("Song Details", 
            $"Title: {song.Title}\nArtist: {song.Artist}\nKey: {song.OriginalKey}", 
            "OK");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        _currentPage = 1;
        await LoadSongsAsync();
    }

    [RelayCommand]
    private async Task NavigateToLoginAsync()
    {
        await Shell.Current.GoToAsync("login");
    }

    public async Task CheckAuthenticationAsync()
    {
        IsAuthenticated = await _authService.IsAuthenticatedAsync();
    }
}
