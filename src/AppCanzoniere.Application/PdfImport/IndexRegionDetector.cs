using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>The song entries of an index column: how many, and their titles in comparison form.</summary>
internal sealed record IndexReading(int EntryCount, IReadOnlyList<string> Titles);

/// <summary>Recognises columns that list song titles (a table of contents) instead of holding songs.</summary>
internal static partial class IndexRegionDetector
{
    private const int MinLines = 5;
    private const int MinFollowingEntries = 2;
    private const double NumberedShare = 0.4;
    private const double PageNumberShare = 0.6;
    private const double FollowingShare = 0.5;

    // A heading sits at least this many columns left of the entries it introduces; right-aligned page numbers shift by less.
    private const int HeadingIndentGap = 3;
    private const int MinTitleLetters = 4;
    private const double LeaderShare = 0.5;

    [GeneratedRegex(@"\.{3,}")]
    private static partial Regex Leader();

    [GeneratedRegex(@"\s{5,}")]
    private static partial Regex WideGap();

    [GeneratedRegex(@"[\s.…]*(?:pag\.?\s*)?[-–]?\s*[0-9]{1,4}(?:\s*[-–])?$", RegexOptions.IgnoreCase)]
    private static partial Regex TrailingPage();

    /// <summary>Returns the number of index entries when the column is an index, otherwise null.</summary>
    public static int? CountEntries(Region region, bool followsIndex) => Read(region, followsIndex)?.EntryCount;

    /// <summary>Reads the song entries of an index column, leaving out section headings and running footers; null when the column is no index.</summary>
    public static IndexReading? Read(Region region, bool followsIndex)
    {
        var content = Enumerable.Range(0, region.Lines.Count).Where(i => !SongLineKinds.IsBlank(region.Lines[i]) && !SongLineKinds.IsNoise(region.Lines[i])).ToList();
        if (content.Count < MinFollowingEntries || content.Any(i => region.IsChord[i]))
            return null;

        var numbered = content.Count(i => SongLineKinds.IsNumbered(region.Lines[i]));
        var withPage = content.Count(i => SongLineKinds.EndsWithNumber(region.Lines[i]));
        var isIndex = IsFullIndex(content.Count, numbered, withPage) || (followsIndex && numbered >= MinFollowingEntries && numbered >= FollowingShare * content.Count);
        if (!isIndex)
            return null;

        var entries = content.Where(i => SongLineKinds.IsNumbered(region.Lines[i]) || SongLineKinds.EndsWithNumber(region.Lines[i])).ToList();
        var songs = WithoutHeadings(region, WithoutFooter(region, entries, content));
        var titles = songs.Select(i => Comparable(EntryTitle(region.Lines[i]))).Where(t => t.Length >= MinTitleLetters).Distinct().ToList();
        return new IndexReading(songs.Count, titles);
    }

    /// <summary>The title of an index line without its number or dot leader and page number.</summary>
    public static string EntryTitle(string line) =>
        SongLineKinds.StripNumber(TrailingPage().Replace(line.Trim(), string.Empty)).Text;

    /// <summary>The letters and digits of a title in lower case, so an index entry and the title in the song compare equal.</summary>
    public static string Comparable(string text) =>
        new(text.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

    // A footer is the last line, with a gap before its page number and no leader while the entries above have one.
    private static List<int> WithoutFooter(Region region, List<int> entries, List<int> content)
    {
        if (entries.Count < 3 || entries[^1] != content[^1])
            return entries;

        var last = region.Lines[entries[^1]];
        var others = entries.Take(entries.Count - 1).ToList();
        var leadered = others.Count(i => Leader().IsMatch(region.Lines[i]));
        return !Leader().IsMatch(last) && WideGap().IsMatch(last.Trim()) && leadered >= LeaderShare * others.Count ? others : entries;
    }

    // A heading is set well left of the usual entry indent, which is the widest indent shared by two entries.
    private static List<int> WithoutHeadings(Region region, List<int> entries)
    {
        var shared = entries.GroupBy(i => SongLineKinds.Indent(region.Lines[i])).Where(g => g.Count() >= 2).Select(g => g.Key).DefaultIfEmpty(-1).Max();
        return shared < 0 ? entries : entries.Where(i => SongLineKinds.Indent(region.Lines[i]) > shared - HeadingIndentGap).ToList();
    }

    private static bool IsFullIndex(int lines, int numbered, int withPage) =>
        lines >= MinLines && ((numbered >= MinLines && numbered >= NumberedShare * lines) || withPage >= PageNumberShare * lines);
}
