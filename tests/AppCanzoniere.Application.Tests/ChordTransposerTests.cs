using AppCanzoniere.Application.Services;
using Xunit;

namespace AppCanzoniere.Application.Tests;

public class ChordTransposerTests
{
    private readonly ChordTransposer _transposer;

    public ChordTransposerTests()
    {
        _transposer = new ChordTransposer();
    }

    [Theory]
    [InlineData("C", 0, "C")]
    [InlineData("C", 1, "C#")]
    [InlineData("C", 2, "D")]
    [InlineData("C", 5, "F")]
    [InlineData("C", 12, "C")]
    [InlineData("C", -1, "B")]
    [InlineData("C", -2, "A#")]
    public void TransposeChord_SimpleChord_ReturnsCorrectResult(string chord, int steps, string expected)
    {
        // Act
        var result = _transposer.TransposeChord(chord, steps);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Cm", 0, "Cm")]
    [InlineData("Dm", 2, "Em")]
    [InlineData("Am", 3, "Cm")]
    [InlineData("G7", 2, "A7")]
    [InlineData("Dmaj7", 3, "Fmaj7")]
    [InlineData("Csus4", 5, "Fsus4")]
    public void TransposeChord_ComplexChord_PreservesQuality(string chord, int steps, string expected)
    {
        // Act
        var result = _transposer.TransposeChord(chord, steps);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("C/G", 0, "C/G")]
    [InlineData("D/F#", 2, "E/G#")]
    [InlineData("Am/E", 3, "Cm/G")]
    public void TransposeChord_ChordWithBass_TransposesBothNotes(string chord, int steps, string expected)
    {
        // Act
        var result = _transposer.TransposeChord(chord, steps);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("C", 1, true, "Db")]
    [InlineData("C", 3, true, "Eb")]
    [InlineData("G", 1, true, "Ab")]
    [InlineData("A", 1, true, "Bb")]
    public void TransposeChord_WithFlatsPreference_UsesFlats(string chord, int steps, bool useFlats, string expected)
    {
        // Act
        var result = _transposer.TransposeChord(chord, steps, useFlats);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("C", 0, "C")]
    [InlineData("C", 2, "D")]
    [InlineData("Am", 3, "Cm")]
    [InlineData("G", 5, "C")]
    public void TransposeKey_ValidKey_ReturnsCorrectKey(string key, int steps, string expected)
    {
        // Act
        var result = _transposer.TransposeKey(key, steps);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(12, 0)]
    [InlineData(13, 1)]
    [InlineData(-1, 11)]
    [InlineData(-12, 0)]
    [InlineData(-13, 11)]
    [InlineData(24, 0)]
    public void NormalizeSteps_VariousSteps_ReturnsNormalizedValue(int steps, int expected)
    {
        // Act
        var result = _transposer.NormalizeSteps(steps);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("C#", 1, "D")]
    [InlineData("Db", 1, "D")]
    [InlineData("F#", 1, "G")]
    [InlineData("Bb", 1, "B")]
    public void TransposeChord_HandlesEnharmonicEquivalents(string chord, int steps, string expected)
    {
        // Act
        var result = _transposer.TransposeChord(chord, steps);

        // Assert
        Assert.Equal(expected, result);
    }
}
