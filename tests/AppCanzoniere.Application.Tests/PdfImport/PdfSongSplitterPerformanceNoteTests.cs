using AppCanzoniere.Application.PdfImport;
using static AppCanzoniere.Application.Tests.PdfImport.SyntheticPages;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfSongSplitterPerformanceNoteTests
{
    private const string Verses =
        "  la-           sol        do    la-\n1  Luna alta sopra il lago      vento lieve tra i canneti\n         do  sol la-\n   barche ferme       sull'acqua\n\n"
        + "  la-           sol        do    la-\n2  Rondini tornano al tetto     fiori nuovi sul sentiero\n         do  sol la-\n   passi lenti       verso casa";

    private const string TitleWithNote = "7 NOTTE DI FESTA CANTA tono reale MI- (chitarra capotasto 4° tasto LA-)";

    [Fact]
    public void A_numbered_title_with_a_long_performance_note_is_a_title_and_the_note_is_a_subtitle()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"{TitleWithNote}\n                       (tastiera +2 SI-)\n\n{Verses}") });

        var song = Assert.Single(result.Songs);
        Assert.Equal("NOTTE DI FESTA CANTA", song.Title);
        Assert.Contains(song.Subtitles, s => s.Contains("capotasto"));
        Assert.Contains(song.Lines, l => l.Contains("Luna alta sopra il lago"));
    }

    [Fact]
    public void A_keyboard_version_with_the_same_number_continues_the_song()
    {
        var first = $"{TitleWithNote}\n\nACCORDI PER CHITARRA TRASPOSTA +4\n{Verses}";
        var second = $"                (PER TASTIERA)\n\n7 BIS  NOTTE DI FESTA CANTA tono reale MI-  (tastiera +2 SI-)\n\nACCORDI PER TASTIERA TRASPOSTA +2\n{Verses}";

        var result = PdfSongSplitter.Split(new[] { Page(1, first), Page(2, second) });

        var song = Assert.Single(result.Songs);
        Assert.Equal("NOTTE DI FESTA CANTA", song.Title);
        Assert.Equal((1, 2), (song.FirstPage, song.LastPage));
    }

    [Fact]
    public void A_title_with_a_key_note_and_its_number_on_the_next_line_is_a_title()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"SERA TRANQUILLA (in LA)\n\n12\nChitarra LA- capotasto in 3° tasto\n{Verses}") });

        var song = Assert.Single(result.Songs);
        Assert.Equal("SERA TRANQUILLA", song.Title);
    }

    [Theory]
    [InlineData("Tastiera RE- trasp -2")]
    [InlineData("Chitarra LA- capotasto in 3° tasto")]
    [InlineData("ACCORDI PER CHITARRA TRASPOSTA +4")]
    [InlineData("(PER TASTIERA)")]
    [InlineData("Capotasto 2° tasto")]
    public void A_hint_line_on_a_later_page_is_not_a_new_song(string hint)
    {
        var second = $"{hint}\n\n{Verses}";

        var result = PdfSongSplitter.Split(new[] { Page(1, $"SERA TRANQUILLA (in LA)\n\n{Verses}"), Page(2, second) });

        var song = Assert.Single(result.Songs);
        Assert.Equal("SERA TRANQUILLA", song.Title);
    }

    [Fact]
    public void A_numbered_hint_line_and_a_repeated_title_for_another_key_continue_the_song()
    {
        var first = $"SERA TRANQUILLA (in LA)\n\n12  Chitarra LA- capotasto in 3° tasto\n{Verses}";
        var second = $"SERA TRANQUILLA (in RE)\n\n12  Tastiera RE- trasp -2\n{Verses}";

        var result = PdfSongSplitter.Split(new[] { Page(1, first), Page(2, second) });

        var song = Assert.Single(result.Songs);
        Assert.Equal("SERA TRANQUILLA", song.Title);
        Assert.Equal((1, 2), (song.FirstPage, song.LastPage));
    }

    [Fact]
    public void A_second_version_that_numbers_its_verses_again_stays_in_the_song()
    {
        var verses = "  la-    sol\n1) Luna alta sopra il lago\n  do  sol\n  vento lieve\n\n  la-    sol\n2) Rondini al tetto\n  do  sol\n  fiori nuovi\n\n  la-    sol\n3) Passi verso casa\n  do  sol\n  notte calma";
        var first = $"SERA TRANQUILLA (in LA)\n\n12\n{verses}";
        var second = $"SERA TRANQUILLA (in RE)\n\n12  Tastiera RE-\n{verses}\n\n  la-    sol\n4) Fine del cammino\n  do  sol\n  alba chiara";

        var result = PdfSongSplitter.Split(new[] { Page(1, first), Page(2, second) });

        Assert.Single(result.Songs);
    }

    [Fact]
    public void A_title_with_a_word_added_and_the_same_number_is_the_same_song()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"12 SERA TRANQUILLA\n\n{Verses}"), Page(2, $"12 SERA TRANQUILLA ABBASSATO\n\n{Verses}") });

        Assert.Equal("SERA TRANQUILLA", Assert.Single(result.Songs).Title);
    }

    [Theory]
    [InlineData("Tastiera RE- trasp -2")]
    [InlineData("(tastiera +2 SI-)")]
    [InlineData("Chitarra LA- capotasto in 3° tasto")]
    [InlineData("ACCORDI PER CHITARRA TRASPOSTA +4")]
    public void An_untitled_sheet_is_not_titled_by_a_hint_line(string hint)
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"{hint}\n  la-    sol\nCanto la sera sul mare\n  do    la-\nvento lieve tra i canneti") });

        Assert.Equal("Canto la sera sul mare", Assert.Single(result.Songs).Title);
    }

    [Fact]
    public void A_lyric_that_starts_with_an_instrument_word_is_still_a_lyric()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, "  la-    sol\nChitarre e tamburi nella piazza\n  do    la-\nvento lieve tra i canneti") });

        Assert.Equal("Chitarre e tamburi nella piazza", Assert.Single(result.Songs).Title);
    }
}
