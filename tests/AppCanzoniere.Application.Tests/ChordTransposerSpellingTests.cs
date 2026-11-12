using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class ChordTransposerSpellingTests
{
    private readonly ChordTransposer _transposer = new();

    [Theory]
    [InlineData("A", 1, "A#")]
    [InlineData("Am", 1, "A#m")]
    [InlineData("Cm", 1, "C#m")]
    [InlineData("Ebm7", 0, "D#m7")]
    [InlineData("Db", 2, "D#")]
    [InlineData("B♭", 2, "C")]
    public void Sharps_SpellAlteredNotesWithSharps(string chord, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Sharps));
    }

    [Theory]
    [InlineData("A", 1, "Bb")]
    [InlineData("Am", 1, "Bbm")]
    [InlineData("F#m", 3, "Am")]
    [InlineData("Cm", 1, "Dbm")]
    [InlineData("Bbm", 2, "Cm")]
    [InlineData("Ebm7", 0, "Ebm7")]
    [InlineData("Db", 2, "Eb")]
    [InlineData("Bb", 3, "Db")]
    [InlineData("F#", 2, "Ab")]
    [InlineData("C#m7b5", 1, "Dm7b5")]
    [InlineData("B♭", 3, "Db")]
    [InlineData("C#", 0, "Db")]
    public void Flats_SpellMinorsAndAlteredRootsWithFlats(string chord, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Flats));
    }

    [Theory]
    [InlineData("D/F#", 1, "Eb/G")]
    [InlineData("Am/G", 1, "Bbm/Ab")]
    [InlineData("C/E", 1, "Db/F")]
    [InlineData("G7/B", 3, "Bb7/D")]
    public void Flats_BassOfSlashChordFollowsTheSamePreference(string chord, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Flats));
    }

    [Fact]
    public void Sharps_BassOfSlashChordFollowsTheSamePreference()
    {
        Assert.Equal("C#/F", _transposer.TransposeChord("C/E", 1, AccidentalPreference.Sharps));
        Assert.Equal("A#m/G#", _transposer.TransposeChord("Am/G", 1, AccidentalPreference.Sharps));
    }

    [Theory]
    [InlineData("A-", 2, "B-")]
    [InlineData("Amin7", 2, "Bmin7")]
    [InlineData("A-7", 1, "Bb-7")]
    public void MinorMarkerAsTyped_IsPreservedByTransposition(string chord, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Flats));
    }

    [Theory]
    [InlineData("N.C.")]
    [InlineData("%")]
    [InlineData("")]
    [InlineData("Bridge")]
    public void NotAChord_IsReturnedUnchanged(string text)
    {
        Assert.Equal(text, _transposer.TransposeChord(text, 3, AccidentalPreference.Flats));
        Assert.Equal(text, _transposer.TransposeChord(text, 3, AccidentalPreference.Auto, "C"));
    }

    [Theory]
    [InlineData("C", 5, "F")]
    [InlineData("C", 10, "Bb")]
    [InlineData("C", 1, "Db")]
    [InlineData("C", 3, "Eb")]
    [InlineData("C", 6, "F#")]
    [InlineData("C", 7, "G")]
    [InlineData("C", 9, "A")]
    [InlineData("Am", 1, "Bbm")]
    [InlineData("Am", 2, "Bm")]
    [InlineData("Am", 3, "Cm")]
    [InlineData("Am", 4, "C#m")]
    [InlineData("Am", 5, "Dm")]
    [InlineData("Am", 6, "Ebm")]
    [InlineData("Am", 11, "G#m")]
    public void TransposeKey_Auto_FollowsTheKeySignatureOfTheResult(string key, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeKey(key, steps, AccidentalPreference.Auto));
    }

    [Theory]
    [InlineData("C", 3, "Eb")]
    [InlineData("Am", 1, "Bbm")]
    public void TransposeKey_Flats_UsesFlats(string key, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeKey(key, steps, AccidentalPreference.Flats));
    }

    [Fact]
    public void TransposeKey_NotAKey_IsReturnedUnchanged()
    {
        Assert.Equal("?", _transposer.TransposeKey("?", 2, AccidentalPreference.Auto));
    }

    [Theory]
    [InlineData("C", "C", 5, "F")]
    [InlineData("F", "C", 5, "Bb")]
    [InlineData("G7", "C", 5, "C7")]
    [InlineData("Am", "C", 5, "Dm")]
    [InlineData("D7", "C", 5, "G7")]
    [InlineData("E", "C", 5, "A")]
    [InlineData("B7", "C", 5, "E7")]
    [InlineData("F", "C", 7, "C")]
    [InlineData("B", "C", 7, "F#")]
    [InlineData("A#", "F", 0, "Bb")]
    [InlineData("A#", "Dm", 0, "Bb")]
    [InlineData("A", "Am", 5, "D")]
    [InlineData("F", "Am", 5, "Bb")]
    [InlineData("E7", "Am", 5, "A7")]
    public void TransposeChord_Auto_UsesTheTargetKey(string chord, string key, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Auto, key));
    }

    [Theory]
    [InlineData("C", 10, "Bb")]
    [InlineData("C", 1, "Db")]
    [InlineData("Cm", 1, "C#m")]
    [InlineData("C", 6, "F#")]
    [InlineData("Am", 1, "Bbm")]
    [InlineData("D/F#", 1, "Eb/G")]
    public void TransposeChord_AutoWithoutKey_TreatsTheResultAsItsOwnKey(string chord, int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Auto));
    }

    [Theory]
    [InlineData(5, false, true)]
    [InlineData(10, false, true)]
    [InlineData(3, false, true)]
    [InlineData(8, false, true)]
    [InlineData(1, false, true)]
    [InlineData(0, false, false)]
    [InlineData(2, false, false)]
    [InlineData(7, false, false)]
    [InlineData(9, false, false)]
    [InlineData(4, false, false)]
    [InlineData(11, false, false)]
    [InlineData(6, false, false)]
    [InlineData(2, true, true)]
    [InlineData(7, true, true)]
    [InlineData(0, true, true)]
    [InlineData(5, true, true)]
    [InlineData(10, true, true)]
    [InlineData(3, true, true)]
    [InlineData(9, true, false)]
    [InlineData(4, true, false)]
    [InlineData(11, true, false)]
    [InlineData(6, true, false)]
    [InlineData(1, true, false)]
    [InlineData(8, true, false)]
    public void KeySignature_PrefersFlats_MatchesTheCircleOfFifths(int tonic, bool minor, bool flats)
    {
        Assert.Equal(flats, KeySignature.PrefersFlats(tonic, minor));
    }

    [Fact]
    public void KeySignature_PrefersFlats_NormalisesTheTonic()
    {
        Assert.True(KeySignature.PrefersFlats(17, false));
        Assert.True(KeySignature.PrefersFlats(-2, false));
    }
}
