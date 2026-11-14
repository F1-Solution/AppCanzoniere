using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// A bracketed lyric word that only the other notation reads as a chord is text.
public class BracketedOtherNotationTests
{
    [Theory]
    [InlineData("Vieni [e] canta", "Vieni (e) canta")]
    [InlineData("Va [a] casa", "Va (a) casa")]
    [InlineData("Tu [E] noi", "Tu (E) noi")]
    [InlineData("Io [Am] te", "Io (Am) te")]
    public void ItalianDocument_ReplacesBracketsAroundEnglishOnlyWords(string lyric, string expected)
    {
        var result = Run("Do      Sol", "Canta il Signore", "", lyric);

        Assert.Equal(Out("[Do]Canta il [Sol]Signore", "", expected), result.ChordPro);
        Assert.Contains(SheetWarning.BracketsReplaced, result.Diag(4).Warnings);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(4).Kind);
    }

    [Theory]
    [InlineData("Come [la] home", "Come (la) home")]
    [InlineData("Sing [Do] it", "Sing (Do) it")]
    [InlineData("We [mi] go", "We (mi) go")]
    public void EnglishDocument_ReplacesBracketsAroundItalianOnlyWords(string lyric, string expected)
    {
        var result = Run("C       G", "Sing to the Lord", "", lyric);

        Assert.Equal(Out("[C]Sing to [G]the Lord", "", expected), result.ChordPro);
        Assert.Contains(SheetWarning.BracketsReplaced, result.Diag(4).Warnings);
    }

    [Fact]
    public void BracketedChordOfTheDocumentNotation_StaysAChord() =>
        Assert.Equal(SheetLineKind.InlineChordLyric, Run("Do      Sol", "Canta il Signore", "", "Vieni [Re] canta").Diag(4).Kind);

    [Fact]
    public void LineMadeOnlyOfBracketedChords_KeepsChordsOfTheOtherNotation()
    {
        var result = Run("Do      Sol", "Canta il Signore", "", "[G] [Do]");

        Assert.Equal(Out("[Do]Canta il [Sol]Signore", "", "[G] [Do]"), result.ChordPro);
    }

    [Fact]
    public void ChordGluedToALetter_KeepsItsNotationEvenOnOneLineOfTheOtherNotation()
    {
        var result = Run("Do      Sol", "Canta il Signore", "", "[G]Vieni [e] canta");

        Assert.Equal(Out("[Do]Canta il [Sol]Signore", "", "[G]Vieni [e] canta"), result.ChordPro);
    }
}
