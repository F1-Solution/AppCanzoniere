using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.Music;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Single bare-note lines are weak evidence: they never outvote real chord lines when the notation is detected.
public class SingleNoteDetectionTests
{
    [Fact]
    public void SingleNoteLinesOfTheOtherNotation_DoNotChangeTheDetectedNotation()
    {
        var result = Run("Re      Sol", "Santo Signore", "E", "A");

        Assert.Equal(Notation.Italian, result.DetectedNotation);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.StartsWith("[Re]Santo", result.ChordPro);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public void BareNoteLinesOfTheOtherNotation_AreFlaggedInsteadOfPassingSilently(int lineNumber)
    {
        var result = Run("Re      Sol", "Santo Signore", "E", "A");
        var line = result.Diag(lineNumber);

        Assert.Equal(SheetLineKind.Lyric, line.Kind);
        Assert.Contains(SheetWarning.OtherNotation, line.Warnings);
        Assert.True(line.Confidence <= 0.6);
    }

    [Fact]
    public void SingleNoteLines_AreEnoughWhenTheyAreTheOnlyEvidence()
    {
        var result = Run("Santo", "E", "Signore");

        Assert.Equal(Notation.English, result.DetectedNotation);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(2).Kind);
    }

    [Fact]
    public void RealChordLines_StillDecideTheNotationWhenSingleNotesDisagree()
    {
        var result = Run("C   G", "Lord hear", "Re", "Sol", "Mi");

        Assert.Equal(Notation.English, result.DetectedNotation);
    }
}
