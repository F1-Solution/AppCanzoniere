using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public class AlignedTextRendererTests
{
    private static IReadOnlyList<string> Render(PageBuilder page) =>
        AlignedTextRenderer.Render(TextRows.Group(page.Words)).Select(l => l.Text).ToList();

    [Fact]
    public void Render_ReturnsNoLines_ForNoRows()
    {
        Assert.Empty(AlignedTextRenderer.Render(Array.Empty<TextRow>()));
    }

    [Fact]
    public void Render_KeepsLyricsWithSingleSpaces()
    {
        var lines = Render(new PageBuilder().Text("il mio Signore mi guida", 50, 700));

        Assert.Equal(new[] { "il mio Signore mi guida" }, lines);
    }

    [Fact]
    public void Render_PutsEachChordOverTheCharacterItSitsOver_WithAProportionalFont()
    {
        // The narrow letters before "Signore" make it start at character 7 while its x position
        // corresponds to fewer average-width characters; a plain character grid would put RE too far left.
        var page = new PageBuilder().ChordsOver("^il mio ^Signore mi gu^ida", new[] { "DO", "RE", "SOL" }, 50, 700);

        var lines = Render(page);

        Assert.Equal(2, lines.Count);
        Assert.Equal("il mio Signore mi guida", lines[1]);
        Assert.Equal(lines[1].IndexOf("Signore", StringComparison.Ordinal), lines[0].IndexOf("RE", StringComparison.Ordinal));
        Assert.Equal(lines[1].IndexOf("ida", StringComparison.Ordinal), lines[0].IndexOf("SOL", StringComparison.Ordinal));
        Assert.StartsWith("DO", lines[0], StringComparison.Ordinal);
    }

    [Fact]
    public void Render_PutsAChordInsideAWord_OverTheRightSyllable()
    {
        var page = new PageBuilder().ChordsOver("alle^luia alle^luia", new[] { "MI-", "LA" }, 50, 700);

        var lines = Render(page);

        Assert.Equal(4, lines[0].IndexOf("MI-", StringComparison.Ordinal));
        Assert.Equal(13, lines[0].IndexOf("LA", StringComparison.Ordinal));
    }

    [Fact]
    public void Render_KeepsAtLeastOneSpaceBetweenChords_ThatAreCloserThanTheirText()
    {
        // Two chords over adjacent narrow letters would otherwise overlap.
        var page = new PageBuilder().ChordsOver("^i^l sole", new[] { "SOL7", "DO" }, 50, 700);

        var lines = Render(page);

        Assert.Contains("SOL7 DO", lines[0], StringComparison.Ordinal);
    }

    [Fact]
    public void Render_AlignsAChordLine_OnTheLyricAbove_WhenNoLyricFollows()
    {
        var page = new PageBuilder()
            .Text("il mio Signore mi guida", 50, 700)
            .Word("RE", PageBuilder.XOf("il mio Signore mi guida", 7, 50), 688);

        var lines = Render(page);

        Assert.Equal(7, lines[1].IndexOf("RE", StringComparison.Ordinal));
    }

    [Fact]
    public void Render_IndentsLinesThatStartFurtherRight()
    {
        var page = new PageBuilder().Text("prima riga del canto", 50, 700).Text("rientrata", 50 + (4 * PageBuilder.Width("e")), 688);

        var lines = Render(page);

        Assert.Equal("    rientrata", lines[1]);
    }

    [Fact]
    public void Render_LeavesABlankLine_WhereTheVerticalGapMarksANewStanza()
    {
        var page = new PageBuilder()
            .Text("prima strofa riga uno", 50, 700)
            .Text("prima strofa riga due", 50, 688)
            .Text("prima strofa riga tre", 50, 676)
            .Text("seconda strofa riga uno", 50, 640)
            .Text("seconda strofa riga due", 50, 628);

        var lines = Render(page);

        Assert.Equal(6, lines.Count);
        Assert.Equal(string.Empty, lines[3]);
        Assert.Null(AlignedTextRenderer.Render(TextRows.Group(page.Words))[3].Row);
    }

    [Fact]
    public void Render_KeepsTheRowOfEveryTextLine_SoChordsCanLaterBePlacedByPosition()
    {
        var page = new PageBuilder().ChordsOver("^il mio ^Signore", new[] { "DO", "RE" }, 50, 700);

        var lines = AlignedTextRenderer.Render(TextRows.Group(page.Words));

        Assert.All(lines, l => Assert.NotNull(l.Row));
        Assert.Equal(new[] { "DO", "RE" }, lines[0].Row!.Words.Select(w => w.Text));
    }
}
