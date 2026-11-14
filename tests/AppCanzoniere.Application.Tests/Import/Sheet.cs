using AppCanzoniere.Application.Import;

namespace AppCanzoniere.Application.Tests.Import;

/// <summary>Helpers shared by the chords-above-lyrics tests; expectations follow the spec's test plan.</summary>
internal static class Sheet
{
    public static ChordSheetResult Run(params string[] lines) =>
        ChordSheetConverter.Convert(string.Join("\n", lines));

    public static ChordSheetResult RunWith(ChordSheetOptions options, params string[] lines) =>
        ChordSheetConverter.Convert(string.Join("\n", lines), options);

    /// <summary>Expected output: lines joined and terminated by a newline.</summary>
    public static string Out(params string[] lines) => string.Join("\n", lines) + "\n";

    /// <summary>Diagnostic of the 1-based input line.</summary>
    public static SheetLineDiagnostic Diag(this ChordSheetResult result, int lineNumber) => result.Lines[lineNumber - 1];
}
