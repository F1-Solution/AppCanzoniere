namespace AppCanzoniere.Application.PdfImport;

/// <summary>One monospaced output line and the row it came from; Row is null for a blank line between stanzas.</summary>
public sealed record AlignedLine(string Text, TextRow? Row);

/// <summary>Turns the rows of one region into monospaced lines that keep each word over the character it sits over on the page.</summary>
public static class AlignedTextRenderer
{
    public static IReadOnlyList<AlignedLine> Render(IReadOnlyList<TextRow> rows) => Array.Empty<AlignedLine>();
}
