using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Spec 9.1 cases 9-13, 17, 18, 51 and the table of section 5.1.
public class ClassificationTests
{
    [Fact]
    public void Case09_NoteWordStartingALyric_StaysLyricUnderChords() =>
        Assert.Equal(Out("[Re]La mia [La]vita è tua"), Run("Re     La", "La mia vita è tua").ChordPro);

    [Theory]
    [InlineData("La mia vita")]
    [InlineData("Re dei re")]
    [InlineData("Si, lo voglio")]
    [InlineData("Mi fido di te")]
    [InlineData("Sol per te")]
    [InlineData("Do la vita")]
    [InlineData("Fa' che io viva")]
    [InlineData("La la la la")]
    [InlineData("LA LA LA")]
    public void Case10_ItalianAmbiguities_AreLyrics(string line)
    {
        var result = Run("Do       Sol", "Signore, ascolta", line);

        Assert.Equal(Out("[Do]Signore, [Sol]ascolta", line), result.ChordPro);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(3).Kind);
    }

    [Theory]
    [InlineData("A child is born")]
    [InlineData("Am I alone")]
    [InlineData("Go and tell")]
    public void Case11_EnglishAmbiguities_AreLyrics(string line)
    {
        var result = Run("C     G", "Lord, hear our prayer", line);

        Assert.Equal(Out("[C]Lord, [G]hear our prayer", line), result.ChordPro);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(3).Kind);
    }

    [Fact]
    public void Case12_SingleTokenWithContext_IsAChordLine()
    {
        var result = Run("Do Sol", "Ti lodiamo", "Re", "Amen");

        Assert.Equal(Out("[Do]Ti [Sol]lodiamo", "[Re]Amen"), result.ChordPro);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(3).Kind);
        Assert.Equal(0.5, result.Diag(3).Confidence);
        Assert.Contains(SheetWarning.AmbiguousSingleToken, result.Diag(3).Warnings);
    }

    [Fact]
    public void Case13_SingleTokenWithoutContext_IsALyric()
    {
        var result = Run("Si", "lo voglio");

        Assert.Equal(Out("Si", "lo voglio"), result.ChordPro);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(1).Kind);
        Assert.Contains(SheetWarning.AmbiguousSingleToken, result.Diag(1).Warnings);
    }

    [Fact]
    public void Case17_NoChordMergesWithAChord() =>
        Assert.Equal(Out("[*N.C.]Taci, [Do]poi canta"), Run("N.C.  Do", "Taci, poi canta").ChordPro);

    [Fact]
    public void Case18_NoChordAloneAboveALyric_IsAChordLineWithLowConfidence()
    {
        var result = Run("N.C.", "Taci");

        Assert.Equal(Out("[*N.C.]Taci"), result.ChordPro);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Equal(0.6, result.Diag(1).Confidence);
    }

    [Fact]
    public void Case51_OneStrayWord_IsKeptAsAnnotationOnAnInstrumentalLine()
    {
        var result = Run("Do  Sol  Lam  Fa  lento");

        Assert.Equal(Out("[Do] [Sol] [Lam] [Fa] lento"), result.ChordPro);
        Assert.Contains(SheetWarning.StrayWordOnChordLine, result.Diag(1).Warnings);
        Assert.Equal(0.6, result.Diag(1).Confidence);
    }

    [Fact]
    public void StrayWordAboveALyric_IsAnAnnotation() =>
        Assert.Equal(
            Out("[Do]Ti [Sol]lodiamo [Lam] [Fa] [*lento]"),
            Run("Do Sol         Lam Fa lento", "Ti lodiamo").ChordPro);

    [Fact]
    public void ChordLineConfidence_DropsWhenAmbiguousNamesAreInvolved()
    {
        var result = Run("Lam  Do", "Ti lodiamo", "Re  Sol", "Ti lodiamo", "Lam  Rem", "Ti lodiamo");

        Assert.Equal(0.9, result.Diag(1).Confidence);
        Assert.Equal(0.8, result.Diag(3).Confidence);
        Assert.Equal(1.0, result.Diag(5).Confidence);
    }

    [Fact]
    public void LyricsWithChordLikeWords_AreFlaggedAndDoubtful()
    {
        var result = Run("Do       Sol", "Signore, ascolta", "La mia vita");

        Assert.Contains(SheetWarning.AmbiguousNoteWords, result.Diag(3).Warnings);
        Assert.Contains(result.DoubtfulLines, l => l.LineNumber == 3);
        Assert.DoesNotContain(result.DoubtfulLines, l => l.LineNumber == 1);
    }
}
