namespace AppCanzoniere.Application.PdfImport;

/// <summary>What the detector needs to know of the songs already read: the last title number, whether titles are numbered and the next verse number of the current song (0 when its verses are not numbered).</summary>
internal readonly record struct TitleContext(int LastNumber, bool NumberedBook, int NextVerse = 0);

/// <summary>Decides which lines of a column are song titles and which adjacent lines are their subtitles.</summary>
internal static class SongTitleDetector
{
    private const int MaxTitleLength = 70;
    private const int MaxTitleWords = 10;
    private const int MaxSequenceTitleWords = 7;
    private const int MinTitleLetters = 2;
    private const int MinUpperLetters = 4;
    private const int MinSubtitleLetters = 3;
    private const int MaxSubtitleLength = 50;
    private const int MaxAuthorLength = 30;
    private const int MaxSubtitles = 2;
    private const int CenteredIndent = 4;

    /// <summary>A title found at a line: its text, its number if any, and the line after its last subtitle.</summary>
    public sealed record Found(string Title, int? Number, IReadOnlyList<string> Subtitles, int NextLine);

    public static Found? Detect(Region region, int index, TitleContext context)
    {
        var (number, title) = SongLineKinds.StripNumber(region.Lines[index]);
        if (!IsTitle(region, index, number, title, context))
            return null;

        var subtitles = new List<string>();
        var next = index + 1;
        while (next < region.Lines.Count && subtitles.Count < MaxSubtitles && IsSubtitle(region, next))
        {
            subtitles.Add(region.Lines[next].Trim());
            next++;
        }

        return new Found(title, number, subtitles, next);
    }

    private static bool IsTitle(Region region, int i, int? number, string text, TitleContext context)
    {
        var line = region.Lines[i];
        if (SongLineKinds.IsBlank(line) || region.IsChord[i] || SongLineKinds.IsNoise(line) || !LooksLikeTitleText(text))
            return false;
        if (number is not null && number == context.NextVerse && !SongLineKinds.IsUpperCase(text, MinTitleLetters))
            return false;
        if (IsFirstVerseOfUntitledText(region, i, number, text, context))
            return false;
        if (i > 0 && !SongLineKinds.IsBlank(region.Lines[i - 1]) && !FollowsInSequence(region, i, number, context))
            return false;

        var next = NextContent(region, i);
        if (number is not null)
            return next >= 0 && (SongLineKinds.IsUpperCase(text, MinTitleLetters) || region.IsChord[next] || IsNextInSequence(number, text, context));
        if (SongLineKinds.IsUpperCase(text, MinUpperLetters))
            return next >= 0;

        return !context.NumberedBook && LooksLikeMixedCaseTitle(region, i, text, next);
    }

    // A mixed-case "1 text" below other text, before any numbered title, is the first verse of the song above.
    private static bool IsFirstVerseOfUntitledText(Region region, int i, int? number, string text, TitleContext context) =>
        number == 1 && !context.NumberedBook && !SongLineKinds.IsUpperCase(text, MinTitleLetters) && !IsFirstContent(region, i);

    // The next number of a numbered book is a title even when its song has no chord lines, if the text is short like a title.
    private static bool IsNextInSequence(int? number, string text, TitleContext context) =>
        context.NumberedBook && number == context.LastNumber + 1 && SongLineKinds.WordCount(text) <= MaxSequenceTitleWords && !text.Contains(',');

    // A numbered title may follow text directly when it carries the next number of the book and the line above is not a chord line.
    private static bool FollowsInSequence(Region region, int i, int? number, TitleContext context) =>
        context.NumberedBook && number == context.LastNumber + 1 && !region.IsChord[i - 1];

    private static bool LooksLikeMixedCaseTitle(Region region, int i, string text, int next)
    {
        if (text.Contains(',') || text[^1] == '.' || next < 0)
            return false;
        if (!IsFirstContent(region, i) && SongLineKinds.Indent(region.Lines[i]) < CenteredIndent)
            return false;

        return region.IsChord[next] || IsAuthorLine(region, next);
    }

    // A short indented line under the title, followed by chords, is the author; the title above it is still a title.
    private static bool IsAuthorLine(Region region, int i)
    {
        var after = NextContent(region, i);
        return after >= 0 && region.IsChord[after] && !region.IsChord[i] && IsShortIndented(region.Lines[i]);
    }

    private static bool IsShortIndented(string line) =>
        line.Trim().Length <= MaxAuthorLength && SongLineKinds.Indent(line) >= CenteredIndent;

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

        return SongLineKinds.IsUpperCase(text, MinSubtitleLetters) || (text[0] == '(' && text[^1] == ')') || IsAuthorLine(region, i);
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
