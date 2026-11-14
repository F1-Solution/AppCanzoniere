using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Display;

/// <summary>Tells which notation a song's chords are written in.</summary>
internal static class ChordNotationDetector
{
    /// <summary>Italian when at least as many chords read as Do-Re-Mi as read as C-D-E, and one does; else English.</summary>
    public static NotationOptions Detect(IEnumerable<string> chords)
    {
        var italian = 0;
        var english = 0;
        foreach (var chord in chords)
        {
            if (ChordSymbolParser.TryParse(chord, Notation.Italian, out _))
                italian++;
            if (ChordSymbolParser.TryParse(chord, Notation.English, out _))
                english++;
        }

        return italian > 0 && italian >= english ? NotationOptions.Italian : NotationOptions.English;
    }
}
