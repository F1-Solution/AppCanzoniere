using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Ordinary words that the shared chord parser would accept are not definite chords.
public class WordLikeChordTests
{
    [Theory]
    [InlineData("Dono")]
    [InlineData("Sino")]
    [InlineData("Reno")]
    [InlineData("Mino")]
    [InlineData("Remi")]
    [InlineData("Mimi")]
    [InlineData("Domi")]
    [InlineData("Fami")]
    public void ItalianWords_NeverBecomeChords(string word)
    {
        var result = Run("Do       Sol", "Signore, ascolta", word);

        Assert.Equal(Out("[Do]Signore, [Sol]ascolta", word), result.ChordPro);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(3).Kind);
        Assert.Equal(1.0, result.Diag(3).Confidence);
    }

    [Theory]
    [InlineData("Ami")]
    [InlineData("Ebo")]
    [InlineData("Eno")]
    public void EnglishWords_NeverBecomeChords(string word)
    {
        var result = Run("C     G", "Lord, hear us", word);

        Assert.Equal(Out("[C]Lord, [G]hear us", word), result.ChordPro);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(3).Kind);
    }

    [Theory]
    [InlineData("Dono")]
    [InlineData("Ami")]
    public void WordAloneInADocumentWithoutChords_IsALyric(string word)
    {
        var result = Run("Santo", word);

        Assert.Equal(Out("Santo", word), result.ChordPro);
        Assert.All(result.Lines, l => Assert.Equal(SheetLineKind.Lyric, l.Kind));
    }

    [Fact]
    public void RealItalianMinorAndFlatChords_StayChordsOnAChordLine()
    {
        var result = Run("Do  Lab  Sim  Rem  Mim  Lam  Solm  Fam  Dom", "Ti lodo Signore mio");

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Equal(1.0, result.Diag(1).Confidence);
        Assert.StartsWith("[Do]", result.ChordPro);
        Assert.Equal(9, result.ChordPro.Count(c => c == '['));
    }

    [Fact]
    public void RealEnglishQualities_StayChordsOnAChordLine()
    {
        var result = Run("C  Em  Gsus  Cmaj  Dmin  Aaug  Bdim  Bb  F#m  Dsus", "Lord hear us all today please");

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Equal(1.0, result.Diag(1).Confidence);
        Assert.Equal(10, result.ChordPro.Count(c => c == '['));
    }

    [Fact]
    public void DoubtfulWord_InsideAClearChordLine_IsKeptAsChordAndFlagged()
    {
        var result = Run("Do   Sol   Dono   Re", "Ti lodo Signore mio");

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Contains("[Dono]", result.ChordPro);
        Assert.Contains(SheetWarning.AmbiguousNoteWords, result.Diag(1).Warnings);
        Assert.True(result.Diag(1).Confidence <= 0.6);
    }

    [Theory]
    [InlineData("Dom")]
    [InlineData("Lab")]
    [InlineData("Sim")]
    [InlineData("Rem")]
    [InlineData("Lam")]
    public void RealChordAlone_FollowsTheSingleTokenRule(string chord)
    {
        var alone = Run("Santo", chord);
        var withContext = Run("Do Sol", "Ti lodo", chord, "Signore");

        Assert.Equal(SheetLineKind.Lyric, alone.Diag(2).Kind);
        Assert.Contains(SheetWarning.AmbiguousSingleToken, alone.Diag(2).Warnings);
        Assert.Equal(SheetLineKind.ChordLine, withContext.Diag(3).Kind);
        Assert.Equal(Out("[Do]Ti [Sol]lodo", "[" + chord + "]Signore"), withContext.ChordPro);
    }
}
