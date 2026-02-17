using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppCanzoniere.MAUI.ViewModels;

public partial class AlbumListViewModel : ObservableObject
{
    private readonly IAlbumRepository _albumRepository;

    [ObservableProperty]
    private ObservableCollection<Album> _albums = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public AlbumListViewModel(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    [RelayCommand]
    private async Task LoadAlbumsAsync()
    {
        IsLoading = true;
        try
        {
            var albums = await _albumRepository.GetAllAsync();
            Albums = new ObservableCollection<Album>(albums);
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
            await LoadAlbumsAsync();
            return;
        }

        IsLoading = true;
        try
        {
            var albums = await _albumRepository.SearchAsync(SearchText);
            Albums = new ObservableCollection<Album>(albums);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToAlbumDetailAsync(Album album)
    {
        if (album == null) return;

        await Shell.Current.GoToAsync("albumdetail", new Dictionary<string, object>
        {
            { "Album", album }
        });
    }

    [RelayCommand]
    private async Task NavigateToCreateAlbumAsync()
    {
        await Shell.Current.GoToAsync("albumdetail");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAlbumsAsync();
    }
}
