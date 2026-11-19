namespace AppCanzoniere.Application.PdfImport;

/// <summary>Relative glyph widths of a typical proportional font, to place a position inside a word whose letters are unknown.</summary>
internal static class GlyphWidths
{
    private const string Narrow = "ijl.,:;!|'";
    private const string Slim = "ftr()-I1";
    private const string Wide = "mwMW";
    private const string NarrowCapitals = "EFLPSTZ";

    public static double Of(char c) => c switch
    {
        _ when Narrow.Contains(c) => 0.3,
        _ when Slim.Contains(c) => 0.38,
        _ when Wide.Contains(c) => char.IsUpper(c) ? 0.92 : 0.76,
        _ when NarrowCapitals.Contains(c) => 0.6,
        _ when char.IsUpper(c) => 0.72,
        _ => 0.5
    };
}
