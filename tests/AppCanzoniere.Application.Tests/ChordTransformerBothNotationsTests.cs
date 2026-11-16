using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Tests;

public class ChordTransformerBothNotationsTests
{
    private static ChordTransformer Make(bool both, NotationOptions input, NotationOptions output, int steps = 2) =>
        new(new ChordProRenderOptions { TransposeSteps = steps, InputNotation = input, OutputNotation = output, ReadBothNotations = both }, null, 0);

    [Fact]
    public void ByDefault_AChordOfTheOtherNotationIsLeftAlone()
    {
        Assert.Equal("C", Make(false, NotationOptions.Italian, NotationOptions.Italian).Chord("C"));
    }

    [Fact]
    public void WhenAsked_AChordOfTheOtherNotationIsTransposedAndWrittenInTheOutput()
    {
        var transformer = Make(true, NotationOptions.Italian, NotationOptions.Italian);

        Assert.Equal("Re", transformer.Chord("C"));
        Assert.Equal("Sim", transformer.Chord("Am"));
    }

    [Fact]
    public void WhenAsked_TheInputNotationStillReadsFirst()
    {
        Assert.Equal("D", Make(true, NotationOptions.English, NotationOptions.English).Chord("C"));
    }

    [Fact]
    public void WhenAsked_ATextThatIsNoChordInEitherNotationIsReturnedUnchanged()
    {
        Assert.Equal("Ritornello", Make(true, NotationOptions.English, NotationOptions.English).Chord("Ritornello"));
    }

    [Fact]
    public void WhenAsked_TheKeyFollowsEvenIfMostChordsAreOfTheOtherNotation()
    {
        var options = new ChordProRenderOptions { TransposeSteps = 2, InputNotation = NotationOptions.English, OutputNotation = NotationOptions.English, ReadBothNotations = true };
        var transformer = new ChordTransformer(options, "G", 0, new[] { "Sol", "Do", "Re" });

        Assert.Equal("A", transformer.Key("G"));
    }
}
