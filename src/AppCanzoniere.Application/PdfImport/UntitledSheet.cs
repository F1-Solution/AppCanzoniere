using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>Builds the single song of a sheet in which no title row was found.</summary>
internal static partial class UntitledSheet
{
    private const int MaxTitleLength = 70;
    private const string FallbackTitle = "Senza titolo";

    [GeneratedRegex(@"^(?:ritornello|rit|coro)\b\.?\s*:?\s*", RegexOptions.IgnoreCase)]
    private static partial Regex SectionPrefix();

    [GeneratedRegex(@"-{2,}")]
    private static partial Regex StretchedSyllables();

    /// <summary>One song over the regions' pages, titled by the metadata title, else the first lyric line, else the file name.</summary>
    public static SplitSong? Build(IReadOnlyList<Region> regions, string? documentTitle, string? fileName)
    {
        var lines = new List<string>();
        var first = 0;
        var last = 0;
        foreach (var region in regions)
        {
            if (lines.Count > 0)
                lines.Add(string.Empty);
            lines.AddRange(region.Lines.Where(l => !SongLineKinds.IsNoise(l)).Select(l => l.TrimEnd()));
            if (region.Lines.Any(l => !SongLineKinds.IsBlank(l) && !SongLineKinds.IsNoise(l)))
            {
                first = first == 0 ? region.Page : first;
                last = region.Page;
            }
        }

        var trimmed = Trim(lines);
        if (trimmed.Count == 0)
            return null;

        var title = Clean(PdfDocumentTitle.Clean(documentTitle)) ?? FirstLyricLine(regions) ?? Clean(Path.GetFileNameWithoutExtension(fileName)) ?? FallbackTitle;
        return new SplitSong(title, first, last, trimmed, Array.Empty<string>());
    }

    private static List<string> Trim(List<string> lines)
    {
        var start = lines.FindIndex(l => !SongLineKinds.IsBlank(l));
        var end = lines.FindLastIndex(l => !SongLineKinds.IsBlank(l));
        return start < 0 ? new List<string>() : lines.GetRange(start, end - start + 1);
    }

    private static string? FirstLyricLine(IReadOnlyList<Region> regions)
    {
        foreach (var region in regions)
        {
            for (var i = 0; i < region.Lines.Count; i++)
            {
                var line = region.Lines[i];
                if (SongLineKinds.IsBlank(line) || SongLineKinds.IsNoise(line) || region.IsChord[i])
                    continue;
                var title = Clean(SectionPrefix().Replace(SongLineKinds.StripNumber(line).Text, string.Empty));
                if (title is not null && title.Count(char.IsLetter) >= 2 && title.Length <= MaxTitleLength)
                    return title;
            }
        }

        return null;
    }

    private static string? Clean(string? text)
    {
        var t = text is null ? null : StretchedSyllables().Replace(text, string.Empty).Trim().TrimEnd(',', ';', ':').Trim();
        return string.IsNullOrEmpty(t) ? null : t;
    }
}
