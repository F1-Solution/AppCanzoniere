using AppCanzoniere.Application.PdfImport;
using static AppCanzoniere.Application.Tests.PdfImport.SyntheticPages;

namespace AppCanzoniere.Application.Tests.PdfImport;

// A section heading that the index shows over its entries is not text of any song.
public sealed class PdfSongSplitterSectionHeadingTests
{
    private const string Chords = "Do        Sol\nLa luna sale piano\nLa-       Fa\nsopra il mare scuro";

    private static string Leader(string indent, string title, int page) => $"{indent}{title} {new string('.', 40 - title.Length)} {page}";

    private static string IndexWithHeading(string heading, params string[] entries) =>
        "   INDICE\n" + string.Join("\n", new[] { Leader("", heading, 2) }.Concat(entries.Select((e, n) => Leader("   ", e, n + 2)))
            .Concat(new[] { Leader("   ", "FILLER UNO", 20), Leader("   ", "FILLER DUE", 21) }));

    [Fact]
    public void A_section_heading_at_the_top_of_a_page_is_neither_a_title_nor_part_of_a_song()
    {
        var pages = new[]
        {
            Page(1, $"PRIMO CANTO\n\n{Chords}"),
            Page(2, $"KYRIE - GLORIA\n\nSENOR TEN PIEDAD\n\n{Chords}"),
            Page(3, IndexWithHeading("KYRIE - GLORIA", "PRIMO CANTO", "SENOR TEN PIEDAD")),
        };

        var result = PdfSongSplitter.Split(pages);

        Assert.Equal(new[] { "PRIMO CANTO", "SENOR TEN PIEDAD" }, result.Songs.Select(s => s.Title));
        Assert.All(result.Songs, s => Assert.DoesNotContain(s.Lines.Concat(s.Subtitles), l => l.Contains("KYRIE")));
    }

    [Fact]
    public void A_heading_that_is_also_listed_as_an_entry_still_starts_its_song()
    {
        var pages = new[]
        {
            Page(1, $"PRIMO CANTO\n\n{Chords}"),
            Page(2, $"MARIA\n\n{Chords}"),
            Page(3, IndexWithHeading("MARIA", "PRIMO CANTO", "MARIA")),
        };

        var result = PdfSongSplitter.Split(pages);

        Assert.Equal(new[] { "PRIMO CANTO", "MARIA" }, result.Songs.Select(s => s.Title));
    }

    [Fact]
    public void A_line_equal_to_a_heading_inside_a_verse_is_kept()
    {
        var pages = new[]
        {
            Page(1, $"PRIMO CANTO\n\n{Chords}\nKYRIE - GLORIA\nsopra il mare chiaro"),
            Page(2, IndexWithHeading("KYRIE - GLORIA", "PRIMO CANTO")),
        };

        var result = PdfSongSplitter.Split(pages);

        Assert.Contains(result.Songs[0].Lines, l => l.Contains("KYRIE"));
    }

    // A song under a dropped heading is never lost: it stays a song of its own, named by its first lyric line.
    [Fact]
    public void A_song_without_a_title_under_a_section_heading_is_kept_as_a_song_of_its_own()
    {
        var pages = new[]
        {
            Page(1, $"PRIMO CANTO\n\n{Chords}"),
            Page(2, $"KYRIE - GLORIA\n\n{Chords}"),
            Page(3, IndexWithHeading("KYRIE - GLORIA", "PRIMO CANTO")),
        };

        var result = PdfSongSplitter.Split(pages);

        Assert.Equal(2, result.Songs.Count);
        Assert.Equal(1, result.Songs[0].LastPage);
        Assert.Equal("La luna sale piano", result.Songs[1].Title);
        Assert.Equal(2, result.Songs[1].FirstPage);
        Assert.DoesNotContain(result.Songs, s => s.Lines.Any(l => l.Contains("KYRIE")));
    }

    [Fact]
    public void A_footer_equal_to_a_heading_at_the_bottom_of_a_page_does_not_split_the_song()
    {
        var pages = new[]
        {
            Page(1, $"PRIMO CANTO\n\n{Chords}\n\nKYRIE - GLORIA"),
            Page(2, "sopra il mare chiaro\ne il vento canta ancora"),
            Page(3, IndexWithHeading("KYRIE - GLORIA", "PRIMO CANTO")),
        };

        var result = PdfSongSplitter.Split(pages);

        Assert.Single(result.Songs);
        Assert.Equal("PRIMO CANTO", result.Songs[0].Title);
        Assert.Contains(result.Songs[0].Lines, l => l.Contains("e il vento canta ancora"));
    }

    [Fact]
    public void A_line_equal_to_a_heading_between_blank_lines_inside_a_song_is_kept()
    {
        var pages = new[]
        {
            Page(1, $"PRIMO CANTO\n\n{Chords}\n\nAlleluia!\n\nsopra il mare chiaro"),
            Page(2, IndexWithHeading("ALLELUIA", "PRIMO CANTO")),
        };

        var result = PdfSongSplitter.Split(pages);

        Assert.Single(result.Songs);
        Assert.Contains(result.Songs[0].Lines, l => l.Contains("Alleluia!"));
    }

    [Fact]
    public void An_index_whose_titles_are_all_headings_does_not_drop_any_title()
    {
        var index = "   INDICE\n" + string.Join("\n", new[] { "PRIMO CANTO", "SECONDO CANTO", "TERZO CANTO" }.SelectMany((t, n) => new[] { Leader("", t, n + 1), Leader("   ", "la luna sale piano", n + 1) }));
        var pages = new[]
        {
            Page(1, $"PRIMO CANTO\n\n{Chords}"),
            Page(2, $"SECONDO CANTO\n\n{Chords}"),
            Page(3, $"TERZO CANTO\n\n{Chords}"),
            Page(4, index),
        };

        var result = PdfSongSplitter.Split(pages);

        Assert.Equal(new[] { "PRIMO CANTO", "SECONDO CANTO", "TERZO CANTO" }, result.Songs.Select(s => s.Title));
    }
}
