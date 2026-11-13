using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>All 12 major and 12 minor keys, in both notations, with and without a capo on fret 2.</summary>
public class KeyMatrixTests
{
    private const int Key = 0;
    private const int Italian = 1;
    private const int ShapeEnglish = 2;
    private const int ShapeItalian = 3;

    private readonly ChordProParser _parser = new();

    // Key (written), Italian name, shape English with capo 2 (also the new key line), shape Italian with capo 2.
    private static readonly string[][] Rows =
    {
        new[] { "C", "Do", "Bb", "Sib" },
        new[] { "Db", "Reb", "B", "Si" },
        new[] { "D", "Re", "C", "Do" },
        new[] { "Eb", "Mib", "Db", "Reb" },
        new[] { "E", "Mi", "D", "Re" },
        new[] { "F", "Fa", "Eb", "Mib" },
        new[] { "F#", "Fa#", "E", "Mi" },
        new[] { "G", "Sol", "F", "Fa" },
        new[] { "Ab", "Lab", "F#", "Fa#" },
        new[] { "A", "La", "G", "Sol" },
        new[] { "Bb", "Sib", "Ab", "Lab" },
        new[] { "B", "Si", "A", "La" },
        new[] { "Cm", "Dom", "Bbm", "Sibm" },
        new[] { "C#m", "Do#m", "Bm", "Sim" },
        new[] { "Dm", "Rem", "Cm", "Dom" },
        new[] { "Ebm", "Mibm", "C#m", "Do#m" },
        new[] { "Em", "Mim", "Dm", "Rem" },
        new[] { "Fm", "Fam", "Ebm", "Mibm" },
        new[] { "F#m", "Fa#m", "Em", "Mim" },
        new[] { "Gm", "Solm", "Fm", "Fam" },
        new[] { "G#m", "Sol#m", "F#m", "Fa#m" },
        new[] { "Am", "Lam", "Gm", "Solm" },
        new[] { "Bbm", "Sibm", "G#m", "Sol#m" },
        new[] { "Bm", "Sim", "Am", "Lam" }
    };

    public static IEnumerable<object[]> KeyOnly => Pick(Key);

    public static IEnumerable<object[]> KeyAndItalian => Pick(Key, Italian);

    public static IEnumerable<object[]> KeyAndShape => Pick(Key, ShapeEnglish);

    public static IEnumerable<object[]> KeyItalianAndShapeItalian => Pick(Key, Italian, ShapeItalian);

    private static IEnumerable<object[]> Pick(params int[] columns) =>
        Rows.Select(row => columns.Select(c => (object)row[c]).ToArray());

    private string Render(string key, ChordProRenderOptions options) =>
        _parser.Render(_parser.Parse($"{{key:{key}}}\n[{key}]x"), options with { Accidentals = AccidentalPreference.Auto });

    [Theory]
    [MemberData(nameof(KeyOnly))]
    public void English_NoCapo_KeepsTheKey(string key)
    {
        Assert.Equal($"{{key:{key}}}\n[{key}]x\n", Render(key, new ChordProRenderOptions()));
    }

    [Theory]
    [MemberData(nameof(KeyAndItalian))]
    public void Italian_NoCapo_WritesTheKeyInSolfege(string key, string italian)
    {
        var options = new ChordProRenderOptions { OutputNotation = NotationOptions.Italian };

        Assert.Equal($"{{key:{italian}}}\n[{italian}]x\n", Render(key, options));
    }

    [Theory]
    [MemberData(nameof(KeyAndShape))]
    public void English_CapoTwo_ShowsTheShapesAndTheKeyOfTheShapes(string key, string shape)
    {
        var options = new ChordProRenderOptions { Capo = 2 };

        Assert.Equal($"{{key:{shape}}}\n{{capo:2}}\n[{shape}]x\n", Render(key, options));
    }

    [Theory]
    [MemberData(nameof(KeyItalianAndShapeItalian))]
    public void Italian_CapoTwo_ShowsTheShapesAndTheKeyOfTheShapes(string key, string italian, string shapeItalian)
    {
        var options = new ChordProRenderOptions { Capo = 2, OutputNotation = NotationOptions.Italian };

        Assert.Equal($"{{key:{shapeItalian}}}\n{{capo:2}}\n[{shapeItalian}]x\n", Render(key, options));
    }

    [Theory]
    [MemberData(nameof(KeyAndItalian))]
    public void Italian_ToEnglish_RoundTripsEveryKey(string key, string italian)
    {
        Assert.Equal(key, ChordNotation.Convert(italian, NotationOptions.Italian, NotationOptions.English));
        Assert.Equal(italian, ChordNotation.Convert(key, NotationOptions.English, NotationOptions.Italian));
    }

    [Theory]
    [MemberData(nameof(KeyAndItalian))]
    public void ItalianDash_WritesMinorKeysWithAHyphen(string key, string italian)
    {
        var expected = key.EndsWith('m') ? italian[..^1] + "-" : italian;

        Assert.Equal(expected, ChordNotation.Convert(key, NotationOptions.English, NotationOptions.ItalianDash));
    }
}
