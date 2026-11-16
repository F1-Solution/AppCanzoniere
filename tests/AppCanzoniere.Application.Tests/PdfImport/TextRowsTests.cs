using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public class TextRowsTests
{
    [Fact]
    public void Group_ReturnsNoRows_ForNoWords()
    {
        Assert.Empty(TextRows.Group(Array.Empty<PositionedWord>()));
    }

    [Fact]
    public void Group_PutsWordsOnTheSameBaselineInOneRow_OrderedLeftToRight()
    {
        var words = new PageBuilder().Word("mondo", 120, 700).Word("ciao", 50, 700).Word("sotto", 50, 688).Words;

        var rows = TextRows.Group(words);

        Assert.Equal(2, rows.Count);
        Assert.Equal(new[] { "ciao", "mondo" }, rows[0].Words.Select(w => w.Text));
        Assert.Equal(new[] { "sotto" }, rows[1].Words.Select(w => w.Text));
    }

    [Fact]
    public void Group_ToleratesSmallBaselineJitter_WithinARow()
    {
        // Words of one line can sit a fraction of a point apart, for example after a font change.
        var words = new PageBuilder().Word("uno", 50, 700).Word("due", 80, 700.8).Word("tre", 110, 699.4).Words;

        var rows = TextRows.Group(words);

        Assert.Single(rows);
        Assert.Equal(3, rows[0].Words.Count);
    }

    [Fact]
    public void Group_ListsRowsFromTheTopOfThePage()
    {
        var words = new PageBuilder().Word("basso", 50, 100).Word("alto", 50, 700).Words;

        var rows = TextRows.Group(words);

        Assert.Equal(new[] { "alto", "basso" }, rows.Select(r => r.Words[0].Text));
    }

    [Fact]
    public void Coverage_IsLowForChordsFarApart_AndHighForLyrics()
    {
        var chords = TextRows.Group(new PageBuilder().Word("DO", 50, 700).Word("SOL", 300, 700).Words).Single();
        var lyric = TextRows.Group(new PageBuilder().Text("canto la mia gioia", 50, 700).Words).Single();

        Assert.True(chords.Coverage < 0.2);
        Assert.True(lyric.Coverage > 0.8);
    }
}
