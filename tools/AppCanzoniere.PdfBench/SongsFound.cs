using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.PdfBench;

/// <summary>The songs a PDF splits into, each converted, with the number of index entries seen.</summary>
internal sealed record SongsFound(IReadOnlyList<ConvertedSong> Songs, int IndexEntries, IReadOnlyList<int> SkippedPages)
{
    public static readonly SongsFound None = new(Array.Empty<ConvertedSong>(), 0, Array.Empty<int>());

    public static SongsFound Measure(PdfTextResult text)
    {
        if (!text.IsSuccess)
            return None;

        var split = PdfSongSplitter.Split(text.Pages);
        return new SongsFound(split.Songs.Select(PdfSongConverter.Convert).ToList(), split.IndexEntryCount, split.SkippedPages);
    }
}
