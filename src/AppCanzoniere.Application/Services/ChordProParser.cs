using System.Text.RegularExpressions;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Services;

public class ChordProParser : IChordProParser
{
    private static readonly Regex ChordRegex = new Regex(@"\[([^\]]+)\]", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex DirectiveRegex = new Regex(@"^\{([^:}]+)(?::(.*))?\}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex TabEndRegex = new Regex(@"^\{\s*(?:end_of_tab|eot)\s*\}$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private const char Bom = (char)0xFEFF;

    public ChordProDocument Parse(string chordProContent)
    {
        var document = new ChordProDocument();
        if (string.IsNullOrEmpty(chordProContent))
            return document;

        var state = new ParseState();
        var source = new List<ChordProSourceLine>();
        var lines = SplitLines(chordProContent);
        for (var i = 0; i < lines.Length; i++)
        {
            state.Line = i;
            var kind = ParseLine(i == 0 ? lines[i].TrimStart(Bom) : lines[i], document, state);
            source.Add(new ChordProSourceLine(kind, lines[i]));
        }

        document.SourceLines = source.AsReadOnly();
        document.Snapshot = ChordProDocumentSnapshot.Of(document, state.HeaderLines);
        return document;
    }

    private static string[] SplitLines(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        return lines.Length > 1 && lines[^1].Length == 0
            ? lines[..^1] // A terminating newline ends the last line, it does not start another.
            : lines;
    }

    private static ChordProSourceKind ParseLine(string line, ChordProDocument document, ParseState state)
    {
        var trimmed = line.Trim();
        if (state.InTab)
        {
            return ParseTabLine(line, trimmed, document, state);
        }

        if (trimmed.Length == 0)
        {
            document.Lines.Add(new ChordProLine { Type = ChordProLineType.Empty, InOpenSection = state.Section != ChordProLineType.Verse });
            return ChordProSourceKind.Verbatim;
        }

        if (line[0] == '#')
            return ChordProSourceKind.Verbatim; // A ChordPro file comment, not a lyric.

        if (DirectiveRegex.Match(trimmed) is { Success: true } directive)
            return ApplyDirective(directive, document, state);

        var chordLine = ParseChordLine(line, state.Section);
        chordLine.StartsSection = state.TakeSectionStart();
        document.Lines.Add(chordLine);
        return ChordProSourceKind.Lyrics;
    }

    private static ChordProSourceKind ParseTabLine(string line, string trimmed, ChordProDocument document, ParseState state)
    {
        if (TabEndRegex.IsMatch(trimmed))
        {
            state.InTab = false;
            return ChordProSourceKind.Verbatim;
        }

        document.Lines.Add(new ChordProLine
        {
            Type = ChordProLineType.Tab,
            Text = line,
            StartsTabBlock = state.TabStart,
            InOpenSection = state.Section != ChordProLineType.Verse,
            TabLabel = state.TabStart ? state.TabLabel : string.Empty
        });
        state.TabStart = false;
        return ChordProSourceKind.Tab;
    }

    private static ChordProSourceKind ApplyDirective(Match directive, ChordProDocument document, ParseState state)
    {
        var name = directive.Groups[1].Value.Trim().ToLowerInvariant();
        var value = directive.Groups[2].Value;
        switch (name)
        {
            case "title" or "t" when string.IsNullOrWhiteSpace(value):
            case "key" when string.IsNullOrWhiteSpace(value):
                break; // An empty value never erases an earlier one.
            case "title" or "t":
                state.HeaderLines[(int)HeaderField.Title] = state.Line;
                document.Title = value;
                document.Lines.Add(new ChordProLine { Type = ChordProLineType.Title, Text = value });
                break;
            case "comment" or "c":
                document.Lines.Add(new ChordProLine
                {
                    Type = ChordProLineType.Comment,
                    Text = value,
                    StartsSection = state.TakeSectionStart(),
                    InOpenSection = state.Section != ChordProLineType.Verse
                });
                break;
            case "start_of_tab" or "sot":
                state.StartTab(value.Trim());
                break;
            default:
                ApplyMetadataOrSection(name, value, document, state);
                break;
        }

        return name switch
        {
            "key" => ChordProSourceKind.Key,
            "capo" => ChordProSourceKind.Capo,
            _ => ChordProSourceKind.Verbatim
        };
    }

    private static void ApplyMetadataOrSection(string name, string value, ChordProDocument document, ParseState state)
    {
        switch (name)
        {
            case "artist" or "subtitle" or "st":
                state.HeaderLines[(int)HeaderField.Artist] = state.Line;
                document.Artist = value;
                break;
            case "key":
                state.HeaderLines[(int)HeaderField.Key] = state.Line;
                document.Key = value;
                break;
            case "capo":
                state.HeaderLines[(int)HeaderField.Capo] = state.Line;
                document.Capo = value.Trim();
                break;
            case "tempo":
                state.HeaderLines[(int)HeaderField.Tempo] = state.Line;
                document.Tempo = value;
                break;
            case "time":
                state.HeaderLines[(int)HeaderField.Time] = state.Line;
                document.Time = value;
                break;
            case "start_of_chorus" or "soc":
                state.OpenSection(ChordProLineType.Chorus);
                break;
            case "start_of_bridge" or "sob":
                state.OpenSection(ChordProLineType.Bridge);
                break;
            case "end_of_chorus" or "eoc" or "end_of_bridge" or "eob":
                state.OpenSection(ChordProLineType.Verse);
                break;
        }
    }

    private static ChordProLine ParseChordLine(string line, ChordProLineType section)
    {
        var chordLine = new ChordProLine { Type = section, Chords = new List<ChordPosition>() };
        var removed = 0;
        foreach (Match match in ChordRegex.Matches(line))
        {
            chordLine.Chords.Add(new ChordPosition { Chord = match.Groups[1].Value, Position = match.Index - removed });
            removed += match.Length;
        }

        chordLine.Text = ChordRegex.Replace(line, string.Empty);
        return chordLine;
    }

    private sealed class ParseState
    {
        public ChordProLineType Section { get; set; } = ChordProLineType.Verse;

        public bool InTab { get; set; }

        public int Line { get; set; }

        public int[] HeaderLines { get; } = Enumerable.Repeat(-1, 6).ToArray();

        private bool _sectionStartPending;

        public void OpenSection(ChordProLineType section)
        {
            Section = section;
            _sectionStartPending = section != ChordProLineType.Verse;
        }

        /// <summary>True once for the first line after an explicit section opens.</summary>
        public bool TakeSectionStart()
        {
            var pending = _sectionStartPending;
            _sectionStartPending = false;
            return pending;
        }

        public bool TabStart { get; set; }

        public string TabLabel { get; set; } = string.Empty;

        public void StartTab(string label)
        {
            InTab = true;
            TabStart = true;
            TabLabel = label;
        }
    }

    public string Render(ChordProDocument document, int transposeSteps = 0) =>
        Render(document, new ChordProRenderOptions { TransposeSteps = transposeSteps });

    public string Render(ChordProDocument document, ChordProRenderOptions options)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(options);

        var transformer = new ChordTransformer(options, document.Key, document.CapoFret ?? 0, ChordProKeys.ChordsOf(document));
        if (document.KeepsSourceText)
        {
            var edits = document.Snapshot!.HeaderEdits(document);
            var lines = edits.Count == 0 ? document.SourceLines! : ChordProHeaderEditor.Apply(document.SourceLines!, edits, document.Snapshot!);
            return ChordProSourceRenderer.Render(lines, transformer, options.Capo);
        }

        return ChordProTypedRenderer.Render(document, transformer, options);
    }
}
