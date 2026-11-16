namespace AppCanzoniere.Application.PdfImport;

/// <summary>Decides which lines of a column are song titles and which adjacent lines are their subtitles.</summary>
internal static class SongTitleDetector
{
    private const int MaxTitleLength = 70;
    private const int MaxTitleWords = 10;
    private const int MinTitleLetters = 2;
    private const int MinUpperLetters = 4;
    private const int MinSubtitleLetters = 3;
    private const int MaxSubtitleLength = 50;
    private const int MaxSubtitles = 2;
    private const int CenteredIndent = 4;

    /// <summary>A title found at a line: its text and the line after its last subtitle.</summary>
    public sealed record Found(string Title, IReadOnlyList<string> Subtitles, int NextLine);

    public static Found? Detect(Region region, int index)
    {
        if (!IsTitle(region, index))
            return null;

        var title = SongLineKinds.StripNumber(region.Lines[index]).Text;
        var subtitles = new List<string>();
        var next = index + 1;
        while (next < region.Lines.Count && subtitles.Count < MaxSubtitles && IsSubtitle(region, next))
        {
            subtitles.Add(region.Lines[next].Trim());
            next++;
        }

        return new Found(title, subtitles, next);
    }

    private static bool IsTitle(Region region, int i)
    {
        var line = region.Lines[i];
        if (SongLineKinds.IsBlank(line) || region.IsChord[i] || SongLineKinds.IsNoise(line))
            return false;
        if (i > 0 && !SongLineKinds.IsBlank(region.Lines[i - 1]))
            return false;

        var (numbered, text) = SongLineKinds.StripNumber(line);
        if (!LooksLikeTitleText(text))
            return false;

        var next = NextContent(region, i);
        if (numbered)
            return next >= 0 && (SongLineKinds.IsUpperCase(text, MinTitleLetters) || region.IsChord[next]);
        if (SongLineKinds.IsUpperCase(text, MinUpperLetters))
            return next >= 0;

        return (IsFirstContent(region, i) || SongLineKinds.Indent(line) >= CenteredIndent) && next >= 0 && region.IsChord[next];
    }

    private static bool LooksLikeTitleText(string text) =>
        text.Length is > 0 and <= MaxTitleLength
        && text.Count(char.IsLetter) >= MinTitleLetters
        && SongLineKinds.WordCount(text) <= MaxTitleWords
        && !",;:".Contains(text[^1])
        && !SongLineKinds.IsSectionLabel(text);

    private static bool IsSubtitle(Region region, int i)
    {
        var line = region.Lines[i];
        var text = line.Trim();
        if (text.Length == 0 || text.Length > MaxSubtitleLength || region.IsChord[i] || SongLineKinds.IsNoise(line) || SongLineKinds.IsNumbered(text))
            return false;

        return SongLineKinds.IsUpperCase(text, MinSubtitleLetters) || (text[0] == '(' && text[^1] == ')');
    }

    private static int NextContent(Region region, int i)
    {
        for (var j = i + 1; j < region.Lines.Count; j++)
        {
            if (!SongLineKinds.IsBlank(region.Lines[j]) && !SongLineKinds.IsNoise(region.Lines[j]))
                return j;
        }

        return -1;
    }

    private static bool IsFirstContent(Region region, int i) =>
        Enumerable.Range(0, i).All(j => SongLineKinds.IsBlank(region.Lines[j]));
}
