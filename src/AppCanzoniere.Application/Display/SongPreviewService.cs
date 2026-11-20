using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.Application.Display;

/// <summary>A preview drawing, or the reason none could be made; exactly one of them is set.</summary>
public sealed record SongPreview(SongDrawing? Drawing, string? Error)
{
    public bool IsValid => Drawing is not null;
}

/// <summary>Turns ChordPro being edited into the song drawing model for a live preview, never throwing on bad text.</summary>
public interface ISongPreviewService
{
    SongPreview Create(string? chordPro, double width, ITextMeasurer measurer, ChordDisplaySettings? settings = null, string? languageTag = null);
}

/// <inheritdoc />
public sealed class SongPreviewService(ISongDisplayService display) : ISongPreviewService
{
    public SongPreview Create(string? chordPro, double width, ITextMeasurer measurer, ChordDisplaySettings? settings = null, string? languageTag = null)
    {
        if (measurer is null)
            return new SongPreview(null, "No text measurer was given.");
        if (!double.IsFinite(width) || width <= 0)
            return new SongPreview(null, "The preview width is not usable.");

        try
        {
            var prepared = display.Prepare(new SongDisplayRequest(chordPro ?? string.Empty, 0, settings ?? ChordDisplaySettings.Default, languageTag));
            return new SongPreview(prepared.LayoutAt(width, measurer), null);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            return new SongPreview(null, string.IsNullOrWhiteSpace(ex.Message) ? "The text could not be previewed." : ex.Message);
        }
    }
}
