using System.Diagnostics;
using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Display;

/// <summary>What the song screen has to show for one song at one transposition.</summary>
/// <param name="ChordPro">The song as stored; empty when it has only another kind of content.</param>
/// <param name="OtherContent">Text to show when there is no ChordPro; null shows a placeholder.</param>
/// <param name="AppLanguage">Language picked in the app settings; blank when none is stored.</param>
/// <param name="SystemLanguage">Language of the device, used when the app has none.</param>
public sealed record SongScreenRequest(
    string? ChordPro,
    string? OtherContent,
    int TransposeSteps,
    ChordDisplaySettings Settings,
    string? AppLanguage,
    string? SystemLanguage);

/// <summary>The texts of the song screen; an empty key, capo or error means nothing to show.</summary>
public sealed record SongScreen(string Text, string Key, string Capo, string Error);

/// <summary>Prepares what the song screen shows, falling back to the stored text when the layout fails.</summary>
public sealed class SongScreenPresenter
{
    public const string NoContent = "No content available";
    public const string LayoutFailed = "The chords could not be laid out, so the song is shown as stored.";

    private readonly ISongDisplayService _display;

    public SongScreenPresenter(ISongDisplayService display)
    {
        ArgumentNullException.ThrowIfNull(display);
        _display = display;
    }

    public SongScreen Present(SongScreenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrEmpty(request.ChordPro))
            return new SongScreen(request.OtherContent ?? NoContent, string.Empty, string.Empty, string.Empty);

        try
        {
            var language = string.IsNullOrWhiteSpace(request.AppLanguage) ? request.SystemLanguage : request.AppLanguage.Trim();
            var shown = _display.Render(new SongDisplayRequest(request.ChordPro, request.TransposeSteps, request.Settings, language));
            var capo = shown.Capo > 0 ? $"Capo {shown.Capo}" : string.Empty;
            return new SongScreen(shown.Text, shown.Key, capo, string.Empty);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Song display failed: {ex}");
            return new SongScreen(request.ChordPro, string.Empty, string.Empty, LayoutFailed);
        }
    }
}
