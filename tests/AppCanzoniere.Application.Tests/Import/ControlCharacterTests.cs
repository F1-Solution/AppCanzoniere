using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Form feed (pdftotext page breaks) and vertical tab are removed before columns are counted.
public class ControlCharacterTests
{
    private readonly Xunit.Abstractions.ITestOutputHelper _output;

    public ControlCharacterTests(Xunit.Abstractions.ITestOutputHelper output) => _output = output;

    [Fact]
    public void FormFeedInsideALyric_DoesNotShiftColumnsOrSurvive()
    {
        var result = Run("Do Sol", "Ti\u000C lodo");

        Assert.Equal(Out("[Do]Ti [Sol]lodo"), result.ChordPro);
    }

    [Fact]
    public void FormFeedAndVerticalTabInsideALine_AreRemoved() =>
        Assert.Equal(Out("Santo Signore"), Run("Sa\u000Cnto Sign\u000Bore").ChordPro);

    [Fact]
    public void FormFeedAloneOnALine_IsABlankLine() =>
        Assert.Equal(Out("Santo", "Dio", "", "Pace"), Run("Santo", "Dio", "\u000C", "Pace").ChordPro);

    [Fact]
    public void FormFeedInAChordLine_DoesNotShiftTheChords() =>
        Assert.Equal(Out("[Do]Ti [Sol]lodo"), Run("\u000CDo Sol", "Ti lodo").ChordPro);

    [Fact]
    public void RandomDocumentsWithControlCharacters_KeepTheirLyrics()
    {
        DocumentProperties.CheckFaithful(RandomSheets.Many(13, 3000, SheetFeatures.All), _output, 1000, result =>
        {
            Assert.DoesNotContain('\u000C', result.ChordPro);
            Assert.DoesNotContain('\u000B', result.ChordPro);
        });
    }
}
