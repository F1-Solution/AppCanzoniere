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

        var section = new OpenSection(ChordProLineType.Verse, false);
        var inTab = false;
        for (var i = 0; i < body.Count; i++)
        {
            var line = body[i];
            var continuesTab = inTab && line.Type == ChordProLineType.Tab && !line.StartsTabBlock;
            if (inTab && !continuesTab)
            {
                sb.Append("{end_of_tab}\n");
                inTab = false;
            }

            if (!continuesTab)
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

        AppendSectionEnd(sb, section.Kind);
        return sb.ToString();
    }

    private static bool AppendMetadata(StringBuilder sb, ChordProDocument document, ChordTransformer transformer, ChordProRenderOptions options)
    {
        var start = sb.Length;
        AppendDirective(sb, "title", document.Title);
        AppendDirective(sb, "artist", document.Artist);
        AppendDirective(sb, "key", transformer.Key(HeaderText.Clean(document.Key)));
        AppendDirective(sb, "capo", options.Capo is { } fret ? (fret > 0 ? fret.ToString() : string.Empty) : document.Capo);
        AppendDirective(sb, "tempo", document.Tempo);
        AppendDirective(sb, "time", document.Time);
        return sb.Length > start;
    }

    private static void AppendDirective(StringBuilder sb, string name, string? value)
    {
        var clean = HeaderText.Clean(value);
        if (clean.Length > 0)
            sb.Append('{').Append(name).Append(':').Append(clean).Append("}\n");
    }

    // Closes and opens sections for the line at the index and returns the section that is open for it.
    private static OpenSection MoveSection(StringBuilder sb, List<ChordProLine> body, int index, OpenSection open)
    {
        var line = body[index];
        var next = SectionOf(body, index, open.Kind);
        if (next == ChordProLineType.Verse)
        {
            AppendSectionEnd(sb, open.Kind);
            return new OpenSection(next, false);
        }

        // A section opened by a blank or tab line is not reopened by the first line that carries its start.
        if (next == open.Kind && !(line.StartsSection && open.Started))
            return open with { Started = open.Started || line.StartsSection };

        AppendSectionEnd(sb, open.Kind);
        sb.Append(next == ChordProLineType.Chorus ? "{start_of_chorus}\n" : "{start_of_bridge}\n");
        return new OpenSection(next, line.StartsSection);
    }

    private static ChordProLineType SectionOf(List<ChordProLine> body, int index, ChordProLineType open)
    {
        var line = body[index];
        if (line.Type is ChordProLineType.Chorus or ChordProLineType.Bridge)
            return line.Type;

        if (line.Type == ChordProLineType.Verse || !(line.StartsSection || line.InOpenSection))
            return ChordProLineType.Verse;

        if (line.StartsSection || open == ChordProLineType.Verse)
            return TypeOfNextSection(body, index);

        return line.Type == ChordProLineType.Tab ? TabSection(body, index, open) : open;
    }

    // A tab right before a lyric line that starts a section of another kind is the first line of that section.
    private static ChordProLineType TabSection(List<ChordProLine> body, int index, ChordProLineType open)
    {
        for (var i = index + 1; i < body.Count; i++)
        {
            var line = body[i];
            if (line.Type is ChordProLineType.Chorus or ChordProLineType.Bridge)
                return line.StartsSection ? line.Type : open;

            if (line.Type is not (ChordProLineType.Tab or ChordProLineType.Empty))
                break; // A comment would take the start of the new section.
        }

        return open;
    }

    // A comment, blank line or tab that opens a section does not say its kind, so the first lyric line inside it does.
    private static ChordProLineType TypeOfNextSection(List<ChordProLine> body, int index)
    {
        var seenStart = body[index].StartsSection;
        for (var i = index + 1; i < body.Count; i++)
        {
            var line = body[i];
            if (line.Type is ChordProLineType.Chorus or ChordProLineType.Bridge)
                return line.Type;

            if (line.Type == ChordProLineType.Verse || (line.StartsSection && seenStart))
                break;

            seenStart |= line.StartsSection;
        }

        return ChordProLineType.Chorus;
    }

    private readonly record struct OpenSection(ChordProLineType Kind, bool Started);

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
