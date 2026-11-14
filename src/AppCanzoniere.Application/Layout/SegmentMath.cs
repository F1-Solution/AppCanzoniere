namespace AppCanzoniere.Application.Layout;

/// <summary>A stretch of line text starting at a chord, or at the line start with no chord.</summary>
internal readonly record struct Seg(int Start, int End, int ChordIndex, double LyricWidth, double ChordWidth);

/// <summary>The horizontal advance rules of the chord-over-lyric placement.</summary>
internal static class SegmentMath
{
    public const double FitTolerance = 1e-9;

    /// <summary>Advance of a segment: a chord keeps its gap from the next chord unless it is last on the row.</summary>
    public static double Advance(bool hasChord, double lyricWidth, double chordWidth, bool last, double gap)
    {
        if (!hasChord)
            return lyricWidth;
        return Math.Max(lyricWidth, last ? chordWidth : chordWidth + gap);
    }

    public static double Advance(in Seg seg, bool last, double gap) =>
        Advance(seg.ChordIndex >= 0, seg.LyricWidth, seg.ChordWidth, last, gap);

    public static double RowWidth(IReadOnlyList<Seg> segs, double gap)
    {
        var width = 0.0;
        for (var i = 0; i < segs.Count; i++)
            width += Advance(segs[i], i == segs.Count - 1, gap);
        return width;
    }
}
