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

    [Theory]
    [InlineData("[C]a [Am]b [F]c [G]d", 1, "[Db]a [Bbm]b [Gb]c [Ab]d")]
    [InlineData("[Am]a [C]b [G]c", 1, "[Bbm]a [Db]b [Ab]c")]
    [InlineData("[N.C.]a [C]b [F]c [G7]d", 5, "[N.C.]a [F]b [Bb]c [C7]d")]
    [InlineData("[C]a [Ab]b [Bb]c [C]d", 7, "[G]a [Eb]b [F]c [G]d")]
    [InlineData("[E]a [A]b [B7]c", 3, "[G]a [C]b [D7]c")]
    public void Render_AutoWithoutAKeyLine_UsesTheFirstChordAsTheKeyForTheWholeSong(string song, int steps, string expected)
    {
        var result = Render(song, new ChordProRenderOptions { TransposeSteps = steps });

        Assert.Equal(expected + "\n", result);
    }

    [Fact]
    public void Render_AutoWithoutAKeyLineAndACapo_SpellsTheShapesFromTheFirstChord()
    {
        var result = Render("[G]a [C]b [D]c", new ChordProRenderOptions { Capo = 2 });

        Assert.Equal("{capo:2}\n\n[F]a [Bb]b [C]c\n", result);
    }

    [Fact]
    public void Render_AutoWithoutAKeyLine_IgnoresChordsInsideTabBlocks()
    {
        var result = Render("{sot}\ne|-[F#]-\n{eot}\n[C]a [F]b", new ChordProRenderOptions { TransposeSteps = 1 });

        Assert.Equal("{start_of_tab}\ne|-[F#]-\n{end_of_tab}\n[Db]a [Gb]b\n", result);
    }

    [Fact]
    public void TransposeChord_AutoWithoutAKey_StillTreatsTheChordAsItsOwnKey()
    {
        Assert.Equal("Bbm", _transposer.TransposeChord("Am", 1, AccidentalPreference.Auto));
        Assert.Equal("Db", _transposer.TransposeChord("C", 1, AccidentalPreference.Auto));
    }

    [Fact]
    public void Render_AutoWithoutShift_LeavesChordsAndKeyAsWritten()
    {
        var result = Render("{key:Gb}\n[Gb]a [Cb]b [Db]c [Ebm]d", new ChordProRenderOptions());

        Assert.Equal("{key:Gb}\n\n[Gb]a [Cb]b [Db]c [Ebm]d\n", result);
    }

    [Fact]
    public void Render_AutoWithoutShiftToItalian_ConvertsTheNotationOnly()
    {
        var result = Render("{key:Gb}\n[Gb]a [Db]b", new ChordProRenderOptions { OutputNotation = NotationOptions.Italian });

        Assert.Equal("{key:Solb}\n\n[Solb]a [Reb]b\n", result);
    }

    [Theory]
    [InlineData("C", "F#")]
    [InlineData("Am", "Ebm")]
    public void Render_AutoShiftIntoTheAmbiguousKeys_KeepsTheCurrentChoice(string key, string expected)
    {
        var result = Render($"{{key:{key}}}\n[{key}]a", new ChordProRenderOptions { TransposeSteps = 6 });

        Assert.Equal($"{{key:{expected}}}\n\n[{expected}]a\n", result);
    }
}
