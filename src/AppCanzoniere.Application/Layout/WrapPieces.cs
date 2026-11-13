namespace AppCanzoniere.Application.Layout;

/// <summary>An atom of a wrapped line: text between chord positions and word starts; Candidate marks a legal row start.</summary>
internal readonly record struct WrapPiece(int Start, int End, int ChordIndex, double TextWidth, double ChordWidth, bool Candidate);

/// <summary>Running width of a row under construction, following the chord advance rules.</summary>
internal struct RowAccumulator
{
    private readonly double _gap;
    private double _done;
    private double _segLyric;
    private double _segChord;
    private bool _segHasChord;
    private bool _open;

    public RowAccumulator(double gap)
    {
        _gap = gap;
    }

    public void Add(in WrapPiece piece)
    {
        if (piece.ChordIndex >= 0)
        {
            if (_open)
                _done += SegmentMath.Advance(_segHasChord, _segLyric, _segChord, false, _gap);
            _segHasChord = true;
            _segChord = piece.ChordWidth;
            _segLyric = piece.TextWidth;
        }
        else if (_open)
        {
            _segLyric += piece.TextWidth;
        }
        else
        {
            _segHasChord = false;
            _segChord = 0;
            _segLyric = piece.TextWidth;
        }

        _open = true;
    }

    /// <summary>Row width if it ended now, with the last piece shortened by <paramref name="trimmedAway"/>.</summary>
    public readonly double Width(double trimmedAway) =>
        _done + SegmentMath.Advance(_segHasChord, _segLyric - trimmedAway, _segChord, true, _gap);
}

/// <summary>Cuts a line into wrap pieces and finds where a row may start.</summary>
internal static class WrapPieces
{
    public static List<WrapPiece> Build(in LineModel model, IReadOnlyList<Seg> segments, FlowContext ctx)
    {
        var candidates = Candidates(model.Text, model.Chords);
        var pieces = new List<WrapPiece>(segments.Count + candidates.Count);
        var next = 0;
        foreach (var seg in segments)
        {
            var flagged = next < candidates.Count && candidates[next] == seg.Start && seg.Start > 0;
            if (flagged)
                next++;

            var start = seg.Start;
            var chord = seg.ChordIndex;
            var chordWidth = seg.ChordWidth;
            var split = false;
            while (next < candidates.Count && candidates[next] < seg.End)
            {
                var cut = candidates[next++];
                pieces.Add(Piece(model, ctx, start, cut, chord, chordWidth, flagged));
                (start, chord, chordWidth, flagged, split) = (cut, -1, 0, true, true);
            }

            pieces.Add(split
                ? Piece(model, ctx, start, seg.End, chord, chordWidth, flagged)
                : new WrapPiece(seg.Start, seg.End, seg.ChordIndex, seg.LyricWidth, seg.ChordWidth, flagged));
        }

        return pieces;
    }

    private static WrapPiece Piece(in LineModel model, FlowContext ctx, int start, int end, int chord, double chordWidth, bool candidate) =>
        new(start, end, chord, ctx.WidthOf(model.Text.Substring(start, end - start), model.LyricRole), chordWidth, candidate);

    /// <summary>Word starts, moved to the first chord of the whitespace run before them so the chord travels with the word.</summary>
    private static List<int> Candidates(string text, IReadOnlyList<LayoutChord> chords)
    {
        var result = new List<int>();
        var chord = 0;
        for (var b = 1; b < text.Length; b++)
        {
            if (char.IsWhiteSpace(text[b]) || !char.IsWhiteSpace(text[b - 1]))
                continue;

            var runStart = b - 1;
            while (runStart > 0 && char.IsWhiteSpace(text[runStart - 1]))
                runStart--;

            if (runStart == 0)
                continue;

            while (chord < chords.Count && chords[chord].Position < runStart)
                chord++;
            result.Add(chord < chords.Count && chords[chord].Position <= b ? chords[chord].Position : b);
        }

        return result;
    }
}
