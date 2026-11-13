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
    public void LowerCaseBareNamesWithWideGaps_AreAChordLineOnlyAboveAlignedLyrics() =>
        Assert.Equal(Out("[re]Santo [sol]Signore [do]dei tanti"), Run("re    sol     do", "Santo Signore dei tanti").ChordPro);

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

    [Fact]
    public void IndentedLowerCaseBareNames_AreLyric() =>
        Assert.Equal(Out("        do sol re sol"), Run("        do sol re sol").ChordPro);

    [Fact]
    public void ParenthesisedLowerCaseKeyAndSongNumber_BeforeChords() =>
        Assert.Equal(Out("134 [do] [fa] [do] [mi] [la-]"), Run("134 (do) fa    do    mi   la-").ChordPro);

    [Fact]
    public void SpacedParenthesesAroundAChord_AreAParenthesisedChord() =>
        Assert.Equal(Out("[Am] [F7+] [C] [F7+]"), Run("Am   F7+   C ( F7+ )").ChordPro);

    [Fact]
    public void BassOnlyChordAndArrow_AreNeutral() =>
        Assert.Equal(Out("[Re-] [La-] /do [Sib7+] → [Sol]"), Run("Re- La- /do Sib7+ → Sol").ChordPro);
}
