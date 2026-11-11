using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppCanzoniere.MAUI.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly ISongRepository _songRepository;
    private readonly IAlbumRepository _albumRepository;

    [ObservableProperty]
    private ObservableCollection<Song> _songs = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public HomeViewModel(ISongRepository songRepository, IAlbumRepository albumRepository)
    {
        _songRepository = songRepository;
        _albumRepository = albumRepository;
    }

    [RelayCommand]
    private async Task LoadSongsAsync()
    {
        IsLoading = true;
        try
        {
            var songs = await _songRepository.GetAllAsync();
            Songs = new ObservableCollection<Song>(songs);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadSongsAsync();
            return;
        }

        IsLoading = true;
        try
        {
            var songs = await _songRepository.SearchAsync(SearchText);
            Songs = new ObservableCollection<Song>(songs);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToSongDetailAsync(Song song)
    {
        if (song == null) return;

        await Shell.Current.GoToAsync($"songdetail", new Dictionary<string, object>
        {
            { "Song", song }
        });
    }

    [RelayCommand]
    private async Task NavigateToCreateSongAsync()
    {
        await Shell.Current.GoToAsync("songdetail");
    }

    [RelayCommand]
    private async Task NavigateToSearchAsync()
    {
        await Shell.Current.GoToAsync("//search");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadSongsAsync();
    }
}
