using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Pins how a bracketed chord of the other notation on a lyric line is judged in an Italian document.
public class BracketBetweenWordsTests
{
    private static string Convert(string lyric) => Run("Do      Sol", "Canta il Signore", "", lyric).ChordPro;

    private static string Expected(string lyric) => Out("[Do]Canta il [Sol]Signore", "", lyric);

    // Punctuation written as its own word ("…", "«", "»") does not stop the line from being a lyric.
    [Theory]
    [InlineData("Vieni [e] canta …", "Vieni (e) canta …")]
    [InlineData("« Vieni [e] canta »", "« Vieni (e) canta »")]
    public void PunctuationWords_DoNotKeepTheBracketAsAChord(string lyric, string expected) =>
        Assert.Equal(Expected(expected), Convert(lyric));

    // A bracket glued to the punctuation before it does not stand between words (a space must precede it), so it
    // stays a chord as written.
    [Theory]
    [InlineData("Vieni,[G] e canta")]
    [InlineData("«[G] e canta")]
    public void BracketRightAfterPunctuation_StaysAChord(string lyric) =>
        Assert.Equal(Expected(lyric), Convert(lyric));

    // The line is judged with its other brackets already replaced: "[bis]" becomes "(bis)", so "[G]" stands between
    // words. Judged on the raw text, the "[" after "[G]" would wrongly keep it a chord.
    [Fact]
    public void BracketBeforeAnotherReplacedBracket_IsJudgedOnTheReplacedText() =>
        Assert.Equal(Expected("Vieni e canta (G) (bis)"), Convert("Vieni e canta [G] [bis]"));
}
