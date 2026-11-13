using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>The key line is the key of the chords as written; the capo raises the sounding key.</summary>
public class CapoKeySemanticsTests
{
    private static readonly string[] Keys = { "C", "Db", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B" };

    private readonly ChordProParser _parser = new();

    private string Render(string song, ChordProRenderOptions options) => _parser.Render(_parser.Parse(song), options);

    private static int Pitch(string chord) =>
        ChordSymbolParser.TryParse(chord, Notation.English, out var symbol) ? symbol.Root.PitchClass : throw new InvalidOperationException(chord);

    [Fact]
    public void NoCapoRequested_ChordsAndKeyShiftByTheTransposition_AndTheCapoLineIsUntouched()
    {
        var result = Render("{key:G}\n{capo:2}\n[G]a [Em]b", new ChordProRenderOptions { TransposeSteps = 2 });

        Assert.Equal("{key:A}\n{capo:2}\n[A]a [F#m]b\n", result);
    }

    [Theory]
    [InlineData("G", 0, 0, 3, "E", "{capo:3}\n")]
    [InlineData("G", 2, 0, 0, "A", "")]
    [InlineData("G", 2, 3, 5, "G", "{capo:5}\n")]
    [InlineData("G", 0, 0, 0, "G", "")]
    [InlineData("C", 0, 5, 2, "Eb", "{capo:2}\n")]
    public void CapoRequested_ChordsAndKeyShiftByTranspositionPlusDocumentCapoMinusRequested(string key, int documentCapo, int steps, int capo, string expectedKey, string expectedCapoLine)
    {
        var song = (documentCapo > 0 ? $"{{key:{key}}}\n{{capo:{documentCapo}}}\n" : $"{{key:{key}}}\n") + $"[{key}]x";
        var options = new ChordProRenderOptions { TransposeSteps = steps, Capo = capo, Accidentals = AccidentalPreference.Auto };

        Assert.Equal($"{{key:{expectedKey}}}\n{expectedCapoLine}[{expectedKey}]x\n", Render(song, options));
    }

    [Fact]
    public void EveryKeyDocumentCapoTranspositionAndCapo_KeepKeyAndChordsTogetherAndTheSoundingKeyAtKeyPlusCapoPlusSteps()
    {
        foreach (var key in Keys)
        {
            foreach (var documentCapo in new[] { 0, 2, 5 })
            {
                foreach (var steps in new[] { -5, 0, 3, 8 })
                {
                    foreach (var capo in new[] { 0, 1, 4, 7 })
                    {
                        var song = $"{{key:{key}}}\n{(documentCapo > 0 ? $"{{capo:{documentCapo}}}\n" : string.Empty)}[{key}]x";
                        var options = new ChordProRenderOptions { TransposeSteps = steps, Capo = capo, Accidentals = AccidentalPreference.Auto };

                        var document = _parser.Parse(_parser.Render(_parser.Parse(song), options));
                        var written = Pitch(document.Key);
                        var chord = Pitch(document.Lines[^1].Chords[0].Chord);
                        var sounding = Pitch(ChordProKeys.SoundingKey(_parser.Parse(song), options)!);
                        var expected = Pitch(key) + documentCapo + steps;

                        var label = $"{key} d{documentCapo} T{steps} c{capo}";
                        Assert.True(written == chord, label);
                        Assert.True(((written + capo - expected) % 12 + 12) % 12 == 0, label);
                        Assert.True(((sounding - expected) % 12 + 12) % 12 == 0, label);
                    }
                }
            }
        }
    }

    [Fact]
    public void LegacyRender_ShiftsChordsAndKeyBySteps()
    {
        Assert.Equal("{key:D}\n[D]x\n", _parser.Render(_parser.Parse("{key:C}\n[C]x"), 2));
    }

    [Fact]
    public void Auto_WithoutAReadableKey_UsesTheFirstChordShifted()
    {
        var result = Render("[F]a [Bb]b [C]c", new ChordProRenderOptions { TransposeSteps = 1, Capo = 0, Accidentals = AccidentalPreference.Auto });

        Assert.Equal("[F#]a [B]b [C#]c\n", result);
    }

    [Theory]
    [InlineData(AccidentalPreference.Flats)]
    [InlineData(AccidentalPreference.Sharps)]
    [InlineData(AccidentalPreference.Auto)]
    public void PinnedExample_WrittenInD_PlusTwoWithCapoThree_EnglishShapesAreSpelledByTheirOwnKey(AccidentalPreference preference)
    {
        var options = new ChordProRenderOptions { TransposeSteps = 2, Capo = 3, Accidentals = preference };

        var result = Render("{key:D}\n[D]a [Bm]b [F#m]c [A/C#]d [G]e", options);

        Assert.Equal("{key:Db}\n{capo:3}\n[Db]a [Bbm]b [Fm]c [Ab/C]d [Gb]e\n", result);
    }

    [Theory]
    [InlineData(AccidentalPreference.Flats)]
    [InlineData(AccidentalPreference.Sharps)]
    public void PinnedExample_WrittenInD_PlusTwoWithCapoThree_ItalianWithLam(AccidentalPreference preference)
    {
        var options = new ChordProRenderOptions { TransposeSteps = 2, Capo = 3, Accidentals = preference, OutputNotation = NotationOptions.Italian };

        var result = Render("{key:D}\n[D]a [Bm]b [F#m]c [A/C#]d [G]e", options);

        Assert.Equal("{key:Reb}\n{capo:3}\n[Reb]a [Sibm]b [Fam]c [Lab/Do]d [Solb]e\n", result);
    }

    [Theory]
    [InlineData(AccidentalPreference.Flats)]
    [InlineData(AccidentalPreference.Sharps)]
    [InlineData(AccidentalPreference.Auto)]
    public void PinnedExample_WrittenInG_CapoThree_ShapesInETheirOwnNaturalSpelling(AccidentalPreference preference)
    {
        var options = new ChordProRenderOptions { Capo = 3, Accidentals = preference };

        var english = Render("{key:G}\n[G]a [Em]b [Bm]c [D/F#]d", options);
        var italian = Render("{key:G}\n[G]a [Em]b [Bm]c [D/F#]d", options with { OutputNotation = NotationOptions.Italian });

        Assert.Equal("{key:E}\n{capo:3}\n[E]a [C#m]b [G#m]c [B/D#]d\n", english);
        Assert.Equal("{key:Mi}\n{capo:3}\n[Mi]a [Do#m]b [Sol#m]c [Si/Re#]d\n", italian);
    }

    [Fact]
    public void ExplicitPreference_IsOverriddenByTheShapesWhenTheEffectiveCapoIsAboveZero()
    {
        var withoutCapo = Render("{key:A}\n[A]x", new ChordProRenderOptions { TransposeSteps = 1, Accidentals = AccidentalPreference.Sharps });
        var documentCapo = Render("{key:A}\n{capo:2}\n[A]x", new ChordProRenderOptions { TransposeSteps = 1, Accidentals = AccidentalPreference.Sharps });
        var requestedCapo = Render("{key:A}\n[A]x", new ChordProRenderOptions { TransposeSteps = 1, Capo = 2, Accidentals = AccidentalPreference.Sharps });

        Assert.Equal("{key:A#}\n[A#]x\n", withoutCapo);
        Assert.Equal("{key:Bb}\n{capo:2}\n[Bb]x\n", documentCapo);
        Assert.Equal("{key:Ab}\n{capo:2}\n[Ab]x\n", requestedCapo);
    }

    [Fact]
    public void ExplicitPreference_AtCapoZeroStillAppliesToChordsAndKey()
    {
        var result = Render("{key:A}\n{capo:2}\n[A]x", new ChordProRenderOptions { Capo = 0, TransposeSteps = 0, Accidentals = AccidentalPreference.Flats });

        Assert.Equal("{key:B}\n[B]x\n", result);
    }

    [Fact]
    public void SoundingKey_IsTheWrittenKeyPlusDocumentCapoPlusSteps()
    {
        var document = _parser.Parse("{key:D}\n{capo:2}\n[D]x");

        Assert.Equal("E", ChordProKeys.SoundingKey(document, new ChordProRenderOptions()));
        Assert.Equal("E", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { Capo = 5 }));
        Assert.Equal("F#", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { TransposeSteps = 2 }));
        Assert.Equal("D", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { TransposeSteps = -2 }));
        Assert.Equal("Mi", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { OutputNotation = NotationOptions.Italian }));
    }

    [Fact]
    public void SoundingKey_TheExplicitPreferenceNamesTheSoundingKey()
    {
        var document = _parser.Parse("{key:D}\n[D]x");
        var up = new ChordProRenderOptions { TransposeSteps = 1, Capo = 2 };

        Assert.Equal("Eb", ChordProKeys.SoundingKey(document, up with { Accidentals = AccidentalPreference.Flats }));
        Assert.Equal("D#", ChordProKeys.SoundingKey(document, up with { Accidentals = AccidentalPreference.Sharps }));
        Assert.Equal("Eb", ChordProKeys.SoundingKey(document, up with { Accidentals = AccidentalPreference.Auto }));
        Assert.Equal("F#", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { TransposeSteps = 4, Accidentals = AccidentalPreference.Sharps }));
        Assert.Equal("Gb", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { TransposeSteps = 4, Accidentals = AccidentalPreference.Flats }));
    }

    [Fact]
    public void SoundingKey_OfAMinorKeyKeepsTheMinorMarker()
    {
        var document = _parser.Parse("{key:Am}\n[Am]x");

        Assert.Equal("Cm", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { Capo = 0, TransposeSteps = 3, Accidentals = AccidentalPreference.Flats }));
        Assert.Equal("Lam", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { OutputNotation = NotationOptions.Italian }));
    }

    [Fact]
    public void WrittenKeyAndEffectiveCapo_DescribeWhatTheScreenPlays()
    {
        var document = _parser.Parse("{key:D}\n[D]x");
        var options = new ChordProRenderOptions { TransposeSteps = 2, Capo = 3 };

        Assert.Equal("Db", ChordProKeys.WrittenKey(document, options));
        Assert.Equal(3, ChordProKeys.EffectiveCapo(document, options));
        Assert.Equal("E", ChordProKeys.SoundingKey(document, options));
        Assert.Equal(0, ChordProKeys.EffectiveCapo(document, new ChordProRenderOptions()));
        Assert.Equal(2, ChordProKeys.EffectiveCapo(_parser.Parse("{capo:2}"), new ChordProRenderOptions()));
        Assert.Equal(0, ChordProKeys.EffectiveCapo(_parser.Parse("{capo:2}"), new ChordProRenderOptions { Capo = 0 }));
    }

    [Theory]
    [InlineData("[C]x")]
    [InlineData("{key:boh}\n[C]x")]
    [InlineData("{key:}\n[C]x")]
    public void WithoutAReadableKey_NoKeyIsReported(string song)
    {
        var document = _parser.Parse(song);

        Assert.Null(ChordProKeys.WrittenKey(document, new ChordProRenderOptions()));
        Assert.Null(ChordProKeys.SoundingKey(document, new ChordProRenderOptions()));
    }

    [Fact]
    public void HandBuiltDocument_ReportsItsKeysToo()
    {
        var document = new ChordProDocument { Key = "G", Capo = "2" };

        Assert.Equal("G", ChordProKeys.WrittenKey(document, new ChordProRenderOptions()));
        Assert.Equal("A", ChordProKeys.SoundingKey(document, new ChordProRenderOptions()));
    }

    [Fact]
    public void CapoOutOfRange_IsRejectedByTheKeyHelpers()
    {
        var document = _parser.Parse("{key:G}");

        Assert.Throws<ArgumentOutOfRangeException>(() => ChordProKeys.SoundingKey(document, new ChordProRenderOptions { Capo = 12 }));
    }
}
