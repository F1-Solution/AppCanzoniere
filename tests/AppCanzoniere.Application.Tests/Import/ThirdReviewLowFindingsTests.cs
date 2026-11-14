using System.Text.RegularExpressions;
using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Small findings of the third review: directives, punctuation in groups, lone accidental chords and labelled lower-case chords.
public class ThirdReviewLowFindingsTests
{
    [Theory]
    [InlineData("{album: X}")]
    [InlineData("{columns: 2}")]
    [InlineData("  {sorttitle}  ")]
    [InlineData("{x_my-meta_1: a b}")]
    public void WholeLineDirective_MakesTheTextChordPro(string directive)
    {
        var result = Run(directive, "[C] Sing", "[G] Lord");

        Assert.True(result.WasChordPro);
    }

    [Theory]
    [InlineData("{x2}")]
    [InlineData("{bis}")]
    public void RepeatMarkInBraces_IsNotADirective(string mark) =>
        Assert.False(Run("Santo", mark).WasChordPro);

    [Fact]
    public void ParenthesisedGroupWithCommas_NeverProducesAChordWithPunctuation()
    {
        var result = Run("Do   (Si, Re)   Sol", "Canta il Signore oggi sempre");

        Assert.DoesNotMatch(new Regex(@"\[[^\]]*[,;:.]\]"), result.ChordPro);
    }

    [Theory]
    [InlineData("C   G", "Sing to the Lord", "F#mi", "[F#mi]Hear us")]
    [InlineData("C   G", "Sing to the Lord", "Bbmi", "[Bbmi]Hear us")]
    [InlineData("Do   Sol", "Canta il Signore", "Do#m", "[Do#m]Hear us")]
    [InlineData("Do   Sol", "Canta il Signore", "Sib", "[Sib]Hear us")]
    [InlineData("Do   Sol", "Canta il Signore", "Lab", "[Lab]Hear us")]
    public void LoneChordWithAnAccidental_IsAChordLine(string chords, string lyric, string lone, string expected)
    {
        var result = Run(chords, lyric, "", lone, "Hear us");

        Assert.Equal(Out(Run(chords, lyric).ChordPro.TrimEnd('\n'), "", expected), result.ChordPro);
    }

    [Fact]
    public void LabelFollowedByLowerCaseBareNotes_CarriesChordsInADocumentWithChordLines()
    {
        var result = Run("Do   Sol", "Canta il Signore", "", "Intro: do sol re sol");

        Assert.Equal(Out("[Do]Canta [Sol]il Signore", "", "{comment:Intro}", "[do] [sol] [re] [sol]"), result.ChordPro);
        Assert.True(result.Diag(4).CarriesChords);
    }

    [Fact]
    public void LabelFollowedByLowerCaseBareNotes_IsALyricWithoutOtherChordLines() =>
        Assert.DoesNotContain("[do]", Run("Intro: do sol re sol", "Canta il Signore").ChordPro);
}
