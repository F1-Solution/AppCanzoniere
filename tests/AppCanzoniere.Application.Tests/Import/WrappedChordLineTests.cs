using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Chord lines written with parentheses or brackets are still chord lines.
public class WrappedChordLineTests
{
    [Fact]
    public void ParenthesisedAlternatives_AreMergedIntoTheLyricBelow()
    {
        var result = Run("Do   (Lam)  Sol  (Re)", "Santo ascolta il Signore");

        Assert.Equal(Out("[Do]Santo [Lam]ascolt[Sol]a il [Re]Signore"), result.ChordPro);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Contains(SheetWarning.ParenthesesRemoved, result.Diag(1).Warnings);
    }

    [Fact]
    public void BracketedChordsAlone_AreAnInstrumentalChordLine()
    {
        var result = Run("[B]  [G#m]");

        Assert.Equal(Out("[B] [G#m]"), result.ChordPro);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Equal(1, result.ChordLineCount);
        Assert.Equal(Music.Notation.English, result.DetectedNotation);
    }

    [Fact]
    public void BracketedChordsAboveALyric_AreMerged() =>
        Assert.Equal(Out("[Re]Pane [La]del cielo"), Run("[Re]   [La]", "Pane del cielo").ChordPro);

    [Fact]
    public void ChordLineAboveALyricWithInlineChords_IsKeptAlone()
    {
        var result = Run("Fa            Do", "(Re)Pane del (La)cielo");

        Assert.Equal(Out("[Fa] [Do]", "[Re]Pane del [La]cielo"), result.ChordPro);
        Assert.Contains(SheetWarning.ChordLineKeptAlone, result.Diag(1).Warnings);
        Assert.Equal(SheetLineKind.InlineChordLyric, result.Diag(2).Kind);
    }

    [Fact]
    public void LyricWithSomeInlineParenthesisedChords_StaysALyric()
    {
        var result = Run("Santo", "Ti lodo (Lam) Signore (Re)mio");

        Assert.Equal(Out("Santo", "Ti lodo [Lam] Signore [Re]mio"), result.ChordPro);
        Assert.Equal(SheetLineKind.InlineChordLyric, result.Diag(2).Kind);
    }

    [Fact]
    public void RandomDocumentsWithWrappedChords_KeepTheirLyricsAndNeverInventChords()
    {
        var documents = 0;
        foreach (var sheet in RandomSheets.Many(7, 3000, SheetFeatures.Brackets | SheetFeatures.WrappedChords))
        {
            var result = ChordSheetConverter.Convert(sheet.Text);
            if (result.WasChordPro)
                continue;

            documents++;
            BracketSafetyTests.AssertParsedFaithfully(sheet, result.ChordPro);
        }

        Assert.True(documents > 2000);
    }
}
