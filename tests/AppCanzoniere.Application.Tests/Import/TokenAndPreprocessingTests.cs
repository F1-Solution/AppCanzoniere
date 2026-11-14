using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Spec 9.1 cases 14, 16, 19-24, 50 plus token edge cases of 3.1 and 3.2.
public class TokenAndPreprocessingTests
{
    [Fact]
    public void Case14_BarsAroundChords_AreKeptInInstrumentalLine() =>
        Assert.Equal(Out("| [Do] | [Sol] | [Lam] | [Fa] |"), Run("| Do | Sol | Lam | Fa |").ChordPro);

    [Theory]
    [InlineData("Do  Sol  (2 v.)", "[Do] [Sol] (2 v.)")]
    [InlineData("Re  La  %", "[Re] [La] %")]
    public void Case16_RepeatMarks_AreKeptAsWritten(string input, string expected) =>
        Assert.Equal(Out(expected), Run(input).ChordPro);

    [Fact]
    public void Case19_ParenthesisedChord_IsWrittenWithoutParentheses()
    {
        var result = Run("Do     (Lam)", "Vieni, Signore");

        Assert.Equal(Out("[Do]Vieni, [Lam]Signore"), result.ChordPro);
        Assert.Contains(SheetWarning.ParenthesesRemoved, result.Diag(1).Warnings);
    }

    [Fact]
    public void Case20_DashSeparators_AreKeptBetweenChords() =>
        Assert.Equal(Out("[Do] - [Lam] - [Fa] - [Sol]"), Run("Do - Lam - Fa - Sol").ChordPro);

    [Fact]
    public void Case21_DashJoinedToken_IsSplitIntoChords() =>
        Assert.Equal(Out("[Do]-[Lam]-[Fa]-[Sol]"), Run("Do-Lam-Fa-Sol").ChordPro);

    [Fact]
    public void Case22_DashAfterNote_IsMinor() =>
        Assert.Equal(Out("[La-]Vieni, [Re]Signore"), Run("La-    Re", "Vieni, Signore").ChordPro);

    [Fact]
    public void Case23_RealTab_IsExpandedToTheNextTabStop()
    {
        var result = Run("Do\tSol", "Ti lodo e canto");

        Assert.Equal(Out("[Do]Ti lodo [Sol]e canto"), result.ChordPro);
        Assert.Contains(SheetWarning.TabExpanded, result.Diag(1).Warnings);
    }

    [Fact]
    public void Case23_TabWidthOption_ChangesTheColumn() =>
        Assert.Equal(
            Out("[Do]Ti [Sol]lodo e canto"),
            RunWith(new ChordSheetOptions { TabWidth = 3 }, "Do\tSol", "Ti lodo e canto").ChordPro);

    [Fact]
    public void Case24_NonBreakingSpace_CountsAsASpace() =>
        Assert.Equal(Out("[Do]Ti [Sol]lodiamo"), Run("Do\u00A0Sol", "Ti lodiamo").ChordPro);

    [Theory]
    [InlineData("Do\u202FSol")]
    [InlineData("Do\u2003Sol")]
    [InlineData("Do\u200B Sol")]
    [InlineData("\uFEFFDo Sol")]
    public void Case24_OtherInvisibleSpaces_AreNormalised(string chordLine) =>
        Assert.Equal(Out("[Do]Ti [Sol]lodiamo"), Run(chordLine, "Ti lodiamo").ChordPro);

    [Fact]
    public void Case50_UpperCaseItalianChords_AreKeptAsWritten() =>
        Assert.Equal(Out("[DO]Ti [LAm]lodiamo"), Run("DO LAm", "Ti lodiamo").ChordPro);

    [Fact]
    public void LineEndings_AreAllAccepted_AndATerminatingNewlineAddsNoLine()
    {
        var result = ChordSheetConverter.Convert("Do       Sol\r\nSignore, ascolta\r\n");

        Assert.Equal(Out("[Do]Signore, [Sol]ascolta"), result.ChordPro);
        Assert.Equal(2, result.Lines.Count);
        Assert.Equal(Out("[Do]Signore, [Sol]ascolta"), ChordSheetConverter.Convert("Do       Sol\rSignore, ascolta").ChordPro);
    }

    [Fact]
    public void TrailingWhitespace_IsIgnored() =>
        Assert.Equal(Out("[Do]Signore, [Sol]ascolta"), Run("Do       Sol   ", "Signore, ascolta  ").ChordPro);

    [Fact]
    public void SequenceWithDoubleDash_IsAWord() =>
        Assert.Equal(Out("La--Re"), Run("La--Re").ChordPro);

    [Fact]
    public void SequenceOfBareNotes_IsSplit() =>
        Assert.Equal(Out("[Re]-[Sol]"), Run("Re-Sol").ChordPro);

    [Fact]
    public void BarsStuckToChords_AreSplitOff() =>
        Assert.Equal(Out("|[Do]|[Fa]| [Sol]|"), Run("|Do|Fa| Sol|").ChordPro);

    [Fact]
    public void RepeatMarkVariants_AreNeutral() =>
        Assert.Equal(Out("[Do] [Sol] (x2) bis"), Run("Do Sol (x2) bis").ChordPro);

    [Fact]
    public void ChordWithBassAndSuffix_IsOneChord() =>
        Assert.Equal(Out("[Do/Mi] [Sol7] [Lam]"), Run("Do/Mi  Sol7  Lam").ChordPro);

    [Fact]
    public void LowerCaseNoteNames_AreWords() =>
        Assert.Equal(Out("la sol Lam"), Run("la sol Lam").ChordPro);
}
