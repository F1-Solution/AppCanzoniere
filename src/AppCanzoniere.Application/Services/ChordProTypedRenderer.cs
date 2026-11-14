using System.Text;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Services;

/// <summary>Writes a document from its typed lines, following the section flags the parser sets.</summary>
internal static class ChordProTypedRenderer
{
    public static string Render(ChordProDocument document, ChordTransformer transformer, ChordProRenderOptions options)
    {
        var sb = new StringBuilder();
        var hasMetadata = AppendMetadata(sb, document, transformer, options);
        var body = document.Lines.Where(l => l.Type != ChordProLineType.Title).ToList();
        if (hasMetadata && body.Count > 0 && body[0].Type != ChordProLineType.Empty)
            sb.Append('\n');

        var section = ChordProLineType.Verse;
        var inTab = false;
        for (var i = 0; i < body.Count; i++)
        {
            var line = body[i];
            if (inTab && !(line.Type == ChordProLineType.Tab && !line.StartsTabBlock))
            {
                sb.Append("{end_of_tab}\n");
                inTab = false;
            }

            section = MoveSection(sb, body, i, section);
            if (line.Type == ChordProLineType.Tab && !inTab)
            {
                sb.Append(line.TabLabel.Length > 0 ? $"{{start_of_tab:{line.TabLabel}}}\n" : "{start_of_tab}\n");
                inTab = true;
            }

            AppendLine(sb, line, transformer);
        }

        if (inTab)
            sb.Append("{end_of_tab}\n");

        AppendSectionEnd(sb, section);
        return sb.ToString();
    }

    private static bool AppendMetadata(StringBuilder sb, ChordProDocument document, ChordTransformer transformer, ChordProRenderOptions options)
    {
        var start = sb.Length;
        if (!string.IsNullOrEmpty(document.Title))
            sb.Append($"{{title:{document.Title}}}\n");
        if (!string.IsNullOrEmpty(document.Artist))
            sb.Append($"{{artist:{document.Artist}}}\n");
        if (!string.IsNullOrEmpty(document.Key))
            sb.Append($"{{key:{transformer.Key(document.Key)}}}\n");

        var capo = options.Capo is { } fret ? (fret > 0 ? fret.ToString() : string.Empty) : document.Capo;
        if (!string.IsNullOrEmpty(capo))
            sb.Append($"{{capo:{capo}}}\n");
        if (!string.IsNullOrEmpty(document.Tempo))
            sb.Append($"{{tempo:{document.Tempo}}}\n");
        if (!string.IsNullOrEmpty(document.Time))
            sb.Append($"{{time:{document.Time}}}\n");
        return sb.Length > start;
    }

    // Closes and opens sections for the line at the index and returns the section that is open for it.
    private static ChordProLineType MoveSection(StringBuilder sb, List<ChordProLine> body, int index, ChordProLineType open)
    {
        var line = body[index];
        var next = SectionOf(body, index, open);
        var opensNew = line.StartsSection && next != ChordProLineType.Verse;
        if (next == open && !opensNew)
            return open;

        AppendSectionEnd(sb, open);
        if (next == ChordProLineType.Chorus)
            sb.Append("{start_of_chorus}\n");
        else if (next == ChordProLineType.Bridge)
            sb.Append("{start_of_bridge}\n");

        return next;
    }

    private static ChordProLineType SectionOf(List<ChordProLine> body, int index, ChordProLineType open)
    {
        var line = body[index];
        if (line.Type is ChordProLineType.Chorus or ChordProLineType.Bridge)
            return line.Type;

        if (line.Type == ChordProLineType.Verse || !(line.StartsSection || line.InOpenSection))
            return ChordProLineType.Verse;

        return open != ChordProLineType.Verse && !line.StartsSection ? open : TypeOfNextSection(body, index);
    }

    // A comment, blank line or tab that opens a section does not say its kind, so the first line inside it does.
    private static ChordProLineType TypeOfNextSection(List<ChordProLine> body, int index)
    {
        for (var i = index + 1; i < body.Count; i++)
        {
            var line = body[i];
            if (line.Type is ChordProLineType.Chorus or ChordProLineType.Bridge)
                return line.StartsSection ? ChordProLineType.Chorus : line.Type;

            if (line.Type == ChordProLineType.Verse || line.StartsSection)
                break;
        }

        return ChordProLineType.Chorus;
    }

    private static void AppendSectionEnd(StringBuilder sb, ChordProLineType type)
    {
        if (type == ChordProLineType.Chorus)
            sb.Append("{end_of_chorus}\n");
        else if (type == ChordProLineType.Bridge)
            sb.Append("{end_of_bridge}\n");
    }

    private static void AppendLine(StringBuilder sb, ChordProLine line, ChordTransformer transformer)
    {
        switch (line.Type)
        {
            case ChordProLineType.Empty:
                sb.Append('\n');
                break;
            case ChordProLineType.Comment:
                sb.Append($"{{comment:{line.Text}}}\n");
                break;
            case ChordProLineType.Tab:
                sb.Append(line.Text).Append('\n');
                break;
            case ChordProLineType.Title:
                break; // Written with the metadata.
            default:
                sb.Append(RenderChordLine(line, transformer)).Append('\n');
                break;
        }
    }

    private static string RenderChordLine(ChordProLine line, ChordTransformer transformer)
    {
        var result = new StringBuilder();
        var cursor = 0;
        foreach (var chord in (line.Chords ?? new List<ChordPosition>()).OrderBy(c => c.Position))
        {
            var position = Math.Clamp(chord.Position, cursor, line.Text.Length);
            result.Append(line.Text, cursor, position - cursor);
            var text = transformer.Chord(chord.Chord);
            result.Append('[').Append(text).Append(']');
            cursor = position;
        }

        return result.Append(line.Text, cursor, line.Text.Length - cursor).ToString();
    }
}
