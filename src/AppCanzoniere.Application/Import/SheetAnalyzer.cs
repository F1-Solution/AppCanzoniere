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
    /// <summary>Lines that explain the notation: definite chord lines and inline chord lines, single bare notes left out.</summary>
    public int DetectionCount => Lines.Count(l => l.Content is { } c && VotesForNotation(c) && !IsSingleNote(c));

    /// <summary>Like <see cref="DetectionCount"/> but single bare-note lines vote too; used when they are the only evidence.</summary>
    public int DetectionCountWithSingles => Lines.Count(l => l.Content is { } c && VotesForNotation(c));

    private bool VotesForNotation(ContentUnit c) =>
        c.Kind == UnitKind.Chord ? c.Class.CountsForDetection : c.Kind == UnitKind.Inline && (c.Class.Unambiguous >= 1 || c.Class.ChordCount >= 2);

    private bool IsSingleNote(ContentUnit c) =>
        c.Kind == UnitKind.Chord && c.Class.Tokens is [{ Kind: TokenKind.Chord } only] && ChordSymbolParser.TryParseNote(only.Chord, Notation, out _);

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
    private const int MinTitleCapitals = 2;

    private static readonly Regex ContentsEntry = new(
        @"(?:\bpag(?:ina|\.)?\s*\d+|\bp\.\s*\d+|\.{3,}\s*\d+)\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex NumberedTitle = new(@"^\s*\d{1,3}[.)]\s+\S", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex TabStart = new(@"^\s*[A-Ga-g][#b]?\s?[|:]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static DocumentAnalysis Analyze(IReadOnlyList<NormalizedLine> lines, Notation notation)
    {
        var analyzed = new List<AnalyzedLine>(lines.Count);
        var inHeader = true;
        var lowerCaseStyle = HasLowerCaseStyle(lines, notation);
        var seenContent = false;
        for (var i = 0; i < lines.Count; i++)
        {
            var context = new LineContext(i + 1 < lines.Count ? lines[i + 1].Text : null, lowerCaseStyle, !seenContent);
            var line = AnalyzeLine(i, lines[i], notation, inHeader, context);
            analyzed.Add(line);
            seenContent |= lines[i].Text.Length > 0;
            inHeader = inHeader && !EndsHeader(line);
        }

        return new DocumentAnalysis(notation, GuessTitle(ResolvePending(analyzed), notation));
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
        other.Content is { Class: { Words: 0, ChordCount: >= 1 } c } && (c.Unambiguous >= 1 || IsBareNoteLine(c));

    // A lone ambiguous note such as "A" or "Mi" is flagged too, never silently left as a lyric.
    private static bool IsBareNoteLine(LineClassification c) => c.Tokens.All(t => t.Kind == TokenKind.Chord);

    // A document with several lower-case chord lines writes all its chords in lower case.
    private static bool HasLowerCaseStyle(IReadOnlyList<NormalizedLine> lines, Notation notation) =>
        lines.Count(l => l.Text.Length > 0 && LooseChords.IsStyleEvidence(SheetTokenizer.Tokenize(l.Text, 0, notation), notation)) >= LooseChords.MinStyleLines;

    private static AnalyzedLine AnalyzeLine(int index, NormalizedLine line, Notation notation, bool inHeader, LineContext context)
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

        if (!IsNotALabel(text, context) && SectionLabelParser.TryParse(text) is { } label)
            return AnalyzeLabel(index, line, label, notation, warnings, context);

        var start = text.Length - text.TrimStart().Length;
        var unit = LineClassifier.BuildUnit(text, start, true, notation, context);
        var kind = unit.Kind switch
        {
            UnitKind.Chord => SheetLineKind.ChordLine,
            UnitKind.Inline => SheetLineKind.InlineChordLyric,
            _ => SheetLineKind.Lyric
        };
        return new AnalyzedLine(index, line, kind, unit.Class.Confidence, warnings.Concat(unit.Class.Warnings).ToList(), unit);
    }

    // A song number before a title and a contents entry look like a numbered verse but are plain text.
    private static bool IsNotALabel(string text, LineContext context) =>
        ContentsEntry.IsMatch(text) || (NumberedTitle.IsMatch(text) && IsAllCaps(text) && (context.FirstContent || IsBlankOrMetadata(context.Below)));

    private static bool IsAllCaps(string text) => !text.Any(char.IsLower) && text.Count(char.IsUpper) >= MinTitleCapitals;

    private static bool IsBlankOrMetadata(string? below) =>
        below is not null && (below.Length == 0 || MetadataParser.TryParse(below.Trim()) is not null);

    private static AnalyzedLine AnalyzeLabel(int index, NormalizedLine line, SectionLabel label, Notation notation, IReadOnlyList<SheetWarning> warnings, LineContext context)
    {
        if (label.RestStart < 0)
            return new AnalyzedLine(index, line, SheetLineKind.SectionLabel, 1.0, warnings, Label: label);

        var unit = LineClassifier.BuildUnit(line.Text, label.RestStart, false, notation, context);
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

    private static bool IsChordLineOfOtherNotation(string text, Notation notation)
    {
        var other = notation == Notation.Italian ? Notation.English : Notation.Italian;
        var classification = LineClassifier.Classify(SheetTokenizer.Tokenize(text, 0, other), other);
        return classification.IsChord || classification.Pending;
    }

    private static IReadOnlyList<AnalyzedLine> GuessTitle(IReadOnlyList<AnalyzedLine> lines, Notation notation)
    {
        if (lines.Any(l => l.Meta?.Kind == MetaKind.Title))
            return lines;

        var first = lines.FirstOrDefault(l => l.Kind != SheetLineKind.Blank);
        if (first is not { Kind: SheetLineKind.Lyric, Content: { } content } || content.Class.ChordCount > 1)
            return lines;

        if (!lines.Skip(first.Index + 1).Any(l => l.Kind != SheetLineKind.Blank) || IsChordLineOfOtherNotation(first.Source.Text, notation))
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
