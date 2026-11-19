using AppCanzoniere.Application.PdfImport;
using static AppCanzoniere.Application.Tests.PdfImport.SyntheticPages;

namespace AppCanzoniere.Application.Tests.PdfImport;

[Collection(TimingCollection.Name)]
public sealed class PdfSongSplitterTests
{
    private const string SongA = "Do        Sol\nLa luna sale piano\nLa-       Fa\nsopra il mare scuro";
    private const string SongB = "Re        La\nIl vento canta forte\nSi-       Sol\nsulla collina verde";

    private static string[] Titles(SongSplitResult result) => result.Songs.Select(s => s.Title).ToArray();

    [Fact]
    public void Empty_document_has_no_songs()
    {
        var result = PdfSongSplitter.Split(Array.Empty<PageText>());

        Assert.Empty(result.Songs);
        Assert.Equal(0, result.IndexEntryCount);
    }

    [Fact]
    public void Upper_case_titles_split_one_page_into_songs()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"LA LUNA\n\n{SongA}\n\nIL VENTO\n\n{SongB}") });

        Assert.Equal(new[] { "LA LUNA", "IL VENTO" }, Titles(result));
        Assert.Contains("La luna sale piano", result.Songs[0].Lines);
        Assert.DoesNotContain("IL VENTO", result.Songs[0].Lines);
        Assert.Contains("Il vento canta forte", result.Songs[1].Lines);
        Assert.All(result.Songs, s => Assert.Equal((1, 1), (s.FirstPage, s.LastPage)));
    }

    [Theory]
    [InlineData("12. LA LUNA", "LA LUNA")]
    [InlineData("12 - La luna", "La luna")]
    [InlineData("12) La luna", "La luna")]
    public void Numbered_titles_drop_the_number(string line, string title)
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"{line}\n{SongA}\n\n13. IL VENTO\n{SongB}") });

        Assert.Equal(new[] { title, "IL VENTO" }, Titles(result));
    }

    [Fact]
    public void Mixed_case_title_at_the_top_of_a_page_is_found()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"Risveglio\n\n{SongA}"), Page(2, $"A Te o Eterno\n\n{SongB}") });

        Assert.Equal(new[] { "Risveglio", "A Te o Eterno" }, Titles(result));
    }

    [Fact]
    public void A_song_continues_on_the_next_page_when_it_has_no_title_there()
    {
        var songs = PdfSongSplitter.Split(new[] { Page(1, $"LA LUNA\n\n{SongA}"), Page(2, $"{SongB}\n\nIL VENTO\n\n{SongA}") }).Songs;

        Assert.Equal(2, songs.Count);
        Assert.Equal((1, 2), (songs[0].FirstPage, songs[0].LastPage));
        Assert.Contains("Il vento canta forte", songs[0].Lines);
        Assert.Equal(2, songs[1].FirstPage);
    }

    [Fact]
    public void A_song_continues_from_the_left_column_into_the_right_one()
    {
        var result = PdfSongSplitter.Split(new[] { TwoColumns(1, $"LA LUNA\n\n{SongA}", $"{SongB}\n\nIL VENTO\n\n{SongA}") });

        Assert.Equal(new[] { "LA LUNA", "IL VENTO" }, Titles(result));
        Assert.Contains("La luna sale piano", result.Songs[0].Lines);
        Assert.Contains("Il vento canta forte", result.Songs[0].Lines);
    }

    [Fact]
    public void Each_column_can_start_a_new_song_at_its_top()
    {
        var result = PdfSongSplitter.Split(new[] { TwoColumns(1, $"1. LA LUNA\n{SongA}", $"2. IL VENTO\n{SongB}") });

        Assert.Equal(new[] { "LA LUNA", "IL VENTO" }, Titles(result));
        Assert.DoesNotContain("Il vento canta forte", result.Songs[0].Lines);
    }

    [Fact]
    public void Index_pages_with_page_numbers_are_skipped_and_counted()
    {
        var index = Page(2, string.Join('\n', Enumerable.Range(1, 8).Select(n => $"{n}. TITOLO NUMERO {n}      {n + 8}")));

        var result = PdfSongSplitter.Split(new[] { Page(1, "CANTI\nDI PROVA"), index, Page(3, $"1. LA LUNA\n{SongA}") });

        Assert.Equal(8, result.IndexEntryCount);
        Assert.Equal(new[] { "LA LUNA" }, Titles(result));
        Assert.Contains(1, result.SkippedPages);
        Assert.Contains(2, result.SkippedPages);
    }

    [Fact]
    public void An_index_without_page_numbers_is_skipped_too()
    {
        var index = Page(2, "Canti Scout\n\n" + string.Join('\n', Enumerable.Range(1, 10).Select(n => $"{n}.  Canto numero {n}")));

        var result = PdfSongSplitter.Split(new[] { index, Page(3, $"1. LA LUNA\n{SongA}") });

        Assert.Equal(10, result.IndexEntryCount);
        Assert.Equal(new[] { "LA LUNA" }, Titles(result));
    }

    [Fact]
    public void A_short_last_index_page_after_an_index_page_is_still_an_index()
    {
        var full = Page(1, string.Join('\n', Enumerable.Range(1, 8).Select(n => $"{n}. TITOLO {n}      {n}")));
        var tail = Page(2, "9. ALTRO TITOLO      9\n10. ULTIMO TITOLO      9");

        var result = PdfSongSplitter.Split(new[] { full, tail, Page(3, $"1. LA LUNA\n{SongA}") });

        Assert.Equal(10, result.IndexEntryCount);
        Assert.Single(result.Songs);
    }

    [Fact]
    public void Blank_and_scanned_pages_are_skipped_not_turned_into_songs()
    {
        var result = PdfSongSplitter.Split(new[] { Blank(1), Page(2, $"LA LUNA\n\n{SongA}"), Blank(3) });

        Assert.Single(result.Songs);
        Assert.Equal(new[] { 1, 3 }, result.SkippedPages);
    }

    [Fact]
    public void Text_before_the_first_title_is_not_a_song()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, "By Qualcuno"), Page(2, $"LA LUNA\n\n{SongA}") });

        Assert.Equal(new[] { "LA LUNA" }, Titles(result));
    }

    [Fact]
    public void Verse_numbers_and_section_labels_do_not_start_songs()
    {
        var text = "LA LUNA\n\nDo        Sol\n1. La luna sale piano\nLa-       Fa\nsopra il mare scuro\n\nRIT.\nDo   Sol\nOh luna\n\nCORO:\nSol La\n\n2. Seconda strofa";

        var songs = PdfSongSplitter.Split(new[] { Page(1, text) }).Songs;

        Assert.Single(songs);
        Assert.Contains("RIT.", songs[0].Lines);
        Assert.Contains("2. Seconda strofa", songs[0].Lines);
    }

    [Fact]
    public void A_heading_followed_by_another_title_is_not_a_song()
    {
        var text = $"INVOCAZIONI DELLO SPIRITO\n\n          God's Spirit is in my heart\n\n{SongA}";

        var result = PdfSongSplitter.Split(new[] { Page(1, text) });

        Assert.Equal(new[] { "God's Spirit is in my heart" }, Titles(result));
    }

    [Fact]
    public void An_adjacent_upper_case_line_is_a_subtitle_not_a_new_song()
    {
        var songs = PdfSongSplitter.Split(new[] { Page(1, $"SANTO\nGEN ROSSO\n\n{SongA}") }).Songs;

        Assert.Single(songs);
        Assert.Equal("SANTO", songs[0].Title);
        Assert.Equal(new[] { "GEN ROSSO" }, songs[0].Subtitles);
        Assert.DoesNotContain("GEN ROSSO", songs[0].Lines);
    }

    [Fact]
    public void Page_numbers_and_link_lines_are_dropped_from_the_text()
    {
        var songs = PdfSongSplitter.Split(new[] { Page(1, $"LA LUNA\nhttps://example.test/watch?v=1\n\n{SongA}\n\n- 3 -") }).Songs;

        Assert.DoesNotContain(songs[0].Lines, l => l.Contains("example.test") || l.Contains("- 3 -"));
    }

    [Fact]
    public void A_title_without_any_text_after_it_is_not_a_song()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"LA LUNA\n\n{SongA}\n\nIL VENTO") });

        Assert.Equal(new[] { "LA LUNA" }, Titles(result));
    }

    [Fact]
    public void The_next_numbered_title_may_follow_a_label_without_a_blank_line()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{SongA}\nRIT.\n2. IL VENTO\n{SongB}") });

        Assert.Equal(new[] { "LA LUNA", "IL VENTO" }, Titles(result));
    }

    [Fact]
    public void The_next_number_in_sequence_is_a_title_even_when_its_song_has_no_chords()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{SongA}\n\n2. Il vento\nsolo testo\nancora testo") });

        Assert.Equal(new[] { "LA LUNA", "Il vento" }, Titles(result));
    }

    [Fact]
    public void A_verse_number_after_text_is_not_a_title()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{SongA}\nsopra il mare\n5. Do Sol\nRe La\nverso") });

        Assert.Equal(new[] { "LA LUNA" }, Titles(result));
    }

    [Fact]
    public void Mixed_case_lyrics_at_the_top_of_a_column_are_not_titles_in_a_numbered_book()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{SongA}"), Page(2, "Canta e cammina\nDo   Sol\ntesto") });

        Assert.Equal(new[] { "LA LUNA" }, Titles(result));
    }

    [Fact]
    public void An_indented_author_line_under_a_mixed_case_title_is_a_subtitle()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"   Manda me\n                Civico\n\n{SongA}") });

        Assert.Equal(new[] { "Manda me" }, Titles(result));
        Assert.Equal(new[] { "Civico" }, result.Songs[0].Subtitles);
    }

    [Fact]
    public void Splitting_time_grows_linearly_with_the_number_of_songs()
    {
        ScalingAssert.StaysLinear(songs =>
        {
            var pages = Enumerable.Range(1, songs / 10)
                .Select(n => Page(n, string.Join("\n\n", Enumerable.Range(0, 10).Select(k => $"{n * 10 + k}. TITOLO {n * 10 + k}\n{SongA}"))))
                .ToList();
            return () => Assert.Equal(songs, PdfSongSplitter.Split(pages).Songs.Count);
        }, 400);
    }

    [Theory]
    [InlineData("１２. LA LUNA")]
    [InlineData("١٢. LA LUNA")]
    public void Numbered_lines_in_other_digit_scripts_do_not_throw(string line)
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"{line}\n{SongA}\n\n13. IL VENTO\n{SongB}") });

        Assert.NotEmpty(result.Songs);
    }

    [Fact]
    public void A_numbered_verse_after_a_blank_line_stays_inside_its_song()
    {
        var text = "1. LA LUNA\nDo        Sol\n1. La luna sale piano\n\n2. Tu sei la mia vita\nDo        Sol\nNel cuore ti porto\n\n2. IL VENTO\n" + SongB;
        var result = PdfSongSplitter.Split(new[] { Page(1, text) });

        Assert.Equal(new[] { "LA LUNA", "IL VENTO" }, Titles(result));
        Assert.Contains("2. Tu sei la mia vita", result.Songs[0].Lines);
    }

    [Fact]
    public void Splitting_does_not_change_its_input()
    {
        var pages = new[] { Page(1, $"LA LUNA\n\n{SongA}") };
        var before = pages[0].Text;

        PdfSongSplitter.Split(pages);

        Assert.Equal(before, pages[0].Text);
    }
}
