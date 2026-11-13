using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Reduced cases with invented lyrics, written after running the local corpus.
public class CorpusFindingsTests
{
    [Fact]
    public void LowerCaseChordLine_WithDashMinorAndWideGaps_IsAChordLine()
    {
        var result = Run("la-     sol     do", "Ti lodo Signore mio");

        Assert.Equal(Out("[la-]Ti lodo [sol]Signore [do]mio"), result.ChordPro);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Equal(Music.Notation.Italian, result.DetectedNotation);
    }

    [Fact]
    public void LowerCaseBareNamesWithWideGaps_AreAChordLine() =>
        Assert.Equal(Out("[re] [do] [sol]"), Run("re    do    sol").ChordPro);

    [Theory]
    [InlineData("mi fa sol")]
    [InlineData("la sol do")]
    [InlineData("la- la- la-")]
    public void LowerCaseNoteWordsInARow_StayLyric(string line)
    {
        var result = Run("Do       Sol", "Signore, ascolta", line);

        Assert.Equal(SheetLineKind.Lyric, result.Diag(3).Kind);
        Assert.Equal(Out("[Do]Signore, [Sol]ascolta", line), result.ChordPro);
    }

    [Fact]
    public void ChordsOfTheOtherNotationOnAMostlyItalianChordLine_AreKeptAsChords()
    {
        var result = Run("Do  Sol  Re   Mi   B  E");

        Assert.Equal(Out("[Do] [Sol] [Re] [Mi] [B] [E]"), result.ChordPro);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
    }

    [Fact]
    public void ShortNumberOnAChordLine_IsNeutral() =>
        Assert.Equal(Out("[La-] [Do] [Re] / 7"), Run("La- Do Re / 7").ChordPro);

    [Fact]
    public void CoroLabel_IsAChorus() =>
        Assert.Equal(Out("{start_of_chorus}", "Santo", "{end_of_chorus}"), Run("CORO:", "Santo").ChordPro);
}
