using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Auto spelling names each root and bass by its degree in the key, not by one sharps or flats flag.</summary>
public class AutoSpellingTests
{
    private readonly ChordTransposer _transposer = new();
    private readonly ChordProParser _parser = new();

    private string Render(string content, ChordProRenderOptions options) =>
        _parser.Render(_parser.Parse(content), options with { Accidentals = AccidentalPreference.Auto });

    [Theory]
    [InlineData("Bb", 0, "C", "Bb")]
    [InlineData("Eb", 0, "C", "Eb")]
    [InlineData("Ab", 0, "C", "Ab")]
    [InlineData("A#", 0, "C", "Bb")]
    [InlineData("Gb", 0, "C", "F#")]
    [InlineData("Ab", 7, "C", "Eb")]
    [InlineData("Bb", 7, "C", "F")]
    [InlineData("Bb", 0, "G", "Bb")]
    [InlineData("F", 0, "G", "F")]
    [InlineData("Bb", 0, "D", "Bb")]
    public void Major_BorrowedChordsUseFlatsAndTheRaisedFourthUsesASharp(string chord, int steps, string key, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Auto, key));
    }

    [Theory]
    [InlineData("E", 0, "Am", "E")]
    [InlineData("E7", 0, "Am", "E7")]
    [InlineData("G#dim", 0, "Am", "G#dim")]
    [InlineData("F#m7b5", 0, "Am", "F#m7b5")]
    [InlineData("Bb", 0, "Am", "Bb")]
    [InlineData("C#", 0, "Am", "C#")]
    [InlineData("Db", 0, "Am", "C#")]
    [InlineData("E", 0, "Cm", "E")]
    [InlineData("A", 0, "Cm", "A")]
    [InlineData("B", 0, "Cm", "B")]
    [InlineData("F#", 0, "Dm", "F#")]
    [InlineData("Ab", 0, "Cm", "Ab")]
    [InlineData("F", 0, "Am", "F")]
    public void Minor_RaisedThirdSixthAndSeventhUseSharpsAndTheBorrowedSecondAFlat(string chord, int steps, string key, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Auto, key));
    }

    [Theory]
    [InlineData("D/F#", 0, "F", "D/F#")]
    [InlineData("A7/C#", 0, "F", "A7/C#")]
    [InlineData("Bb/D", 0, "C", "Bb/D")]
    [InlineData("Eb/G", 0, "C", "Eb/G")]
    [InlineData("D/F#", 2, "F", "E/G#")]
    [InlineData("A7/C#", 2, "F", "B7/D#")]
    [InlineData("C/E", 1, "C", "Db/F")]
    [InlineData("Bb/D", 5, "C", "Eb/G")]
    [InlineData("C/Bb", 0, "C", "C/Bb")]
    [InlineData("C/Bb", 7, "C", "G/F")]
    [InlineData("Am/G", 0, "C", "Am/G")]
    [InlineData("E/G#", 0, "Am", "E/G#")]
    public void SlashChords_SpellABassThatIsTheMajorThirdAsThatThird(string chord, int steps, string key, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, steps, AccidentalPreference.Auto, key));
    }

    [Theory]
    [InlineData("Bb", AccidentalPreference.Sharps, "A#")]
    [InlineData("A#", AccidentalPreference.Flats, "Bb")]
    [InlineData("D/F#", AccidentalPreference.Flats, "D/Gb")]
    public void ExplicitPreferences_KeepTheirBluntBehaviour(string chord, AccidentalPreference preference, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, 0, preference, "C"));
    }

    [Fact]
    public void Render_ShiftedSongWithBorrowedChords_SpellsThemByDegree()
    {
        var result = Render("{key:C}\n[C]a [Ab]b [Bb]c [C]d", new ChordProRenderOptions { TransposeSteps = 7 });

        Assert.Equal("{key:G}\n\n[G]a [Eb]b [F]c [G]d\n", result);
    }

    [Fact]
    public void Render_SongWithSlashChordsAfterAShift_KeepsTheThirdSpelledAsAThird()
    {
        var result = Render("{key:F}\n[D/F#]a [A7/C#]b", new ChordProRenderOptions { TransposeSteps = 2 });

        Assert.Equal("{key:G}\n\n[E/G#]a [B7/D#]b\n", result);
    }

    [Fact]
    public void Render_ShiftedMinorSong_KeepsTheRaisedSeventhAsASharp()
    {
        var result = Render("{key:Am}\n[Am]a [E7/G#]b [F]c", new ChordProRenderOptions { TransposeSteps = 5 });

        Assert.Equal("{key:Dm}\n\n[Dm]a [A7/C#]b [Bb]c\n", result);
    }
}
