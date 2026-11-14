using System.Text.RegularExpressions;

namespace AppCanzoniere.PdfBench;

/// <summary>How one extracted line relates to the columns of the pdftotext reference page.</summary>
internal enum SplitOutcome
{
    /// <summary>The line comes from one column of the reference, or is a whole full-width reference line.</summary>
    Correct,

    /// <summary>The line holds words from both columns of a two-column reference line: a missed split.</summary>
    Joined,

    /// <summary>The line is cut from a reference line where only a word space separates it from the rest: a wrong split.</summary>
    WrongSplit,

    /// <summary>The line matched no reference line word by word, so its columns cannot be checked.</summary>
    Unverified
}

/// <summary>Split outcome counts over a set of extracted lines.</summary>
internal sealed record SplitScore(int Correct, int Joined, int WrongSplit, int Unverified)
{
    public static readonly SplitScore None = new(0, 0, 0, 0);

    public int Checked => Correct + Joined + WrongSplit;

    public double? ErrorRate => Checked == 0 ? null : (double)(Joined + WrongSplit) / Checked;

    public static SplitScore operator +(SplitScore a, SplitScore b) =>
        new(a.Correct + b.Correct, a.Joined + b.Joined, a.WrongSplit + b.WrongSplit, a.Unverified + b.Unverified);

    public static SplitScore Of(SplitOutcome outcome) => outcome switch
    {
        SplitOutcome.Correct => new(1, 0, 0, 0),
        SplitOutcome.Joined => new(0, 1, 0, 0),
        SplitOutcome.WrongSplit => new(0, 0, 1, 0),
        _ => new(0, 0, 0, 1),
    };
}

/// <summary>
/// Finds the column boundary of a pdftotext -layout page, independently of the app's layout code, and judges whether an
/// extracted line keeps to one column. pdftotext starts every line of the right column at the same character column and
/// separates columns by at least two spaces, while words of running text are one space apart.
/// </summary>
internal static partial class ColumnReference
{
    // The right column starts within this share of the page's longest line.
    private const double MinBoundaryShare = 0.3;
    private const double MaxBoundaryShare = 0.7;

    // A right-column start follows at least this many spaces.
    private const int MinGapSpaces = 2;

    // The boundary needs at least this many lines whose right part, starting there, is text rather than chords.
    private const int MinStarts = 4;

    [GeneratedRegex(@"\S+")]
    private static partial Regex Word();

    /// <summary>Character column where the right column starts, or null when the page reads as one column.</summary>
    public static int? Boundary(IReadOnlyList<string> lines)
    {
        var width = lines.Count == 0 ? 0 : lines.Max(l => l.TrimEnd().Length);
        var from = (int)(MinBoundaryShare * width);
        var to = (int)(MaxBoundaryShare * width);
        var starts = new Dictionary<int, int>();
        foreach (var line in lines)
        {
            for (var c = Math.Max(from, MinGapSpaces); c <= Math.Min(to, line.Length - 1); c++)
            {
                if (IsPieceStart(line, c) && !LineTruth.LooksLikeChords(Piece(line, c)))
                    starts[c] = starts.GetValueOrDefault(c) + 1;
            }
        }

        if (starts.Count == 0)
            return null;

        var best = starts.MaxBy(s => s.Value);
        return best.Value >= MinStarts ? best.Key : null;
    }

    // A word that follows a gap of spaces after other text on the same line.
    private static bool IsPieceStart(string line, int c) =>
        line[c] != ' ' && line.AsSpan(c - MinGapSpaces, MinGapSpaces).IsWhiteSpace() && line[..c].Trim().Length > 0;

    // The text from c up to the next gap of spaces.
    private static string Piece(string line, int c)
    {
        var gap = line.IndexOf(new string(' ', MinGapSpaces), c, StringComparison.Ordinal);
        return gap < 0 ? line[c..] : line[c..gap];
    }

    /// <summary>Judges an extracted line whose words are words tokenStart.. of the reference line.</summary>
    public static SplitOutcome Judge(string source, int tokenStart, int tokenCount, int? boundary)
    {
        var words = Word().Matches(source);
        if (tokenStart < 0 || tokenStart + tokenCount > words.Count)
            return SplitOutcome.Unverified;

        var first = words[tokenStart];
        var last = words[tokenStart + tokenCount - 1];
        if (boundary is { } r && first.Index < r && last.Index + last.Length > r && IsTwoPart(source, r))
            return SplitOutcome.Joined;

        var cutBefore = tokenStart > 0 && first.Index - End(words[tokenStart - 1]) < MinGapSpaces;
        var cutAfter = tokenStart + tokenCount < words.Count && words[tokenStart + tokenCount].Index - End(last) < MinGapSpaces;
        return cutBefore || cutAfter ? SplitOutcome.WrongSplit : SplitOutcome.Correct;
    }

    private static int End(Match word) => word.Index + word.Length;

    // Text on both sides of the boundary, with a gap of spaces just before it.
    private static bool IsTwoPart(string line, int r) =>
        line.Length > r && line[..r].Trim().Length > 0 && line[r..].Trim().Length > 0 && line.AsSpan(r - MinGapSpaces, MinGapSpaces).IsWhiteSpace();
}
