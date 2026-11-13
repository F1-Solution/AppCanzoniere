using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.Music;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// "mi" is a minor quality in English notation only; Italian note-like words stay words.
public class EnglishMinorTests
{
    [Fact]
    public void MiMinors_AreAChordLineAboveALyric()
    {
        var result = Run("Dmi  Gmi  Ami", "Lord hear us now");

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Equal(Out("[Dmi]Lord [Gmi]hear [Ami]us now"), result.ChordPro);
        Assert.Equal(Notation.English, result.DetectedNotation);
    }

    [Fact]
    public void MiMinorWithADigit_IsAChordLine() =>
        Assert.Equal(SheetLineKind.ChordLine, Run("Dmi7 Gmi Ami").Diag(1).Kind);

    [Theory]
    [InlineData("Ami")]
    [InlineData("Emi")]
    public void SingleMiMinor_FollowsTheSingleTokenRule(string line)
    {
        var result = RunWith(new ChordSheetOptions { Notation = Notation.English }, line);

        Assert.Equal(SheetLineKind.Lyric, result.Diag(1).Kind);
        Assert.Contains(SheetWarning.AmbiguousSingleToken, result.Diag(1).Warnings);
    }

    [Theory]
    [InlineData("Remi Mimi Domi Fami")]
    [InlineData("Remi Mimi Do Fami")]
    public void ItalianWords_StayWords(string line)
    {
        var result = RunWith(new ChordSheetOptions { Notation = Notation.Italian }, line);

        Assert.Equal(SheetLineKind.Lyric, result.Diag(1).Kind);
    }
}
