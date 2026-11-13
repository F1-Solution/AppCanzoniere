using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>An altered tension right after a natural root is written in parentheses so it is not read as the root's accidental.</summary>
public class ChordAlteredTensionTests
{
    private readonly ChordTransposer _transposer = new();

    [Theory]
    [InlineData("Bbb5", 2, "C(b5)")]
    [InlineData("F##5", 1, "G(#5)")]
    [InlineData("C#b5", 1, "D(b5)")]
    [InlineData("Bbb9", 2, "C(b9)")]
    [InlineData("Ebb13/G", 2, "F(b13)/A")]
    public void NaturalRootBeforeAnAlteredTension_GetsParentheses(string chord, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Sharps));
    }

    [Theory]
    [InlineData("B♭♭5", 1, "B(♭5)")]
    [InlineData("B♭♭5", 2, "C(♭5)")]
    [InlineData("F♯♯5", 1, "G(♯5)")]
    [InlineData("C♯♭5", 1, "D(♭5)")]
    [InlineData("E♭♭9/G", 2, "F(♭9)/A")]
    public void UnicodeAccidentalTension_GetsParenthesesLikeTheAsciiOne(string chord, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Sharps));
        Assert.True(ChordSymbolParser.TryParse(expected, Notation.English, out var parsed));
        Assert.Equal(0, parsed.Root.Accidental);
    }

    [Theory]
    [InlineData("Bb7b9", 2, "C7b9")]
    [InlineData("Cm7b5", 2, "Dm7b5")]
    [InlineData("C7#9", 2, "D7#9")]
    [InlineData("Bbmb5", 2, "Cmb5")]
    [InlineData("Ab5", 2, "Bb5")]
    public void OtherSuffixes_AreWrittenAsBefore(string chord, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Flats));
    }

    [Fact]
    public void ParenthesisedTension_ReadsBackAsTheSameChordAndKeepsItsParentheses()
    {
        Assert.True(ChordSymbolParser.TryParse("C(b5)", Notation.English, out var chord));
        Assert.Equal(new Note('C', 0), chord.Root);
        Assert.Equal("(b5)", chord.Suffix);
        Assert.Equal("C#(b5)", _transposer.TransposeChord("C(b5)", 1, AccidentalPreference.Sharps));
        Assert.Equal("Db(b5)", _transposer.TransposeChord("C(b5)", 1, AccidentalPreference.Flats));
        Assert.Equal("D(b5)", _transposer.TransposeChord("C(b5)", 2, AccidentalPreference.Sharps));
    }

    [Fact]
    public void ItalianNotation_AlsoWrapsTheTension()
    {
        var options = new ChordProRenderOptions { TransposeSteps = 2, OutputNotation = NotationOptions.Italian };
        var parser = new ChordProParser();

        Assert.Equal("[Do(b5)]x\n", parser.Render(parser.Parse("[Bbb5]x"), options with { Accidentals = AccidentalPreference.Flats }));
    }
}
