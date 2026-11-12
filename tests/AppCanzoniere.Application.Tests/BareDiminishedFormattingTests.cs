using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>A chord with the bare "o" marker is written so that it reads back: with a degree sign in Italian and on D.</summary>
public class BareDiminishedFormattingTests
{
    private readonly ChordTransposer _transposer = new();
    private readonly ChordProParser _parser = new();

    [Theory]
    [InlineData("Bo7", "Si°7")]
    [InlineData("Co", "Do°")]
    [InlineData("Go/Bb", "Sol°/Sib")]
    [InlineData("Bbo7", "Sib°7")]
    public void English_ToItalian_WritesADegreeSign(string english, string italian)
    {
        Assert.Equal(italian, ChordNotation.Convert(english, NotationOptions.English, NotationOptions.Italian));
    }

    [Theory]
    [InlineData("Bo7", "B°7")]
    [InlineData("Co", "C°")]
    [InlineData("Go/Bb", "G°/Bb")]
    [InlineData("Bbo7", "Bb°7")]
    [InlineData("F#o7", "F#°7")]
    [InlineData("Ao", "A°")]
    public void RoundTrip_EnglishItalianEnglish_ReturnsTheChordWithADegreeSign(string english, string expected)
    {
        var italian = ChordNotation.Convert(english, NotationOptions.English, NotationOptions.Italian);

        Assert.Equal(expected, ChordNotation.Convert(italian, NotationOptions.Italian, NotationOptions.English));
    }

    [Theory]
    [InlineData("Bo7", 3, "D°7")]
    [InlineData("Co", 2, "D°")]
    [InlineData("Bo7", 2, "C#o7")]
    public void English_RootD_WritesADegreeSignAndOtherRootsKeepTheO(string chord, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, false));
    }

    [Fact]
    public void Render_ShiftedUpAndBack_GivesAChordOnTheSameRoot()
    {
        var up = _parser.Render(_parser.Parse("[Bo7]a"), 3);
        var back = _parser.Render(_parser.Parse(up), -3);

        Assert.Equal("[D°7]a\n", up);
        Assert.Equal("[B°7]a\n", back);
    }

    [Fact]
    public void Render_ItalianTextFromOChords_TransposesWhenReadBackAsItalian()
    {
        var italian = _parser.Render(_parser.Parse("[Bo7]a [Co]b"), new ChordProRenderOptions { OutputNotation = NotationOptions.Italian });
        var shifted = _parser.Render(_parser.Parse(italian), new ChordProRenderOptions
        {
            InputNotation = NotationOptions.Italian,
            OutputNotation = NotationOptions.Italian,
            TransposeSteps = 2
        });

        Assert.Equal("[Si°7]a [Do°]b\n", italian);
        Assert.Equal("[Do#°7]a [Re°]b\n", shifted);
    }
}
