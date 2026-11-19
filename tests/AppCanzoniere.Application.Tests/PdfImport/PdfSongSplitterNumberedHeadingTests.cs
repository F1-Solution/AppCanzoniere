using AppCanzoniere.Application.PdfImport;
using static AppCanzoniere.Application.Tests.PdfImport.SyntheticPages;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfSongSplitterNumberedHeadingTests
{
    private const string Songs =
        "1. Lodate Dio\n   Do        Sol\n   Lodate il nome del Signore\n   La-       Fa\n   sopra tutta la terra\n\n"
        + "2. Il Signore mio pastore\n   Do        Sol\n   Nulla mi manca mai\n   La-       Fa\n   mi guida all'acqua chiara\n\n"
        + "3. Vieni Spirito\n   Do        Sol\n   Scendi sopra di noi\n   La-       Fa\n   come vento leggero";

    [Theory]
    [InlineData("CANTI D'INGRESSO")]
    [InlineData("Canti d'ingresso")]
    public void A_numbered_songbook_under_a_heading_gives_one_song_per_number(string heading)
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, $"{heading}\n\n{Songs}") });

        Assert.Equal(new[] { "Lodate Dio", "Il Signore mio pastore", "Vieni Spirito" }, result.Songs.Select(s => s.Title));
    }

    [Theory]
    [InlineData("   Do        Sol\n1  Il vento canta forte\n   La-       Fa\n   sulla collina verde\n\n   Do        Sol\n2  La luna sale piano\n   La-       Fa\n   nel cielo sereno")]
    [InlineData("   Do        Sol\nRit. Canterò al nuovo giorno\n   La-       Fa\n   sopra il mare scuro\n\n   Do        Sol\n1  Il vento canta forte\n   La-       Fa\n   sulla collina verde\n\n   Do        Sol\n2  La luna sale piano\n   La-       Fa\n   nel cielo sereno")]
    public void An_untitled_sheet_whose_verses_are_numbered_is_still_one_song(string sheet)
    {
        var result = PdfSongSplitter.Split(new[] { Page(1, sheet) });

        var song = Assert.Single(result.Songs);
        Assert.Contains(song.Lines, l => l.Trim() == "nel cielo sereno");
    }
}
