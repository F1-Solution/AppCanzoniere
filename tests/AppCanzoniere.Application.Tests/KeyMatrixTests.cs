using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>All 12 major and 12 minor keys, in both notations, with and without a capo on fret 2.</summary>
public class KeyMatrixTests
{
    private readonly ChordProParser _parser = new();

    // Key (sounding), Italian name, shape English with capo 2, shape Italian with capo 2.
    public static TheoryData<string, string, string, string> Keys => new()
    {
        { "C", "Do", "Bb", "Sib" },
        { "Db", "Reb", "B", "Si" },
        { "D", "Re", "C", "Do" },
        { "Eb", "Mib", "Db", "Reb" },
        { "E", "Mi", "D", "Re" },
        { "F", "Fa", "Eb", "Mib" },
        { "F#", "Fa#", "E", "Mi" },
        { "G", "Sol", "F", "Fa" },
        { "Ab", "Lab", "F#", "Fa#" },
        { "A", "La", "G", "Sol" },
        { "Bb", "Sib", "Ab", "Lab" },
        { "B", "Si", "A", "La" },
        { "Cm", "Dom", "Bbm", "Sibm" },
        { "C#m", "Do#m", "Bm", "Sim" },
        { "Dm", "Rem", "Cm", "Dom" },
        { "Ebm", "Mibm", "C#m", "Do#m" },
        { "Em", "Mim", "Dm", "Rem" },
        { "Fm", "Fam", "Ebm", "Mibm" },
        { "F#m", "Fa#m", "Em", "Mim" },
        { "Gm", "Solm", "Fm", "Fam" },
        { "G#m", "Sol#m", "F#m", "Fa#m" },
        { "Am", "Lam", "Gm", "Solm" },
        { "Bbm", "Sibm", "G#m", "Sol#m" },
        { "Bm", "Sim", "Am", "Lam" }
    };

    private string Render(string key, ChordProRenderOptions options) =>
        _parser.Render(_parser.Parse($"{{key:{key}}}\n[{key}]x"), options with { Accidentals = AccidentalPreference.Auto });

    [Theory]
    [MemberData(nameof(Keys))]
    public void English_NoCapo_KeepsTheKey(string key, string italian, string shape, string shapeItalian)
    {
        Assert.Equal($"{{key:{key}}}\n\n[{key}]x\n", Render(key, new ChordProRenderOptions()));
    }

    [Theory]
    [MemberData(nameof(Keys))]
    public void Italian_NoCapo_WritesTheKeyInSolfege(string key, string italian, string shape, string shapeItalian)
    {
        var options = new ChordProRenderOptions { OutputNotation = NotationOptions.Italian };

        Assert.Equal($"{{key:{italian}}}\n\n[{italian}]x\n", Render(key, options));
    }

    [Theory]
    [MemberData(nameof(Keys))]
    public void English_CapoTwo_ShowsTheShapesAndTheSoundingKey(string key, string italian, string shape, string shapeItalian)
    {
        var options = new ChordProRenderOptions { Capo = 2 };

        Assert.Equal($"{{key:{key}}}\n{{capo:2}}\n\n[{shape}]x\n", Render(key, options));
    }

    [Theory]
    [MemberData(nameof(Keys))]
    public void Italian_CapoTwo_ShowsTheShapesAndTheSoundingKey(string key, string italian, string shape, string shapeItalian)
    {
        var options = new ChordProRenderOptions { Capo = 2, OutputNotation = NotationOptions.Italian };

        Assert.Equal($"{{key:{italian}}}\n{{capo:2}}\n\n[{shapeItalian}]x\n", Render(key, options));
    }

    [Theory]
    [MemberData(nameof(Keys))]
    public void Italian_ToEnglish_RoundTripsEveryKey(string key, string italian, string shape, string shapeItalian)
    {
        Assert.Equal(key, ChordNotation.Convert(italian, NotationOptions.Italian, NotationOptions.English));
        Assert.Equal(italian, ChordNotation.Convert(key, NotationOptions.English, NotationOptions.Italian));
    }

    [Theory]
    [MemberData(nameof(Keys))]
    public void ItalianDash_WritesMinorKeysWithAHyphen(string key, string italian, string shape, string shapeItalian)
    {
        var expected = key.EndsWith('m') ? italian[..^1] + "-" : italian;

        Assert.Equal(expected, ChordNotation.Convert(key, NotationOptions.English, NotationOptions.ItalianDash));
    }
}
