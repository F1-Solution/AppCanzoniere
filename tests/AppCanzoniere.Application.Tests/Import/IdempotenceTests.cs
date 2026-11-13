using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Converting the converter's own output returns it byte for byte.
public class IdempotenceTests
{
    [Fact]
    public void NoChordMarkLine_IsWrittenAsAnAnnotation() =>
        Assert.Equal(Out("[*N.C.]"), Run("N.C.").ChordPro);

    [Fact]
    public void NeutralOnlyChordLine_IsWrittenAsAnnotations() =>
        Assert.Equal(Out("[*N.C.] [*x2]"), Run("N.C.   x2").ChordPro);

    [Fact]
    public void ChordLinesKeepTheirNeutralMarksAsWritten() =>
        Assert.Equal(Out("| [Do] | [Fa] | x2"), Run("| Do | Fa | x2").ChordPro);

    [Theory]
    [InlineData("N.C.", "Do       Sol", "Signore, ascolta")]
    [InlineData("N.C.", "Taci")]
    [InlineData("Do  Sol", "Taci, poi canta", "", "Fa  Re", "Vieni")]
    [InlineData("Do  Sol", "Amen", "Fa")]
    public void OutputWithoutDirectives_IsStable(params string[] input)
    {
        var first = Run(input).ChordPro;

        Assert.Equal(first, ChordSheetConverter.Convert(first).ChordPro);
    }

    [Fact]
    public void RandomDocuments_AreStableWhenConvertedAgain()
    {
        var checkedDocuments = 0;
        foreach (var sheet in RandomSheets.Many(99, 3000, SheetFeatures.All))
        {
            var first = ChordSheetConverter.Convert(sheet.Text).ChordPro;
            var second = ChordSheetConverter.Convert(first).ChordPro;
            Assert.True(first == second, $"Unstable.\nInput: {sheet.Text.Replace("\n", "\n\n")}\nFirst: {first}\nSecond: {second}");
            checkedDocuments++;
        }

        Assert.Equal(3000, checkedDocuments);
    }
}
