using AppCanzoniere.Application.PdfImport;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace AppCanzoniere.PdfBench;

/// <summary>
/// Chord placement counts: chords measured, chords over the right character, one off, two or more off; the same for
/// chords over loose lyric rows; and chord rows the output separates from their lyric with a blank line.
/// </summary>
internal sealed record AlignmentScore(int Chords, int Exact, int OffOne, int OffTwo, int LooseChords, int LooseOffTwo, int Separated)
{
    public static readonly AlignmentScore None = new(0, 0, 0, 0, 0, 0, 0);

    public double? OffTwoRate => Chords == 0 ? null : (double)OffTwo / Chords;

    public double? LooseOffTwoRate => LooseChords == 0 ? null : (double)LooseOffTwo / LooseChords;

    public double? TightOffTwoRate => Chords == LooseChords ? null : (double)(OffTwo - LooseOffTwo) / (Chords - LooseChords);

    public static AlignmentScore operator +(AlignmentScore a, AlignmentScore b) => new(
        a.Chords + b.Chords, a.Exact + b.Exact, a.OffOne + b.OffOne, a.OffTwo + b.OffTwo,
        a.LooseChords + b.LooseChords, a.LooseOffTwo + b.LooseOffTwo, a.Separated + b.Separated);
}

/// <summary>
/// Measures, on every page, whether each chord in the output text stands over the lyric character that the PDF prints
/// under the chord's left edge. The truth comes from PdfPig's letter boxes, read here independently of the app.
/// </summary>
internal static class ChordAlignment
{
    // A row is a chord row when at least this share of its words, dashes aside, are chords.
    private const double MinChordShare = 0.8;

    // A chord row followed by a blank line and then a lyric within this many text heights counts as separated.
    private const double MaxSeparatedHeights = 3.0;

    // The tight-row rule used by the reviewer: two words, 60% coverage, no gap wider than the text height.
    private const double TightCoverage = 0.6;

    // A letter contains x when x lies at least this many points inside its right edge.
    private const double InsideTolerancePoints = 0.01;

    private static readonly ParsingOptions Options = new() { UseLenientParsing = true, SkipMissingFonts = true };

    private static readonly Dictionary<char, int> LigatureLengths = new()
    {
        ['ﬀ'] = 2,
        ['ﬁ'] = 2,
        ['ﬂ'] = 2,
        ['ﬃ'] = 3,
        ['ﬄ'] = 3,
        ['ﬅ'] = 2,
        ['ﬆ'] = 2,
    };

    public static AlignmentScore Measure(string pdfPath, PdfTextResult text)
    {
        if (text.Pages.All(p => p.Blocks.Count == 0))
            return AlignmentScore.None;

        using var document = PdfDocument.Open(pdfPath, Options);
        var score = AlignmentScore.None;
        foreach (var page in text.Pages.Where(p => p.Blocks.Count > 0))
        {
            var pig = document.GetPage(page.PageNumber);
            var origin = pig.CropBox.Bounds;
            var words = pig.GetWords()
                .GroupBy(w => (w.BoundingBox.Left - origin.Left, w.BoundingBox.Bottom - origin.Bottom))
                .ToDictionary(g => g.Key, g => g.First());
            foreach (var block in page.Blocks)
                score += Block(block.Lines, words);
        }

        return score;
    }

    private static AlignmentScore Block(IReadOnlyList<AlignedLine> lines, Dictionary<(double, double), Word> words)
    {
        var score = AlignmentScore.None;
        for (var i = 0; i + 1 < lines.Count; i++)
        {
            if (lines[i].Row is not { } chords || !IsChordRow(chords))
                continue;

            if (lines[i + 1].Row is null && i + 2 < lines.Count && lines[i + 2].Row is { } next && !IsChordRow(next)
                && chords.Baseline - next.Baseline < MaxSeparatedHeights * chords.Height)
            {
                score += AlignmentScore.None with { Separated = 1 };
                continue;
            }

            if (lines[i + 1].Row is { } lyric && !IsChordRow(lyric) && lyric.Words.Count >= 2)
                score += Pair(lines[i], lines[i + 1], words);
        }

        return score;
    }

    private static AlignmentScore Pair(AlignedLine chordLine, AlignedLine lyricLine, Dictionary<(double, double), Word> words)
    {
        var chordColumns = Columns(chordLine.Text, chordLine.Row!.Words);
        var lyricColumns = Columns(lyricLine.Text, lyricLine.Row!.Words);
        if (chordColumns is null || lyricColumns is null)
            return AlignmentScore.None;

        var loose = !IsTight(lyricLine.Row);
        var score = AlignmentScore.None;
        for (var k = 0; k < chordLine.Row.Words.Count; k++)
        {
            if (TruthColumn(chordLine.Row.Words[k].Box.Left, lyricLine.Row, lyricColumns, words) is not { } truth)
                continue;

            var off = Math.Abs(chordColumns[k] - truth);
            score += new AlignmentScore(1, off == 0 ? 1 : 0, off == 1 ? 1 : 0, off >= 2 ? 1 : 0, loose ? 1 : 0, loose && off >= 2 ? 1 : 0, 0);
        }

        return score;
    }

    // Output column of the lyric letter whose box contains x, or null when x falls between letters or outside the row.
    private static int? TruthColumn(double x, TextRow lyric, int[] columns, Dictionary<(double, double), Word> words)
    {
        for (var j = 0; j < lyric.Words.Count; j++)
        {
            if (!words.TryGetValue((lyric.Words[j].Box.Left, lyric.Words[j].Box.Bottom), out var word))
                return null;

            var column = columns[j];
            var shift = lyric.Words[j].Box.Left - word.BoundingBox.Left;
            foreach (var letter in word.Letters)
            {
                var left = letter.BoundingBox.Left + shift;
                if (left > x)
                    return null;
                if (letter.BoundingBox.Right + shift > x + InsideTolerancePoints)
                    return column;
                column += letter.Value.Sum(c => LigatureLengths.GetValueOrDefault(c, 1));
            }
        }

        return null;
    }

    private static bool IsChordRow(TextRow row)
    {
        var tokens = row.Words.Select(w => w.Text).Where(t => t is not "-" and not "–").ToList();
        return tokens.Count > 0 && tokens.Count(LineTruth.IsChord) >= MinChordShare * tokens.Count;
    }

    private static bool IsTight(TextRow row) =>
        row.Words.Count >= 2
        && row.Coverage >= TightCoverage
        && row.Words.Zip(row.Words.Skip(1), (a, b) => b.Box.Left - a.Box.Right).Max() <= row.Height;

    // Start column of each word in the line text, found left to right.
    private static int[]? Columns(string text, IReadOnlyList<PositionedWord> words)
    {
        var columns = new int[words.Count];
        var cursor = 0;
        for (var i = 0; i < words.Count; i++)
        {
            var at = text.IndexOf(words[i].Text, cursor, StringComparison.Ordinal);
            if (at < 0)
                return null;
            columns[i] = at;
            cursor = at + words[i].Text.Length;
        }

        return columns;
    }
}
