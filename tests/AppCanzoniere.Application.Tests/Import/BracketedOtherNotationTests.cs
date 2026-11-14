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

    // Real lyric lines are full of one-letter words, note-name words, digits and typographic punctuation;
    // none of them makes the bracketed word of the other notation a chord.
    [Theory]
    [InlineData("Vieni [e] la luce", "Vieni (e) la luce")]
    [InlineData("Vieni [e] canta e balla", "Vieni (e) canta e balla")]
    [InlineData("È [a] te che canto", "È (a) te che canto")]
    [InlineData("Vieni [e] canta 2 volte", "Vieni (e) canta 2 volte")]
    [InlineData("«Vieni [e] canta…»", "«Vieni (e) canta…»")]
    [InlineData("Vieni [e] Re dei re", "Vieni (e) Re dei re")]
    public void ItalianDocument_ReplacesBracketsOnLyricsWithShortWordsAndPunctuation(string lyric, string expected)
    {
        var result = Run("Do      Sol", "Canta il Signore", "", lyric);

        Assert.Equal(Out("[Do]Canta il [Sol]Signore", "", expected), result.ChordPro);
        Assert.Contains(SheetWarning.BracketsReplaced, result.Diag(4).Warnings);
    }

    [Fact]
    public void ItalianDocument_KeepsTheChordOfItsNotationAndReplacesTheOtherOnTheSameLine()
    {
        var result = Run("Do      Sol", "Canta il Signore", "", "Canta [Lam] e va [a] casa");

        Assert.Equal(Out("[Do]Canta il [Sol]Signore", "", "Canta [Lam] e va (a) casa"), result.ChordPro);
    }

    [Theory]
    [InlineData("Come [la] home to a house", "Come (la) home to a house")]
    [InlineData("Go [si] now", "Go (si) now")]
    public void EnglishDocument_ReplacesBracketsOnLyricsWithShortWords(string lyric, string expected)
    {
        var result = Run("C       G", "Sing to the Lord", "", lyric);

        Assert.Equal(Out("[C]Sing to [G]the Lord", "", expected), result.ChordPro);
    }

    // The converter writes chords that do not fall on a letter after the lyric or over a wide gap;
    // a bracket in those places is read back as a chord, so the output converts to itself again.
    [Theory]
    [InlineData("Vieni e canta [G]")]
    [InlineData("Vieni e canta [G] [Am]")]
    [InlineData("Vieni [G]    e canta")]
    [InlineData("Vieni [G], e canta")]
    public void BracketAfterTheLyricOrBeforeAWideGap_StaysAChord(string lyric)
    {
        var result = Run("Do      Sol", "Canta il Signore", "", lyric);

        Assert.Equal(Out("[Do]Canta il [Sol]Signore", "", lyric), result.ChordPro);
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
