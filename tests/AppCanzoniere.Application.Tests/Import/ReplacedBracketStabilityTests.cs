using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.Music;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// A bracketed chord of the other notation between lyric words is written in parentheses. The lower-case note line
// above that lyric must be read the same way on both conversions: on the first, the Italian reading sees "[Fa]" and
// does not use the lyric's word starts as alignment evidence; on the second it sees "(Fa)" and must not use them
// either, otherwise "do sol re" becomes a chord line, the notation vote ties and the document turns Italian.
public class ReplacedBracketStabilityTests
{
    [Fact]
    public void EnglishDocumentWithAnItalianBracketUnderLowerCaseNotes_KeepsItsNotationWhenConvertedAgain()
    {
        var first = Run("G       D       Em      C", "Vieni Signore", "do    sol   re", "Canta [Fa] e loda");
        var second = ChordSheetConverter.Convert(first.ChordPro);

        Assert.Equal(Out("[G]Vieni Si[D]gnore [Em] [C]", "do    sol   re", "Canta (Fa) e loda"), first.ChordPro);
        Assert.Equal(first.ChordPro, second.ChordPro);
        Assert.Equal(Notation.English, second.DetectedNotation);
    }

    // Documents found by the anchor-less generator: each turned Italian on the second conversion.
    [Theory]
    [InlineData(" [Am] [C]        La\ncasa [Do]\n\nsol     re   fa sol\nCantate [mi] Re Go\n\ncielo Signore\n")]
    [InlineData("mi    re   mi la\nVieni [Fa] Re la\n\n    [D7]\tDo\ncanta... sing Vieni .now [La] come")]
    [InlineData(" [Em]   Lam\t[E]\n-Vieni -Vieni    sing Vieni now home\n\nla     mi   sol\ngloria [Do] to [E]\n[Re] come    Vieni E [e]")]
    public void GeneratedDocumentsWithReplacedBrackets_AreStable(string input)
    {
        var first = ChordSheetConverter.Convert(input);
        var second = ChordSheetConverter.Convert(first.ChordPro);

        Assert.Equal(first.ChordPro, second.ChordPro);
        Assert.Equal(first.DetectedNotation, second.DetectedNotation);
    }
}
