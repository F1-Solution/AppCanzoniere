using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class ChordSuffixTests
{
    private readonly ChordTransposer _transposer = new();

    [Theory]
    [InlineData("Bo7", 2, "C#o7")]
    [InlineData("Bo", 1, "Co")]
    [InlineData("B°", 2, "C#°")]
    [InlineData("B°7", 2, "C#°7")]
    [InlineData("Bø", 2, "C#ø")]
    [InlineData("Bø7", 2, "C#ø7")]
    [InlineData("C^", 2, "D^")]
    [InlineData("C^7", 2, "D^7")]
    [InlineData("CΔ7", 2, "DΔ7")]
    [InlineData("C△7", 2, "D△7")]
    [InlineData("C7omit3", 2, "D7omit3")]
    [InlineData("Cmaj7omit3", 2, "Dmaj7omit3")]
    [InlineData("Cm/maj7", 2, "Dm/maj7")]
    [InlineData("Cm/Maj7", 2, "Dm/Maj7")]
    [InlineData("C/maj7", 2, "D/maj7")]
    [InlineData("Cm/add9", 2, "Dm/add9")]
    [InlineData("C/Bb", 2, "D/C")]
    [InlineData("C/9", 2, "D/9")]
    public void ChordsThatWereTransposedBefore_AreStillTransposed(string chord, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Sharps));
    }

    [Theory]
    [InlineData("Bridge")]
    [InlineData("Chorus")]
    [InlineData("Dog")]
    [InlineData("Add9")]
    [InlineData("Intro")]
    [InlineData("Verse")]
    [InlineData("Cm/Bridge")]
    [InlineData("C/Chorus")]
    public void WordsThatAreNotChords_AreStillRejected(string text)
    {
        Assert.Equal(text, _transposer.TransposeChord(text, 2, AccidentalPreference.Sharps));
        Assert.False(ChordSymbolParser.TryParse(text, Notation.English, out _));
    }
}
