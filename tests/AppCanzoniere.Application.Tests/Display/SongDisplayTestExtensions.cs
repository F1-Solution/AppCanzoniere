using AppCanzoniere.Application.Display;

namespace AppCanzoniere.Application.Tests.Display;

internal static class SongDisplayTestExtensions
{
    /// <summary>The song as the old monospace screen showed it.</summary>
    public static SongDisplay Render(this SongDisplayService service, SongDisplayRequest request)
    {
        var prepared = service.Prepare(request);
        return new SongDisplay(prepared.ToMonospaceText(), prepared.Key, prepared.Capo);
    }
}
