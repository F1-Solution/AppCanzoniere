using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppCanzoniere.MAUI.ViewModels;

[QueryProperty(nameof(SongId), "SongId")]
public partial class SongMetadataViewModel : ObservableObject
{
    private readonly ISongRepository _songRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ILiturgicalMomentRepository _liturgicalMomentRepository;

    [ObservableProperty]
    private Guid _songId;

    [ObservableProperty]
    private Song? _song;

    [ObservableProperty]
    private ObservableCollection<Tag> _availableTags = new();

    [ObservableProperty]
    private ObservableCollection<Tag> _selectedTags = new();

    [ObservableProperty]
    private ObservableCollection<LiturgicalMoment> _availableLiturgicalMoments = new();

    [ObservableProperty]
    private ObservableCollection<LiturgicalMoment> _selectedLiturgicalMoments = new();

    [ObservableProperty]
    private ObservableCollection<BibleReference> _bibleReferences = new();

    [ObservableProperty]
    private string _newTagName = string.Empty;

    [ObservableProperty]
    private string _newBibleBook = string.Empty;

    [ObservableProperty]
    private string _newBibleChapter = string.Empty;

    [ObservableProperty]
    private string _newBibleVerse = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public SongMetadataViewModel(
        ISongRepository songRepository,
        ITagRepository tagRepository,
        ILiturgicalMomentRepository liturgicalMomentRepository)
    {
        _songRepository = songRepository;
        _tagRepository = tagRepository;
        _liturgicalMomentRepository = liturgicalMomentRepository;
    }

    partial void OnSongIdChanged(Guid value)
    {
        if (value != Guid.Empty)
        {
            LoadDataAsync().ConfigureAwait(false);
        }
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            // Load song with relationships
            Song = await _songRepository.GetByIdAsync(SongId);
            if (Song == null) return;

            // Load all available tags and liturgical moments
            var allTags = await _tagRepository.GetAllAsync();
            var allLiturgicalMoments = await _liturgicalMomentRepository.GetAllAsync();

            // Separate selected from available
            var songTagIds = Song.SongTags.Select(st => st.TagId).ToHashSet();
            var songLiturgicalIds = Song.SongLiturgicalMoments.Select(sl => sl.LiturgicalMomentId).ToHashSet();

            SelectedTags = new ObservableCollection<Tag>(
                Song.SongTags.Select(st => st.Tag).Where(t => t != null)!
            );

            AvailableTags = new ObservableCollection<Tag>(
                allTags.Where(t => !songTagIds.Contains(t.Id))
            );

            SelectedLiturgicalMoments = new ObservableCollection<LiturgicalMoment>(
                Song.SongLiturgicalMoments.Select(sl => sl.LiturgicalMoment).Where(lm => lm != null)!
            );

            AvailableLiturgicalMoments = new ObservableCollection<LiturgicalMoment>(
                allLiturgicalMoments.Where(lm => !songLiturgicalIds.Contains(lm.Id))
            );

            BibleReferences = new ObservableCollection<BibleReference>(
                Song.BibleReferences ?? new List<BibleReference>()
            );
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddTagAsync(Tag tag)
    {
        if (Song == null || tag == null) return;

        try
        {
            var songTag = new SongTag
            {
                SongId = Song.Id,
                TagId = tag.Id
            };

            Song.SongTags.Add(songTag);
            await _songRepository.UpdateAsync(Song);

            SelectedTags.Add(tag);
            AvailableTags.Remove(tag);

            await Shell.Current.DisplayAlert("Success", $"Tag '{tag.Name}' added", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to add tag: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task RemoveTagAsync(Tag tag)
    {
        if (Song == null || tag == null) return;

        try
        {
            var songTag = Song.SongTags.FirstOrDefault(st => st.TagId == tag.Id);
            if (songTag != null)
            {
                Song.SongTags.Remove(songTag);
                await _songRepository.UpdateAsync(Song);

                SelectedTags.Remove(tag);
                AvailableTags.Add(tag);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to remove tag: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task CreateAndAddTagAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTagName))
        {
            await Shell.Current.DisplayAlert("Validation", "Tag name is required", "OK");
            return;
        }

        try
        {
            var newTag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = NewTagName.Trim(),
                Category = "Custom"
            };

            await _tagRepository.AddAsync(newTag);
            await AddTagAsync(newTag);

            NewTagName = string.Empty;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to create tag: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task AddLiturgicalMomentAsync(LiturgicalMoment moment)
    {
        if (Song == null || moment == null) return;

        try
        {
            var songLiturgical = new SongLiturgicalMoment
            {
                SongId = Song.Id,
                LiturgicalMomentId = moment.Id
            };

            Song.SongLiturgicalMoments.Add(songLiturgical);
            await _songRepository.UpdateAsync(Song);

            SelectedLiturgicalMoments.Add(moment);
            AvailableLiturgicalMoments.Remove(moment);

            await Shell.Current.DisplayAlert("Success", $"Liturgical moment '{moment.Name}' added", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to add liturgical moment: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task RemoveLiturgicalMomentAsync(LiturgicalMoment moment)
    {
        if (Song == null || moment == null) return;

        try
        {
            var songLiturgical = Song.SongLiturgicalMoments.FirstOrDefault(sl => sl.LiturgicalMomentId == moment.Id);
            if (songLiturgical != null)
            {
                Song.SongLiturgicalMoments.Remove(songLiturgical);
                await _songRepository.UpdateAsync(Song);

                SelectedLiturgicalMoments.Remove(moment);
                AvailableLiturgicalMoments.Add(moment);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to remove liturgical moment: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task AddBibleReferenceAsync()
    {
        if (string.IsNullOrWhiteSpace(NewBibleBook))
        {
            await Shell.Current.DisplayAlert("Validation", "Bible book is required", "OK");
            return;
        }

        try
        {
            // Parse chapter and verse
            int? chapter = string.IsNullOrWhiteSpace(NewBibleChapter) ? null : int.TryParse(NewBibleChapter, out var c) ? c : null;
            int? verseStart = string.IsNullOrWhiteSpace(NewBibleVerse) ? null : int.TryParse(NewBibleVerse, out var v) ? v : null;

            var reference = new BibleReference
            {
                Id = Guid.NewGuid(),
                SongId = Song!.Id,
                Book = NewBibleBook.Trim(),
                Chapter = chapter,
                VerseStart = verseStart,
                DisplayText = $"{NewBibleBook.Trim()} {NewBibleChapter},{NewBibleVerse}".Trim()
            };

            Song.BibleReferences.Add(reference);
            await _songRepository.UpdateAsync(Song);

            BibleReferences.Add(reference);

            // Clear inputs
            NewBibleBook = string.Empty;
            NewBibleChapter = string.Empty;
            NewBibleVerse = string.Empty;

            await Shell.Current.DisplayAlert("Success", "Bible reference added", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to add bible reference: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task RemoveBibleReferenceAsync(BibleReference reference)
    {
        if (Song == null || reference == null) return;

        try
        {
            Song.BibleReferences.Remove(reference);
            await _songRepository.UpdateAsync(Song);

            BibleReferences.Remove(reference);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to remove bible reference: {ex.Message}", "OK");
        }
    }
}
