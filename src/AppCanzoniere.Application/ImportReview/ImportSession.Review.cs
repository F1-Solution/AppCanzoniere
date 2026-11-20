using System.Text;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.ImportReview;

public sealed partial class ImportSession
{
    public SongbookImportStage ProgressStage { get; private set; }

    public int ProgressPage { get; private set; }

    public int ProgressPageCount { get; private set; }

    public string ProgressPhaseText => ImportMessages.StageText(ProgressStage);

    public string ProgressPageText => ProgressPageCount > 0 ? $"Pagina {ProgressPage} di {ProgressPageCount}" : string.Empty;

    /// <summary>Share of the pages handled, 0 while the text layer is being read.</summary>
    public double ProgressFraction =>
        ProgressStage == SongbookImportStage.Reading || ProgressPageCount <= 0
            ? 0
            : Math.Clamp((double)ProgressPage / ProgressPageCount, 0, 1);

    /// <summary>Whether the review timer runs while a song is open (off by default).</summary>
    public bool TimerEnabled { get; set; }

    /// <summary>Time of the open song, including the part still running.</summary>
    public TimeSpan ElapsedOfCurrent => TimerEnabled && Current is { } e ? _clock.ElapsedOf(e) : TimeSpan.Zero;

    public void Report(SongbookImportProgress progress)
    {
        if (Phase != ImportPhase.Reading)
            return;

        ProgressStage = progress.Stage;
        ProgressPage = progress.Page;
        ProgressPageCount = progress.PageCount;
        Raise();
    }

    public void Open(int index)
    {
        if (Phase != ImportPhase.Result || index < 0 || index >= _entries.Count)
            return;

        OpenCore(index);
        Raise();
    }

    public void Next() => Open(Math.Min(_entries.Count - 1, (CurrentIndex ?? -1) + 1));

    public void Previous() => Open(Math.Max(0, (CurrentIndex ?? 0) - 1));

    /// <summary>Opens the first unsaved song after the open one, when there is one.</summary>
    public void OpenNextUnsaved()
    {
        var from = CurrentIndex ?? -1;
        var next = _entries.FirstOrDefault(e => e.Index > from && !e.IsSaved);
        if (next is not null)
            Open(next.Index);
    }

    /// <summary>Leaves the review and shows the list again.</summary>
    public void CloseReview()
    {
        _clock.Stop(Current);
        CurrentIndex = null;
        Raise();
    }

    public void PauseTimer()
    {
        _clock.Pause(Current);
        Raise();
    }

    public void ResumeTimer()
    {
        _clock.Resume(Current, TimerEnabled);
        Raise();
    }

    private void OpenCore(int index)
    {
        _clock.Stop(Current);
        CurrentIndex = index;
        _entries[index].WasOpened = true;
        _clock.Resume(_entries[index], TimerEnabled);
    }

    /// <summary>A text table of the times for the owner's Gate 0 report.</summary>
    public string TimesTable()
    {
        var text = new StringBuilder("Titolo\tPagine\tOrigine\tMinuti\t≤ 2 min\tPiattaforma\n");
        foreach (var e in _entries)
        {
            var time = e.ReviewTime + (ReferenceEquals(e, Current) ? _clock.Running : TimeSpan.Zero);
            var shown = e.IsSaved && !e.WasOpened ? "0:00, non rivisto" : ImportMessages.Duration(time);
            var origin = e.Song.Origin == SongOrigin.Text ? "testo" : "scansione";
            var ok = time <= TimeSpan.FromMinutes(2) ? "sì" : "no";
            text.Append($"{e.Song.Title}\t{ImportMessages.PageRange(e.Song.FirstPage, e.Song.LastPage)}\t{origin}\t{shown}\t{ok}\t{_platform}\n");
        }

        return text.ToString();
    }

    /// <summary>Counts the time of one open song with an injectable clock.</summary>
    private sealed class ReviewClock(TimeProvider time)
    {
        private ImportEntry? _entry;
        private long _started;
        private bool _running;

        public TimeSpan Running => _running ? time.GetElapsedTime(_started) : TimeSpan.Zero;

        public TimeSpan ElapsedOf(ImportEntry entry) => entry.ReviewTime + (ReferenceEquals(entry, _entry) ? Running : TimeSpan.Zero);

        public void Resume(ImportEntry? entry, bool enabled)
        {
            if (!enabled || entry is null || _running)
                return;

            _entry = entry;
            _started = time.GetTimestamp();
            _running = true;
        }

        public void Pause(ImportEntry? _) => Stop(_entry);

        public void Stop(ImportEntry? _)
        {
            if (_running && _entry is not null)
                _entry.ReviewTime += time.GetElapsedTime(_started);

            _running = false;
            _entry = null;
        }
    }
}
