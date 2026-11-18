using AppCanzoniere.Application.PdfImport;
using static AppCanzoniere.Application.Tests.PdfImport.SyntheticPages;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfSongSplitterLyricHintTests
{
    [Theory]
    [InlineData("Trasparente come l'acqua", "Trasparente come l'acqua")]
    [InlineData("1. Tono di festa nel cuore", "Tono di festa nel cuore")]
    [InlineData("Trasportami sulle ali del vento", "Trasportami sulle ali del vento")]
    [InlineData("Tonocoro di voci nella sera", "Tonocoro di voci nella sera")]
    public void A_lyric_line_that_starts_like_a_hint_is_kept(string line, string text)
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"LA SERA\n\n   Do        Sol\n{line}\n   La-       Fa\n   sopra il mare scuro") });

        var song = Assert.Single(result.Songs);
        Assert.Contains(song.Lines, l => l.Contains(text));
    }

    [Theory]
    [InlineData("Trasposto +2")]
    [InlineData("Trasp. -2")]
    [InlineData("TRASPOSTA 4")]
    [InlineData("Tono reale MI-")]
    [InlineData("Tono LA")]
    [InlineData("Tono SI-")]
    public void A_real_hint_line_is_still_dropped(string hint)
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"LA SERA\n{hint}\n   Do        Sol\nCanto la luna alta\n   La-       Fa\n   sopra il mare scuro") });

        var song = Assert.Single(result.Songs);
        Assert.DoesNotContain(song.Lines, l => l.Contains(hint));
    }

    [Fact]
    public void A_long_line_that_begins_with_a_hint_word_is_a_lyric()
    {
        var lyric = "Tono SI mi dicevano tutti gli amici lungo il fiume stasera";

        var result = PdfSongSplitter.Split(new[] { Page(1, $"LA SERA\n\n   Do        Sol\n{lyric}\n   La-       Fa\n   sopra il mare scuro") });

        Assert.Contains(Assert.Single(result.Songs).Lines, l => l.Contains(lyric));
    }

    [Fact]
    public void An_untitled_sheet_drops_hint_lines_like_a_titled_one()
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, "Tastiera RE- trasp -2\n   Do        Sol\nCanterò al nuovo giorno\n   La-       Fa\n   sopra il mare scuro") });

        var song = Assert.Single(result.Songs);
        Assert.DoesNotContain(song.Lines, l => l.Contains("trasp"));
    }
}
