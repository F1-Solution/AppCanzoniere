using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Interfaces;

public interface IChordProParser
{
    /// <summary>
    /// Parse ChordPro content and extract chords and lyrics
    /// </summary>
    ChordProDocument Parse(string chordProContent);

    /// <summary>
    /// Render ChordPro document with transposed chords
    /// </summary>
    string Render(ChordProDocument document, int transposeSteps = 0);

    /// <summary>Render with transposition, spelling, notation and capo.</summary>
    string Render(ChordProDocument document, ChordProRenderOptions options);
}

public class ChordProDocument
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Capo { get; set; } = string.Empty;

    /// <summary>Capo fret read from <see cref="Capo"/>; null when absent or not a fret from 0 to 11.</summary>
    public int? CapoFret =>
        int.TryParse(Capo?.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var fret) && fret <= 11 ? fret : null;
    public string Tempo { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public List<ChordProLine> Lines { get; set; } = new();
}

public class ChordProLine
{
    public ChordProLineType Type { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<ChordPosition> Chords { get; set; } = new();

    /// <summary>True on the first line of a tab block.</summary>
    public bool StartsTabBlock { get; set; }

    /// <summary>Optional label of the tab block, set on its first line.</summary>
    public string TabLabel { get; set; } = string.Empty;
}

public class ChordPosition
{
    public string Chord { get; set; } = string.Empty;
    public int Position { get; set; }
}

public enum ChordProLineType
{
    Verse,
    Chorus,
    Bridge,
    Title,
    Comment,
    Empty,

    /// <summary>Line of a tab block: kept verbatim, monospace, never transposed or converted.</summary>
    Tab
}
