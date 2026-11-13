using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// CarriesChords tells chord lines and labels with chords of their own from lyrics that merely received chords.
public class ChordCarrierTests
{
    [Fact]
    public void ChordLine_CarriesChords_AndItsLyricDoesNot()
    {
        var result = Run("Do       Sol", "Signore, ascolta");

        Assert.True(result.Diag(1).CarriesChords);
        Assert.False(result.Diag(2).CarriesChords);
    }

    [Fact]
    public void LabelWithChordsOfItsOwn_CarriesChords()
    {
        var result = Run("Intro: Do Sol Re Mi", "", "Santo");

        Assert.True(result.Diag(1).CarriesChords);
        Assert.False(result.Diag(3).CarriesChords);
    }

    [Fact]
    public void LabelWithLyricThatReceivedChords_DoesNotCarryChords()
    {
        var result = Run("   Do       Sol", "1. Signore, ascolta");

        Assert.True(result.Diag(1).CarriesChords);
        Assert.False(result.Diag(2).CarriesChords);
    }

    [Fact]
    public void LyricAndBlankLines_DoNotCarryChords()
    {
        var result = Run("Santo", "", "Dio");

        Assert.All(result.Lines, l => Assert.False(l.CarriesChords));
    }
}
