using AppCanzoniere.Application.Display;

namespace AppCanzoniere.Application.Interfaces;

public interface ISongDisplayService
{
    /// <summary>Renders a stored ChordPro song as chords above lyrics in the requested notation and transposition.</summary>
    SongDisplay Render(SongDisplayRequest request);
}
