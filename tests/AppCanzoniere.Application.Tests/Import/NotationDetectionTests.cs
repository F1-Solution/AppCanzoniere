using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.Music;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Spec 9.1 cases 1, 2, 8, 49, 55 and the detection rules of section 4.
public class NotationDetectionTests
{
    [Fact]
    public void Case01_BasicItalian()
    {
        var result = Run("Do       Sol", "Signore, ascolta");

        Assert.Equal(Out("[Do]Signore, [Sol]ascolta"), result.ChordPro);
        Assert.Equal(Notation.Italian, result.DetectedNotation);
        Assert.Equal(1, result.ChordLineCount);
        Assert.Equal(1, result.MergedChordLineCount);
        Assert.False(result.WasChordPro);
    }

    [Fact]
    public void Case02_BasicEnglish()
    {
        var result = Run("C     G", "Lord, hear our prayer");

        Assert.Equal(Out("[C]Lord, [G]hear our prayer"), result.ChordPro);
        Assert.Equal(Notation.English, result.DetectedNotation);
    }

    [Fact]
    public void Case08_LyricsOnly_HasNoNotation()
    {
        var result = Run("Ti lodiamo");

        Assert.Equal(Out("Ti lodiamo"), result.ChordPro);
        Assert.Null(result.DetectedNotation);
        Assert.Equal(0, result.ChordLineCount);
    }

    // Spec 4.2 counts only lines with an unambiguous chord, which would make case 1's "Do Sol" count for nobody and
    // case 49 English; counting every definite chord line gives the tie that case 49 expects.
    [Fact]
    public void Case49_ChordsOfTheOtherNotation_StayLyricWithWarning()
    {
        var result = Run("Do       Sol", "Signore, ascolta", "C  G", "E sia la pace");

        Assert.Equal(Out("[Do]Signore, [Sol]ascolta", "C  G", "E sia la pace"), result.ChordPro);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(3).Kind);
        Assert.Contains(SheetWarning.OtherNotation, result.Diag(3).Warnings);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(4).Kind);
        Assert.Empty(result.Diag(4).Warnings);
    }

    [Fact]
    public void Case55_ForcedNotation_DoesNotFlagTheOtherNotation()
    {
        var input = new[] { "C     G", "Lord, hear our prayer" };

        var result = RunWith(new ChordSheetOptions { Notation = Notation.Italian }, input);

        Assert.Equal(Out(input), result.ChordPro);
        Assert.All(result.Lines, l =>
        {
            Assert.Equal(SheetLineKind.Lyric, l.Kind);
            Assert.Equal(1.0, l.Confidence);
            Assert.Empty(l.Warnings);
        });
    }

    [Fact]
    public void ForcedEnglish_ReadsEnglishChords()
    {
        var result = RunWith(new ChordSheetOptions { Notation = Notation.English }, "C     G", "Lord, hear our prayer");

        Assert.Equal(Out("[C]Lord, [G]hear our prayer"), result.ChordPro);
        Assert.Equal(Notation.English, result.DetectedNotation);
    }

    [Fact]
    public void MoreLinesInOneNotation_DecideTheNotation()
    {
        var result = Run("Lam    Rem", "Signore, ascolta", "Am  Dm", "Lord, hear", "C   G", "Lord, hear");

        Assert.Equal(Notation.English, result.DetectedNotation);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(1).Kind);
        Assert.Contains(SheetWarning.OtherNotation, result.Diag(1).Warnings);
    }

    [Fact]
    public void Tie_UsesItalianByDefault()
    {
        var result = Run("Lam  Rem", "", "Am  Dm");

        Assert.Equal(Out("[Lam] [Rem]", "", "Am  Dm"), result.ChordPro);
        Assert.Equal(Notation.Italian, result.DetectedNotation);
    }

    [Fact]
    public void Tie_UsesTheTieBreakNotation()
    {
        var result = RunWith(new ChordSheetOptions { TieBreakNotation = Notation.English }, "Lam  Rem", "", "Am  Dm");

        Assert.Equal(Out("Lam  Rem", "", "[Am] [Dm]"), result.ChordPro);
        Assert.Equal(Notation.English, result.DetectedNotation);
    }

    [Fact]
    public void OnlyAmbiguousChordLines_AreReadInTheTieBreakNotation()
    {
        var result = Run("Do   Sol", "Ti lodiamo");

        Assert.Equal(Out("[Do]Ti [Sol]lodiamo"), result.ChordPro);
        Assert.Equal(Notation.Italian, result.DetectedNotation);
    }

    [Fact]
    public void EnglishAmbiguousChords_AreReadWhenTheyAreTheOnlyChords()
    {
        var result = RunWith(new ChordSheetOptions { TieBreakNotation = Notation.English }, "A    Am", "Lord, hear");

        Assert.Equal(Out("[A]Lord, [Am]hear"), result.ChordPro);
    }
}
