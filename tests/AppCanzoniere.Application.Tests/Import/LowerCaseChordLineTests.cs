using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// A lower-case line is a chord line only with a real chord feature or wide gaps aligned with the lyric below; indentation proves nothing.
public class LowerCaseChordLineTests
{
    private static readonly string Margin = new(' ', 40);

    [Theory]
    [InlineData("      fa la la la la")]
    [InlineData("do re mi fa sol")]
    [InlineData("      do re mi")]
    [InlineData("la la la")]
    public void SolfegeLyrics_StayLyricsWhereverTheyAreIndented(string line)
    {
        var result = Run("Do       Sol", "Signore, ascolta", line);

        Assert.Equal(SheetLineKind.Lyric, result.Diag(3).Kind);
        Assert.DoesNotContain("[", result.ChordPro.Split('\n')[1]);
    }

    [Fact]
    public void BookletIndentedBy40Columns_KeepsSolfegeRefrainAsLyric()
    {
        var result = Run(Margin + "Do       Sol", Margin + "Signore, ascolta", Margin + "fa la la la la", Margin + "do re mi");

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(3).Kind);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(4).Kind);
        Assert.Equal(1, result.ChordLineCount);
    }

    [Theory]
    [InlineData("sol7  do", "[sol7] [do]")]
    [InlineData("fa#  re", "[fa#] [re]")]
    [InlineData("do  mi/sol", "[do] [mi/sol]")]
    [InlineData("la-  re", "[la-] [re]")]
    public void RealChordFeature_MakesALowerCaseChordLine_EvenIndentedAndAlone(string chords, string expected) =>
        Assert.Equal(Out(expected), Run(Margin + chords).ChordPro);

    [Fact]
    public void WideGapsWithoutALyricBelow_AreNotEnough() =>
        Assert.Equal(Out("re    do    sol"), Run("re    do    sol").ChordPro);

    [Fact]
    public void WideGapsThatDoNotAlignWithTheLyric_AreNotEnough()
    {
        var result = Run("re        do        sol", "Ti lodo");

        Assert.Equal(SheetLineKind.Lyric, result.Diag(1).Kind);
    }

    [Fact]
    public void WideGapsAlignedWithTheLyricBelow_AreAChordLineEvenWhenIndented()
    {
        var result = Run(Margin + "re    sol     do", Margin + "Santo Signore dei tanti");

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Equal(Out("[re]Santo [sol]Signore [do]dei tanti"), result.ChordPro);
    }

    [Fact]
    public void SolfegeRunInADocumentWithLowerCaseChordLines_StaysALyric()
    {
        var result = Run(
            "do sol7 lam", "Il Signore e il mio pastore", "re- la mi7", "Non manco di nulla",
            "fa do/mi sol", "Su pascoli verdi", "do re mi fa sol", "Mi fa riposare");

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(5).Kind);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(7).Kind);
        Assert.Contains("\ndo re mi fa sol\n", result.ChordPro);
    }

    [Fact]
    public void WideGapsInADocumentWithLowerCaseChordLines_AreStillAChordLine()
    {
        var result = Run(
            "do sol7 lam", "Il Signore e il mio pastore", "re- la mi7", "Non manco di nulla",
            "fa do/mi sol", "Su pascoli verdi", "do   re   mi", "Mi fa riposare");

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(7).Kind);
    }

    [Fact]
    public void DocumentWithSeveralLowerCaseChordLines_ReadsLowerCaseLinesWithoutGapsAsChords()
    {
        var result = Run(
            "   sol               do sol re sol", "", "Rit. Canto al Signore", "",
            "        re   do sol re", "", "Benedetto sia il mare", "",
            "   re      mi    la", "", "Terra e cielo", "",
            "        sol do sol re sol", "", "fa la la la la");

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(5).Kind);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(9).Kind);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(13).Kind);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(15).Kind);
    }
}
