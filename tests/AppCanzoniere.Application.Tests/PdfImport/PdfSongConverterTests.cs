using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfSongConverterTests
{
    private static SplitSong Song(string title, params string[] lines) =>
        new(title, 3, 4, lines, Array.Empty<string>());

    [Fact]
    public void Chords_are_merged_into_the_lyrics_and_the_title_is_set()
    {
        var song = Song("LA LUNA", "Do        Sol", "La luna sale piano", "La-       Fa", "sopra il mare scuro");

        var converted = PdfSongConverter.Convert(song);

        Assert.StartsWith("{title:LA LUNA}", converted.ChordPro);
        Assert.Contains("[Do]", converted.ChordPro);
        Assert.Contains("[Sol]", converted.ChordPro);
        Assert.Equal(2, converted.ChordLineCount);
        Assert.Equal((3, 4), (converted.FirstPage, converted.LastPage));
    }

    [Fact]
    public void Subtitles_are_written_after_the_title()
    {
        var song = new SplitSong("SANTO", 1, 1, new[] { "Sol   Mim", "Santo santo santo" }, new[] { "GEN ROSSO" });

        var converted = PdfSongConverter.Convert(song);

        var head = converted.ChordPro.Split('\n').Take(2).Select(l => l.TrimEnd('\r')).ToArray();
        Assert.Equal(new[] { "{title:SANTO}", "{subtitle:GEN ROSSO}" }, head);
    }

    [Fact]
    public void Titles_with_brackets_are_made_safe()
    {
        var converted = PdfSongConverter.Convert(Song("A {strano} [titolo]", "Do   Sol", "testo"));

        var first = converted.ChordPro.Split('\n')[0];
        Assert.StartsWith("{title:", first);
        Assert.DoesNotContain("[", first);
        Assert.DoesNotContain("]", first);
    }

    [Fact]
    public void A_song_without_chords_reports_none()
    {
        var converted = PdfSongConverter.Convert(Song("SOLO TESTO", "Una riga di testo", "Un'altra riga"));

        Assert.Equal(0, converted.ChordLineCount);
    }

    [Fact]
    public void Doubtful_lines_come_back_as_warnings()
    {
        var converted = PdfSongConverter.Convert(Song("LA LUNA", "Do   Sol", "testo", "Mi", "altro testo"));

        Assert.Contains(SheetWarning.AmbiguousSingleToken, converted.Warnings);
        Assert.True(converted.DoubtfulLineCount > 0);
    }

    [Fact]
    public void Every_split_song_can_be_converted()
    {
        var page = SyntheticPages.Page(1, "LA LUNA\n\nDo   Sol\ntesto uno\n\nIL VENTO\n\nRe   La\ntesto due");

        var converted = PdfSongSplitter.Split(new[] { page }).Songs.Select(PdfSongConverter.Convert).ToList();

        Assert.Equal(new[] { "LA LUNA", "IL VENTO" }, converted.Select(c => c.Title).ToArray());
        Assert.All(converted, c =>
        {
            Assert.Contains("[", c.ChordPro);
            Assert.Equal(1, c.ChordLineCount);
        });
    }
}
