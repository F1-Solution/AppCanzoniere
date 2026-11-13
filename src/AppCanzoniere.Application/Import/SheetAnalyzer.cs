using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>Everything the writer needs to know about one input line.</summary>
internal sealed record AnalyzedLine(
    int Index,
    NormalizedLine Source,
    SheetLineKind Kind,
    double Confidence,
    IReadOnlyList<SheetWarning> Warnings,
    ContentUnit? Content = null,
    SectionLabel? Label = null,
    MetadataItem? Meta = null);

/// <summary>Result of reading the whole document with one notation.</summary>
internal sealed record DocumentAnalysis(Notation Notation, IReadOnlyList<AnalyzedLine> Lines)
{
    /// <summary>Lines that explain the notation: definite chord lines and inline chord lines.</summary>
    public int DetectionCount => Lines.Count(l => l.Content is { } c &&
        (c.Kind == UnitKind.Chord ? c.Class.CountsForDetection : c.Kind == UnitKind.Inline && (c.Class.Unambiguous >= 1 || c.Class.ChordCount >= 2)));

    public bool HasChords => Lines.Any(l => l.Content is { } c &&
        (c.Kind == UnitKind.Inline ? c.Class.ChordCount > 0 : c.Kind == UnitKind.Chord && c.Class.Tokens.Any(t => t.Kind == TokenKind.Chord)));
}

/// <summary>Classifies every line of a document for one notation (spec 4, 5, 7).</summary>
internal static class SheetAnalyzer
{
    private const double TitleConfidence = 0.7;
    private const double UnreadableConfidence = 0.5;
    private const double OtherNotationConfidence = 0.6;
    private const int MaxTitleLength = 60;
    private const int MinTabDashes = 4;

    private static readonly Regex TabStart = new(@"^\s*[A-Ga-g][#b]?\s?[|:]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static DocumentAnalysis Analyze(IReadOnlyList<NormalizedLine> lines, Notation notation)
    {
        var analyzed = new List<AnalyzedLine>(lines.Count);
        var inHeader = true;
        for (var i = 0; i < lines.Count; i++)
        {
            var line = AnalyzeLine(i, lines[i], notation, inHeader);
            analyzed.Add(line);
            inHeader = inHeader && !EndsHeader(line);
        }

        return new DocumentAnalysis(notation, GuessTitle(ResolvePending(analyzed)));
    }

    /// <summary>Marks plain lyric lines that would be chord lines in the other notation.</summary>
    public static DocumentAnalysis FlagOtherNotation(DocumentAnalysis chosen, DocumentAnalysis other)
    {
        var lines = chosen.Lines.Select(l => IsOtherNotation(l, other.Lines[l.Index])
            ? l with
            {
                Confidence = Math.Min(l.Confidence, OtherNotationConfidence),
                Warnings = l.Warnings.Append(SheetWarning.OtherNotation).ToList()
            }
            : l).ToList();
        return chosen with { Lines = lines };
    }

    private static bool IsOtherNotation(AnalyzedLine line, AnalyzedLine other) =>
        line.Kind == SheetLineKind.Lyric && other.Kind == SheetLineKind.ChordLine &&
        other.Content is { Class: { Pending: false, Unambiguous: >= 1, Words: 0 } };

    private static AnalyzedLine AnalyzeLine(int index, NormalizedLine line, Notation notation, bool inHeader)
    {
        var text = line.Text;
        IReadOnlyList<SheetWarning> warnings = line.TabExpanded ? new[] { SheetWarning.TabExpanded } : Array.Empty<SheetWarning>();
        if (text.Length == 0)
            return new AnalyzedLine(index, line, SheetLineKind.Blank, 1.0, warnings);

        if (IsTabLine(text))
            return new AnalyzedLine(index, line, SheetLineKind.Tab, 1.0, warnings);

        if (inHeader && MetadataParser.TryParse(text.Trim()) is { } meta)
        {
            var unreadable = meta.Unreadable;
            return new AnalyzedLine(
                index, line, SheetLineKind.Metadata, unreadable ? UnreadableConfidence : 1.0,
                unreadable ? warnings.Append(SheetWarning.UnreadableMetadata).ToList() : warnings, Meta: meta);
        }

        if (SectionLabelParser.TryParse(text) is { } label)
            return AnalyzeLabel(index, line, label, notation, warnings);

        var start = text.Length - text.TrimStart().Length;
        var unit = LineClassifier.BuildUnit(text, start, true, notation);
        var kind = unit.Kind switch
        {
            UnitKind.Chord => SheetLineKind.ChordLine,
            UnitKind.Inline => SheetLineKind.InlineChordLyric,
            _ => SheetLineKind.Lyric
        };
        return new AnalyzedLine(index, line, kind, unit.Class.Confidence, warnings.Concat(unit.Class.Warnings).ToList(), unit);
    }

    private static AnalyzedLine AnalyzeLabel(int index, NormalizedLine line, SectionLabel label, Notation notation, IReadOnlyList<SheetWarning> warnings)
    {
        if (label.RestStart < 0)
            return new AnalyzedLine(index, line, SheetLineKind.SectionLabel, 1.0, warnings, Label: label);

        var unit = LineClassifier.BuildUnit(line.Text, label.RestStart, false, notation);
        return new AnalyzedLine(
            index, line, SheetLineKind.SectionLabel, unit.Class.Confidence,
            warnings.Concat(unit.Class.Warnings).ToList(), unit, label);
    }

    private static bool IsTabLine(string text) =>
        TabStart.IsMatch(text) && text.Count(c => c == '-') >= MinTabDashes;

    private static bool EndsHeader(AnalyzedLine line) =>
        line.Kind is SheetLineKind.SectionLabel or SheetLineKind.Tab or SheetLineKind.InlineChordLyric ||
        (line.Kind == SheetLineKind.ChordLine && line.Content is { Class.Pending: false });

    private static IReadOnlyList<AnalyzedLine> ResolvePending(List<AnalyzedLine> lines)
    {
        var hasChordLine = lines.Any(l => l.Content is { Kind: UnitKind.Chord });
        if (!hasChordLine)
            return lines;

        return lines.Select(l => l.Content is { Class.Pending: true } c
            ? l with
            {
                Kind = l.Label is null ? SheetLineKind.ChordLine : l.Kind,
                Content = LineClassifier.ResolvePending(c)
            }
            : l).ToList();
    }

    private static IReadOnlyList<AnalyzedLine> GuessTitle(IReadOnlyList<AnalyzedLine> lines)
    {
        if (lines.Any(l => l.Meta?.Kind == MetaKind.Title))
            return lines;

        var first = lines.FirstOrDefault(l => l.Kind != SheetLineKind.Blank);
        if (first is not { Kind: SheetLineKind.Lyric, Content: { } content } || content.Class.ChordCount > 1)
            return lines;

        var title = first.Source.Text.Trim();
        var next = first.Index + 1 < lines.Count ? lines[first.Index + 1] : null;
        var nextEndsTitle = next is { Kind: SheetLineKind.Blank or SheetLineKind.Metadata };
        if (title.Length > MaxTitleLength || ",;:".Contains(title[^1]) || !nextEndsTitle)
            return lines;

        var guessed = first with
        {
            Kind = SheetLineKind.Title,
            Confidence = TitleConfidence,
            Warnings = first.Warnings.Where(w => w == SheetWarning.TabExpanded).ToList(),
            Content = null,
            Meta = new MetadataItem(MetaKind.Title, title, false)
        };
        return lines.Select(l => l.Index == first.Index ? guessed : l).ToList();
    }
}
