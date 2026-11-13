namespace AppCanzoniere.Application.Layout;

/// <summary>The text, chords and role of one lyric line being laid out.</summary>
internal readonly record struct LineModel(string Text, IReadOnlyList<LayoutChord> Chords, TextRole LyricRole, bool Instrumental)
{
    public static bool IsBlank(string text) => string.IsNullOrWhiteSpace(text);

    public static bool IsAnnotation(string chordText) => chordText.Length > 0 && chordText[0] == '*';
}

/// <summary>The segments of one visual row and whether it ends in a hard break.</summary>
internal sealed record RowSpec(IReadOnlyList<Seg> Segments, bool HardBreakAfter);
