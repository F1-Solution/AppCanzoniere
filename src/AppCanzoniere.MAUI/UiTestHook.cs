#if DEBUG
using AppCanzoniere.Domain.Entities;

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
        var song = new Song { Title = "Prova automatica", Artist = "Test", ChordProContent = SyntheticSong() };
        await shell.GoToAsync("songdetail", new Dictionary<string, object> { { "Song", song } });
        if (edit && shell.CurrentPage?.BindingContext is ViewModels.SongDetailViewModel vm)
            vm.ToggleEditModeCommand.Execute(null);
    }

    // Long enough for several screens at any window size used by the tests.
    private static string SyntheticSong()
    {
        var lines = new List<string> { "{title: Prova automatica}", "{key: C}" };
        for (var verse = 1; verse <= 12; verse++)
        {
            lines.Add($"{{comment: Strofa {verse}}}");
            for (var row = 1; row <= 6; row++)
                lines.Add($"[C]Riga {verse}.{row} di prova [G]per il [Am]leggio [F]automatico");
            lines.Add(string.Empty);
        }

        return string.Join('\n', lines);
    }
}
#endif
