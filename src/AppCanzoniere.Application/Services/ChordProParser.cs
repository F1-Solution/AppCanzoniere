using AppCanzoniere.Application.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.Services;

public class ChordProParser : IChordProParser
{
    private static readonly Regex ChordRegex = new Regex(@"\[([^\]]+)\]", RegexOptions.Compiled);
    private static readonly Regex DirectiveRegex = new Regex(@"\{([^:}]+)(?::(.+))?\}", RegexOptions.Compiled);

    public ChordProDocument Parse(string chordProContent)
    {
        if (string.IsNullOrWhiteSpace(chordProContent))
            return new ChordProDocument();

        var document = new ChordProDocument();
        var lines = chordProContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        ChordProLineType currentSection = ChordProLineType.Verse;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

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
                var directive = directiveMatch.Groups[1].Value.ToLowerInvariant();
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
                        document.Capo = value;
                        break;
                    case "tempo":
                        document.Tempo = value;
                        break;
                    case "time":
                        document.Time = value;
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
            var chordMatches = ChordRegex.Matches(line);
            foreach (Match match in chordMatches)
            {
                chordLine.Chords.Add(new ChordPosition
                {
                    Chord = match.Groups[1].Value,
                    Position = match.Index
                });
            }

            // Remove chord markers to get lyrics
            chordLine.Text = ChordRegex.Replace(line, string.Empty);

            document.Lines.Add(chordLine);
        }

        return document;
    }

    public string Render(ChordProDocument document, int transposeSteps = 0)
    {
        var sb = new StringBuilder();
        var transposer = new ChordTransposer();

        // Add metadata
        if (!string.IsNullOrEmpty(document.Title))
            sb.AppendLine($"{{title:{document.Title}}}");
        if (!string.IsNullOrEmpty(document.Artist))
            sb.AppendLine($"{{artist:{document.Artist}}}");
        if (!string.IsNullOrEmpty(document.Key))
        {
            var transposedKey = transposeSteps != 0
                ? transposer.TransposeKey(document.Key, transposeSteps)
                : document.Key;
            sb.AppendLine($"{{key:{transposedKey}}}");
        }
        if (!string.IsNullOrEmpty(document.Capo))
            sb.AppendLine($"{{capo:{document.Capo}}}");

        sb.AppendLine();

        ChordProLineType previousType = ChordProLineType.Verse;

        foreach (var line in document.Lines)
        {
            // Add section markers
            if (line.Type == ChordProLineType.Chorus && previousType != ChordProLineType.Chorus)
                sb.AppendLine("{start_of_chorus}");
            else if (line.Type == ChordProLineType.Bridge && previousType != ChordProLineType.Bridge)
                sb.AppendLine("{start_of_bridge}");
            else if (previousType == ChordProLineType.Chorus && line.Type != ChordProLineType.Chorus)
                sb.AppendLine("{end_of_chorus}");
            else if (previousType == ChordProLineType.Bridge && line.Type != ChordProLineType.Bridge)
                sb.AppendLine("{end_of_bridge}");

            if (line.Type == ChordProLineType.Empty)
            {
                sb.AppendLine();
            }
            else if (line.Type == ChordProLineType.Comment)
            {
                sb.AppendLine($"{{comment:{line.Text}}}");
            }
            else if (line.Type == ChordProLineType.Title)
            {
                // Already added in metadata
            }
            else
            {
                // Build line with chords
                var lineText = line.Text;
                var chordPositions = line.Chords.OrderByDescending(c => c.Position).ToList();

                foreach (var chordPos in chordPositions)
                {
                    var transposedChord = transposeSteps != 0
                        ? transposer.TransposeChord(chordPos.Chord, transposeSteps)
                        : chordPos.Chord;

                    // Insert chord at position
                    var insertPos = Math.Min(chordPos.Position, lineText.Length);
                    lineText = lineText.Insert(insertPos, $"[{transposedChord}]");
                }

                sb.AppendLine(lineText);
            }

            previousType = line.Type;
        }

        // Close any open sections
        if (previousType == ChordProLineType.Chorus)
            sb.AppendLine("{end_of_chorus}");
        else if (previousType == ChordProLineType.Bridge)
            sb.AppendLine("{end_of_bridge}");

        return sb.ToString();
    }
}
