using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// After a sung-section label, bare lower-case note names need the evidence of a bare line; only instrumental labels carry them alone.
public class SungLabelNoteTests
{
    [Theory]
    [InlineData("Rit. fa la la la la", "[fa]")]
    [InlineData("Rit: do re mi fa sol", "[do]")]
    [InlineData("Coro: si fa", "[si]")]
    [InlineData("Strofa: re mi fa sol", "[re]")]
    public void LowerCaseWordsAfterASungLabel_StayLyrics(string labelled, string chord)
    {
        var result = Run("Do   Sol", "Santo ascolta", "", labelled, "Ti lodo Signore");

        Assert.DoesNotContain(chord, result.ChordPro);
        Assert.Contains(labelled.Split(' ', 2)[1], result.ChordPro);
        Assert.False(result.Diag(4).CarriesChords);
    }

    [Theory]
    [InlineData("Intro: do sol re sol")]
    [InlineData("Finale. do sol re sol")]
    [InlineData("Ponte strumentale: do sol re sol")]
    public void LowerCaseNotesAfterAnInstrumentalLabel_AreChords(string labelled)
    {
        var result = Run("Do   Sol", "Santo ascolta", "", labelled, "", "Lam   Re", "Ti lodo Signore");

        Assert.True(result.Diag(4).CarriesChords);
    }

    [Theory]
    [InlineData("Rit. LAm  FA  SOL")]
    [InlineData("Rit: Do   Sol   Lam")]
    public void UpperCaseChordsAfterASungLabel_AreStillChords(string labelled)
    {
        var result = Run("Do   Sol", "Santo ascolta", "", labelled, "Ti lodo Signore");

        Assert.True(result.Diag(4).CarriesChords);
    }
}
