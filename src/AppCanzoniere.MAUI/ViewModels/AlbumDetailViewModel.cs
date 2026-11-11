using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppCanzoniere.MAUI.ViewModels;

[QueryProperty(nameof(Album), "Album")]
public partial class AlbumDetailViewModel : ObservableObject
{
    private readonly IAlbumRepository _albumRepository;
    private readonly ISongRepository _songRepository;

    [ObservableProperty]
    private Album? _album;

    [ObservableProperty]
    private ObservableCollection<AlbumSong> _albumSongs = new();

    [ObservableProperty]
    private ObservableCollection<Song> _availableSongs = new();

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private bool _isAddingSongs;

    [ObservableProperty]
    private string _searchAvailableText = string.Empty;

    public ObservableCollection<string> AlbumTypes { get; } = new()
    {
        "Liturgical",
        "Thematic",
        "Artist",
        "Custom"
    };

    public AlbumDetailViewModel(IAlbumRepository albumRepository, ISongRepository songRepository)
    {
        _albumRepository = albumRepository;
        _songRepository = songRepository;
    }

    partial void OnAlbumChanged(Album? value)
    {
        if (value != null && value.Id != Guid.Empty)
        {
            LoadAlbumSongsAsync().ConfigureAwait(false);
        }
        else if (value == null)
        {
            // Creating new album
            Album = new Album
            {
                Id = Guid.Empty,
                Name = string.Empty,
                Description = string.Empty,
                Type = AlbumType.Custom,
                CreatedAt = DateTime.UtcNow
            };
            IsEditMode = true;
        }
    }

    [RelayCommand]
    private async Task LoadAlbumSongsAsync()
    {
        if (Album == null || Album.Id == Guid.Empty) return;

        var albumWithSongs = await _albumRepository.GetByIdWithSongsAsync(Album.Id);
        if (albumWithSongs != null)
        {
            AlbumSongs = new ObservableCollection<AlbumSong>(
                albumWithSongs.AlbumSongs.OrderBy(x => x.OrderIndex)
            );
        }
    }

    [RelayCommand]
    private async Task LoadAvailableSongsAsync()
    {
        IsAddingSongs = true;

        var allSongs = string.IsNullOrWhiteSpace(SearchAvailableText)
            ? await _songRepository.GetAllAsync()
            : await _songRepository.SearchAsync(SearchAvailableText);

        // Exclude songs already in album
        var existingSongIds = AlbumSongs.Select(x => x.SongId).ToHashSet();
        var available = allSongs.Where(s => !existingSongIds.Contains(s.Id)).ToList();

        AvailableSongs = new ObservableCollection<Song>(available);
    }

    [RelayCommand]
    private async Task AddSongToAlbumAsync(Song song)
    {
        if (Album == null || song == null) return;

        // If album is new, save it first
        if (Album.Id == Guid.Empty)
        {
            await _albumRepository.AddAsync(Album);
        }

        var albumSong = new AlbumSong
        {
            AlbumId = Album.Id,
            SongId = song.Id,
            OrderIndex = AlbumSongs.Count + 1,
            AddedAt = DateTime.UtcNow,
            Song = song
        };

        await _albumRepository.AddSongAsync(Album.Id, song.Id, albumSong.OrderIndex);

        AlbumSongs.Add(albumSong);
        AvailableSongs.Remove(song);

        await Shell.Current.DisplayAlert("Success", $"'{song.Title}' added to album", "OK");
    }

    [RelayCommand]
    private async Task RemoveSongFromAlbumAsync(AlbumSong albumSong)
    {
        if (Album == null || albumSong == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirm Remove",
            $"Remove '{albumSong.Song?.Title}' from album?",
            "Yes",
            "No");

        if (!confirm) return;

        try
        {
            await _albumRepository.RemoveSongAsync(Album.Id, albumSong.SongId);
            AlbumSongs.Remove(albumSong);

            // Reorder remaining songs
            await ReorderSongsAsync();

            await Shell.Current.DisplayAlert("Success", "Song removed from album", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to remove song: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task MoveSongUpAsync(AlbumSong albumSong)
    {
        var index = AlbumSongs.IndexOf(albumSong);
        if (index <= 0) return;

        AlbumSongs.Move(index, index - 1);
        await ReorderSongsAsync();
    }

    [RelayCommand]
    private async Task MoveSongDownAsync(AlbumSong albumSong)
    {
        var index = AlbumSongs.IndexOf(albumSong);
        if (index >= AlbumSongs.Count - 1) return;

        AlbumSongs.Move(index, index + 1);
        await ReorderSongsAsync();
    }

    private async Task ReorderSongsAsync()
    {
        if (Album == null) return;

        for (int i = 0; i < AlbumSongs.Count; i++)
        {
            AlbumSongs[i].OrderIndex = i + 1;
        }

        await _albumRepository.UpdateSongOrderAsync(Album.Id,
            AlbumSongs.ToDictionary(x => x.SongId, x => x.OrderIndex));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Album == null) return;

        if (string.IsNullOrWhiteSpace(Album.Name))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Album name is required", "OK");
            return;
        }

        try
        {
            if (Album.Id == Guid.Empty)
            {
                Album.Id = Guid.NewGuid();
                await _albumRepository.AddAsync(Album);
            }
            else
            {
                await _albumRepository.UpdateAsync(Album);
            }

            IsEditMode = false;
            await Shell.Current.DisplayAlert("Success", "Album saved successfully", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to save album: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Album == null || Album.Id == Guid.Empty) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirm Delete",
            $"Are you sure you want to delete '{Album.Name}'?",
            "Yes",
            "No");

        if (!confirm) return;

        try
        {
            await _albumRepository.DeleteAsync(Album.Id);
            await Shell.Current.DisplayAlert("Success", "Album deleted successfully", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to delete album: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private void ToggleEditMode()
    {
        IsEditMode = !IsEditMode;
    }

    [RelayCommand]
    private void ToggleAddingSongs()
    {
        IsAddingSongs = !IsAddingSongs;
        if (IsAddingSongs)
        {
            LoadAvailableSongsAsync().ConfigureAwait(false);
        }
    }

    [RelayCommand]
    private async Task CancelAddingSongsAsync()
    {
        IsAddingSongs = false;
        SearchAvailableText = string.Empty;
        AvailableSongs.Clear();
    }
}
