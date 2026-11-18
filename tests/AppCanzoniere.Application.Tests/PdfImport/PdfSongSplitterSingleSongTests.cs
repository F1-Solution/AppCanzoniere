using AppCanzoniere.Application.PdfImport;
using static AppCanzoniere.Application.Tests.PdfImport.SyntheticPages;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfSongSplitterSingleSongTests
{
    private const string Untitled = "   Do        Sol\nRit. Canterò al nuovo giorno\n   La-       Fa\n   sopra il mare scuro\n\n   Do        Sol\n1  Il vento canta forte\n   La-       Fa\n   sulla collina verde";
    private const string Verses = "   Do        Sol\n2  La luna sale piano\n   La-       Fa\n   nel cielo sereno";

    [Fact]
    public void A_sheet_without_a_title_row_is_one_song_over_all_its_pages()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, Untitled), Page(2, Verses) });

        var song = Assert.Single(result.Songs);
        Assert.Equal((1, 2), (song.FirstPage, song.LastPage));
        Assert.Contains(song.Lines, l => l.Trim() == "sulla collina verde");
        Assert.Contains(song.Lines, l => l.Trim() == "nel cielo sereno");
    }

    // Title order for a sheet without a title row: PDF metadata title, then the first lyric line, then the file name.
    [Fact]
    public void An_untitled_sheet_takes_the_metadata_title_first()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, Untitled) }, "Il mio canto", "scan0034.pdf");

        Assert.Equal("Il mio canto", Assert.Single(result.Songs).Title);
    }

    [Fact]
    public void An_untitled_sheet_without_metadata_takes_the_first_lyric_line_without_its_label()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, Untitled) }, null, "scan0034.pdf");

        Assert.Equal("Canterò al nuovo giorno", Assert.Single(result.Songs).Title);
    }

    [Fact]
    public void A_lyric_line_title_drops_the_dashes_that_stretch_a_syllable()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, "Do  Sol\nCanterò al Signo------re\nLa-  Fa\nsopra il mare scuro") });

        Assert.Equal("Canterò al Signore", Assert.Single(result.Songs).Title);
    }

    [Fact]
    public void An_untitled_sheet_with_chords_only_takes_the_file_name()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, "Do  Sol  La-\nFa  Do  Sol") }, "  ", "04-io-cantero.pdf");

        Assert.Equal("04-io-cantero", Assert.Single(result.Songs).Title);
    }

    [Fact]
    public void An_untitled_two_column_sheet_is_still_one_song()
    {
        var result = PdfSongSplitter.Split(new[] { TwoColumns(1, Untitled, Verses) });

        Assert.Single(result.Songs);
    }

    [Fact]
    public void A_sheet_with_a_title_keeps_its_own_title_over_the_hints()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"LA LUNA\n\n{Untitled}") }, "Altro", "altro.pdf");

        Assert.Equal("LA LUNA", Assert.Single(result.Songs).Title);
    }

    [Fact]
    public void A_document_without_any_text_has_no_song_even_with_hints()
    {
        var result = PdfSongSplitter.Split(new[] { Blank(1) }, "Titolo", "scan.pdf");

        Assert.Empty(result.Songs);
    }
}
