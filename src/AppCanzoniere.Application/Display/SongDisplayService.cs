using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Display;

/// <summary>Turns stored ChordPro into the text the song screen shows.</summary>
public sealed class SongDisplayService : ISongDisplayService
{
    public SongDisplayService(IChordProParser parser) => throw new NotImplementedException();

    public SongDisplay Render(SongDisplayRequest request) => throw new NotImplementedException();
}
