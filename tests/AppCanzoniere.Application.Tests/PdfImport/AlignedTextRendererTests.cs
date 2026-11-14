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
    public void Render_AnchorsChords_OnALyricThatStartsWithAVerseNumberFarFromItsText()
    {
        // As in a real booklet, the verse number "3" stands more than a text height before its verse; the chords above
        // still belong to that verse, not to the line above it.
        var page = new PageBuilder { WithLetters = true }
            .Text("questa riga sta sopra e non porta accordi", 50, 724)
            .Word("3", 50, 700);
        page.ChordsOver("i potenti rovescio dai ^troni il Si^gnor", new[] { "LA-", "MI" }, 50 + PageBuilder.Width("3") + 12, 700);

        var lines = Render(page);

        var lyric = lines.Single(l => l.Contains("troni", StringComparison.Ordinal));
        var chords = lines[lines.ToList().IndexOf(lyric) - 1];
        Assert.Equal(lyric.IndexOf("troni", StringComparison.Ordinal), chords.IndexOf("LA-", StringComparison.Ordinal));
        Assert.Equal(lyric.IndexOf("gnor", StringComparison.Ordinal), chords.IndexOf("MI", StringComparison.Ordinal));
    }

    [Fact]
    public void Render_AnchorsChords_OnTheLyricBelow_EvenWhenItsSyllablesAreSpreadApart()
    {
        // Psalm sheets pull syllables apart to make room for the chords ("fo   ssa"); such a lyric is still the anchor.
        var page = new PageBuilder { WithLetters = true }.Text("verso il tuo tempio santo o Signor", 50, 724);
        var x = 50.0;
        foreach (var (word, gap) in new[] { ("nella", 3.0), ("fo", 25.0), ("ssa", 25.0), ("o", 3.0), ("Signor", 0.0) })
        {
            page.Word(word, x, 700);
            x += PageBuilder.Width(word) + gap;
        }

        var ssa = 50 + PageBuilder.Width("nella") + 3 + PageBuilder.Width("fo") + 25;
        page.Word("RE", ssa, 712).Word("SOL", x - PageBuilder.Width("Signor"), 712);

        var lines = Render(page);

        Assert.Equal(lines[2].IndexOf("ssa", StringComparison.Ordinal), lines[1].IndexOf("RE", StringComparison.Ordinal));
        Assert.Equal(lines[2].IndexOf("Signor", StringComparison.Ordinal), lines[1].IndexOf("SOL", StringComparison.Ordinal));
    }

    [Fact]
    public void Render_PlacesEveryChordOverItsLetter_WhenChordsArePackedLikeWords()
    {
        // Chords close together look like a line of words by spacing alone; they must still follow the lyric below.
        var page = new PageBuilder { WithLetters = true }.ChordsOver("^mamma ^mia ^vieni ^qui", new[] { "DO", "RE", "SOL", "LA" }, 50, 700);

        var lines = Render(page);

        Assert.Equal(lines[1].IndexOf("mia", StringComparison.Ordinal), lines[0].IndexOf("RE", StringComparison.Ordinal));
        Assert.Equal(lines[1].IndexOf("vieni", StringComparison.Ordinal), lines[0].IndexOf("SOL", StringComparison.Ordinal));
        Assert.Equal(lines[1].IndexOf("qui", StringComparison.Ordinal), lines[0].IndexOf("LA", StringComparison.Ordinal));
    }

    [Fact]
    public void Render_PutsAChordOverTheLetterUnderIt_FromTheLettersOwnPositions()
    {
        // Wide letters first, narrow letters after: an average letter width would put the chord two letters too far right.
        var page = new PageBuilder { WithLetters = true }.ChordsOver("mmmmi^iiii ok", new[] { "FA" }, 50, 700);

        var lines = Render(page);

        Assert.Equal(5, lines[0].IndexOf("FA", StringComparison.Ordinal));
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
    public void Render_NeverPutsABlankLineBetweenAChordRowAndItsLyric()
    {
        // The chords stand a little higher above this verse than the usual line pitch; they still belong to it.
        var page = new PageBuilder { WithLetters = true }
            .Text("prima strofa riga uno", 50, 760)
            .Text("prima strofa riga due", 50, 748)
            .Text("prima strofa riga tre", 50, 736)
            .Text("prima strofa riga quattro", 50, 724);
        page.ChordsOver("^seconda strofa ^riga", new[] { "DO", "SOL" }, 50, 670, pitch: 20);

        var lines = Render(page);

        Assert.Equal("seconda strofa riga", lines[^1]);
        Assert.StartsWith("DO", lines[^2], StringComparison.Ordinal);
        Assert.Equal(string.Empty, lines[^3]);
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
