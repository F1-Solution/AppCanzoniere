using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Auto spelling writes passing diminished chords with sharps and a chromatic bass a semitone below the root as that lower letter.</summary>
public class DiminishedSpellingTests
{
    private readonly ChordTransposer _transposer = new();
    private readonly ChordProParser _parser = new();

    private string Auto(string chord, string key, int steps = 0) =>
        _transposer.TransposeChord(chord, steps, AccidentalPreference.Auto, key);

    [Theory]
    [InlineData("C#dim7", "C")]
    [InlineData("D#dim7", "C")]
    [InlineData("F#dim7", "C")]
    [InlineData("G#dim7", "C")]
    [InlineData("A#dim7", "C")]
    [InlineData("G#dim7", "G")]
    [InlineData("A#dim7", "G")]
    [InlineData("D#dim7", "G")]
    [InlineData("D#dim7", "D")]
    [InlineData("A#dim7", "D")]
    [InlineData("A#dim7", "A")]
    [InlineData("F#dim7", "F")]
    [InlineData("G#dim7", "F")]
    [InlineData("C#dim7", "F")]
    [InlineData("C#dim7", "Bb")]
    [InlineData("F#dim7", "Bb")]
    [InlineData("C#dim", "C")]
    [InlineData("C#°", "C")]
    [InlineData("C#°7", "C")]
    [InlineData("C#o", "C")]
    [InlineData("C#o7", "C")]
    [InlineData("C#m7b5", "C")]
    [InlineData("C#ø", "C")]
    public void ChromaticDiminishedRoots_AreSpelledWithASharp(string chord, string key)
    {
        Assert.Equal(chord, Auto(chord, key));
    }

    [Theory]
    [InlineData("Dbdim7", "C", "C#dim7")]
    [InlineData("Ebdim7", "C", "D#dim7")]
    [InlineData("Bbdim", "C", "A#dim")]
    [InlineData("Abdim7", "G", "G#dim7")]
    [InlineData("Bbm7b5", "C", "A#m7b5")]
    public void FlatWrittenDiminishedRoots_ComeOutWithASharp(string chord, string key, string expected)
    {
        Assert.Equal(expected, Auto(chord, key));
    }

    [Theory]
    [InlineData("Bdim", "C")]
    [InlineData("Adim7", "C")]
    [InlineData("Bbdim", "F")]
    [InlineData("Ebdim", "Bb")]
    public void DiatonicDiminishedRoots_KeepTheKeySpelling(string chord, string key)
    {
        Assert.Equal(chord, Auto(chord, key));
    }

    [Theory]
    [InlineData("Ab", "C")]
    [InlineData("Bb", "C")]
    [InlineData("Eb", "C")]
    [InlineData("Bbm", "C")]
    [InlineData("Ebm", "C")]
    [InlineData("Db", "C")]
    [InlineData("Gb", "F")]
    public void BorrowedMajorAndMinorChords_StayFlats(string chord, string key)
    {
        Assert.Equal(chord, Auto(chord, key));
    }

    [Theory]
    [InlineData("C#dim7", 5, "C", "F#dim7")]
    [InlineData("D#dim7", 5, "C", "G#dim7")]
    [InlineData("G#dim7", 5, "C", "C#dim7")]
    [InlineData("A#dim7", 5, "C", "D#dim7")]
    [InlineData("C#dim7", 7, "C", "G#dim7")]
    [InlineData("D#dim7", 7, "C", "A#dim7")]
    [InlineData("G#dim7", 7, "C", "D#dim7")]
    [InlineData("C#dim7", 2, "C", "D#dim7")]
    [InlineData("G#dim7", 2, "C", "A#dim7")]
    [InlineData("C#dim7", 9, "C", "A#dim7")]
    [InlineData("F#dim7", 9, "C", "D#dim7")]
    [InlineData("D#dim7", 10, "C", "C#dim7")]
    [InlineData("G#dim7", 10, "C", "F#dim7")]
    public void ShiftedDiminishedChords_AreSpelledWithASharpInTheTargetKey(string chord, int steps, string key, string expected)
    {
        Assert.Equal(expected, Auto(chord, key, steps));
    }

    [Fact]
    public void Render_ShiftedSongWithPassingDiminishedChords_SpellsThemWithSharps()
    {
        var song = "{key:C}\n[C]a [C#dim7]b [Dm]c [D#dim7]d [Em]e [F#dim7]f [G]g [G#dim7]h [Am]i";
        var steps = new (int Steps, string Key, string Expected)[]
        {
            (5, "F", "[F]a [F#dim7]b [Gm]c [G#dim7]d [Am]e [Bdim7]f [C]g [C#dim7]h [Dm]i"),
            (7, "G", "[G]a [G#dim7]b [Am]c [A#dim7]d [Bm]e [C#dim7]f [D]g [D#dim7]h [Em]i"),
            (10, "Bb", "[Bb]a [Bdim7]b [Cm]c [C#dim7]d [Dm]e [Edim7]f [F]g [F#dim7]h [Gm]i")
        };

        foreach (var (shift, key, expected) in steps)
        {
            var result = _parser.Render(_parser.Parse(song), new ChordProRenderOptions { TransposeSteps = shift, Accidentals = AccidentalPreference.Auto });
            Assert.Equal($"{{key:{key}}}\n{expected}\n", result);
        }
    }

    [Theory]
    [InlineData("Am/G#", "C")]
    [InlineData("Dm/C#", "C")]
    [InlineData("Em/D#", "C")]
    [InlineData("Bm/A#", "C")]
    [InlineData("Gm/F#", "C")]
    [InlineData("Dm/C#", "F")]
    [InlineData("Gm/F#", "F")]
    [InlineData("Gm/F#", "Bb")]
    [InlineData("Am/Ab", "C")]
    public void ABassASemitoneBelowTheRoot_IsTheLetterBelowTheRoot(string chord, string key)
    {
        var expected = chord == "Am/Ab" ? "Am/G#" : chord;
        Assert.Equal(expected, Auto(chord, key));
    }

    [Theory]
    [InlineData("Am/G#", 2, "C", "Bm/A#")]
    [InlineData("Dm/C#", 5, "C", "Gm/F#")]
    [InlineData("Am/G#", 5, "C", "Dm/C#")]
    public void ShiftedBassASemitoneBelowTheRoot_IsTheLetterBelowTheRoot(string chord, int steps, string key, string expected)
    {
        Assert.Equal(expected, Auto(chord, key, steps));
    }

    [Theory]
    [InlineData("F#/F", "C", "F#/F")]
    [InlineData("C/B", "C", "C/B")]
    [InlineData("F/E", "C", "F/E")]
    public void ABassThatWouldNeedAnAwkwardNote_FallsBackToTheKeyRule(string chord, string key, string expected)
    {
        Assert.Equal(expected, Auto(chord, key));
    }

    [Theory]
    [InlineData("Ab/C", "Db")]
    [InlineData("Eb/G", "Db")]
    [InlineData("Bb/D", "Eb")]
    [InlineData("Db/F", "Db")]
    [InlineData("Gb/Bb", "Db")]
    public void DiatonicChordsInAFlatKey_KeepTheirRoots(string chord, string key)
    {
        Assert.Equal(chord, Auto(chord, key));
    }
}
