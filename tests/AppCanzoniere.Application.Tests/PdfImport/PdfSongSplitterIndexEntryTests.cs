using AppCanzoniere.Application.PdfImport;
using static AppCanzoniere.Application.Tests.PdfImport.SyntheticPages;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfSongSplitterIndexEntryTests
{
    private const string Chords = "Do        Sol\nLa luna sale piano\nLa-       Fa\nsopra il mare scuro";

    private static string Leader(string indent, string title, int page) => $"{indent}{title} {new string('.', 40 - title.Length)} {page}";

    // Section headings sit at the margin over entries that are indented; they are not songs.
    [Fact]
    public void Section_headings_over_indented_entries_are_not_counted()
    {
        var rows = new[]
        {
            Leader("", "CANTI D'INGRESSO", 3), Leader("   ", "ABRAMO", 3), Leader("   ", "APRI LE TUE BRACCIA", 4), Leader("   ", "CANTIAMO TE", 5),
            Leader("", "SALMI", 6), Leader("   ", "BEATO L'UOMO", 6), Leader("   ", "ECCOMI", 7),
        };

        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{Chords}"), Page(2, "   INDICE\n" + string.Join("\n", rows)) });

        Assert.Equal(5, result.IndexEntryCount);
    }

    // A running footer "110     Indice" has no dot leader while the entries do; it is not an entry.
    [Fact]
    public void A_running_footer_without_leader_is_not_counted()
    {
        var rows = Enumerable.Range(1, 6).Select(n => Leader("   ", $"TITOLO NUMERO {n}", n + 7)).ToList();
        rows.Add("    110                                          Indice");

        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{Chords}"), Page(2, "   INDICE\n" + string.Join("\n", rows)) });

        Assert.Equal(6, result.IndexEntryCount);
    }

    [Fact]
    public void A_footer_with_the_page_number_at_the_end_is_not_counted()
    {
        var rows = Enumerable.Range(1, 6).Select(n => Leader("   ", $"TITOLO NUMERO {n}", n + 7)).ToList();
        rows.Add("  Indice                                          111");

        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{Chords}"), Page(2, string.Join("\n", rows)) });

        Assert.Equal(6, result.IndexEntryCount);
    }

    // Right-aligned numbers shift the first letter by a column or two; that is not a heading.
    [Fact]
    public void Numbers_of_different_width_do_not_make_headings()
    {
        var rows = new[] { "  8 TITOLO OTTO", "  9 TITOLO NOVE", " 10 TITOLO DIECI", " 11 TITOLO UNDICI", "100 TITOLO CENTO", "101 TITOLO CENTUNO" };

        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{Chords}"), Page(2, "INDICE\n" + string.Join("\n", rows)) });

        Assert.Equal(6, result.IndexEntryCount);
    }

    // Titles that wrap put their dot leader and page number on an indented continuation line; the margin entries stay entries.
    [Fact]
    public void Wrapped_long_titles_do_not_hide_the_entries_at_the_margin()
    {
        var rows = Enumerable.Range(1, 12).Select(n => Leader("", $"TITOLO NUMERO {n}", n + 7)).ToList();
        rows.Insert(3, "UN TITOLO MOLTO LUNGO CHE VA A CAPO");
        rows.Insert(4, "   NELLA RIGA SUCCESSIVA ........................ 30");
        rows.Add("ALTRO TITOLO LUNGHISSIMO DEL LIBRETTO");
        rows.Add("   CHE CONTINUA SOTTO ............................ 31");

        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{Chords}"), Page(2, "INDICE\n" + string.Join("\n", rows)) });

        Assert.Equal(14, result.IndexEntryCount);
    }

    // An index without dot leaders keeps counting every line, as before.
    [Fact]
    public void Entries_without_leaders_keep_counting_every_line()
    {
        var rows = Enumerable.Range(1, 6).Select(n => $"TITOLO NUMERO {n}      {n + 7}").ToList();

        var result = PdfSongSplitter.Split(new[] { Page(1, $"1. LA LUNA\n{Chords}"), Page(2, string.Join("\n", rows)) });

        Assert.Equal(6, result.IndexEntryCount);
    }

    private static string Index(params string[] titles) =>
        "   INDICE\n" + string.Join("\n", titles.Select((t, n) => Leader("   ", t, n + 3)).Concat(new[] { Leader("   ", "FILLER UNO", 20), Leader("   ", "FILLER DUE", 21), Leader("   ", "FILLER TRE", 22) }));

    // A centred line slightly larger than the body that the index lists is a title, even without a blank line above it.
    [Fact]
    public void A_centred_slightly_larger_line_listed_in_the_index_starts_a_song()
    {
        var songs = $"~                 PRIMO CANTO\n\n{Chords}\n2 strofa ancora\n\n~                 Noi ti chiediamo\n\n{Chords}";

        var result = PdfSongSplitter.Split(new[] { Page(1, songs), Page(2, Index("PRIMO CANTO", "Noi ti chiediamo")) });

        Assert.Equal(new[] { "PRIMO CANTO", "Noi ti chiediamo" }, result.Songs.Select(s => s.Title));
    }

    // The same line is no title when the index does not list it (DEC 35 stays).
    [Fact]
    public void A_centred_slightly_larger_line_not_in_the_index_is_not_a_title()
    {
        var songs = $"~                 PRIMO CANTO\n\n{Chords}\n2 strofa ancora\n~                 Un ritornello rientrato\n\n{Chords}";

        var result = PdfSongSplitter.Split(new[] { Page(1, songs), Page(2, Index("PRIMO CANTO", "ALTRO")) });

        Assert.Single(result.Songs);
    }

    // A refrain set in the body size never starts a song, even when its words are an index entry.
    [Fact]
    public void A_centred_refrain_in_body_size_that_matches_an_index_entry_is_not_a_title()
    {
        var songs = $"~                 PRIMO CANTO\n\n{Chords}\n\n                  Noi ti chiediamo\n\nsopra il mare scuro";

        var result = PdfSongSplitter.Split(new[] { Page(1, songs), Page(2, Index("PRIMO CANTO", "Noi ti chiediamo")) });

        Assert.Single(result.Songs);
    }

    // A line much larger than the body is judged by the usual title rules, which reject an indented refrain without a blank line above.
    [Fact]
    public void A_listed_line_much_larger_than_the_body_is_left_to_the_usual_rules()
    {
        var songs = $"~                 PRIMO CANTO\n\n{Chords}\n2 strofa ancora\n^      Noi ti chiediamo\nsopra il mare scuro";

        var result = PdfSongSplitter.Split(new[] { Page(1, songs), Page(2, Index("PRIMO CANTO", "Noi ti chiediamo")) });

        Assert.Single(result.Songs);
    }

    // A listed line repeated at the same centred indent is a refrain, not a title.
    [Fact]
    public void A_listed_line_repeated_centred_on_the_page_is_not_a_title()
    {
        var songs = $"~                 PRIMO CANTO\n\n{Chords}\n\n~                 Noi ti chiediamo\n\nsopra il mare scuro\n\n~                 Noi ti chiediamo\n\nsopra il mare scuro";

        var result = PdfSongSplitter.Split(new[] { Page(1, songs), Page(2, Index("PRIMO CANTO", "Noi ti chiediamo")) });

        Assert.Single(result.Songs);
    }

    // DEC 35: an indented refrain in the middle of a verse never splits a song, even when the index lists it.
    [Fact]
    public void A_listed_refrain_in_the_middle_of_a_verse_without_a_blank_line_above_is_not_a_title()
    {
        var songs = $"~                 PRIMO CANTO\n\n{Chords}\n2 strofa ancora\n~                 Noi ti chiediamo\nsopra il mare scuro";

        var result = PdfSongSplitter.Split(new[] { Page(1, songs), Page(2, Index("PRIMO CANTO", "Noi ti chiediamo")) });

        Assert.Single(result.Songs);
    }

    // A listed line that comes back at the margin further down is a refrain whatever its first indent.
    [Fact]
    public void A_listed_line_repeated_at_the_margin_is_not_a_title()
    {
        var songs = $"~                 PRIMO CANTO\n\n{Chords}\n\n~                 Noi ti chiediamo\n\nsopra il mare scuro\n\nNoi ti chiediamo\nsopra il mare scuro";

        var result = PdfSongSplitter.Split(new[] { Page(1, songs), Page(2, Index("PRIMO CANTO", "Noi ti chiediamo")) });

        Assert.Single(result.Songs);
    }

    // The usual title checks apply: a line ending in a comma is no title even when listed and set off.
    [Fact]
    public void A_listed_centred_line_ending_in_a_comma_is_not_a_title()
    {
        var songs = $"~                 PRIMO CANTO\n\n{Chords}\n\n~                 Noi ti chiediamo,\n\n{Chords}";

        var result = PdfSongSplitter.Split(new[] { Page(1, songs), Page(2, Index("PRIMO CANTO", "Noi ti chiediamo,")) });

        Assert.Single(result.Songs);
    }

    // The title echoed at the margin as the first lyric line does not count as a repeat.
    [Fact]
    public void A_title_echoed_at_the_margin_still_starts_its_song()
    {
        var songs = $"~                 PRIMO CANTO\n\n{Chords}\n\n~         O Signore, invia il tuo Spirito\n\nLa-  Re-\nO Signore, invia il tuo Spirito\nSol7 Do\ne rinnova il volto";

        var result = PdfSongSplitter.Split(new[] { Page(1, songs), Page(2, Index("PRIMO CANTO", "O Signore, invia il tuo Spirito")) });

        Assert.Equal(new[] { "PRIMO CANTO", "O Signore, invia il tuo Spirito" }, result.Songs.Select(s => s.Title));
    }
}
