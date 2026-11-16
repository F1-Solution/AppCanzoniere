using AppCanzoniere.Application.Display;

namespace AppCanzoniere.Application.Interfaces;

public interface ISongDisplayService
{
    /// <summary>Prepares a stored ChordPro song in the requested notation and transposition, ready to lay out at any width.</summary>
    PreparedSong Prepare(SongDisplayRequest request);
}
