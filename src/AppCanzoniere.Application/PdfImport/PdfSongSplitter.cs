namespace AppCanzoniere.Application.PdfImport;

/// <summary>Cuts the laid-out pages of a songbook into songs, skipping covers, blank pages and indexes.</summary>
public static class PdfSongSplitter
{
    /// <summary>Splits the pages, read column by column in order, into songs; one pass over the text.</summary>
    public static SongSplitResult Split(IReadOnlyList<PageText> pages, string? documentTitle = null, string? fileName = null)
    {
        ArgumentNullException.ThrowIfNull(pages);
        var builder = new SongCollector();
        var indexEntries = 0;
        var contentRegions = new List<Region>();
        var followsIndex = false;
        foreach (var page in pages.Where(p => !p.IsScanned))
        {
            foreach (var block in page.Blocks)
            {
                var region = Region.Of(page.PageNumber, block.Lines.Select(l => l.Text).ToList());
                var entries = IndexRegionDetector.CountEntries(region, followsIndex);
                followsIndex = entries is not null;
                if (entries is { } count)
                {
                    indexEntries += count;
                    builder.EndSong();
                    continue;
                }

                contentRegions.Add(region);
                builder.AddRegion(region);
            }
        }

        var songs = builder.Finish();
        if (songs.Count == 0 && UntitledSheet.Build(contentRegions, documentTitle, fileName) is { } sheet)
            songs = new[] { sheet };

        return new SongSplitResult(songs, indexEntries, SkippedPages(pages, songs));
    }

    private static IReadOnlyList<int> SkippedPages(IReadOnlyList<PageText> pages, IReadOnlyList<SplitSong> songs) =>
        pages.Select(p => p.PageNumber).Where(n => !songs.Any(s => n >= s.FirstPage && n <= s.LastPage)).ToList();
}

/// <summary>Collects the lines of the song being read and the finished songs.</summary>
internal sealed class SongCollector
{
    private readonly List<SplitSong> _songs = new();
    private string? _title;
    private IReadOnlyList<string> _subtitles = Array.Empty<string>();
    private List<string> _lines = new();
    private int _firstPage;
    private int _lastPage;
    private int _lastNumber;
    private bool _numberedBook;
    private int _verseNumber;

    public void AddRegion(Region region)
    {
        SeparateFromPreviousRegion();
        var i = 0;
        while (i < region.Lines.Count)
        {
            var found = SongTitleDetector.Detect(region, i, new TitleContext(_lastNumber, _numberedBook, _verseNumber == 0 ? 0 : _verseNumber + 1));
            if (found is not null && IsSameSongAgain(found))
            {
                _lastPage = region.Page;
                _verseNumber = 0;
                i = found.NextLine;
                continue;
            }

            if (found is not null)
            {
                EndSong();
                _title = found.Title;
                _subtitles = found.Subtitles;
                NoteNumber(found.Number);
                _firstPage = _lastPage = region.Page;
                i = found.NextLine;
                continue;
            }

            if (!PerformanceHints.IsHint(region.Lines[i]) && !PerformanceHints.IsBisOfLast(region.Lines[i], _lastNumber))
                AddLine(region.Lines[i], region.Page);
            i++;
        }
    }

    // A title repeated for another key or instrument, with the same number, continues the song.
    private bool IsSameSongAgain(SongTitleDetector.Found found) =>
        _title is not null
        && (found.Number is null ? found.HadNote : found.Number == _lastNumber)
        && (string.Equals(found.Title, _title, StringComparison.OrdinalIgnoreCase) || (found.Number is not null && found.Title.StartsWith(_title, StringComparison.OrdinalIgnoreCase)));

    public void EndSong()
    {
        if (_title is not null)
            Keep();
        _title = null;
        _subtitles = Array.Empty<string>();
        _lines = new List<string>();
        _verseNumber = 0;
    }

    public IReadOnlyList<SplitSong> Finish()
    {
        EndSong();
        return _songs;
    }

    private void NoteNumber(int? number)
    {
        if (number is null)
            return;
        _lastNumber = number.Value;
        _numberedBook = true;
    }

    private void Keep()
    {
        var lines = Trim(_lines);
        if (lines.Count > 0)
            _songs.Add(new SplitSong(_title!, _firstPage, _lastPage, lines, _subtitles));
    }

    private void AddLine(string line, int page)
    {
        if (SongLineKinds.IsNoise(line))
            return;
        NoteVerse(line);
        if (_title is null)
            return;
        _lines.Add(line.TrimEnd());
        if (!SongLineKinds.IsBlank(line))
            _lastPage = page;
    }

    // Numbered verses start at 1 and count up inside a song.
    private void NoteVerse(string line)
    {
        var (number, _) = SongLineKinds.StripNumber(line);
        if (number is not null && number == _verseNumber + 1)
            _verseNumber = number.Value;
    }

    private void SeparateFromPreviousRegion()
    {
        if (_title is not null && _lines.Count > 0 && !SongLineKinds.IsBlank(_lines[^1]))
            _lines.Add(string.Empty);
    }

    private static List<string> Trim(List<string> lines)
    {
        var start = lines.FindIndex(l => !SongLineKinds.IsBlank(l));
        var end = lines.FindLastIndex(l => !SongLineKinds.IsBlank(l));
        return start < 0 ? new List<string>() : lines.GetRange(start, end - start + 1);
    }
}
