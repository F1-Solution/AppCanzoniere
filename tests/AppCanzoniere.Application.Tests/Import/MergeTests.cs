using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Spec 9.1 cases 3-7, 15, 25-27 and the merge algorithm of section 5.4.
public class MergeTests
{
    [Fact]
    public void Case03_ChordInsideAWord_StaysInsideAndIsFlagged()
    {
        var result = Run("Sol Re", "Alleluia");

        Assert.Equal(Out("[Sol]Alle[Re]luia"), result.ChordPro);
        Assert.Contains(SheetWarning.ChordInsideWord, result.Diag(1).Warnings);
    }

    [Fact]
    public void Case04_ChordPastTheEnd_IsAppendedAfterTheLyric() =>
        Assert.Equal(Out("[Do]Amen [Sol]"), Run("Do          Sol", "Amen").ChordPro);

    [Fact]
    public void Case05_TwoChordsPastTheEnd_KeepColumnOrder() =>
        Assert.Equal(Out("[Do]Amen [Fa] [Sol]"), Run("Do    Fa  Sol", "Amen").ChordPro);

    [Fact]
    public void Case06_InstrumentalLineThenPair()
    {
        var result = Run("Do  Sol  Lam  Fa", "Do       Sol", "Signore, ascolta");

        Assert.Equal(Out("[Do] [Sol] [Lam] [Fa]", "[Do]Signore, [Sol]ascolta"), result.ChordPro);
        Assert.Equal(2, result.ChordLineCount);
        Assert.Equal(1, result.MergedChordLineCount);
    }

    [Fact]
    public void Case07_BlankLineBlocksTheMerge() =>
        Assert.Equal(Out("[Re] [La]", "", "Ti lodiamo"), Run("Re  La", "", "Ti lodiamo").ChordPro);

    [Fact]
    public void Case15_RepeatMarkPastTheEnd_BecomesAnAnnotation() =>
        Assert.Equal(
            Out("[Do]Signore, [Sol]ascolta [*x2]"),
            Run("Do       Sol       x2", "Signore, ascolta").ChordPro);

    [Fact]
    public void Case25_ChordOnASpace_SnapsToTheNextWordStart()
    {
        var result = Run("Do      Sol", "Gloria a te");

        Assert.Equal(Out("[Do]Gloria a [Sol]te"), result.ChordPro);
        Assert.Contains(SheetWarning.SnappedColumn, result.Diag(1).Warnings);
    }

    [Fact]
    public void Case26_NoWordStartWithinReach_KeepsTheColumn() =>
        Assert.Equal(Out("[Do]Amen, [Sol]    amen"), Run("Do    Sol", "Amen,     amen").ChordPro);

    [Fact]
    public void Case26_SnapColumnsOption_WidensTheReach() =>
        Assert.Equal(
            Out("[Do]Amen,     [Sol]amen"),
            RunWith(new ChordSheetOptions { SnapColumns = 4 }, "Do    Sol", "Amen,     amen").ChordPro);

    [Fact]
    public void Case27_IndentedPair_AppliesTheSameShiftToBothLines() =>
        Assert.Equal(Out("[Re]Il Si[Sol]gnore"), Run("     Re   Sol", "     Il Signore").ChordPro);

    [Fact]
    public void ChordsBeforeTheLyricIndent_AreMovedToTheStart() =>
        Assert.Equal(Out("[Re][Sol]Il Signore"), Run("Re  Sol", "     Il Signore").ChordPro);

    [Fact]
    public void ChordLineDiagnostics_PointToTheMergedOutputLine()
    {
        var result = Run("Do  Sol  Lam  Fa", "Do       Sol", "Signore, ascolta");

        Assert.Equal(0, result.Diag(1).OutputLine);
        Assert.Equal(1, result.Diag(2).OutputLine);
        Assert.Equal(1, result.Diag(3).OutputLine);
    }

    [Fact]
    public void ChordLineAboveAChordLine_IsInstrumental() =>
        Assert.Equal(Out("[Do] [Sol]", "[Lam] [Fa]"), Run("Do Sol", "Lam Fa").ChordPro);
}
