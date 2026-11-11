using System.Text;
using System.Text.RegularExpressions;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Services;

public class ChordProParser : IChordProParser
{
    private static readonly Regex ChordRegex = new Regex(@"\[([^\]]+)\]", RegexOptions.Compiled);
    private static readonly Regex DirectiveRegex = new Regex(@"\{([^:}]+)(?::(.*))?\}", RegexOptions.Compiled);
    private static readonly Regex TabEndRegex = new Regex(@"^\{\s*(?:end_of_tab|eot)\s*\}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public ChordProDocument Parse(string chordProContent)
    {
        if (string.IsNullOrWhiteSpace(chordProContent))
            return new ChordProDocument();

        var document = new ChordProDocument();
        var lines = chordProContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        if (lines.Length > 1 && lines[^1].Length == 0)
            lines = lines[..^1]; // A terminating newline ends the last line, it does not start another.

        ChordProLineType currentSection = ChordProLineType.Verse;
        var inTab = false;
        var tabStart = false;
        var tabLabel = string.Empty;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (inTab)
            {
                if (TabEndRegex.IsMatch(trimmedLine))
                {
                    inTab = false;
                    continue;
                }

                document.Lines.Add(new ChordProLine { Type = ChordProLineType.Tab, Text = line, StartsTabBlock = tabStart, TabLabel = tabStart ? tabLabel : string.Empty });
                tabStart = false;
                continue;
            }

            // Skip empty lines but add them to document
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                document.Lines.Add(new ChordProLine { Type = ChordProLineType.Empty });
                continue;
            }

            // Check for directives {title:...}, {artist:...}, etc.
            var directiveMatch = DirectiveRegex.Match(trimmedLine);
            if (directiveMatch.Success)
            {
                var directive = directiveMatch.Groups[1].Value.Trim().ToLowerInvariant();
                var value = directiveMatch.Groups.Count > 2 ? directiveMatch.Groups[2].Value : string.Empty;

                switch (directive)
                {
                    case "title" or "t":
                        document.Title = value;
                        document.Lines.Add(new ChordProLine
                        {
                            Type = ChordProLineType.Title,
                            Text = value
                        });
                        break;
                    case "artist" or "subtitle" or "st":
                        document.Artist = value;
                        break;
                    case "key":
                        document.Key = value;
                        break;
                    case "capo":
                        document.Capo = value.Trim();
                        break;
                    case "tempo":
                        document.Tempo = value;
                        break;
                    case "time":
                        document.Time = value;
                        break;
                    case "start_of_tab" or "sot":
                        inTab = true;
                        tabStart = true;
                        tabLabel = value.Trim();
                        break;
                    case "start_of_chorus" or "soc":
                        currentSection = ChordProLineType.Chorus;
                        break;
                    case "end_of_chorus" or "eoc":
                        currentSection = ChordProLineType.Verse;
                        break;
                    case "start_of_bridge" or "sob":
                        currentSection = ChordProLineType.Bridge;
                        break;
                    case "end_of_bridge" or "eob":
                        currentSection = ChordProLineType.Verse;
                        break;
                    case "comment" or "c":
                        document.Lines.Add(new ChordProLine
                        {
                            Type = ChordProLineType.Comment,
                            Text = value
                        });
                        break;
                }
                continue;
            }

            // Parse lines with chords [C] [G] etc.
            var chordLine = new ChordProLine
            {
                Type = currentSection,
                Chords = new List<ChordPosition>()
            };

            // Extract chords and their positions
            var removed = 0;
            foreach (Match match in ChordRegex.Matches(line))
            {
                chordLine.Chords.Add(new ChordPosition
                {
                    Chord = match.Groups[1].Value,
                    Position = match.Index - removed
                });
                removed += match.Length;
            }

            // Remove chord markers to get lyrics
            chordLine.Text = ChordRegex.Replace(line, string.Empty);

            document.Lines.Add(chordLine);
        }

        return document;
    }

    public string Render(ChordProDocument document, int transposeSteps = 0) =>
        Render(document, new ChordProRenderOptions { TransposeSteps = transposeSteps });

    public string Render(ChordProDocument document, ChordProRenderOptions options)
    {
        var sb = new StringBuilder();
        var transformer = new ChordTransformer(options, document.Key, document.CapoFret ?? 0);

        var hasMetadata = AppendMetadata(sb, document, transformer, options);
        var body = document.Lines.Where(l => l.Type != ChordProLineType.Title).ToList();
        if (hasMetadata && body.Count > 0 && body[0].Type != ChordProLineType.Empty)
            sb.Append('\n');

        var previousType = ChordProLineType.Verse;
        foreach (var line in body)
        {
            AppendSectionBoundary(sb, previousType, line);
            AppendLine(sb, line, transformer);
            previousType = line.Type;
        }

        AppendSectionEnd(sb, previousType);
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
        return sb.Length > start;
    }

    private static void AppendSectionBoundary(StringBuilder sb, ChordProLineType previous, ChordProLine line)
    {
        var current = line.Type;
        var newTabBlock = current == ChordProLineType.Tab && line.StartsTabBlock;
        if (previous == current && !newTabBlock)
            return;

        AppendSectionEnd(sb, previous);
        if (current == ChordProLineType.Chorus)
            sb.Append("{start_of_chorus}\n");
        else if (current == ChordProLineType.Bridge)
            sb.Append("{start_of_bridge}\n");
        else if (current == ChordProLineType.Tab)
            sb.Append(line.TabLabel.Length > 0 ? $"{{start_of_tab:{line.TabLabel}}}\n" : "{start_of_tab}\n");
    }

    private static void AppendSectionEnd(StringBuilder sb, ChordProLineType type)
    {
        if (type == ChordProLineType.Chorus)
            sb.Append("{end_of_chorus}\n");
        else if (type == ChordProLineType.Bridge)
            sb.Append("{end_of_bridge}\n");
        else if (type == ChordProLineType.Tab)
            sb.Append("{end_of_tab}\n");
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
        foreach (var chord in line.Chords.OrderBy(c => c.Position))
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
