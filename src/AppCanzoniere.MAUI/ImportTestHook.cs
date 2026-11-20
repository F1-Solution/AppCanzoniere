#if DEBUG
namespace AppCanzoniere.MAUI;

/// <summary>Debug builds only: opens the import page and imports the PDF at APPCANZONIERE_UITEST_IMPORT, with no file picker.</summary>
internal static class ImportTestHook
{
    public const string Variable = "APPCANZONIERE_UITEST_IMPORT";

    public static bool IsRequested => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Variable));

    public static async Task RunAsync(Shell shell)
    {
        try
        {
            var path = Environment.GetEnvironmentVariable(Variable)!;
            await shell.GoToAsync("//import");
            for (var i = 0; i < 50 && shell.CurrentPage?.BindingContext is not ViewModels.ImportViewModel; i++)
                await Task.Delay(100);

            if (shell.CurrentPage?.BindingContext is ViewModels.ImportViewModel vm)
                await vm.ImportFromPathAsync(path);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"The import test hook failed: {ex}");
        }
    }
}
#endif
