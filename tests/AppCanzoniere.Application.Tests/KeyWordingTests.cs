using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>A key written in words or in the other notation is recognised and rewritten in the style it was written in.</summary>
public class KeyWordingTests
{
    private readonly ChordProParser _parser = new();

    private static AccidentalPreference? Preference(string code) => code switch
    {
        "S" => AccidentalPreference.Sharps,
        "F" => AccidentalPreference.Flats,
        "A" => AccidentalPreference.Auto,
        _ => null
    };

    private string RenderKey(string value, ChordProRenderOptions options) =>
        _parser.Render(_parser.Parse($"{{key:{value}}}"), options).TrimEnd('\n');

    [Theory]
    [InlineData("Lam", 2, "-", "Sim")]
    [InlineData("La-", 2, "-", "Si-")]
    [InlineData("A-", 2, "-", "B-")]
    [InlineData("Sib", 2, "-", "Do")]
    [InlineData("Bb", 2, "-", "C")]
    [InlineData("Fa#m", 1, "-", "Solm")]
    [InlineData("F#m", 1, "-", "Gm")]
    [InlineData("Fa#m", 0, "F", "Solbm")]
    [InlineData("am", 2, "-", "Bm")]
    [InlineData(" Am ", 2, "-", " Bm ")]
    [InlineData("La minore", 2, "-", "Si minore")]
    [InlineData("la minore", 2, "-", "Si minore")]
    [InlineData("LA MINORE", 2, "-", "Si MINORE")]
    [InlineData("La   minore", 2, "-", "Si minore")]
    [InlineData("A minor", 2, "-", "B minor")]
    [InlineData("A MINOR", 2, "-", "B MINOR")]
    [InlineData("A min", 2, "-", "B min")]
    [InlineData("La min", 2, "-", "Si min")]
    [InlineData("A maj", 1, "F", "B flat maj")]
    [InlineData("La magg.", 1, "F", "Si bemolle magg.")]
    [InlineData("E major", 1, "-", "F major")]
    [InlineData("Si bemolle maggiore", 1, "-", "Si maggiore")]
    [InlineData("La maggiore", 1, "F", "Si bemolle maggiore")]
    [InlineData("La maggiore", 1, "A", "Si bemolle maggiore")]
    [InlineData("La maggiore", 1, "-", "La diesis maggiore")]
    [InlineData("Re maggiore", 1, "F", "Mi bemolle maggiore")]
    [InlineData("Do minore", 1, "-", "Do diesis minore")]
    [InlineData("Mi bemolle minore", 1, "-", "Mi minore")]
    [InlineData("Fa diesis minore", 2, "-", "Sol diesis minore")]
    [InlineData("Sib maggiore", 2, "-", "Do maggiore")]
    [InlineData("Sib maggiore", 0, "S", "La# maggiore")]
    [InlineData("Bb major", 2, "-", "C major")]
    [InlineData("B flat major", 1, "-", "B major")]
    [InlineData("F sharp minor", 1, "-", "G minor")]
    [InlineData("A major", 1, "F", "B flat major")]
    public void ShiftedKey_IsRewrittenInTheStyleItWasWrittenIn(string value, int steps, string preference, string expected)
    {
        var result = RenderKey(value, new ChordProRenderOptions { TransposeSteps = steps, Accidentals = Preference(preference) });

        Assert.Equal($"{{key:{expected}}}", result);
    }

    [Theory]
    [InlineData("La minore", "A minor")]
    [InlineData("Si bemolle maggiore", "B flat major")]
    [InlineData("Fa diesis minore", "F sharp minor")]
    [InlineData("La magg.", "A maj")]
    [InlineData("La min", "A min")]
    [InlineData("Sib maggiore", "Bb major")]
    [InlineData("Lam", "Am")]
    [InlineData("La-", "Am")]
    public void ItalianInputToEnglish_TranslatesNameAndWords(string value, string expected)
    {
        var options = new ChordProRenderOptions { InputNotation = NotationOptions.Italian, OutputNotation = NotationOptions.English };

        Assert.Equal($"{{key:{expected}}}", RenderKey(value, options));
    }

    [Theory]
    [InlineData("A minor", "La minore")]
    [InlineData("B flat major", "Si bemolle maggiore")]
    [InlineData("F sharp minor", "Fa diesis minore")]
    [InlineData("A maj", "La magg.")]
    [InlineData("A min", "La min")]
    [InlineData("Bb major", "Sib maggiore")]
    [InlineData("Am", "Lam")]
    [InlineData("La minore", "La minore")]
    public void EnglishInputToItalian_TranslatesNameAndWords(string value, string expected)
    {
        var options = new ChordProRenderOptions { OutputNotation = NotationOptions.Italian };

        Assert.Equal($"{{key:{expected}}}", RenderKey(value, options));
    }

    [Fact]
    public void ConversionTogetherWithAShift_AppliesBoth()
    {
        var options = new ChordProRenderOptions { TransposeSteps = 2, InputNotation = NotationOptions.Italian };

        Assert.Equal("{key:B minor}", RenderKey("La minore", options));
    }

    [Theory]
    [InlineData("  la   MINORE ")]
    [InlineData("Si bemolle maggiore")]
    [InlineData("A flat major")]
    [InlineData("Sib maggiore")]
    [InlineData("La magg.")]
    [InlineData("Lam")]
    [InlineData("La-")]
    [InlineData("boh")]
    public void ZeroShiftAndNoConversion_LeaveTheLineExactlyAsWritten(string value)
    {
        var song = $"{{key:{value}}}\n[Am]x";

        Assert.Equal(LosslessAssert.Expected(song), LosslessAssert.Render(song, new ChordProRenderOptions()));
        Assert.Equal(LosslessAssert.Expected(song), LosslessAssert.Render(song, new ChordProRenderOptions { Accidentals = AccidentalPreference.Auto }));
    }

    [Theory]
    [InlineData("boh")]
    [InlineData("Dorian")]
    [InlineData("La menore")]
    [InlineData("minor")]
    [InlineData("La minore extra")]
    public void ValuesThatAreNotKeys_AreLeftUntouched(string value)
    {
        var options = new ChordProRenderOptions { TransposeSteps = 2, OutputNotation = NotationOptions.Italian, Accidentals = AccidentalPreference.Flats };

        Assert.Equal($"{{key:{value}}}", RenderKey(value, options));
    }

    [Fact]
    public void WordedKey_SteersAutomaticSpelling()
    {
        var result = _parser.Render(_parser.Parse("{key:La minore}\n[Am]x [Dm]y"), new ChordProRenderOptions { TransposeSteps = 1, Accidentals = AccidentalPreference.Auto });

        Assert.Equal("{key:Si bemolle minore}\n[Bbm]x [Ebm]y\n", result);
    }

    [Fact]
    public void KeyInTheOtherNotation_SteersAutomaticSpelling()
    {
        var result = _parser.Render(_parser.Parse("{key:Lam}\n[Am]x [Dm]y"), new ChordProRenderOptions { TransposeSteps = 1, Accidentals = AccidentalPreference.Auto });

        Assert.Equal("{key:Sibm}\n[Bbm]x [Ebm]y\n", result);
    }

    [Fact]
    public void SoundingKeyHelper_ReadsAndWritesWordedKeys()
    {
        var document = _parser.Parse("{key:A minor}\n{capo:2}\n[Am]x");

        Assert.Equal("A minor", ChordProKeys.WrittenKey(document, new ChordProRenderOptions()));
        Assert.Equal("B minor", ChordProKeys.SoundingKey(document, new ChordProRenderOptions()));
        Assert.Equal("Si minore", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { OutputNotation = NotationOptions.Italian }));
        Assert.Equal("G minor", ChordProKeys.WrittenKey(document, new ChordProRenderOptions { Capo = 4 }));
        Assert.Equal("B minor", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { Capo = 4 }));
    }

    [Fact]
    public void SoundingKeyHelper_ReadsAKeyWrittenInTheOtherNotation()
    {
        var document = _parser.Parse("{key:Lam}\n[Am]x");

        Assert.Equal("Sim", ChordProKeys.SoundingKey(document, new ChordProRenderOptions { TransposeSteps = 2 }));
    }

    [Fact]
    public void WordedKeyLines_InsideAFullSong_FollowTheCapo()
    {
        var result = _parser.Render(_parser.Parse("{key: La minore}\n[Am]x [E]y"), new ChordProRenderOptions { Capo = 2 });

        Assert.Equal("{key: Sol minore}\n{capo:2}\n[Gm]x [D]y\n", result);
    }
}
