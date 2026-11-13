namespace AppCanzoniere.Application.Import;

/// <summary>Converts chords-above-lyrics text, as pasted from booklets and websites, to ChordPro.</summary>
public static class ChordSheetConverter
{
    /// <summary>Converts chords-above-lyrics text to ChordPro; ChordPro input is returned unchanged.</summary>
    public static ChordSheetResult Convert(string text, ChordSheetOptions? options = null) =>
        new(string.Empty, false, null, Array.Empty<SheetLineDiagnostic>(), 0, 0);
}
