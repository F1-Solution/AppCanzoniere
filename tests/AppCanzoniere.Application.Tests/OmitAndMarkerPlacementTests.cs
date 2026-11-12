using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>"omit" is not a diminished marker, D can carry "omit", and the bare "o" only follows the root.</summary>
public class OmitAndMarkerPlacementTests
{
    private readonly ChordTransposer _transposer = new();

    private static bool IsChord(string text) => ChordSymbolParser.TryParse(text, Notation.English, out _);

    [Theory]
    [InlineData("Bbomit3", 0)]
    [InlineData("Abomit5", 0)]
    [InlineData("Dbomit3", 0)]
    [InlineData("Bbomit3", 12)]
    [InlineData("Abomit5", 12)]
    [InlineData("Dbomit3", 12)]
    public void Omit_IsNotDiminished_AndKeepsTheKeySpelling(string chord, int steps)
    {
        Assert.Equal(chord, _transposer.TransposeChord(chord, steps, AccidentalPreference.Auto, "C"));
    }

    [Fact]
    public void Omit_ShiftedInAuto_UsesTheKeySpellingOfTheRoot()
    {
        Assert.Equal("Eomit3", _transposer.TransposeChord("Domit3", 2, AccidentalPreference.Auto, "C"));
        Assert.Equal("Abomit3", _transposer.TransposeChord("Gomit3", 1, AccidentalPreference.Auto, "C"));
    }

    [Theory]
    [InlineData("Domit3")]
    [InlineData("Domit5")]
    public void English_DOmit_IsAChord(string text)
    {
        Assert.True(IsChord(text));
        Assert.Equal("Eomit" + text[^1], _transposer.TransposeChord(text, 2, false));
    }

    [Theory]
    [InlineData("Do")]
    [InlineData("Dom")]
    [InlineData("Do7")]
    [InlineData("Domaj7")]
    [InlineData("Do/Mi")]
    [InlineData("Do#m")]
    public void English_ItalianDoReadings_AreStillRejected(string text)
    {
        Assert.False(IsChord(text));
    }

    [Theory]
    [InlineData("Amo")]
    [InlineData("Emo")]
    [InlineData("Bmo7")]
    [InlineData("Cmo/E")]
    public void English_BareOAfterTheMinorMarker_IsNotAChordAndStaysUnchanged(string text)
    {
        Assert.False(IsChord(text));
        Assert.Equal(text, _transposer.TransposeChord(text, 2, false));
    }

    [Theory]
    [InlineData("Ao")]
    [InlineData("Bo7")]
    [InlineData("F#o7")]
    [InlineData("Bbo7/D")]
    public void English_BareOAfterTheRoot_IsStillAChord(string text)
    {
        Assert.True(IsChord(text));
    }
}
