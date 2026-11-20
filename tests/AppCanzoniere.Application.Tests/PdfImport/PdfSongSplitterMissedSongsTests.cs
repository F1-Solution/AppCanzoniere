using AppCanzoniere.Application.PdfImport;
using static AppCanzoniere.Application.Tests.PdfImport.SyntheticPages;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfSongSplitterMissedSongsTests
{
    private const string Song1 = "1. Prima canzone\n\nDo        Sol\nLa luna sale piano\nLa-       Fa\nnel cielo sereno";
    private const string Song2 = "Do        Sol\nIl vento canta forte\nLa-       Fa\nsulla collina verde";

    // A heading left alone at the foot of a column keeps its song, whose text starts the next column.
    [Fact]
    public void A_numbered_title_alone_at_the_foot_of_a_column_still_starts_its_song()
    {
        var left = $"{Song1}\nFa   Do\nUn ultimo verso lento\n  2.  Seconda canzone";
        var right = $"{Song2}\n\n  3.  Terza canzone\n\nDo  Sol\nSi alza il canto\nLa-  Fa\nsopra la valle";

        var result = PdfSongSplitter.Split(new[] { TwoColumns(1, left, right) });

        Assert.Equal(new[] { "Prima canzone", "Seconda canzone", "Terza canzone" }, result.Songs.Select(s => s.Title));
        Assert.Contains(result.Songs[1].Lines, l => l.Trim() == "sulla collina verde");
    }

    [Fact]
    public void A_numbered_title_followed_only_by_a_page_number_still_starts_its_song()
    {
        var page1 = $"{Song1}\n\n  2.  Seconda canzone\n\n                 46";
        var page2 = $"{Song2}\n\n  3.  Terza canzone\n\nDo  Sol\nSi alza il canto";

        var result = PdfSongSplitter.Split(new[] { Page(1, page1), Page(2, page2) });

        Assert.Equal(new[] { "Prima canzone", "Seconda canzone", "Terza canzone" }, result.Songs.Select(s => s.Title));
    }

    [Fact]
    public void A_centred_title_with_a_comma_starts_a_song()
    {
        var page = "                 Prima canzone\n\nDo   Sol\nIl canto sale\nLa-  Fa\nsopra la valle\n\n"
            + "^              Dammi la mano, amico\n\nDo   Sol\nIl vento canta forte\nLa-  Fa\nsulla collina verde";

        var result = PdfSongSplitter.Split(new[] { Page(1, page) });

        Assert.Equal(new[] { "Prima canzone", "Dammi la mano, amico" }, result.Songs.Select(s => s.Title));
    }

    [Fact]
    public void A_centred_title_over_a_song_without_chords_starts_a_song()
    {
        var page = "                 Prima canzone\n\nDo   Sol\nIl canto sale\nLa-  Fa\nsopra la valle\n\n"
            + "^              Il mare (Sal 12)\n\nIl mare canta forte\nsulla spiaggia chiara\n1. Le onde vanno lente\ne tornano a riva";

        var result = PdfSongSplitter.Split(new[] { Page(1, page) });

        Assert.Equal(new[] { "Prima canzone", "Il mare (Sal 12)" }, result.Songs.Select(s => s.Title));
    }

    [Fact]
    public void Centred_lines_of_a_dedication_page_are_not_a_song()
    {
        var page = "       Una frase che ricorda\n       i giorni del campo\n\n          Il Fondatore\n          Roma 1921";

        var result = PdfSongSplitter.Split(new[] { Page(1, page) });

        Assert.Single(result.Songs);
    }

    [Fact]
    public void A_centred_title_over_a_right_aligned_author_is_the_title_not_the_author()
    {
        var page = "^                                  Dov'e il canto\n\n                                                                                       a. Rossi\n\nCapotasto al I                                                                         trasc. b.c.\n\n"
            + "         Do      v'e  ca  ri   ta\n\n                      II       II";

        var result = PdfSongSplitter.Split(new[] { Page(1, page) }, null, "sheet.pdf");

        Assert.Equal("Dov'e il canto", Assert.Single(result.Songs).Title);
    }

    // Roman numerals of a finger-position row are not words.
    [Fact]
    public void A_row_of_roman_numerals_is_not_a_title()
    {
        var page = "                 Canto di prova\n\n  II       II        III\n\nCanto di prova qui\nsopra il monte alto\n\n  II   II";

        var result = PdfSongSplitter.Split(new[] { Page(1, page) }, null, "sheet.pdf");

        var song = Assert.Single(result.Songs);
        Assert.Equal("Canto di prova", song.Title);
    }

    // The same sheet repeated in a lower key, titled with the key variant, is the same song.
    [Theory]
    [InlineData("ABBASSATO")]
    [InlineData("ALZATO")]
    public void A_page_titled_with_a_key_variant_of_the_previous_title_continues_the_song(string variant)
    {
        var first = "                 LA CANZONE\n\nDo   Sol\nIl canto sale\nLa-  Fa\nsopra la valle";
        var second = $"                 LA CANZONE {variant}\n\nRe   La\nIl canto sale\nSi-  Sol\nsopra la valle";

        var result = PdfSongSplitter.Split(new[] { Page(1, first), Page(2, second) });

        var song = Assert.Single(result.Songs);
        Assert.Equal((1, 2), (song.FirstPage, song.LastPage));
    }

    [Fact]
    public void A_different_title_that_extends_the_previous_one_is_another_song()
    {
        var first = "                 LA CANZONE\n\nDo   Sol\nIl canto sale\nLa-  Fa\nsopra la valle";
        var second = "                 LA CANZONE DEL MARE\n\nRe   La\nL'onda sale\nSi-  Sol\nsopra la riva";

        var result = PdfSongSplitter.Split(new[] { Page(1, first), Page(2, second) });

        Assert.Equal(2, result.Songs.Count);
    }
}
