using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Worded keys without a quality word, with a detached flat or sharp sign or a trailing full stop are keys too.</summary>
public class KeyLooseWordingTests
{
    private readonly ChordProParser _parser = new();

    private string RenderKey(string value, ChordProRenderOptions options) =>
        _parser.Render(_parser.Parse($"{{key:{value}}}"), options).TrimEnd('\n');

    private static AccidentalPreference? Preference(string code) => code switch
    {
        "S" => AccidentalPreference.Sharps,
        "F" => AccidentalPreference.Flats,
        _ => null
    };

    [Theory]
    [InlineData("Si bemolle", 3, "S", "Do diesis")]
    [InlineData("Si bemolle", 3, "F", "Re bemolle")]
    [InlineData("Fa diesis", 1, "-", "Sol")]
    [InlineData("Fa diesis", 0, "F", "Sol bemolle")]
    [InlineData("C sharp", 2, "-", "D sharp")]
    [InlineData("B flat", 1, "-", "B")]
    [InlineData("B flat", 3, "-", "C sharp")]
    [InlineData("SI BEMOLLE", 3, "S", "DO DIESIS")]
    [InlineData("Mi b maggiore", 3, "F", "Sol b maggiore")]
    [InlineData("Mi b maggiore", 4, "-", "Sol maggiore")]
    [InlineData("MI B MAGGIORE", 3, "F", "SOL B MAGGIORE")]
    [InlineData("La minore.", 2, "-", "Si minore.")]
    [InlineData("A minor.", 2, "-", "B minor.")]
    [InlineData("Si bemolle.", 3, "S", "Do diesis.")]
    [InlineData("La min.", 2, "-", "Si min.")]
    public void LooseWordedKey_IsRewrittenInTheStyleItWasWrittenIn(string value, int steps, string preference, string expected)
    {
        var options = new ChordProRenderOptions { TransposeSteps = steps, Accidentals = Preference(preference) };

        Assert.Equal($"{{key:{expected}}}", RenderKey(value, options));
    }

    [Theory]
    [InlineData("Si bemolle")]
    [InlineData("Fa diesis")]
    [InlineData("C sharp")]
    [InlineData("B flat")]
    [InlineData("Mi b maggiore")]
    [InlineData("La minore.")]
    public void LooseWordedKey_ZeroShiftLeavesTheLineAsWritten(string value)
    {
        var song = $"{{key:{value}}}\n[Am]x";

        Assert.Equal(LosslessAssert.Expected(song), LosslessAssert.Render(song, new ChordProRenderOptions()));
    }

    [Theory]
    [InlineData("Si bemolle", "B flat")]
    [InlineData("Fa diesis", "F sharp")]
    [InlineData("La minore.", "A minor.")]
    public void LooseWordedKey_TranslatesBetweenNotations(string value, string expected)
    {
        var options = new ChordProRenderOptions { InputNotation = NotationOptions.Italian, OutputNotation = NotationOptions.English };

        Assert.Equal($"{{key:{expected}}}", RenderKey(value, options));
    }

    [Theory]
    [InlineData("Mi b")]
    [InlineData("Si bemolle extra")]
    [InlineData("Si bemolle minore diesis")]
    public void StillNotKeys_AreLeftUntouched(string value)
    {
        Assert.Equal($"{{key:{value}}}", RenderKey(value, new ChordProRenderOptions { TransposeSteps = 2 }));
    }

    [Fact]
    public void EffectiveCapo_RejectsARequestedCapoOutOfRangeLikeTheOtherHelpers()
    {
        var document = _parser.Parse("{key:C}\n{capo:2}\n[C]x");
        var options = new ChordProRenderOptions { Capo = 12 };

        Assert.Throws<ArgumentOutOfRangeException>(() => ChordProKeys.WrittenKey(document, options));
        Assert.Throws<ArgumentOutOfRangeException>(() => ChordProKeys.SoundingKey(document, options));
        Assert.Throws<ArgumentOutOfRangeException>(() => ChordProKeys.EffectiveCapo(document, options));
        Assert.Throws<ArgumentOutOfRangeException>(() => ChordProKeys.EffectiveCapo(document, new ChordProRenderOptions { Capo = -1 }));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("15")]
    [InlineData("")]
    public void EffectiveCapo_CountsAnUnreadableDocumentCapoAsZero(string capo)
    {
        var document = new ChordProDocument { Capo = capo };

        Assert.Equal(0, ChordProKeys.EffectiveCapo(document, new ChordProRenderOptions()));
        Assert.Equal(5, ChordProKeys.EffectiveCapo(document, new ChordProRenderOptions { Capo = 5 }));
    }
}
