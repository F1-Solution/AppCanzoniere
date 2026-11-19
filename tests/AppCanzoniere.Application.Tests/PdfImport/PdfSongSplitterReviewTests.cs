using AppCanzoniere.Application.PdfImport;
using static AppCanzoniere.Application.Tests.PdfImport.SyntheticPages;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfSongSplitterReviewTests
{
    private const string Chords = "Do   Sol\nIl canto sale\nLa-  Fa\nsopra la valle";

    [Fact]
    public void An_indented_refrain_repeated_between_verses_is_not_a_title()
    {
        var page = $"LA LUNA\n\n{Chords}\n\n            Alleluia, alleluia\n\nUn verso qui\nun altro verso\n\n            Alleluia, alleluia\n\nUltimo verso\nsopra il mare";

        Assert.Single(PdfSongSplitter.Split(new[] { Page(1, page) }).Songs);
    }

    [Fact]
    public void An_indented_refrain_in_the_same_font_as_the_verses_is_not_a_title()
    {
        var page = $"LA LUNA\n\n{Chords}\n\n            Gloria al Signore!\n\nUn verso qui\nun altro verso";

        Assert.Single(PdfSongSplitter.Split(new[] { Page(1, page) }).Songs);
    }

    [Fact]
    public void A_larger_indented_line_that_repeats_in_the_region_is_not_a_title()
    {
        var page = $"LA LUNA\n\n{Chords}\n\n^            Gloria al Signore!\n\nUn verso qui\nun altro verso\n\n            Gloria al Signore!\n\nFine";

        Assert.Single(PdfSongSplitter.Split(new[] { Page(1, page) }).Songs);
    }

    [Fact]
    public void A_title_made_of_the_letters_of_roman_numerals_but_not_a_numeral_stays_a_title()
    {
        var page = $"VIVI\n\n{Chords}";

        var song = Assert.Single(PdfSongSplitter.Split(new[] { Page(1, page) }).Songs);

        Assert.Equal("VIVI", song.Title);
    }

    [Theory]
    [InlineData("ABBASSATA")]
    [InlineData("ALZATA")]
    [InlineData("- ABBASSATO")]
    [InlineData("(alzato)")]
    public void Key_variant_forms_continue_the_previous_song(string variant)
    {
        var first = $"                 LA CANZONE\n\n{Chords}";
        var second = $"                 LA CANZONE {variant}\n\n{Chords}";

        var song = Assert.Single(PdfSongSplitter.Split(new[] { Page(1, first), Page(2, second) }).Songs);

        Assert.Equal((1, 2), (song.FirstPage, song.LastPage));
    }

    [Theory]
    [InlineData("ALTO")]
    [InlineData("BASSO")]
    public void A_title_extending_the_previous_one_with_an_ordinary_word_is_another_song(string word)
    {
        var first = $"                 SANTO\n\n{Chords}";
        var second = $"                 SANTO {word}\n\n{Chords}";

        Assert.Equal(2, PdfSongSplitter.Split(new[] { Page(1, first), Page(2, second) }).Songs.Count);
    }
}
