#if DEBUG
namespace AppCanzoniere.MAUI;

/// <summary>Debug builds only: opens the import page and imports the PDF at APPCANZONIERE_UITEST_IMPORT, with no file picker.</summary>
internal static class ImportTestHook
{
    public const string Variable = "APPCANZONIERE_UITEST_IMPORT";

    /// <summary>Milliseconds to wait in the reading state before the file is opened, so a test can cancel a reading in progress.</summary>
    public const string DelayVariable = "APPCANZONIERE_UITEST_IMPORT_DELAY_MS";

    public static bool IsRequested => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Variable));

    /// <summary>Appends a line to import-test.log next to the app, so a UI test can read what happened.</summary>
    public static void Log(string line)
    {
        try
        {
            File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "import-test.log"), $"{DateTime.Now:HH:mm:ss.fff} {line}{Environment.NewLine}");
        }
        catch (IOException)
        {
        }
    }

    public static async Task RunAsync(Shell shell)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) => Log("UNHANDLED " + e.ExceptionObject);
        TaskScheduler.UnobservedTaskException += (_, e) => Log("UNOBSERVED " + e.Exception);
        try
        {
            var path = Environment.GetEnvironmentVariable(Variable)!;
            Log("navigating");
            await shell.GoToAsync("//import");
            Log("navigated");
            for (var i = 0; i < 50 && shell.CurrentPage?.BindingContext is not ViewModels.ImportViewModel; i++)
                await Task.Delay(100);

            if (shell.CurrentPage?.BindingContext is ViewModels.ImportViewModel vm)
            {
                vm.Session.Changed += () => Log(vm.AutomationStatus + ";" + vm.ProgressPageText);
                var delay = int.TryParse(Environment.GetEnvironmentVariable(DelayVariable), out var ms) ? TimeSpan.FromMilliseconds(ms) : TimeSpan.Zero;
                await vm.ImportFromPathAsync(path, delay);
            }
        }
        catch (Exception ex)
        {
            Log("HOOK FAILED " + ex);
        }
    }
}
#endif
