namespace AppCanzoniere.Application.PdfImport;

/// <summary>Recognises columns that list song titles (a table of contents) instead of holding songs.</summary>
internal static class IndexRegionDetector
{
    private const int MinLines = 5;
    private const int MinFollowingEntries = 2;
    private const double NumberedShare = 0.4;
    private const double PageNumberShare = 0.6;
    private const double FollowingShare = 0.5;

    /// <summary>Returns the number of index entries when the column is an index, otherwise null.</summary>
    public static int? CountEntries(Region region, bool followsIndex)
    {
        var content = Enumerable.Range(0, region.Lines.Count).Where(i => !SongLineKinds.IsBlank(region.Lines[i])).ToList();
        if (content.Count < MinFollowingEntries || content.Any(i => region.IsChord[i]))
            return null;

        var numbered = content.Count(i => SongLineKinds.IsNumbered(region.Lines[i]));
        var withPage = content.Count(i => SongLineKinds.EndsWithNumber(region.Lines[i]));
        var isIndex = IsFullIndex(content.Count, numbered, withPage) || (followsIndex && numbered >= MinFollowingEntries && numbered >= FollowingShare * content.Count);
        return isIndex ? content.Count(i => SongLineKinds.IsNumbered(region.Lines[i]) || SongLineKinds.EndsWithNumber(region.Lines[i])) : null;
    }

    private static bool IsFullIndex(int lines, int numbered, int withPage) =>
        lines >= MinLines && ((numbered >= MinLines && numbered >= NumberedShare * lines) || withPage >= PageNumberShare * lines);
}
