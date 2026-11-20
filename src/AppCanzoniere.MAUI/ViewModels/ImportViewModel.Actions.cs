using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.ImportReview;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.MAUI.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AppCanzoniere.MAUI.ViewModels;

public sealed partial class ImportViewModel
{
    private CancellationTokenSource? _importCts;
    private CancellationTokenSource? _previewCts;
    private IDisposable? _wakeLease;
    private string? _tempPath;

    private static readonly PickOptions PdfOptions = new()
    {
        PickerTitle = "Scegli un PDF",
        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            [DevicePlatform.WinUI] = new[] { ".pdf" },
            [DevicePlatform.Android] = new[] { "application/pdf" }
        })
    };

    [RelayCommand]
    private async Task ChoosePdfAsync()
    {
        try
        {
            if (IsReading || !await ConfirmLeaveAsync())
                return;

            var file = await FilePicker.Default.PickAsync(PdfOptions);
            if (file is null)
                return;

            await RunImportAsync(file.FileName, () => CopyToTempAsync(file), ownsTemp: true);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Choosing a PDF failed");
            await ShowAsync("Non riesco ad aprire il file", "Il selettore dei file non ha risposto. Riprova.");
        }
    }

    /// <summary>Debug hook: imports the PDF at a path without the file picker.</summary>
    public Task ImportFromPathAsync(string path) =>
        RunImportAsync(Path.GetFileName(path), () => Task.FromResult(path), ownsTemp: false);

    [RelayCommand]
    private Task RetryAsync() => _lastSource is { } source
        ? RunImportAsync(source.Name, () => Task.FromResult(source.Path), ownsTemp: false)
        : ChoosePdfAsync();

    [RelayCommand]
    private void CancelReading()
    {
        try
        {
            _importCts?.Cancel();
            Session.Cancel();
            AfterSessionEnded();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Cancelling the import failed");
        }
    }

    [RelayCommand]
    private async Task AnotherPdfAsync()
    {
        if (IsReading || !await ConfirmLeaveAsync())
            return;

        Session.Reset();
        AfterSessionEnded();
    }

    [RelayCommand]
    private void SelectAll() => Session.SelectAll();

    [RelayCommand]
    private void SelectNone() => Session.SelectNone();

    [RelayCommand]
    private void OpenRow(ImportRow? row)
    {
        if (row is not null)
            Guard(() => Session.Open(row.Entry.Index), "opening a song");
    }

    [RelayCommand]
    private void BackToList() => Guard(Session.CloseReview, "closing the review");

    [RelayCommand]
    private void PreviousSong() => Guard(Session.Previous, "going to the previous song");

    [RelayCommand]
    private void NextSong() => Guard(Session.Next, "going to the next song");

    [RelayCommand]
    private async Task RestoreAsync()
    {
        if (await ConfirmAsync("Ripristina", "Riportare il ChordPro a quello proposto dall'app? Le tue modifiche a questo canto andranno perse.", "Ripristina"))
        {
            Guard(Session.Restore, "restoring a song");
            SchedulePreview(TimeSpan.Zero);
        }
    }

    [RelayCommand]
    private void SaveCurrent() => Guard(Session.RequestSaveCurrent, "opening the declaration");

    [RelayCommand]
    private void SaveSelected() => Guard(Session.RequestSaveSelected, "opening the declaration");

    [RelayCommand]
    private void CancelDeclaration() => Guard(Session.CancelDeclaration, "closing the declaration");

    [RelayCommand]
    private void ToggleDeclaration() => Guard(() => Session.SetDeclaration(!Session.DeclarationAccepted), "ticking the declaration");

    [RelayCommand]
    private async Task ConfirmSaveAsync()
    {
        try
        {
            if (await Session.SaveAsync((entry, ct) => _saver.SaveAsync(entry, ct)) is not null && Session.Banner is { } banner)
                Announce(banner.Text);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Saving the imported songs failed");
            Session.CancelDeclaration();
            await ShowAsync("Salvataggio non riuscito", "Non è stato possibile salvare i canti. Riprova.");
        }
    }

    [RelayCommand]
    private void DismissBanner() => Guard(Session.DismissBanner, "closing the banner");

    [RelayCommand]
    private void NextUnsaved() => Guard(Session.OpenNextUnsaved, "opening the next song");

    [RelayCommand]
    private Task OpenBannerSongAsync() => OpenSavedAsync(Session.Banner?.OpenIndex);

    [RelayCommand]
    private Task OpenRowSavedAsync(ImportRow? row) => OpenSavedAsync(row?.Entry.Index);

    [RelayCommand]
    private async Task CopyTimesAsync()
    {
        try
        {
            await Clipboard.Default.SetTextAsync(Session.TimesTable());
            Announce("Tempi copiati negli appunti");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Copying the times failed");
            await ShowAsync("Non riesco a copiare", "Gli appunti non rispondono.");
        }
    }

    [RelayCommand]
    private Task ExplainIndexAsync() =>
        ShowAsync("L'indice", "L'indice può contare anche intestazioni di sezione e titoli tradotti.");

    [RelayCommand]
    private Task ShowUnreadDetailsAsync() => ShowAsync("Pagine non lette",
        string.Join("\n", Session.Result?.UnreadPages.Select(p => $"Pagina {p.Page}: {p.Reason}") ?? Array.Empty<string>()));

    [RelayCommand]
    private void ShowPanel(string? panel)
    {
        if (panel is "0" or "1" or "2" && int.TryParse(panel, out var index))
        {
            NarrowPanel = index;
            MediumLeftPanel = index == 0 ? 0 : 1;
        }
    }

    /// <summary>Pauses the timer when the page or the app goes away.</summary>
    public void OnPageHidden() => Guard(Session.PauseTimer, "pausing the timer");

    /// <summary>Restarts the timer when the page comes back.</summary>
    public void OnPageShown() => Guard(Session.ResumeTimer, "resuming the timer");

    /// <summary>Handles a back gesture; true when it was used inside the page.</summary>
    public bool HandleBack()
    {
        if (DeclarationOpen && !IsSaving)
            CancelDeclaration();
        else if (IsReading)
            CancelReading();
        else if (IsReviewing && !IsWide && Session.Entries.Count > 1)
            BackToList();
        else
            return false;

        return true;
    }

    /// <summary>Handles a Windows shortcut; true when it was used.</summary>
    public bool HandleKey(string key, bool ctrl, bool alt)
    {
        switch (key)
        {
            case "Escape" when DeclarationOpen && !IsSaving:
                CancelDeclaration();
                return true;
            case "Escape" when IsReading:
                CancelReading();
                return true;
            case "S" when ctrl && IsReviewing && CanEdit && !DeclarationOpen:
                SaveCurrent();
                return true;
            case "O" when ctrl && !IsReading && !DeclarationOpen:
                _ = ChoosePdfAsync();
                return true;
            case "Left" when alt && ShowNavigation && !DeclarationOpen:
                PreviousSong();
                return true;
            case "Right" when alt && ShowNavigation && !DeclarationOpen:
                NextSong();
                return true;
            default:
                return false;
        }
    }

    /// <summary>Asks before leaving with edited songs that are not saved; true when it is fine to go on.</summary>
    public async Task<bool> ConfirmLeaveAsync()
    {
        var count = Session.ModifiedUnsavedCount;
        if (count == 0)
            return true;

        var text = count == 1 ? "Hai 1 canto modificato non salvato. Uscire e perderlo?" : $"Hai {count} canti modificati non salvati. Uscire e perderli?";
        return await ConfirmAsync("Modifiche non salvate", text, "Esci");
    }

    private (string Name, string Path)? _lastSource;

    private async Task RunImportAsync(string name, Func<Task<string>> source, bool ownsTemp)
    {
        _importCts?.Cancel();
        var cts = _importCts = new CancellationTokenSource();
        Session.BeginReading(name);
        _lastAnnounced = 0;
        _wakeLease?.Dispose();
        _wakeLease = _wakeLock.Acquire();
        try
        {
            var path = await source();
            _lastSource = (name, path);
            var progress = new Progress<SongbookImportProgress>(Session.Report);
            var result = await Task.Run(() => ImportFileAsync(path, name, progress, cts.Token), cts.Token);
            if (!cts.IsCancellationRequested)
                Session.Complete(result);
        }
        catch (OperationCanceledException)
        {
            _log.LogInformation("The import was cancelled");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Importing {File} failed", name);
            if (!cts.IsCancellationRequested)
                Session.Fail(ex);
        }
        finally
        {
            if (ReferenceEquals(_importCts, cts))
                AfterSessionEnded(keepTemp: ownsTemp && Session.Phase == ImportPhase.Error);
        }
    }

    private async Task<SongbookImportResult> ImportFileAsync(string path, string name, IProgress<SongbookImportProgress> progress, CancellationToken ct)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
        return await _import.ImportAsync(stream, name, progress, ct);
    }

    private async Task<string> CopyToTempAsync(FileResult file)
    {
        DeleteTemp();
        var path = Path.Combine(FileSystem.CacheDirectory, $"import-{Guid.NewGuid():N}.pdf");
        await using var input = await file.OpenReadAsync();
        await using var output = File.Create(path);
        await input.CopyToAsync(output);
        _tempPath = path;
        return path;
    }

    private void AfterSessionEnded(bool keepTemp = false)
    {
        _wakeLease?.Dispose();
        _wakeLease = null;
        if (!keepTemp && Session.Phase != ImportPhase.Reading)
            DeleteTemp();
    }

    private void DeleteTemp()
    {
        try
        {
            if (_tempPath is not null && File.Exists(_tempPath))
                File.Delete(_tempPath);
        }
        catch (Exception ex)
        {
            _log.LogDebug(ex, "Deleting the temporary PDF failed");
        }

        _tempPath = null;
    }

    private void SchedulePreview(TimeSpan delay)
    {
        _previewCts?.Cancel();
        var cts = _previewCts = new CancellationTokenSource();
        _ = RefreshPreviewAsync(delay, cts.Token);
    }

    private async Task RefreshPreviewAsync(TimeSpan delay, CancellationToken ct)
    {
        try
        {
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, ct);

            if (Session.Current?.ChordPro is not { } text || !Session.IsReviewing)
                return;

            var settings = _settings.Load();
            var check = _preview.Create(text, 400, new CanvasTextMeasurer(LeggioStyle.Load()), settings, PreviewLanguage);
            if (!check.IsValid)
            {
                PreviewWarning = "! Anteprima non aggiornata: controlla il testo";
                return;
            }

            PreviewSong = _display.Prepare(new SongDisplayRequest(text, 0, settings, PreviewLanguage));
            PreviewWarning = null;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Updating the preview failed");
            PreviewWarning = "! Anteprima non aggiornata: controlla il testo";
        }
    }

    private async Task OpenSavedAsync(int? index)
    {
        try
        {
            if (index is not { } i || i >= Session.Entries.Count || Session.Entries[i].SavedSongId is not { } id)
                return;

            var song = await _songs.GetByIdAsync(id);
            if (song is null)
            {
                await ShowAsync("Canto non trovato", "Il canto salvato non è più nella libreria.");
                return;
            }

            await Shell.Current.GoToAsync("songdetail", new Dictionary<string, object> { { "Song", song } });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Opening the saved song failed");
            await ShowAsync("Non riesco ad aprire il canto", "Riprova dalla libreria.");
        }
    }

    private void Guard(Action action, string what)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed {Action}", what);
        }
    }

    private static async Task ShowAsync(string title, string message)
    {
        try
        {
            if (Shell.Current is { } shell)
                await shell.DisplayAlertAsync(title, message, "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Showing a message failed: {ex}");
        }
    }

    private static async Task<bool> ConfirmAsync(string title, string message, string accept)
    {
        try
        {
            return Shell.Current is { } shell && await shell.DisplayAlertAsync(title, message, accept, "Annulla");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Showing a confirmation failed: {ex}");
            return false;
        }
    }
}
