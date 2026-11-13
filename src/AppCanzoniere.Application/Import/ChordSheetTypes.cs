using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>Settings of the chords-above-lyrics converter.</summary>
public sealed record ChordSheetOptions
{
    /// <summary>Forces the chord notation; null detects it once per document.</summary>
    public Notation? Notation { get; init; }

    /// <summary>Notation used when detection ties (both notations explain the same number of lines).</summary>
    public Notation TieBreakNotation { get; init; } = Music.Notation.Italian;

    /// <summary>Tab stop width used to expand tab characters.</summary>
    public int TabWidth { get; init; } = 8;

    /// <summary>Max columns a chord that falls on a space is moved right to reach a word start.</summary>
    public int SnapColumns { get; init; } = 2;
}

/// <summary>What the converter decided a pasted line is.</summary>
public enum SheetLineKind
{
    Blank,
    Title,
    Metadata,
    SectionLabel,
    ChordLine,
    Lyric,
    InlineChordLyric,
    Tab,
    ChordPro
}

/// <summary>Something a reviewer may want to check on a converted line.</summary>
public enum SheetWarning
{
    /// <summary>One bare note name on its own line.</summary>
    AmbiguousSingleToken,

    /// <summary>Lyric line that contains chord-like words.</summary>
    AmbiguousNoteWords,

    /// <summary>One non-chord word kept as an annotation.</summary>
    StrayWordOnChordLine,

    /// <summary>Line made only of chords of the other notation, kept as lyric.</summary>
    OtherNotation,

    /// <summary>A chord written in parentheses was written without them.</summary>
    ParenthesesRemoved,

    /// <summary>A chord was moved from a space to the next word start.</summary>
    SnappedColumn,

    /// <summary>A chord was inserted in the middle of a word (informative).</summary>
    ChordInsideWord,

    /// <summary>The line contained tab characters.</summary>
    TabExpanded,

    /// <summary>A chorus is recalled but no chorus was written before.</summary>
    ChorusRecallWithoutChorus,

    /// <summary>Both key and capo were found: the owner's key and capo convention applies.</summary>
    KeyAndCapo,

    /// <summary>A metadata-looking line was kept as a comment.</summary>
    UnreadableMetadata,

    /// <summary>A lyric starting with '#' got a leading space.</summary>
    EscapedLyric,

    /// <summary>Square brackets or braces in the text were replaced by parentheses.</summary>
    BracketsReplaced
}

/// <summary>Diagnosis of one input line; OutputLine is the 0-based output line that carries its text.</summary>
public sealed record SheetLineDiagnostic(
    int LineNumber,
    string Source,
    SheetLineKind Kind,
    double Confidence,
    IReadOnlyList<SheetWarning> Warnings,
    int OutputLine);

/// <summary>Converted song with one diagnostic per input line.</summary>
public sealed record ChordSheetResult(
    string ChordPro,
    bool WasChordPro,
    Notation? DetectedNotation,
    IReadOnlyList<SheetLineDiagnostic> Lines,
    int ChordLineCount,
    int MergedChordLineCount)
{
    private const double DoubtfulBelow = 0.75;

    /// <summary>Lines a review screen should highlight: confidence below 0.75 or any warning other than ChordInsideWord.</summary>
    public IEnumerable<SheetLineDiagnostic> DoubtfulLines =>
        Lines.Where(l => l.Confidence < DoubtfulBelow || l.Warnings.Any(w => w != SheetWarning.ChordInsideWord));
}
