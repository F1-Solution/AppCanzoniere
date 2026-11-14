using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>Converts chords-above-lyrics text, as pasted from booklets and websites, to ChordPro.</summary>
public static class ChordSheetConverter
{
    private static readonly Regex Directive = new(
        @"^\s*\{\s*(title|t|subtitle|st|artist|composer|lyricist|key|capo|tempo|time|comment|c|ci|cb|chorus|start_of_\w+|end_of_\w+|so[cbtv]|eo[cbtv]|define|new_song|ns|meta)\b[^}]*\}\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private const int MinAttachedLines = 2;
    private const int MinTabWidth = 1;
    private const int MaxTabWidth = 16;

    /// <summary>Converts chords-above-lyrics text to ChordPro; ChordPro input is returned unchanged.</summary>
    public static ChordSheetResult Convert(string text, ChordSheetOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        options ??= new ChordSheetOptions();
        Validate(options);
        if (text.Length == 0)
            return new ChordSheetResult(string.Empty, false, null, Array.Empty<SheetLineDiagnostic>(), 0, 0);

        var lines = SheetNormalizer.Normalize(text, options.TabWidth);
        if (IsChordPro(lines))
            return ChordProResult(text, lines);

        var analysis = Analyze(lines, options);
        var written = SheetWriter.Write(analysis.Lines, options.SnapColumns);
        var diagnostics = analysis.Lines
            .Select(l => new SheetLineDiagnostic(l.Index + 1, l.Source.Raw, l.Kind, l.Confidence, written.Warnings[l.Index], written.OutputLines[l.Index])
            {
                CarriesChords = l.Kind == SheetLineKind.ChordLine || (l.Kind == SheetLineKind.SectionLabel && l.Content is { Kind: UnitKind.Chord })
            })
            .ToList();
        var chordLines = analysis.Lines.Count(l => l.Kind == SheetLineKind.ChordLine);
        var notation = analysis.HasChords ? analysis.Notation : (Notation?)null;
        return new ChordSheetResult(written.ChordPro, false, notation, diagnostics, chordLines, written.MergedCount);
    }

    private static void Validate(ChordSheetOptions options)
    {
        if (options.Notation is { } notation && !Enum.IsDefined(notation))
            throw new ArgumentOutOfRangeException(nameof(options), notation, "The notation is not defined.");
        if (!Enum.IsDefined(options.TieBreakNotation))
            throw new ArgumentOutOfRangeException(nameof(options), options.TieBreakNotation, "The tie-break notation is not defined.");
        if (options.TabWidth is < MinTabWidth or > MaxTabWidth)
            throw new ArgumentOutOfRangeException(nameof(options), options.TabWidth, "The tab width must be between 1 and 16.");
        if (options.SnapColumns < 0)
            throw new ArgumentOutOfRangeException(nameof(options), options.SnapColumns, "The snap width cannot be negative.");
    }

    private static DocumentAnalysis Analyze(IReadOnlyList<NormalizedLine> lines, ChordSheetOptions options)
    {
        if (options.Notation is { } forced)
            return SheetAnalyzer.Analyze(lines, forced, true);

        var italian = SheetAnalyzer.Analyze(lines, Notation.Italian);
        var english = SheetAnalyzer.Analyze(lines, Notation.English);
        var singlesOnly = italian.DetectionCount + english.DetectionCount == 0;
        var (italianVotes, englishVotes) = singlesOnly
            ? (italian.DetectionCountWithSingles, english.DetectionCountWithSingles)
            : (italian.DetectionCount, english.DetectionCount);
        var (chosen, other) = italianVotes == englishVotes
            ? (options.TieBreakNotation == Notation.Italian ? italian : english, options.TieBreakNotation == Notation.Italian ? english : italian)
            : italianVotes > englishVotes ? (italian, english) : (english, italian);
        // Notation is chosen on the plain reading; only the final reading replaces brackets of the other notation.
        var final = lines.Any(l => l.Text.Contains('[')) ? SheetAnalyzer.Analyze(lines, chosen.Notation, true) : chosen;
        return SheetAnalyzer.FlagOtherNotation(final, other);
    }

    private static ChordSheetResult ChordProResult(string text, IReadOnlyList<NormalizedLine> lines)
    {
        var diagnostics = lines
            .Select((l, i) => new SheetLineDiagnostic(i + 1, l.Raw, SheetLineKind.ChordPro, 1.0, Array.Empty<SheetWarning>(), i))
            .ToList();
        return new ChordSheetResult(text, true, null, diagnostics, 0, 0);
    }

    private static bool IsChordPro(IReadOnlyList<NormalizedLine> lines) =>
        lines.Any(l => Directive.IsMatch(l.Text)) || lines.Count(l => BracketedText.HasAttachedChord(l.Text)) >= MinAttachedLines;
}
