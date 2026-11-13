using System.Text;
using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Columns count text elements: a chord never lands inside a surrogate pair or before a combining accent.
public class TextElementTests
{
    [Fact]
    public void Emoji_IsOneColumn_AndNeverSplit()
    {
        var result = Run("Do Sol", "Ti🙏amo");

        Assert.Equal(Out("[Do]Ti🙏[Sol]amo"), result.ChordPro);
        Assert.Equal("[Do]Ti🙏[Sol]amo\n", Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(result.ChordPro)));
    }

    [Fact]
    public void CombiningAccentWithoutPrecomposedForm_StaysWithItsLetter() =>
        Assert.Equal(Out("[Do]q́ui[Sol]et"), Run("Do Sol", "q́uiet").ChordPro);

    [Fact]
    public void DecomposedAccents_AreComposedAndDoNotShiftColumns()
    {
        var result = Run("Do      Sol", "Perché siamo");

        Assert.Equal(Out("[Do]Perché s[Sol]iamo"), result.ChordPro);
    }

    [Fact]
    public void ChordLineAfterEmojiLabelText_UsesTextElementColumns()
    {
        var result = Run("Do   Sol", "Ca🙏🙏 mío");

        Assert.Equal(Out("[Do]Ca🙏🙏 [Sol]mío"), result.ChordPro);
    }

    [Fact]
    public void RandomDocumentsWithEmojiAndCombiningMarks_KeepEveryCharacterIntact()
    {
        const SheetFeatures features = SheetFeatures.Brackets | SheetFeatures.WrappedChords | SheetFeatures.Emoji | SheetFeatures.Combining;
        var documents = 0;
        foreach (var sheet in RandomSheets.Many(11, 3000, features))
        {
            var result = ChordSheetConverter.Convert(sheet.Text);
            if (result.WasChordPro)
                continue;

            documents++;
            BracketSafetyTests.AssertParsedFaithfully(sheet, result.ChordPro);
        }

        Assert.True(documents > 1500, $"Only {documents} documents were converted.");
    }
}
