using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Form feed (pdftotext page breaks) and vertical tab are removed before columns are counted.
public class ControlCharacterTests
{
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
        var documents = 0;
        foreach (var sheet in RandomSheets.Many(13, 3000, SheetFeatures.All))
        {
            var result = ChordSheetConverter.Convert(sheet.Text);
            if (result.WasChordPro)
                continue;

            documents++;
            BracketSafetyTests.AssertParsedFaithfully(sheet, result.ChordPro);
            Assert.DoesNotContain('\u000C', result.ChordPro);
            Assert.DoesNotContain('\u000B', result.ChordPro);
        }

        Assert.True(documents > 1000, $"Only {documents} documents were converted.");
    }
}
