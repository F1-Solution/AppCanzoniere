using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class ChordCaseTests
{
    private readonly ChordTransposer _transposer = new();

    private static string EnToIt(string chord) =>
        ChordNotation.Convert(chord, NotationOptions.English, NotationOptions.Italian);

    private static string ItToEn(string chord) =>
        ChordNotation.Convert(chord, NotationOptions.Italian, NotationOptions.English);

    [Theory]
    [InlineData("CM7", "DoM7")]
    [InlineData("CM", "DoM")]
    [InlineData("CM9", "DoM9")]
    [InlineData("EM7", "MiM7")]
    [InlineData("CMaj7", "DoMaj7")]
    public void English_UpperCaseM_StaysMajorWhenConverted(string english, string italian)
    {
        Assert.Equal(italian, EnToIt(english));
        Assert.Equal(english, ItToEn(italian));
    }

    [Theory]
    [InlineData("Cm7", "Dom7")]
    [InlineData("Cmin7", "Dom7")]
    [InlineData("CMIN7", "Dom7")]
    [InlineData("CMin7", "Dom7")]
    [InlineData("CMi7", "Dom7")]
    [InlineData("Cmi7", "Dom7")]
    public void English_MinorForms_AreStillReadAsMinor(string english, string italian)
    {
        Assert.Equal(italian, EnToIt(english));
    }

    [Theory]
    [InlineData("CM7", 1, AccidentalPreference.Auto, "DbM7")]
    [InlineData("CM7", 2, AccidentalPreference.Sharps, "DM7")]
    [InlineData("CM", 1, AccidentalPreference.Flats, "DbM")]
    [InlineData("EM7", 1, AccidentalPreference.Auto, "FM7")]
    [InlineData("Cm7", 1, AccidentalPreference.Auto, "C#m7")]
    public void Transposition_KeepsUpperCaseMajorAsTyped(string chord, int steps, AccidentalPreference preference, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, preference));
    }

    // In Italian an all-caps root followed by a lone "M" is ambiguous; it is read as major, the same as in English.
    [Theory]
    [InlineData("LAM", "AM")]
    [InlineData("MIM", "EM")]
    [InlineData("LAM7", "AM7")]
    [InlineData("LAm", "Am")]
    [InlineData("MIm", "Em")]
    [InlineData("Lam", "Am")]
    public void Italian_AllCapsRootWithUpperCaseM_IsMajor(string italian, string english)
    {
        Assert.Equal(english, ItToEn(italian));
    }
}
