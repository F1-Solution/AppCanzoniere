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

    private const int NoisyDocuments = 40000;
    private const int ReportedFailures = 4;
    private readonly Xunit.Abstractions.ITestOutputHelper _output;

    public IdempotenceTests(Xunit.Abstractions.ITestOutputHelper output) => _output = output;

    [Fact]
    public void NoisyDocuments_AreStableWhenConvertedAgain()
    {
        var failures = new List<string>();
        var unstable = 0;
        foreach (var text in NoisySheets.Many(20260101, NoisyDocuments))
        {
            var first = ChordSheetConverter.Convert(text).ChordPro;
            var second = ChordSheetConverter.Convert(first).ChordPro;
            if (first == second)
                continue;

            unstable++;
            if (failures.Count < ReportedFailures)
                failures.Add($"INPUT:\n{text.Replace("\r", "\\r")}\nFIRST:\n{first}\nSECOND:\n{second}");
        }

        _output.WriteLine($"{unstable} of {NoisyDocuments} noisy documents changed on a second conversion.");
        Assert.True(unstable == 0, $"{unstable} unstable.\n" + string.Join("\n=====\n", failures));
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
