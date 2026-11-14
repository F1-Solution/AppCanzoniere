using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Pins the current behaviour: without an anchor line a parenthesised word keeps the pair as plain text.
public class ParenthesisedChordWithoutAnchorTests
{
    [Fact]
    public void ALowerCaseChordLineOverALyricWithAParenthesisedNote_KeepsBothLinesAsPlainText()
    {
        var result = Run("do    sol   re\nCanta (Fa) e loda");

        Assert.Equal(Out("do    sol   re\nCanta (Fa) e loda"), result.ChordPro);
        Assert.DoesNotContain("[", result.ChordPro);
    }
}
