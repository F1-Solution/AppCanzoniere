using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>In Auto spelling a bass a semitone below the root takes the lower letter only when it is outside the shifted key's scale.</summary>
public class SlashBassDiatonicTests
{
    private readonly ChordTransposer _transposer = new();

    private string Auto(string chord, string key, int steps) =>
        _transposer.TransposeChord(chord, steps, AccidentalPreference.Auto, key);

    [Theory]
    [InlineData("C#dim7/C", "C", 3, "Edim7/Eb")]
    [InlineData("C#dim7/C", "C", 8, "Adim7/Ab")]
    [InlineData("C#dim7/C", "C", 10, "Bdim7/Bb")]
    [InlineData("C#dim7/C", "C", 1, "Ddim7/Db")]
    [InlineData("D#dim7/D", "C", 8, "Bdim7/Bb")]
    [InlineData("Db/C", "C", 10, "B/Bb")]
    [InlineData("Db/C", "C", 3, "E/Eb")]
    [InlineData("Db/C", "C", 8, "A/Ab")]
    [InlineData("Bb/A", "Am", 6, "E/Eb")]
    [InlineData("Bb/A", "Am", 1, "B/Bb")]
    [InlineData("F/E", "Am", 6, "B/Bb")]
    public void DiatonicBass_KeepsTheKeySpelling(string chord, string key, int steps, string expected)
    {
        Assert.Equal(expected, Auto(chord, key, steps));
    }

    [Theory]
    [InlineData("Am/G#", "C", 0)]
    [InlineData("Dm/C#", "C", 0)]
    [InlineData("Em/D#", "G", 0)]
    [InlineData("Am/G#", "G", 0)]
    [InlineData("Dm/C#", "F", 0)]
    [InlineData("Gm/F#", "F", 0)]
    [InlineData("Gm/F#", "Bb", 0)]
    [InlineData("B/Bb", "Bb", 0)]
    [InlineData("A/Ab", "Ab", 0)]
    public void ChromaticBassBelowTheRoot_StillTakesTheLowerLetter(string chord, string key, int steps)
    {
        Assert.Equal(chord, Auto(chord, key, steps));
    }

    [Theory]
    [InlineData("Am/G#", "C", 2, "Bm/A#")]
    [InlineData("Dm/C#", "C", 7, "Am/G#")]
    public void ChromaticBassBelowTheRoot_AfterAShift_StillTakesTheLowerLetter(string chord, string key, int steps, string expected)
    {
        Assert.Equal(expected, Auto(chord, key, steps));
    }
}
