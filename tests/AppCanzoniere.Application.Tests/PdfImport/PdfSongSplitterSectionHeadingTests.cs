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

    [Fact]
    public void A_section_heading_over_a_song_without_a_title_does_not_name_it()
    {
        var pages = new[]
        {
            Page(1, $"PRIMO CANTO\n\n{Chords}"),
            Page(2, $"KYRIE - GLORIA\n\n{Chords}"),
            Page(3, IndexWithHeading("KYRIE - GLORIA", "PRIMO CANTO")),
        };

        var result = PdfSongSplitter.Split(pages);

        Assert.DoesNotContain(result.Songs, s => s.Title.Contains("KYRIE"));
        Assert.DoesNotContain(result.Songs, s => s.Lines.Any(l => l.Contains("KYRIE")));
    }
}
