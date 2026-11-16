using System.Text.RegularExpressions;

namespace AppCanzoniere.PdfBench;

/// <summary>How the label of one extracted line was found.</summary>
internal enum TruthSource
{
    /// <summary>No pdftotext line of the page contains the line's words; the strict chord pattern decided.</summary>
    Unmatched,

    /// <summary>The line is a whole pdftotext line, or part of a labelled chord line.</summary>
    Label,

    /// <summary>The line is one column of an unlabelled two-column pdftotext line; the strict chord pattern decided.</summary>
    Fragment
}

/// <summary>The chord-line truth of one extracted line, how it was found, and where its words start in the pdftotext line.</summary>
internal sealed record Truth(TruthSource Source, bool IsChordLine, int SourceLine = 0)
{
    /// <summary>Index of the first matching word in the pdftotext line, or -1 when the match was by compact text or failed.</summary>
    public int TokenStart { get; init; } = -1;
}

/// <summary>
/// Labels an extracted line from the hand labels of the pdftotext lines of the same page, by content:
/// the line's words must appear, in order and contiguous, in one pdftotext line.
/// </summary>
internal static partial class LineTruth
{
    // Compact matching (spaces removed) is tried only for lines at least this long, to avoid chance hits.
    private const int MinCompactLength = 6;

    // A label in front of chords has at most this many words, the last ending in ':' or '.'.
    private const int MaxLabelTokens = 2;

    // One or more chords written together: Italian or English root, accidental, suffixes, optional bass, optional parentheses.
    [GeneratedRegex(
        @"^\(?(?:/?(?:(?i:do|re|mi|fa|sol|la|si)|[A-G])(?:#|b)?(?:m|min|maj|dim|aug|sus|add|M|\+|-|°|\d)*(?:/(?:(?i:do|re|mi|fa|sol|la|si)|[A-G])(?:#|b)?)?)+\)?$",
        RegexOptions.CultureInvariant)]
    private static partial Regex ChordToken();

    // A chord extension set apart from its chord, such as a raised "7".
    [GeneratedRegex(@"^\(?\d+\+?\)?$", RegexOptions.CultureInvariant)]
    private static partial Regex ExtensionToken();

    public static Truth Of(string line, double relativeY, LabelledPage page)
    {
        var tokens = Tokens(line);
        if (tokens.Length == 0)
            return new Truth(TruthSource.Unmatched, false);

        var candidates = page.Lines.Select((l, i) => (Line: l, Index: i)).Where(c => IndexOf(Tokens(c.Line.Text), tokens) >= 0).ToList();
        if (candidates.Count == 0 && Compact(tokens).Length >= MinCompactLength)
            candidates = page.Lines.Select((l, i) => (Line: l, Index: i)).Where(c => Compact(Tokens(c.Line.Text)).Contains(Compact(tokens), StringComparison.Ordinal)).ToList();
        if (candidates.Count == 0)
            return new Truth(TruthSource.Unmatched, IsChordLine(tokens));

        // Repeated lines usually share their label; otherwise the candidate nearest in page position wins.
        var nearest = candidates.MinBy(c => Math.Abs(((double)c.Index / page.Lines.Count) - relativeY));
        return Judge(tokens, nearest.Line) with { SourceLine = nearest.Line.Number, TokenStart = IndexOf(Tokens(nearest.Line.Text), tokens) };
    }

    /// <summary>True when the words are chords only, optionally after a short label; used where no hand label applies.</summary>
    public static bool LooksLikeChords(string line) => IsChordLine(Tokens(line));

    /// <summary>True when the word is one chord or several written together.</summary>
    public static bool IsChord(string token) => ChordToken().IsMatch(token);

    private static Truth Judge(string[] tokens, LabelledLine source)
    {
        var whole = Tokens(source.Text).Length == tokens.Length;
        if (source.IsChordLine)
            return new Truth(TruthSource.Label, whole || tokens.Any(t => ChordToken().IsMatch(t)));

        return whole ? new Truth(TruthSource.Label, false) : new Truth(TruthSource.Fragment, IsChordLine(tokens));
    }

    // Chords only, optionally after a short label such as "RIT." or "BRIDGE 2:".
    private static bool IsChordLine(string[] tokens)
    {
        var first = Array.FindIndex(tokens, t => ChordToken().IsMatch(t));
        if (first < 0 || first > MaxLabelTokens || (first > 0 && !tokens[first - 1].EndsWith(':') && !tokens[first - 1].EndsWith('.')))
            return false;

        return tokens.Skip(first).All(t => ChordToken().IsMatch(t) || ExtensionToken().IsMatch(t));
    }

    public static string[] Tokens(string text) => text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

    private static string Compact(string[] tokens) => string.Concat(tokens);

    private static int IndexOf(string[] haystack, string[] needle)
    {
        for (var start = 0; start + needle.Length <= haystack.Length; start++)
        {
            if (needle.Select((t, i) => t == haystack[start + i]).All(m => m))
                return start;
        }

        return -1;
    }
}
