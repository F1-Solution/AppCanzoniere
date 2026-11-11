using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class ChordProTabTests
{
    private readonly ChordProParser _parser = new();

    private const string SongWithTab =
        "{title:Intro}\n" +
        "[C]Verse line\n" +
        "\n" +
        "{start_of_tab}\n" +
        "  e|--0--[3]--|  \n" +
        "B|---1-------|\n" +
        "E|---\n" +
        "A|---\n" +
        "\n" +
        "{end_of_tab}\n" +
        "[G]After the tab";

    [Fact]
    public void Parse_TabBlock_KeepsEveryLineVerbatimAndFlagsItAsTab()
    {
        var tabLines = _parser.Parse(SongWithTab).Lines.Where(l => l.Type == ChordProLineType.Tab).ToList();

        Assert.Equal(
            new[] { "  e|--0--[3]--|  ", "B|---1-------|", "E|---", "A|---", string.Empty },
            tabLines.Select(l => l.Text));
        Assert.All(tabLines, l => Assert.Empty(l.Chords));
        Assert.True(tabLines[0].StartsTabBlock);
        Assert.All(tabLines.Skip(1), l => Assert.False(l.StartsTabBlock));
    }

    [Theory]
    [InlineData("{sot}", "{eot}")]
    [InlineData("{start_of_tab}", "{end_of_tab}")]
    [InlineData("{SOT}", "{EOT}")]
    [InlineData("{start_of_tab}", "{eot}")]
    public void Parse_TabDirectiveAliases_AreEquivalent(string start, string end)
    {
        var document = _parser.Parse($"{start}\nE|--3--\n{end}\n[C]Lyrics");

        Assert.Equal(new[] { ChordProLineType.Tab, ChordProLineType.Verse }, document.Lines.Select(l => l.Type));
        Assert.Equal("E|--3--", document.Lines[0].Text);
    }

    [Fact]
    public void Parse_LinesAfterTheBlock_AreParsedAsNormalChordLines()
    {
        var after = _parser.Parse(SongWithTab).Lines[^1];

        Assert.Equal(ChordProLineType.Verse, after.Type);
        Assert.Equal("After the tab", after.Text);
        Assert.Equal("G", after.Chords[0].Chord);
    }

    [Theory]
    [InlineData("{ start_of_tab }", "")]
    [InlineData("{sot }", "")]
    [InlineData("{ sot}", "")]
    [InlineData("{sot:}", "")]
    [InlineData("{ SOT : Riff }", "Riff")]
    [InlineData("{ start_of_tab : Intro riff }", "Intro riff")]
    public void Parse_TabDirectiveWithSpacesOrEmptyLabel_OpensATabBlock(string start, string label)
    {
        var document = _parser.Parse($"{start}\n  E|--[G]--\n{{ eot }}\n[C]Lyrics");

        Assert.Equal(new[] { ChordProLineType.Tab, ChordProLineType.Verse }, document.Lines.Select(l => l.Type));
        Assert.Equal("  E|--[G]--", document.Lines[0].Text);
        Assert.Equal(label, document.Lines[0].TabLabel);
        Assert.True(document.Lines[0].StartsTabBlock);
    }

    [Theory]
    [InlineData("{ start_of_tab }", "{start_of_tab}\n")]
    [InlineData("{sot:}", "{start_of_tab}\n")]
    [InlineData("{ SOT : Riff }", "{start_of_tab:Riff}\n")]
    public void Render_TabDirectiveWithSpaces_KeepsTheBlockUntransposed(string start, string opening)
    {
        var rendered = _parser.Render(_parser.Parse($"[C]Verse\n{start}\nE|--[G]--\n{{ eot }}"), 2);

        Assert.Contains("[D]Verse\n", rendered);
        Assert.Contains(opening + "E|--[G]--\n{end_of_tab}\n", rendered);
    }

    [Fact]
    public void Parse_TabLineThatLooksLikeADirective_IsNotInterpreted()
    {
        var document = _parser.Parse("{sot}\n{title:Not a title}\ne|--{3}--\n{eot}");

        Assert.Equal(string.Empty, document.Title);
        Assert.Equal(new[] { "{title:Not a title}", "e|--{3}--" }, document.Lines.Select(l => l.Text));
    }

    [Fact]
    public void Parse_TabWithCrLfInput_HasNoCarriageReturns()
    {
        var document = _parser.Parse("{sot}\r\n  e|--0--\r\n{eot}\r\n");

        Assert.Equal("  e|--0--", document.Lines[0].Text);
    }

    [Fact]
    public void Parse_TabLabel_IsKeptOnTheFirstLine()
    {
        var document = _parser.Parse("{start_of_tab: Intro riff}\ne|--0--\n{end_of_tab}");

        Assert.Equal("Intro riff", document.Lines[0].TabLabel);
    }

    [Fact]
    public void RoundTrip_TabBlock_SurvivesByteForByte()
    {
        const string canonical =
            "{title:Intro}\n" +
            "\n" +
            "[C]Verse line\n" +
            "\n" +
            "{start_of_tab}\n" +
            "  e|--0--[3]--|  \n" +
            "B|---1-------|\n" +
            "E|---\n" +
            "A|---\n" +
            "\n" +
            "{end_of_tab}\n" +
            "[G]After the tab\n";

        Assert.Equal(canonical, _parser.Render(_parser.Parse(canonical), 0));
    }

    [Fact]
    public void RoundTrip_TabBlockWithLabel_KeepsTheLabel()
    {
        var rendered = _parser.Render(_parser.Parse("{sot: Riff}\ne|--0--\n{eot}"), 0);

        Assert.Contains("{start_of_tab:Riff}\ne|--0--\n{end_of_tab}\n", rendered);
    }

    [Fact]
    public void Render_Transposed_NeverTouchesTabLines()
    {
        var content = "{key:C}\n[C]Verse [Am]line\n{sot}\nE|---\nA|---\nC|--0--\n  D|--[G]--\n{eot}\n";

        var rendered = _parser.Render(_parser.Parse(content), 2);

        Assert.Contains("{key:D}\n", rendered);
        Assert.Contains("[D]Verse [Bm]line\n", rendered);
        Assert.Contains("{start_of_tab}\nE|---\nA|---\nC|--0--\n  D|--[G]--\n{end_of_tab}\n", rendered);
    }

    [Fact]
    public void Parse_UnterminatedTabBlock_LosesNoContent()
    {
        var content = "[C]Before\n{start_of_tab}\ne|--0--\nB|--1--\n[G]looks like lyrics\n";

        var document = _parser.Parse(content);

        Assert.Equal(
            new[] { "e|--0--", "B|--1--", "[G]looks like lyrics" },
            document.Lines.Where(l => l.Type == ChordProLineType.Tab).Select(l => l.Text));
    }

    [Fact]
    public void Render_UnterminatedTabBlock_KeepsAllLinesAndClosesTheBlock()
    {
        var rendered = _parser.Render(_parser.Parse("{sot}\ne|--0--\nB|--1--"), 0);

        Assert.Equal("{start_of_tab}\ne|--0--\nB|--1--\n{end_of_tab}\n", rendered);
    }

    [Fact]
    public void RoundTrip_TwoAdjacentBlocks_StayTwoBlocks()
    {
        var content = "{sot}\ne|--0--\n{eot}\n{sot}\ne|--1--\n{eot}";

        var rendered = _parser.Render(_parser.Parse(content), 0);

        Assert.Equal("{start_of_tab}\ne|--0--\n{end_of_tab}\n{start_of_tab}\ne|--1--\n{end_of_tab}\n", rendered);
    }

    [Fact]
    public void RoundTrip_TabBlockAtTheEndOfAChorus_DoesNotLoseOrMoveContent()
    {
        var content = "{soc}\n[C]Chorus\n{eoc}\n{sot}\ne|--0--\n{eot}";

        var rendered = _parser.Render(_parser.Parse(content), 0);

        Assert.Equal(
            "{start_of_chorus}\n[C]Chorus\n{end_of_chorus}\n{start_of_tab}\ne|--0--\n{end_of_tab}\n",
            rendered);
    }

    [Fact]
    public void Parse_EmptyTabBlock_ProducesNoLinesAndDoesNotSwallowWhatFollows()
    {
        var document = _parser.Parse("{sot}\n{eot}\n[C]Next");

        Assert.Single(document.Lines);
        Assert.Equal("Next", document.Lines[0].Text);
    }
}
