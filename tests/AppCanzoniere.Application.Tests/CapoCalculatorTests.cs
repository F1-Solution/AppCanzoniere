using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class CapoCalculatorTests
{
    public static readonly string[] CanonicalMajors = { "C", "Db", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B" };
    public static readonly string[] CanonicalMinors = { "Cm", "C#m", "Dm", "Ebm", "Em", "Fm", "F#m", "Gm", "G#m", "Am", "Bbm", "Bm" };

    [Theory]
    [InlineData("G", 3, "E")]
    [InlineData("C", 1, "B")]
    [InlineData("Bb", 3, "G")]
    [InlineData("D", 2, "C")]
    [InlineData("E", 4, "C")]
    [InlineData("Eb", 3, "C")]
    [InlineData("Am", 2, "Gm")]
    [InlineData("Dm", 5, "Am")]
    [InlineData("F#m", 2, "Em")]
    [InlineData("Fm", 1, "Em")]
    [InlineData("C", 11, "Db")]
    public void ShapeKey_IsTheSoundingKeyLoweredByTheCapo(string key, int capo, string expected)
    {
        Assert.Equal(expected, CapoCalculator.ShapeKey(key, capo));
    }

    [Theory]
    [InlineData("E", 3, "G")]
    [InlineData("A", 3, "C")]
    [InlineData("Em", 5, "Am")]
    [InlineData("C", 2, "D")]
    [InlineData("Bb", 1, "B")]
    [InlineData("Ab", 3, "B")]
    [InlineData("Am", 7, "Em")]
    public void SoundingKey_IsTheShapeKeyRaisedByTheCapo(string shapeKey, int capo, string expected)
    {
        Assert.Equal(expected, CapoCalculator.SoundingKey(shapeKey, capo));
    }

    [Theory]
    [InlineData("Am7", 2, "Gm7")]
    [InlineData("D/F#", 2, "C/E")]
    [InlineData("G", 3, "E")]
    [InlineData("Cmaj7", 4, "Abmaj7")]
    [InlineData("Dsus4", 2, "Csus4")]
    public void ShapeChord_LowersTheWholeChordIncludingBass(string chord, int capo, string expected)
    {
        Assert.Equal(expected, CapoCalculator.ShapeChord(chord, capo, AccidentalPreference.Auto));
    }

    [Fact]
    public void ShapeChord_FollowsTheSpellingOfTheShapeKey()
    {
        // Sounding key F with capo 2 is played as Eb, a flat key.
        Assert.Equal("Ab", CapoCalculator.ShapeChord("Bb", 2, AccidentalPreference.Auto, "F"));
        Assert.Equal("G#", CapoCalculator.ShapeChord("Bb", 2, AccidentalPreference.Sharps, "F"));
        Assert.Equal("Ab", CapoCalculator.ShapeChord("Bb", 2, AccidentalPreference.Flats));
    }

    [Theory]
    [InlineData("E", 3, "G")]
    [InlineData("Am", 2, "Bm")]
    [InlineData("C/E", 2, "D/F#")]
    [InlineData("Em7", 5, "Am7")]
    public void SoundingChord_RaisesTheShape(string shape, int capo, string expected)
    {
        Assert.Equal(expected, CapoCalculator.SoundingChord(shape, capo, AccidentalPreference.Auto));
    }

    [Theory]
    [InlineData("C#m7")]
    [InlineData("Bb/D")]
    [InlineData("N.C.")]
    [InlineData("Dm")]
    public void CapoZero_IsIdentityAndKeepsTheSpelling(string chord)
    {
        Assert.Equal(chord, CapoCalculator.ShapeChord(chord, 0));
        Assert.Equal(chord, CapoCalculator.SoundingChord(chord, 0));
        Assert.Equal(chord, CapoCalculator.ShapeKey(chord, 0));
        Assert.Equal(chord, CapoCalculator.SoundingKey(chord, 0));
    }

    [Theory]
    [InlineData("N.C.")]
    [InlineData("%")]
    [InlineData("")]
    public void NotAChord_IsReturnedUnchangedWithACapo(string text)
    {
        Assert.Equal(text, CapoCalculator.ShapeChord(text, 3));
        Assert.Equal(text, CapoCalculator.SoundingKey(text, 3));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(12)]
    [InlineData(100)]
    [InlineData(int.MinValue)]
    public void CapoOutOfRange_IsRejected(int capo)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CapoCalculator.ShapeChord("C", capo));
        Assert.Throws<ArgumentOutOfRangeException>(() => CapoCalculator.SoundingChord("C", capo));
        Assert.Throws<ArgumentOutOfRangeException>(() => CapoCalculator.ShapeKey("C", capo));
        Assert.Throws<ArgumentOutOfRangeException>(() => CapoCalculator.SoundingKey("C", capo));
    }

    [Fact]
    public void CapoOutOfRange_IsRejectedEvenForNonChords()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CapoCalculator.ShapeChord("N.C.", 12));
    }

    [Fact]
    public void ShapeThenSounding_RoundTripsEveryKeyAndFret()
    {
        foreach (var key in CanonicalMajors.Concat(CanonicalMinors))
        {
            for (var capo = 0; capo <= CapoCalculator.MaxFret; capo++)
            {
                var shape = CapoCalculator.ShapeKey(key, capo);
                Assert.Equal(key, CapoCalculator.SoundingKey(shape, capo));
            }
        }
    }

    [Theory]
    [InlineData("{capo:3}", 3)]
    [InlineData("{capo: 3}", 3)]
    [InlineData("{capo:0}", 0)]
    [InlineData("{capo:11}", 11)]
    public void Parse_CapoDirective_IsReadAsATypedFret(string content, int fret)
    {
        var document = new ChordProParser().Parse(content);

        Assert.Equal(fret, document.CapoFret);
        Assert.Equal(fret.ToString(), document.Capo);
    }

    [Theory]
    [InlineData("{capo:12}")]
    [InlineData("{capo:-1}")]
    [InlineData("{capo:abc}")]
    [InlineData("{title:No capo}")]
    public void Parse_AbsentOrInvalidCapo_HasNoTypedFret(string content)
    {
        var document = new ChordProParser().Parse(content);

        Assert.Null(document.CapoFret);
    }

    [Fact]
    public void CapoFret_FollowsTheStringProperty()
    {
        var document = new ChordProDocument { Capo = " 4 " };

        Assert.Equal(4, document.CapoFret);
    }
}
