#if DEBUG
using Android.Util;
using AppCanzoniere.MAUI.ViewModels;

namespace AppCanzoniere.MAUI;

/// <summary>
/// Debug-only: the intent extras "selftest=import", "importfile=NAME" (a PDF in files/import-fixtures) and "importscenario"
/// (plain, single, songbook or cancel) open the import page, import the file and log "IMPORT ..." lines to logcat.
/// </summary>
internal static class ImportSelfTest
{
    public const string LogTag = "AppCanzoniereSelfTest";
    public const string FileExtra = "importfile";
    public const string ScenarioExtra = "importscenario";
    private static int _running;

    public static void RunIfRequested(string? selftest, string? file, string? scenario)
    {
        if (!string.Equals(selftest, "import", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(file))
            return;

        if (Interlocked.Exchange(ref _running, 1) == 1)
            return;

        _ = Task.Run(() => RunAsync(file, scenario ?? "plain"));
    }

    private static async Task RunAsync(string file, string scenario)
    {
        try
        {
            for (var i = 0; i < 120 && Shell.Current?.CurrentPage is null; i++)
                await Task.Delay(500).ConfigureAwait(false);

            await MainThread.InvokeOnMainThreadAsync(() => Shell.Current!.GoToAsync("//import"));
            ImportViewModel? vm = null;
            for (var i = 0; i < 100 && vm is null; i++)
            {
                vm = Shell.Current?.CurrentPage?.BindingContext as ImportViewModel;
                if (vm is null)
                    await Task.Delay(100).ConfigureAwait(false);
            }

            if (vm is null)
            {
                Log.Info(LogTag, "IMPORT FAIL import page did not open");
                return;
            }

            var last = string.Empty;
            vm.Session.Changed += () =>
            {
                var status = vm.AutomationStatus;
                if (status != last)
                {
                    last = status;
                    Log.Info(LogTag, "IMPORT STATUS " + status);
                }
            };

            var path = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "import-fixtures", file);
            Log.Info(LogTag, $"IMPORT START {file} scenario={scenario} exists={File.Exists(path)}");
            if (scenario == "cancel")
                await RunCancelAsync(vm, path).ConfigureAwait(false);
            else if (scenario == "cancelreal")
                await RunCancelDuringRecognitionAsync(vm, path).ConfigureAwait(false);
            else
                await MainThread.InvokeOnMainThreadAsync(() => vm.ImportFromPathAsync(path));

            Log.Info(LogTag, "IMPORT DONE " + vm.AutomationStatus);
            if (scenario == "single")
                await SaveSingleAsync(vm).ConfigureAwait(false);
            else if (scenario == "songbook")
                await SaveAllAsync(vm).ConfigureAwait(false);

            Log.Info(LogTag, "IMPORT END " + vm.AutomationStatus);
        }
        catch (Exception ex)
        {
            Log.Info(LogTag, $"IMPORT FAIL {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static async Task RunCancelAsync(ImportViewModel vm, string path)
    {
        var import = MainThread.InvokeOnMainThreadAsync(() => vm.ImportFromPathAsync(path, TimeSpan.FromSeconds(6)));
        for (var i = 0; i < 50 && !vm.IsReading; i++)
            await Task.Delay(100).ConfigureAwait(false);

        Log.Info(LogTag, "IMPORT CANCELLING " + vm.AutomationStatus);
        await MainThread.InvokeOnMainThreadAsync(() => vm.CancelReadingCommand.Execute(null));
        Log.Info(LogTag, "IMPORT CANCELLED " + vm.AutomationStatus);
        // The delayed read ends after the cancel; the screen must stay on the cancelled state.
        await Task.WhenAny(import, Task.Delay(TimeSpan.FromSeconds(10))).ConfigureAwait(false);
        await Task.Delay(500).ConfigureAwait(false);
    }

    // Cancels while the page recognition is really running (no artificial delay), then checks the screen stays cancelled.
    private static async Task RunCancelDuringRecognitionAsync(ImportViewModel vm, string path)
    {
        var import = MainThread.InvokeOnMainThreadAsync(() => vm.ImportFromPathAsync(path));
        for (var i = 0; i < 600 && vm.Session.ProgressStage != AppCanzoniere.Application.PdfImport.SongbookImportStage.Recognizing && !import.IsCompleted; i++)
            await Task.Delay(50).ConfigureAwait(false);

        Log.Info(LogTag, $"IMPORT CANCELLING stage={vm.Session.ProgressStage} {vm.AutomationStatus}");
        await MainThread.InvokeOnMainThreadAsync(() => vm.CancelReadingCommand.Execute(null));
        Log.Info(LogTag, "IMPORT CANCELLED " + vm.AutomationStatus);
        await Task.WhenAny(import, Task.Delay(TimeSpan.FromSeconds(60))).ConfigureAwait(false);
        await Task.Delay(1000).ConfigureAwait(false);
    }

    private static async Task SaveSingleAsync(ImportViewModel vm)
    {
        Log.Info(LogTag, $"IMPORT CHECK reviewing={vm.IsReviewing} accepted={vm.DeclarationAccepted}");
        await MainThread.InvokeOnMainThreadAsync(() => vm.SaveCurrentCommand.Execute(null));
        Log.Info(LogTag, $"IMPORT CHECK declaration={vm.DeclarationOpen} accepted={vm.DeclarationAccepted} canSave={vm.CanConfirmSave}");
        await MainThread.InvokeOnMainThreadAsync(() => vm.ToggleDeclarationCommand.Execute(null));
        Log.Info(LogTag, $"IMPORT CHECK accepted={vm.DeclarationAccepted} canSave={vm.CanConfirmSave}");
        await MainThread.InvokeOnMainThreadAsync(() => vm.ConfirmSaveCommand.ExecuteAsync(null));
    }

    private static async Task SaveAllAsync(ImportViewModel vm)
    {
        Log.Info(LogTag, $"IMPORT CHECK all-selected canSave={vm.CanSaveSelected}");
        await MainThread.InvokeOnMainThreadAsync(() => vm.SelectNoneCommand.Execute(null));
        Log.Info(LogTag, $"IMPORT CHECK none-selected canSave={vm.CanSaveSelected}");
        await MainThread.InvokeOnMainThreadAsync(() => vm.SelectAllCommand.Execute(null));
        await MainThread.InvokeOnMainThreadAsync(() => vm.SaveSelectedCommand.Execute(null));
        Log.Info(LogTag, $"IMPORT CHECK declaration={vm.DeclarationOpen} accepted={vm.DeclarationAccepted} canSave={vm.CanConfirmSave}");
        await MainThread.InvokeOnMainThreadAsync(() => vm.ToggleDeclarationCommand.Execute(null));
        await MainThread.InvokeOnMainThreadAsync(() => vm.ConfirmSaveCommand.ExecuteAsync(null));
    }
}
#endif
