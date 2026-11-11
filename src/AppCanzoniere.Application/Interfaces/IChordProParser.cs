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
}

public class ChordProDocument
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Capo { get; set; } = string.Empty;

    /// <summary>Capo fret read from <see cref="Capo"/>; null when absent or not a fret from 0 to 11.</summary>
    public int? CapoFret => null;
    public string Tempo { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public List<ChordProLine> Lines { get; set; } = new();
}

public class ChordProLine
{
    public ChordProLineType Type { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<ChordPosition> Chords { get; set; } = new();
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
    Empty
}
