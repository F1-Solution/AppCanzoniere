using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.ImportReview;

/// <summary>The state of one import screen: reading, list, review, declaration and save, with no UI types.</summary>
public sealed partial class ImportSession
{
    private readonly ReviewClock _clock;
    private readonly ImportPlatform _platform;
    private List<ImportEntry> _entries = new();

    public ImportSession(TimeProvider? time = null, ImportPlatform platform = ImportPlatform.Windows)
    {
        _clock = new ReviewClock(time ?? TimeProvider.System);
        _platform = platform;
    }

    /// <summary>Raised after every change of state.</summary>
    public event Action? Changed;

    public ImportPhase Phase { get; private set; } = ImportPhase.Start;

    public string FileName { get; private set; } = string.Empty;

    public SongbookImportResult? Result { get; private set; }

    public ImportErrorInfo? Error { get; private set; }

    public ImportBanner? Banner { get; private set; }

    public IReadOnlyList<ImportEntry> Entries => _entries;

    public bool OnlyDoubtful { get; set; }

    /// <summary>The entries the list shows, honouring the "only to check" filter.</summary>
    public IReadOnlyList<ImportEntry> VisibleEntries =>
        OnlyDoubtful ? _entries.Where(e => e.NeedsReview).ToList() : _entries;

    public int SelectedCount => _entries.Count(e => e.IsSelected && !e.IsSaved);

    public int UnsavedCount => _entries.Count(e => !e.IsSaved);

    public int ModifiedUnsavedCount => _entries.Count(e => e.IsModified && !e.IsSaved);

    public int? CurrentIndex { get; private set; }

    public ImportEntry? Current => CurrentIndex is { } i && i < _entries.Count ? _entries[i] : null;

    public bool IsReviewing => Phase == ImportPhase.Result && Current is not null;

    /// <summary>Starts a new reading, dropping whatever was there.</summary>
    public void BeginReading(string fileName)
    {
        if (IsLocked)
            return;

        ClearSession();
        FileName = fileName;
        Phase = ImportPhase.Reading;
        Raise();
    }

    /// <summary>Takes the outcome of an import: a list, a single song to review, or an error.</summary>
    public void Complete(SongbookImportResult result)
    {
        if (Phase != ImportPhase.Reading)
            return;

        Result = result;
        if (result.IsSuccess && result.Songs.Count > 0)
        {
            _entries = result.Songs.Select((s, i) => new ImportEntry(i, s)).ToList();
            Phase = ImportPhase.Result;
            if (_entries.Count == 1)
                OpenCore(0);
        }
        else
        {
            Error = result.Status == SongbookImportStatus.NoSongsFound || result.IsSuccess
                ? ImportMessages.NoSongs
                : ImportMessages.ErrorFor(result.Failure, _platform);
            Phase = ImportPhase.Error;
        }

        Raise();
    }

    /// <summary>Shows an unexpected failure as a retryable error.</summary>
    public void Fail(Exception _)
    {
        if (Phase != ImportPhase.Reading)
            return;

        _clock.Stop(Current);
        Error = ImportMessages.Unexpected;
        Phase = ImportPhase.Error;
        Raise();
    }

    /// <summary>Cancels the reading and discards everything.</summary>
    public void Cancel()
    {
        if (IsLocked)
            return;

        ClearSession();
        Phase = ImportPhase.Cancelled;
        Raise();
    }

    /// <summary>Back to the start state with nothing kept.</summary>
    public void Reset()
    {
        if (IsLocked)
            return;

        ClearSession();
        Phase = ImportPhase.Start;
        Raise();
    }

    public void SelectAll() => SetAll(true);

    public void SelectNone() => SetAll(false);

    public void SetSelected(int index, bool selected)
    {
        if (IsLocked || index < 0 || index >= _entries.Count || _entries[index].IsSaved)
            return;

        _entries[index].IsSelected = selected;
        Raise();
    }

    /// <summary>Replaces the ChordPro of the open song; a saved song is read-only.</summary>
    public void Edit(string chordPro)
    {
        if (IsLocked || Current is not { IsSaved: false } entry)
            return;

        entry.ChordPro = chordPro ?? string.Empty;
        Raise();
    }

    /// <summary>Puts the open song back to the engine's proposal.</summary>
    public void Restore()
    {
        if (IsLocked || Current is not { IsSaved: false } entry)
            return;

        entry.ChordPro = entry.Song.ChordPro;
        Raise();
    }

    private void SetAll(bool selected)
    {
        if (IsLocked)
            return;

        foreach (var entry in _entries.Where(e => !e.IsSaved))
            entry.IsSelected = selected;

        Raise();
    }

    private void ClearSession()
    {
        _clock.Stop(Current);
        _entries = new List<ImportEntry>();
        CurrentIndex = null;
        Result = null;
        Error = null;
        Banner = null;
        OnlyDoubtful = false;
        DeclarationOpen = false;
        DeclarationAccepted = false;
        ProgressStage = SongbookImportStage.Reading;
        ProgressPage = 0;
        ProgressPageCount = 0;
        FileName = string.Empty;
    }

    private void Raise() => Changed?.Invoke();
}
