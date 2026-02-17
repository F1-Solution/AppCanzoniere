using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppCanzoniere.MAUI.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private readonly ISongRepository _songRepository;
    private readonly ITagRepository _tagRepository;

    [ObservableProperty]
    private ObservableCollection<Song> _searchResults = new();

    [ObservableProperty]
    private ObservableCollection<Tag> _availableTags = new();

    [ObservableProperty]
    private ObservableCollection<Tag> _selectedTags = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private bool _showTagFilter;

    public SearchViewModel(ISongRepository songRepository, ITagRepository tagRepository)
    {
        _songRepository = songRepository;
        _tagRepository = tagRepository;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var tags = await _tagRepository.GetAllAsync();
        AvailableTags = new ObservableCollection<Tag>(tags);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsSearching = true;
        try
        {
            if (string.IsNullOrWhiteSpace(SearchText) && SelectedTags.Count == 0)
            {
                SearchResults.Clear();
                return;
            }

            IEnumerable<Song> results;

            // If tags are selected, filter by tags
            if (SelectedTags.Count > 0)
            {
                var tagIds = SelectedTags.Select(t => t.Id).ToList();
                results = await _songRepository.GetSongsByTagsAsync(tagIds);

                // Also filter by search text if provided
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var searchLower = SearchText.ToLower();
                    results = results.Where(s =>
                        s.Title.ToLower().Contains(searchLower) ||
                        (s.Artist?.ToLower().Contains(searchLower) ?? false)
                    );
                }
            }
            else
            {
                results = await _songRepository.SearchAsync(SearchText);
            }

            SearchResults = new ObservableCollection<Song>(results);
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToSongAsync(Song song)
    {
        if (song == null) return;

        await Shell.Current.GoToAsync("songdetail", new Dictionary<string, object>
        {
            { "Song", song }
        });
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
        SelectedTags.Clear();
        SearchResults.Clear();
    }

    [RelayCommand]
    private void ToggleTagFilter()
    {
        ShowTagFilter = !ShowTagFilter;
    }

    [RelayCommand]
    private async Task ToggleTagSelectionAsync(Tag tag)
    {
        if (SelectedTags.Contains(tag))
        {
            SelectedTags.Remove(tag);
        }
        else
        {
            SelectedTags.Add(tag);
        }

        // Auto-search when tag selection changes
        await SearchAsync();
    }
}
