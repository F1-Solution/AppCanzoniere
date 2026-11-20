using System.Collections.ObjectModel;
using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.ImportReview;
using AppCanzoniere.Application.PageTurning;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AppCanzoniere.MAUI.ViewModels;

/// <summary>The width class of the import page, decided by the page width and not by the device.</summary>
public enum ImportWidthClass
{
    Narrow,
    Medium,
    Wide
}

/// <summary>Thin binding layer over <see cref="ImportSession"/>: it shows the state and forwards the commands.</summary>
public sealed partial class ImportViewModel : ObservableObject
{
    public const double MediumFrom = 700;
    public const double WideFrom = 1200;
    private const string PreviewLanguage = "it";

    private readonly ISongbookImportService _import;
    private readonly ISongPreviewService _preview;
    private readonly ISongDisplayService _display;
    private readonly IChordDisplaySettingsStore _settings;
    private readonly ImportSaver _saver;
    private readonly ISongRepository _songs;
    private readonly ScreenWakeLock _wakeLock;
    private readonly ILogger<ImportViewModel> _log;
    private readonly Dictionary<int, ImportRow> _rows = new();
    private SongbookImportResult? _rowsSource;
    private ImportEntry? _shownEntry;
    private bool _filterShown;
    private bool _editing;
    private int _lastAnnounced;

    public ImportViewModel(
        ISongbookImportService import,
        ISongPreviewService preview,
        ISongDisplayService display,
        IChordDisplaySettingsStore settings,
        ImportSaver saver,
        ISongRepository songs,
        ScreenWakeLock wakeLock,
        ILogger<ImportViewModel> log)
    {
        _import = import;
        _preview = preview;
        _display = display;
        _settings = settings;
        _saver = saver;
        _songs = songs;
        _wakeLock = wakeLock;
        _log = log;
#if WINDOWS
        Session = new ImportSession(TimeProvider.System, ImportPlatform.Windows);
#else
        Session = new ImportSession(TimeProvider.System, ImportPlatform.Android);
#endif
        Session.Changed += OnSessionChanged;
    }

    public ImportSession Session { get; }

    [ObservableProperty]
    private ImportWidthClass _widthClass = ImportWidthClass.Medium;

    [ObservableProperty]
    private ObservableCollection<ImportRow> _visibleRows = new();

    [ObservableProperty]
    private IReadOnlyList<OriginalLine> _originalLines = Array.Empty<OriginalLine>();

    [ObservableProperty]
    private PreparedSong? _previewSong;

    [ObservableProperty]
    private string? _previewWarning;

    /// <summary>Panel shown on a narrow page: 0 original, 1 ChordPro, 2 preview.</summary>
    [ObservableProperty]
    private int _narrowPanel = 1;

    /// <summary>Left panel shown on a medium page: 0 original, 1 preview.</summary>
    [ObservableProperty]
    private int _mediumLeftPanel;

    public bool IsStart => Session.Phase == ImportPhase.Start;
    public bool IsReading => Session.Phase == ImportPhase.Reading;
    public bool IsResult => Session.Phase == ImportPhase.Result;
    public bool IsError => Session.Phase == ImportPhase.Error;
    public bool IsCancelled => Session.Phase == ImportPhase.Cancelled;
    public bool IsWide => WidthClass == ImportWidthClass.Wide;
    public bool IsNarrow => WidthClass == ImportWidthClass.Narrow;
    public bool IsMedium => WidthClass == ImportWidthClass.Medium;
    public bool IsReviewing => Session.IsReviewing;

    public bool ShowList => IsResult && (IsWide || !IsReviewing);
    public bool ShowReview => IsResult && (IsWide || IsReviewing);
    public bool ShowReviewContent => IsReviewing;
    public bool ShowReviewPlaceholder => IsWide && IsResult && !IsReviewing;
    public bool ShowBackToList => IsReviewing && !IsWide && Session.Entries.Count > 1;
    public bool ShowNavigation => IsReviewing && Session.Entries.Count > 1;
    public bool ShowAnotherPdf => IsResult || IsError || IsCancelled;

#if WINDOWS
    public bool ShowShortcutHint => true;
#else
    public bool ShowShortcutHint => false;
#endif

    public bool TimerEnabled
    {
        get => Session.TimerEnabled;
        set
        {
            if (value == Session.TimerEnabled)
                return;

            Session.TimerEnabled = value;
            if (value)
                Session.ResumeTimer();
            else
                Session.PauseTimer();
        }
    }

    public bool OnlyDoubtful
    {
        get => Session.OnlyDoubtful;
        set
        {
            Session.OnlyDoubtful = value;
            RebuildRows();
            Refresh();
        }
    }

    public string TimerText => "Tempo " + ImportMessages.Duration(Session.ElapsedOfCurrent);
    public bool ShowTimer => Session.TimerEnabled && IsReviewing;
    public bool ShowCopyTimes => Session.TimerEnabled && IsResult;

    // Reading
    public string FileName => Session.FileName;
    public string ProgressPhaseText => Session.ProgressPhaseText;
    public string ProgressPageText => Session.ProgressPageText;
    public double ProgressFraction => Session.ProgressFraction;
    public string ProgressPercentText => $"{(int)Math.Round(Session.ProgressFraction * 100)}%";

    // Error
    public string ErrorTitle => Session.Error?.Title ?? string.Empty;
    public string ErrorExplanation => Session.Error?.Explanation ?? string.Empty;
    public bool ErrorCanRetry => Session.Error?.CanRetry == true;
    public bool ErrorOnlyOther => !ErrorCanRetry;

    // List summary
    public string SummaryPages => Session.Result is { } r
        ? $"{(r.PageCount == 1 ? "1 pagina" : $"{r.PageCount} pagine")}: {r.TextPageCount} di testo, {(r.ScannedPages.Count == 1 ? "1 scansione" : $"{r.ScannedPages.Count} scansioni")}"
        : string.Empty;

    public string SummaryFound => Session.Result is { } r
        ? r.IndexCountMismatch
            ? $"{ImportMessages.Songs(r.Songs.Count)} trovati · l'indice ne conta {r.IndexEntryCount}"
            : $"{ImportMessages.Songs(r.Songs.Count)} trovati"
        : string.Empty;

    public bool HasIndexMismatch => Session.Result?.IndexCountMismatch == true;

    public string UnreadText => Session.Result is { UnreadPages.Count: > 0 } r
        ? $"! {r.UnreadPages.Count} {(r.UnreadPages.Count == 1 ? "pagina non letta" : "pagine non lette")}: {string.Join(", ", r.UnreadPages.Select(p => p.Page))}"
        : string.Empty;

    public bool HasUnread => Session.Result is { UnreadPages.Count: > 0 };

    public string SelectedText => Session.SelectedCount == 1 ? "1 selezionato" : $"{Session.SelectedCount} selezionati";
    public bool CanSaveSelected => Session.SelectedCount > 0;
    public bool SaveSelectedHintVisible => Session.SelectedCount == 0;
    public string SaveSelectedText => Session.SelectedCount == 1 ? "Salva 1 canto selezionato" : $"Salva {ImportMessages.Songs(Session.SelectedCount)} selezionati";

    // Banner
    public bool HasBanner => Session.Banner is not null;
    public string BannerText => Session.Banner?.Text ?? string.Empty;
    public bool BannerShowOpen => Session.Banner?.OpenIndex is not null;
    public bool BannerShowNext => Session.Banner?.CanGoNext == true;

    // Review
    public string ReviewTitle => Session.Current?.Song.Title ?? string.Empty;
    public string PositionText => Session.CurrentIndex is { } i ? $"{i + 1} di {Session.Entries.Count}" : string.Empty;
    public string PagesText => Session.Current is { } e ? ImportMessages.PageRange(e.Song.FirstPage, e.Song.LastPage) : string.Empty;
    public bool CurrentIsScan => Session.Current is { } e && e.Song.Origin != SongOrigin.Text;
    public bool CurrentIsSaved => Session.Current?.IsSaved == true;
    public bool CanEdit => Session.Current is { IsSaved: false };
    public bool CanRestore => Session.Current is { IsSaved: false, IsModified: true };
    public bool HasPreviewWarning => PreviewWarning is not null;

    public IReadOnlyList<string> ReviewBadges => Session.Current is { } e
        ? ReviewBadgeList(e)
        : Array.Empty<string>();

    public bool HasReviewBadges => ReviewBadges.Count > 0;

    public string ChordProText
    {
        get => Session.Current?.ChordPro ?? string.Empty;
        set
        {
            if (Session.Current is not { IsSaved: false } entry || SameText(value, entry.ChordPro))
                return;

            _editing = true;
            try
            {
                Session.Edit(value ?? string.Empty);
            }
            finally
            {
                _editing = false;
            }

            SchedulePreview(TimeSpan.FromMilliseconds(400));
        }
    }

    // The editor may hand the text back with other line endings; that is not an edit.
    private static bool SameText(string? a, string? b) =>
        string.Equals(NormalizeLines(a), NormalizeLines(b), StringComparison.Ordinal);

    private static string NormalizeLines(string? text) => (text ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');

    // Declaration and saving
    public bool DeclarationOpen => Session.DeclarationOpen;
    public bool CanConfirmSave => Session.CanConfirmSave;
    public bool IsSaving => Session.IsSaving;
    public bool ShowDeclarationHelp => Session.DeclarationOpen && !Session.DeclarationAccepted && !Session.IsSaving;
    public string DeclarationTitle => $"Salva {ImportMessages.Songs(Session.DeclarationCount)} nella libreria";
    public string DeclarationText => ImportMessages.DeclarationText;
    public string SaveProgressText => $"Salvataggio {Math.Min(Session.SaveDone + 1, Session.SaveTotal)} di {Session.SaveTotal}";
    public double SaveFraction => Session.SaveTotal == 0 ? 0 : (double)Session.SaveDone / Session.SaveTotal;

    public bool DeclarationAccepted
    {
        get => Session.DeclarationAccepted;
        set => Session.SetDeclaration(value);
    }

    // Automation (debug builds only)
#if DEBUG
    public bool HooksEnabled => true;
#else
    public bool HooksEnabled => false;
#endif

    /// <summary>One line a UI test can read: phase and counts.</summary>
    public string AutomationStatus => HooksEnabled
        ? $"phase={Session.Phase};songs={Session.Entries.Count};selected={Session.SelectedCount};saved={Session.Entries.Count(e => e.IsSaved)};" +
          $"reviewing={Session.IsReviewing};declaration={Session.DeclarationOpen};error={Session.Error?.Title}"
        : string.Empty;

    /// <summary>Called every second by the page so the review timer text moves.</summary>
    public void NotifyTimer() => OnPropertyChanged(nameof(TimerText));

    public void SetWidth(double width)
    {
        var next = width >= WideFrom ? ImportWidthClass.Wide : width >= MediumFrom ? ImportWidthClass.Medium : ImportWidthClass.Narrow;
        if (next != WidthClass)
            WidthClass = next;
    }

    partial void OnWidthClassChanged(ImportWidthClass value) => Refresh();

    partial void OnPreviewWarningChanged(string? value) => OnPropertyChanged(nameof(HasPreviewWarning));

    private static List<string> ReviewBadgeList(ImportEntry entry)
    {
        var list = ImportMessages.WarningLabels(entry.Song).ToList();
        if (entry.Song.DoubtfulLineNumbers.Count > 0)
            list.Insert(0, "Righe da controllare: " + string.Join(", ", entry.Song.DoubtfulLineNumbers));

        return list;
    }

    /// <summary>Updates every binding; called after a change of the session or the width.</summary>
    public void Refresh()
    {
        if (!MainThread.IsMainThread)
        {
            MainThread.BeginInvokeOnMainThread(Refresh);
            return;
        }

        try
        {
            SyncRows();
            SyncCurrent();
            AnnounceProgress();
            OnPropertyChanged(string.Empty);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Refreshing the import screen failed");
        }
    }

    private void OnSessionChanged() => Refresh();

    private void SyncRows()
    {
        if (!ReferenceEquals(_rowsSource, Session.Result) || _filterShown != Session.OnlyDoubtful)
            RebuildRows();

        if (_editing)
        {
            if (Session.Current is { } current && _rows.TryGetValue(current.Index, out var row))
                row.Refresh();

            return;
        }

        foreach (var row in _rows.Values)
            row.Refresh();
    }

    private void RebuildRows()
    {
        if (!ReferenceEquals(_rowsSource, Session.Result))
        {
            _rows.Clear();
            foreach (var entry in Session.Entries)
                _rows[entry.Index] = new ImportRow(entry, Session, SelectRow);

            _rowsSource = Session.Result;
        }

        _filterShown = Session.OnlyDoubtful;
        VisibleRows = new ObservableCollection<ImportRow>(Session.VisibleEntries.Select(e => _rows[e.Index]));
    }

    private void SelectRow(ImportRow row, bool selected) => Session.SetSelected(row.Entry.Index, selected);

    // A different open song rebuilds the original text and the preview at once.
    private void SyncCurrent()
    {
        var current = Session.IsReviewing ? Session.Current : null;
        if (ReferenceEquals(current, _shownEntry))
            return;

        _shownEntry = current;
        OriginalLines = current is null ? Array.Empty<OriginalLine>() : BuildOriginalLines(current);
        PreviewWarning = null;
        SchedulePreview(TimeSpan.Zero);
    }

    private static IReadOnlyList<OriginalLine> BuildOriginalLines(ImportEntry entry)
    {
        var doubtful = entry.Song.DoubtfulLineNumbers.ToHashSet();
        return entry.Song.SourceText
            .Replace("\r\n", "\n")
            .Split('\n')
            .Select((text, i) => new OriginalLine(i + 1, text, doubtful.Contains(i + 1)))
            .ToList();
    }

    private void AnnounceProgress()
    {
        if (!IsReading || Session.ProgressPage <= 0 || Session.ProgressPage % 5 != 0 || Session.ProgressPage == _lastAnnounced)
            return;

        _lastAnnounced = Session.ProgressPage;
        Announce($"{Session.ProgressPhaseText}, {Session.ProgressPageText}");
    }

    private void Announce(string text)
    {
        try
        {
            SemanticScreenReader.Announce(text);
        }
        catch (Exception ex)
        {
            _log.LogDebug(ex, "Announcing to the screen reader failed");
        }
    }
}
