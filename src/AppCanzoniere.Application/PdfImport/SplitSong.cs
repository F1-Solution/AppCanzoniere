namespace AppCanzoniere.Application.PdfImport;

/// <summary>One song cut out of a booklet: its title, subtitles, page range and text lines.</summary>
public sealed record SplitSong(string Title, int FirstPage, int LastPage, IReadOnlyList<string> Lines, IReadOnlyList<string> Subtitles);

/// <summary>Songs found in a document, the number of index entries seen and the pages that hold no song text.</summary>
public sealed record SongSplitResult(IReadOnlyList<SplitSong> Songs, int IndexEntryCount, IReadOnlyList<int> SkippedPages);
