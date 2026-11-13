using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Small findings of the second review: numbered first verse, tab width, diagnostic source text and control characters.
public class ReviewLowFindingsTests
{
    private static string Ch(int codePoint) => ((char)codePoint).ToString();

    [Fact]
    public void NumberedCapsFirstLine_FollowedByChordsOrLyrics_IsAVerseLabel()
    {
        var result = Run("1. SANTO SANTO", "Do Sol", "Ti lodiamo");

        Assert.Equal(SheetLineKind.SectionLabel, result.Diag(1).Kind);
        Assert.DoesNotContain("{title", result.ChordPro);
    }

    [Fact]
    public void NumberedCapsFirstLine_FollowedByABlankLine_IsStillATitle() =>
        Assert.Equal(SheetLineKind.Title, Run("39. INSIEME", "", "Do Sol", "Ti lodiamo").Diag(1).Kind);

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    [InlineData(17)]
    [InlineData(int.MaxValue)]
    public void TabWidthOutsideOneToSixteen_IsRejected(int width) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => RunWith(new ChordSheetOptions { TabWidth = width }, "Do\tSol", "Ti lodo"));

    [Theory]
    [InlineData(1)]
    [InlineData(16)]
    public void TabWidthBoundaries_AreAccepted(int width) =>
        Assert.False(RunWith(new ChordSheetOptions { TabWidth = width }, "Do\tSol", "Ti lodo").WasChordPro);

    [Fact]
    public void DiagnosticSource_IsTheLineExactlyAsPasted()
    {
        var pasted = "Perche" + Ch(0x301) + " siamo";

        var result = Run("Do      Sol", pasted);

        Assert.Equal(pasted, result.Diag(2).Source);
        Assert.Equal(Out("[Do]Perché s[Sol]iamo"), result.ChordPro);
    }

    [Fact]
    public void NulBellEscapeAndOtherControls_AreRemoved()
    {
        var line = "Sa" + Ch(0) + "nto" + Ch(7) + " Di" + Ch(0x1B) + "o" + Ch(0x9B) + Ch(0x7F);

        Assert.Equal(Out("Santo Dio"), Run(line).ChordPro);
    }

    [Theory]
    [InlineData(0x85)]
    [InlineData(0x2028)]
    [InlineData(0x2029)]
    public void NextLineAndSeparators_AreLineBreaks(int codePoint)
    {
        var result = Run("Santo" + Ch(codePoint) + "Dio");

        Assert.Equal(2, result.Lines.Count);
        Assert.Equal(Out("Santo", "Dio"), result.ChordPro);
    }

    [Fact]
    public void ConvertedOutput_NeverContainsAControlCharacterOtherThanTheNewline()
    {
        var random = new Random(31);
        var checkedDocuments = 0;
        foreach (var sheet in RandomSheets.Many(5, 3000, SheetFeatures.All))
        {
            var text = Inject(sheet.Text, random);
            var result = ChordSheetConverter.Convert(text);
            if (result.WasChordPro)
                continue;

            checkedDocuments++;
            Assert.True(result.ChordPro.All(c => c == '\n' || c >= ' ' && !char.IsControl(c)), "A control character reached the output.");
        }

        Assert.True(checkedDocuments > 1000, $"Only {checkedDocuments} documents were converted.");
    }

    private static string Inject(string text, Random random)
    {
        var builder = new System.Text.StringBuilder(text);
        for (var i = 0; i < 6; i++)
        {
            var code = random.Next(3) == 0 ? random.Next(0x7F, 0xA0) : random.Next(0, 0x20);
            if (code is '\n' or '\r' or '\t')
                continue;

            builder.Insert(random.Next(builder.Length + 1), (char)code);
        }

        return builder.ToString();
    }
}
