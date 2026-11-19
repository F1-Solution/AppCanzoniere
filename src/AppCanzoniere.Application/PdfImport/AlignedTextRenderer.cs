using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>One monospaced output line and the row it came from; Row is null for a blank line between stanzas.</summary>
public sealed record AlignedLine(string Text, TextRow? Row);

/// <summary>Turns the rows of one region into monospaced lines that keep each word over the character it sits over on the page.</summary>
public static class AlignedTextRenderer
{
    // A row is a chord row when at least this share of its words, dashes aside, are chord symbols...
    private const double ChordRowShare = 0.8;

    // ...or at least this share when it is also sparse, as a label followed by chords is.
    private const double SparseChordRowShare = 0.5;

    // A row is dense (lyrics) when it has at least two words covering at least this share of its extent...
    private const double DenseCoverageShare = 0.6;
    private const int MinDenseWords = 2;

    // ...and no gap between words wider than this many row heights.
    private const double MaxDenseGapInHeights = 1.0;

    // A row is mapped onto a lyric row at most this many of its heights away; some booklets leave a blank line between.
    private const double MaxAnchorDistanceInHeights = 3.5;

    // A vertical gap larger than this multiple of the usual row pitch becomes a blank line.
    private const double BlankLinePitchFactor = 1.6;

    public static IReadOnlyList<AlignedLine> Render(IReadOnlyList<TextRow> rows) => Render(rows, TextBudget.Unlimited);

    // Each composed line is charged to the budget as soon as it exists, so an oversized page stops early.
    internal static IReadOnlyList<AlignedLine> Render(IReadOnlyList<TextRow> rows, TextBudget budget)
    {
        ArgumentNullException.ThrowIfNull(rows);
        if (rows.Count == 0)
            return Array.Empty<AlignedLine>();

        var grid = CharacterGrid.For(rows);
        var lyric = LyricRows(rows);
        var placed = new PlacedRow?[rows.Count];
        for (var i = 0; i < rows.Count; i++)
            placed[i] = lyric[i] ? Charged(PlacedRow.Natural(rows[i], grid), budget) : null;
        var anchors = rows.Select((_, i) => lyric[i] ? -1 : Anchor(rows, lyric, i)).ToArray();
        for (var i = 0; i < rows.Count; i++)
            placed[i] ??= Charged(PlacedRow.Mapped(rows[i], anchors[i] >= 0 ? placed[anchors[i]] : null, grid), budget);

        return WithBlankLines(rows, placed.Select(p => p!.Text).ToList(), anchors);
    }

    private static PlacedRow Charged(PlacedRow row, TextBudget budget)
    {
        budget.Spend(row.Text.Length + 1);
        return row;
    }

    // Rows laid out with their own spacing: dense rows that are not chords, and any row right under a chord row,
    // however spread out its syllables are. Chord rows and other sparse rows are mapped onto them.
    private static bool[] LyricRows(IReadOnlyList<TextRow> placedRows)
    {
        var rows = placedRows.Select(WithMinorMarkersJoined).ToList();
        var chord = rows.Select(IsChordRow).ToArray();
        for (var i = 1; i < rows.Count; i++)
        {
            // A dense row of lower-case note names right under a sparse chord row is its lyric, as "la la la" is.
            if (chord[i] && chord[i - 1] && IsDense(rows[i]) && !IsDense(rows[i - 1]) && IsClose(rows[i - 1], rows[i]) && IsMostlyLowerCase(rows[i]))
                chord[i] = false;
        }

        return rows.Select((row, i) => !chord[i] && (IsDense(row) || (i > 0 && chord[i - 1] && IsClose(rows[i - 1], row)))).ToArray();
    }

    // A lone "m" right after a chord is its minor marker, so "SI m" is one chord and not two dense words.
    private static TextRow WithMinorMarkersJoined(TextRow row)
    {
        var words = new List<PositionedWord>(row.Words.Count);
        foreach (var word in row.Words)
        {
            if (word.Text == "m" && words.Count > 0 && IsChord(words[^1].Text) && word.Box.Left - words[^1].Box.Right <= row.Height)
                words[^1] = new PositionedWord(words[^1].Text + "m", words[^1].Box with { Right = word.Box.Right });
            else
                words.Add(word);
        }

        return words.Count == row.Words.Count ? row : new TextRow(words);
    }

    private static bool IsChordRow(TextRow row)
    {
        var tokens = row.Words.Select(w => w.Text.Trim('(', ')')).Where(t => t.Length > 0 && !IsDash(t)).ToList();
        if (tokens.Count == 0)
            return false;

        var share = (double)tokens.Count(IsChord) / tokens.Count;
        return share >= ChordRowShare || (share >= SparseChordRowShare && !IsDense(row));
    }

    // Lyrics are written in lower case; chord symbols start with a capital, so a row like "SI- MI LA" stays chords.
    private static bool IsMostlyLowerCase(TextRow row) =>
        row.Words.Count(w => char.IsLower(w.Text[0])) * 2 > row.Words.Count;

    private static bool IsChord(string token) =>
        ChordSymbolParser.TryParse(token, Notation.Italian, out _) || ChordSymbolParser.TryParse(token, Notation.English, out _);

    private static bool IsDash(string token) => token.All(c => c is '-' or '–' or '—');

    // A verse number or label may stand far before its text, so the first gap does not count when a row has more.
    private static bool IsDense(TextRow row)
    {
        var words = row.Words.Count > MinDenseWords ? row.Words.Skip(1).ToList() : row.Words;
        return words.Count >= MinDenseWords
            && new TextRow(words).Coverage >= DenseCoverageShare
            && words.Zip(words.Skip(1), (a, b) => b.Box.Left - a.Box.Right).Max() <= MaxDenseGapInHeights * row.Height;
    }

    private static bool IsClose(TextRow row, TextRow other) =>
        Math.Abs(row.Baseline - other.Baseline) <= MaxAnchorDistanceInHeights * row.Height;

    // Index of the lyric row just below, or else just above, close enough to be the lyric a chord row belongs to; -1 if none.
    private static int Anchor(IReadOnlyList<TextRow> rows, bool[] lyric, int index)
    {
        foreach (var neighbour in new[] { index + 1, index - 1 })
        {
            if (neighbour >= 0 && neighbour < rows.Count && lyric[neighbour] && IsClose(rows[index], rows[neighbour]))
                return neighbour;
        }

        return -1;
    }

    // A wide vertical gap becomes a blank line, except between a row and the lyric below it that it is mapped onto.
    private static IReadOnlyList<AlignedLine> WithBlankLines(IReadOnlyList<TextRow> rows, IReadOnlyList<string> texts, int[] anchors)
    {
        var pitches = rows.Zip(rows.Skip(1), (a, b) => a.Baseline - b.Baseline).Where(p => p > 0).OrderBy(p => p).ToList();
        var pitch = pitches.Count == 0 ? double.MaxValue : pitches[pitches.Count / 2];
        var lines = new List<AlignedLine> { new(texts[0], rows[0]) };
        for (var i = 1; i < rows.Count; i++)
        {
            if (rows[i - 1].Baseline - rows[i].Baseline > BlankLinePitchFactor * pitch && anchors[i - 1] != i)
                lines.Add(new AlignedLine(string.Empty, null));
            lines.Add(new AlignedLine(texts[i], rows[i]));
        }

        return lines;
    }
}
