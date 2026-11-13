using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.Music;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Song numbers, contents lines, chorus labels with colons, capo numerals, one-line pastes and option checks.
public class HeaderAndLabelTests
{
    [Fact]
    public void SongNumberBeforeAnAllCapsTitle_IsNotAVerseLabel()
    {
        var result = Run("39. INSIEME", "", "Do       Sol", "Signore, ascolta");

        Assert.Equal(Out("{title:39. INSIEME}", "", "[Do]Signore, [Sol]ascolta"), result.ChordPro);
        Assert.Equal(SheetLineKind.Title, result.Diag(1).Kind);
    }

    [Fact]
    public void SongNumberBeforeAnAllCapsTitle_FollowedByMetadata_IsNotAVerseLabel()
    {
        var result = Run("7) LODE AL SIGNORE", "Tonalità: Do", "", "Santo");

        Assert.DoesNotContain("start_of_verse", result.ChordPro);
        Assert.Contains("{title:7) LODE AL SIGNORE}", result.ChordPro);
    }

    [Fact]
    public void NumberedVerseInTheBody_StaysAVerseLabel() =>
        Assert.Equal(Out("{start_of_verse:1.}", "Signore", "{end_of_verse}"), Run("1. Signore").ChordPro);

    [Fact]
    public void NumberedAllCapsLineInsideTheSong_StaysAVerseLabel()
    {
        var result = Run("Do  Sol", "Ti lodo", "", "2. BENEDETTO", "Sia il Signore");

        Assert.Contains("{start_of_verse:2.}", result.ChordPro);
    }

    [Fact]
    public void ContentsLineWithAPageReference_IsPlainText()
    {
        var result = Run("Indice", "2. Titolo   pag. 2", "3. Altro canto  pag. 7");

        Assert.Equal(Out("Indice", "2. Titolo   pag. 2", "3. Altro canto  pag. 7"), result.ChordPro);
        Assert.All(result.Lines, l => Assert.Equal(SheetLineKind.Lyric, l.Kind));
    }

    [Theory]
    [InlineData("Rit.:")]
    [InlineData("RIT.:")]
    [InlineData("Rit:")]
    public void ChorusLabelWithPunctuation_IsAChorus(string label) =>
        Assert.Equal(Out("{start_of_chorus}", "Santo", "{end_of_chorus}"), Run(label, "Santo").ChordPro);

    [Fact]
    public void ChorusLabelWithColonAndRest_KeepsTheRest() =>
        Assert.Equal(Out("{start_of_chorus}", "Santo", "{end_of_chorus}"), Run("Rit.: Santo").ChordPro);

    [Theory]
    [InlineData("Capo VX")]
    [InlineData("Capo IIX")]
    [InlineData("Capo IIII")]
    public void InvalidRomanNumeralCapo_IsUnreadableMetadata(string line)
    {
        var result = Run(line, "Do Sol", "Santo");

        Assert.Equal(Out("{comment:" + line + "}", "", "[Do]San[Sol]to"), result.ChordPro);
        Assert.Contains(SheetWarning.UnreadableMetadata, result.Diag(1).Warnings);
    }

    [Theory]
    [InlineData("Capo IV", "4")]
    [InlineData("Capo I", "1")]
    [InlineData("Capo IX", "9")]
    public void ValidRomanNumeralCapo_IsConverted(string line, string fret) =>
        Assert.Equal(Out("{capo:" + fret + "}", "", "Santo"), Run(line, "", "Santo").ChordPro.Replace("{title:Santo}\n", string.Empty).Replace("\n\n\n", "\n\n"));

    [Theory]
    [InlineData("Santo")]
    [InlineData("Santo", "")]
    [InlineData("Santo", "", "")]
    public void OneLinePaste_IsALyricNotATitle(params string[] lines)
    {
        var result = Run(lines);

        Assert.Equal(Out("Santo"), result.ChordPro);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(1).Kind);
    }

    [Fact]
    public void FirstLineThatIsAChordLineInTheOtherNotation_IsNeverTheTitle()
    {
        var result = Run("C   G   Am", "", "Do  Sol", "Ti lodo");

        Assert.DoesNotContain("{title", result.ChordPro);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(1).Kind);
    }

    [Fact]
    public void UndefinedNotation_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ChordSheetConverter.Convert("Do", new ChordSheetOptions { Notation = (Notation)99 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => ChordSheetConverter.Convert("Do", new ChordSheetOptions { TieBreakNotation = (Notation)7 }));
    }

    [Fact]
    public void NegativeSnapWidth_IsRejected() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ChordSheetConverter.Convert("Do", new ChordSheetOptions { SnapColumns = -1 }));

    [Fact]
    public void ZeroSnapWidth_IsAllowed() =>
        Assert.Equal(Out("[Do]Santo"), ChordSheetConverter.Convert("Do\nSanto", new ChordSheetOptions { SnapColumns = 0 }).ChordPro);
}
