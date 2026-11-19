#if DEBUG
using AppCanzoniere.Application.Diagnostics;

namespace AppCanzoniere.MAUI;

/// <summary>Debug builds only: opens a synthetic song when APPCANZONIERE_UITEST_SONG is set, so UI tests need no database or corpus.</summary>
internal static class UiTestHook
{
    public const string Variable = "APPCANZONIERE_UITEST_SONG";

    /// <summary>The value "edit" also opens the song in edit mode.</summary>
    public static bool IsRequested => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Variable));

    public static async Task OpenSongAsync(Shell shell)
    {
        var edit = string.Equals(Environment.GetEnvironmentVariable(Variable), "edit", StringComparison.OrdinalIgnoreCase);
        var song = SyntheticSong.Create();
        await shell.GoToAsync("songdetail", new Dictionary<string, object> { { "Song", song } });
        if (edit && shell.CurrentPage?.BindingContext is ViewModels.SongDetailViewModel vm)
            vm.ToggleEditModeCommand.Execute(null);
    }
}
#endif
