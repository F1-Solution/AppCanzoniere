using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Spec 9.1 cases 28-38 and 48, plus the structure rules of section 6.2.
public class SectionTests
{
    [Fact]
    public void Case28_ChorusWithLabel_IsWrappedInDirectives() =>
        Assert.Equal(
            Out("{start_of_chorus}", "[Do]Ti [Sol]lodiamo", "Signore", "{end_of_chorus}"),
            Run("Rit.", "Do Sol", "Ti lodiamo", "Signore").ChordPro);

    [Theory]
    [InlineData("RIT:")]
    [InlineData("Ritornello")]
    [InlineData("Rit")]
    [InlineData("Chorus")]
    [InlineData("Refrain")]
    [InlineData("rit.")]
    public void Case29_ChorusLabelVariants(string label) =>
        Assert.Equal(Out("{start_of_chorus}", "Santo", "{end_of_chorus}"), Run(label, "Santo").ChordPro);

    [Fact]
    public void Case30_NumberedChorus_KeepsTheLabelAsValue() =>
        Assert.Equal(
            Out("{start_of_chorus:Rit. 2}", "Santo", "{end_of_chorus}"),
            Run("Rit. 2", "Santo").ChordPro);

    [Fact]
    public void Case31_ChorusRecall_BecomesTheChorusDirective() =>
        Assert.Equal(
            Out("{start_of_chorus}", "Santo", "{end_of_chorus}", "", "{start_of_verse:Strofa 2}", "Benedetto", "{end_of_verse}", "", "{chorus}"),
            Run("Rit.", "Santo", "", "Strofa 2", "Benedetto", "", "Rit.").ChordPro);

    [Fact]
    public void Case31_NumberedRecall_KeepsTheValue() =>
        Assert.Equal(
            Out("{start_of_chorus}", "Santo", "{end_of_chorus}", "", "{chorus:Rit. 2}"),
            Run("Rit.", "Santo", "", "Rit. 2").ChordPro);

    [Fact]
    public void Case32_RecallWithoutChorus_IsFlagged()
    {
        var result = Run("Benedetto", "il Signore", "", "Rit.");

        Assert.Equal(Out("Benedetto", "il Signore", "", "{chorus}"), result.ChordPro);
        Assert.Contains(SheetWarning.ChorusRecallWithoutChorus, result.Diag(4).Warnings);
    }

    [Theory]
    [InlineData("Strofa 1", "{start_of_verse:Strofa 1}")]
    [InlineData("Verse 2", "{start_of_verse:Verse 2}")]
    [InlineData("1.", "{start_of_verse:1.}")]
    [InlineData("2)", "{start_of_verse:2)}")]
    [InlineData("Strofa 3:", "{start_of_verse:Strofa 3}")]
    [InlineData("Strofa", "{start_of_verse}")]
    public void Case33_VerseLabels(string label, string directive) =>
        Assert.Equal(Out(directive, "Santo", "{end_of_verse}"), Run(label, "Santo").ChordPro);

    [Fact]
    public void Case34_NumberedVerseInlineWithChords_ShiftsTheChordsByTheLabel() =>
        Assert.Equal(
            Out("{start_of_verse:1.}", "[Do]Signore, [Sol]ascolta", "{end_of_verse}"),
            Run("   Do       Sol", "1. Signore, ascolta").ChordPro);

    [Fact]
    public void Case35_SectionEndsAtTheNextLabel() =>
        Assert.Equal(
            Out("{start_of_chorus}", "Santo", "{end_of_chorus}", "{start_of_verse:Strofa 2}", "Benedetto", "{end_of_verse}"),
            Run("Rit.", "Santo", "Strofa 2", "Benedetto").ChordPro);

    [Fact]
    public void Case36_Bridge() =>
        Assert.Equal(Out("{start_of_bridge}", "Santo", "{end_of_bridge}"), Run("Ponte", "Santo").ChordPro);

    [Fact]
    public void Case37_IntroWithChords_IsACommentPlusInstrumentalLine() =>
        Assert.Equal(Out("{comment:Intro}", "[Do] [Sol] [Lam] [Fa]"), Run("Intro: Do Sol Lam Fa").ChordPro);

    [Fact]
    public void Case37_FinaleAlone_IsAComment() =>
        Assert.Equal(Out("{comment:Finale}"), Run("Finale").ChordPro);

    [Fact]
    public void CommentLabelWithChords_MergesWithTheLyricBelow() =>
        Assert.Equal(
            Out("{comment:Intro}", "[Do]Ti [Sol]lodiamo"),
            Run("Intro: Do Sol", "Ti lodiamo").ChordPro);

    [Fact]
    public void CommentLabel_ClosesTheOpenSection() =>
        Assert.Equal(
            Out("{start_of_chorus}", "Santo", "{end_of_chorus}", "{comment:Coda}", "Amen"),
            Run("Rit.", "Santo", "Coda", "Amen").ChordPro);

    [Theory]
    [InlineData("Stanza mia")]
    [InlineData("Ritorna Signore")]
    public void Case38_NotALabel_StaysLyric(string line)
    {
        var result = Run(line);

        Assert.Equal(Out(line), result.ChordPro);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(1).Kind);
    }

    [Fact]
    public void LabelWithRestText_StartsTheSectionWithThatText() =>
        Assert.Equal(
            Out("{start_of_chorus}", "Alleluia", "{end_of_chorus}"),
            Run("Rit. Alleluia").ChordPro);

    [Fact]
    public void VerseOrBridgeLabelWithoutContent_IsAComment() =>
        Assert.Equal(Out("Santo", "Dio", "", "{comment:Strofa 2}"), Run("Santo", "Dio", "", "Strofa 2").ChordPro);

    [Fact]
    public void UnlabelledStanzas_GetNoEnvironment() =>
        Assert.Equal(Out("Santo", "Dio", "", "Benedetto"), Run("Santo", "Dio", "", "Benedetto").ChordPro);

    [Fact]
    public void Case48_TabBlock_IsWrappedAndKeptVerbatim()
    {
        var result = Run("e|-----0-----|", "B|---1---1---|");

        Assert.Equal(Out("{start_of_tab}", "e|-----0-----|", "B|---1---1---|", "{end_of_tab}"), result.ChordPro);
        Assert.All(result.Lines, l => Assert.Equal(SheetLineKind.Tab, l.Kind));
    }

    [Fact]
    public void TabBlock_ClosesASectionAndFollowsAnInstrumentalChordLine() =>
        Assert.Equal(
            Out("{start_of_chorus}", "Santo", "[Do] [Sol]", "{end_of_chorus}", "{start_of_tab}", "e|-----0-----|", "{end_of_tab}"),
            Run("Rit.", "Santo", "Do Sol", "e|-----0-----|").ChordPro);
}
