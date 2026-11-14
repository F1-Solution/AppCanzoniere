using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// A document is ChordPro only with a whole-line directive or two lines with a chord attached to a letter.
public class ChordProDetectionTests
{
    [Fact]
    public void BracketedChordLinesAboveLyrics_AreConverted()
    {
        var result = Run("[B]     [G#m]", "Lord of all", "[E]     [F#]", "Hear us now");

        Assert.False(result.WasChordPro);
        Assert.Equal(Out("[B]Lord of [G#m]all", "[E]Hear us [F#]now"), result.ChordPro);
    }

    [Theory]
    [InlineData("[C]Sing a [G]song", "[C]Sing a [G]song")]
    [InlineData("Sing[C] a song", "Sing[C] a song")]
    public void TwoLinesWithAttachedChords_AreReturnedUnchanged(string first, string second)
    {
        var input = first + "\n" + second;

        var result = ChordSheetConverter.Convert(input);

        Assert.True(result.WasChordPro);
        Assert.Equal(input, result.ChordPro);
    }

    [Fact]
    public void LoneStarMarkerInALyric_IsConvertedAndKeptAsParenthesisedText()
    {
        var result = Run("Do Sol", "Santo ascolta [*] Dio");

        Assert.False(result.WasChordPro);
        Assert.Equal(Out("[Do]San[Sol]to ascolta (*) Dio"), result.ChordPro);
    }

    [Fact]
    public void TwoLyricsWithSpacedBracketedNoteNames_AreConverted()
    {
        var result = Run("Am   G", "Santo ascolta", "Canta [Do] forte", "Pace [Re] terra");

        Assert.False(result.WasChordPro);
        Assert.Equal(Out("[Am]Santo [G]ascolta", "Canta (Do) forte", "Pace (Re) terra"), result.ChordPro);
    }

    [Fact]
    public void SpacedBracketGroupsAndAnnotations_NeverCountAsChordPro()
    {
        var result = Run("Canta [ A7 ] forte", "Pace [ Re ] terra", "[*x2] Santo", "[*bis] Dio");

        Assert.False(result.WasChordPro);
    }

    [Fact]
    public void OneDirective_ReturnsTheDocumentUnchanged()
    {
        var input = "{comment:Capo 2}\nDo Sol\nTi lodo";

        var result = ChordSheetConverter.Convert(input);

        Assert.True(result.WasChordPro);
        Assert.Equal(input, result.ChordPro);
    }

    [Fact]
    public void AnnotationOutput_IsStableWithoutDocumentDetection()
    {
        var first = Run("N.C.", "Taci").ChordPro;

        Assert.Equal(Out("[*N.C.]Taci"), first);
        Assert.Equal(first, ChordSheetConverter.Convert(first).ChordPro);
    }
}
