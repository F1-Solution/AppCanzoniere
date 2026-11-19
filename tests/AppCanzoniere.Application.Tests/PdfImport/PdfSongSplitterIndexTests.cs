using AppCanzoniere.Application.PdfImport;
using static AppCanzoniere.Application.Tests.PdfImport.SyntheticPages;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfSongSplitterIndexTests
{
    private const string SongA = "Do        Sol\nLa luna sale piano\nLa-       Fa\nsopra il mare scuro";

    // Index rows end with the page between dashes, may start with the previous page, and the page numbers of the sheet sit on their own lines.
    [Fact]
    public void Index_rows_with_dashed_page_numbers_are_all_counted_and_stray_page_numbers_are_not()
    {
        var rows = Enumerable.Range(1, 6).Select(n => $"  {n} TITOLO NUMERO {n}                 - {n + 7} -")
            .Concat(Enumerable.Range(7, 6).Select(n => $"- {n + 50} -   {n}  TITOLO NUMERO {n}            - {n + 7} -"));
        var index = Page(2, "     INDICE\n" + string.Join("\n- 1 -\n", rows) + "\n Pag.\n- 92 -");

        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{SongA}"), index });

        Assert.Equal(12, result.IndexEntryCount);
        Assert.Single(result.Songs);
    }

    [Fact]
    public void A_song_page_that_ends_with_a_dashed_page_number_is_not_an_index()
    {
        var page = Page(1, $"1. LA LUNA\n{SongA}\n\n2. IL VENTO\n{SongA}\n\n- 4 -");

        var result = PdfSongSplitter.Split(new[] { page });

        Assert.Equal(0, result.IndexEntryCount);
        Assert.Equal(2, result.Songs.Count);
    }
}
