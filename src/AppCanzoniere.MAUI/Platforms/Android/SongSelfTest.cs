#if DEBUG
using Android.Util;
using AppCanzoniere.Application.Diagnostics;

namespace AppCanzoniere.MAUI;

/// <summary>Debug-only: the intent extra "selftest=song" opens the song screen on an invented long song.</summary>
internal static class SongSelfTest
{
    public const string LogTag = "AppCanzoniereSelfTest";

    public static void RunIfRequested(string? extra)
    {
        if (!string.Equals(extra, "song", StringComparison.OrdinalIgnoreCase))
            return;
        _ = Task.Run(OpenAsync);
    }

    private static async Task OpenAsync()
    {
        try
        {
            for (var i = 0; i < 120 && Shell.Current?.CurrentPage is null; i++)
                await Task.Delay(500).ConfigureAwait(false);

            await MainThread.InvokeOnMainThreadAsync(() => Shell.Current!.GoToAsync("songdetail", new Dictionary<string, object>
            {
                { "Song", SyntheticSong.Create() }
            }));
            Log.Info(LogTag, "SONG OPENED");
        }
        catch (Exception ex)
        {
            Log.Info(LogTag, $"SONG FAIL {ex.GetType().Name}: {ex.Message}");
        }
    }
}
#endif
