using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>An upper-case B is a flat sign in an all-upper worded key, and rewritten keys follow the case of the written key.</summary>
public class KeyCaseTests
{
    private readonly ChordProParser _parser = new();

    private string RenderKey(string value, ChordProRenderOptions options) =>
        _parser.Render(_parser.Parse($"{{key:{value}}}"), options).TrimEnd('\n');

    private static ChordProRenderOptions Steps(int steps, AccidentalPreference? preference = null) =>
        new() { TransposeSteps = steps, Accidentals = preference };

    [Theory]
    [InlineData("EB MINOR", 2, null, "F MINOR")]
    [InlineData("EB MINOR", 1, null, "E MINOR")]
    [InlineData("SIB MAGGIORE", 2, null, "DO MAGGIORE")]
    [InlineData("BB MAJOR", 1, null, "B MAJOR")]
    [InlineData("BB MAJOR", 2, null, "C MAJOR")]
    [InlineData("AB MAJOR", 2, null, "A# MAJOR")]
    [InlineData("AB MAJOR", 2, "S", "A# MAJOR")]
    [InlineData("LAB MAGGIORE", 1, "F", "LA MAGGIORE")]
    public void UpperCaseB_AfterANote_IsAFlatSign(string value, int steps, string? preference, string expected)
    {
        var options = Steps(steps, preference switch { "S" => AccidentalPreference.Sharps, "F" => AccidentalPreference.Flats, _ => null });

        Assert.Equal($"{{key:{expected}}}", RenderKey(value, options));
    }

    [Theory]
    [InlineData("EB minor")]
    [InlineData("SIB maggiore")]
    public void UpperCaseBInAMixedCaseKey_IsNotAKey_AndTheLineIsUntouched(string value)
    {
        Assert.Equal($"{{key:{value}}}", RenderKey(value, Steps(2)));
    }

    [Theory]
    [InlineData("SIB MAGGIORE", "BB MAJOR")]
    [InlineData("MIB MINORE", "EB MINOR")]
    [InlineData("DO MAGGIORE", "C MAJOR")]
    public void ItalianToEnglish_NeverWritesAControlCharacter(string value, string expected)
    {
        var options = new ChordProRenderOptions { InputNotation = NotationOptions.Italian, OutputNotation = NotationOptions.English };

        Assert.Equal($"{{key:{expected}}}", RenderKey(value, options));
    }

    [Theory]
    [InlineData("AB", 1, "A")]
    [InlineData("AB", 2, "BB")]
    [InlineData("AB", 3, "B")]
    [InlineData("EB", 1, "E")]
    [InlineData("BB", 1, "B")]
    [InlineData("SIB", 2, "DO")]
    [InlineData("MIB", 1, "MI")]
    [InlineData("LAB", 1, "LA")]
    [InlineData("REB", 1, "RE")]
    [InlineData("SOLB", 1, "SOL")]
    [InlineData("EBM", 2, "FM")]
    [InlineData("SIBM", 2, "DOM")]
    [InlineData("ABM", 1, "AM")]
    public void CompactAllCapsKey_ReadsBAfterANoteAsAFlatSign_AndKeepsTheAllCapsStyle(string value, int steps, string expected)
    {
        Assert.Equal($"{{key:{expected}}}", RenderKey(value, Steps(steps, AccidentalPreference.Auto)));
    }

    [Theory]
    [InlineData("AM", 2, "BM")]
    [InlineData("EM", 1, "FM")]
    [InlineData("BM", 1, "CM")]
    public void CompactAllCapsKey_ReadsAnUpperCaseMAfterTheNoteOrFlatAsMinor_AndKeepsIt(string value, int steps, string expected)
    {
        Assert.Equal($"{{key:{expected}}}", RenderKey(value, Steps(steps, AccidentalPreference.Auto)));
    }

    [Theory]
    [InlineData("Ab", 2, "Bb")]
    [InlineData("Bb", 1, "B")]
    [InlineData("Sib", 2, "Do")]
    [InlineData("Am", 2, "Bm")]
    [InlineData("C", 10, "Bb")]
    public void MixedCaseCompactKeys_AreUnchanged(string value, int steps, string expected)
    {
        Assert.Equal($"{{key:{expected}}}", RenderKey(value, Steps(steps, AccidentalPreference.Auto)));
    }

    [Theory]
    [InlineData("DO MAGGIORE", 1, "S", "DO DIESIS MAGGIORE")]
    [InlineData("DO MAGGIORE", 1, null, "DO DIESIS MAGGIORE")]
    [InlineData("DO MAGGIORE", 1, "F", "RE BEMOLLE MAGGIORE")]
    [InlineData("DO MAGGIORE", 1, "A", "RE BEMOLLE MAGGIORE")]
    [InlineData("LA MINORE", 2, null, "SI MINORE")]
    [InlineData("la minore", 2, null, "si minore")]
    [InlineData("La minore", 2, null, "Si minore")]
    [InlineData("do maggiore", 1, "F", "re bemolle maggiore")]
    [InlineData("A MAJOR", 1, "F", "B FLAT MAJOR")]
    [InlineData("a minor", 2, null, "b minor")]
    [InlineData("Si♭ maggiore", 2, "S", "Do maggiore")]
    [InlineData("La♭ maggiore", 2, "F", "Si♭ maggiore")]
    [InlineData("Do♯ minore", 2, "F", "Mi♭ minore")]
    [InlineData("Do maggiore", 1, "F", "Re bemolle maggiore")]
    [InlineData("Re maggiore", 1, "S", "Re diesis maggiore")]
    public void RewrittenWordedKey_FollowsTheCasePatternOfTheWrittenKey(string value, int steps, string? preference, string expected)
    {
        var options = Steps(steps, preference switch { "S" => AccidentalPreference.Sharps, "F" => AccidentalPreference.Flats, "A" => AccidentalPreference.Auto, _ => null });

        Assert.Equal($"{{key:{expected}}}", RenderKey(value, options));
    }

    [Fact]
    public void WordedKeyWithUpperCaseFlatSign_SoundingAndWrittenKeysAreFlatToo()
    {
        var document = _parser.Parse("{key: EB MINOR}\n[Ebm]x");

        Assert.Equal("F MINOR", ChordProKeys.WrittenKey(document, Steps(2))!.Trim());
        Assert.Equal("F MINOR", ChordProKeys.SoundingKey(document, Steps(2))!.Trim());
        Assert.Equal("EB MINOR", ChordProKeys.SoundingKey(document, Steps(0, AccidentalPreference.Flats))!.Trim());
    }

    [Fact]
    public void EveryWordedKeyShiftPreferenceAndNotation_WritesOnlyPrintableCharactersAndKeepsThePitch()
    {
        var checkedKeys = 0;
        foreach (var (written, pitch) in WrittenKeys())
        {
            foreach (var steps in Enumerable.Range(0, 12))
            {
                foreach (var preference in new AccidentalPreference?[] { null, AccidentalPreference.Sharps, AccidentalPreference.Flats, AccidentalPreference.Auto })
                {
                    foreach (var output in new[] { NotationOptions.English, NotationOptions.Italian })
                    {
                        AssertPrintableAndSamePitch(written, pitch, steps, preference, output);
                        checkedKeys++;
                    }
                }
            }
        }

        Assert.True(checkedKeys > 20000);
    }

    private void AssertPrintableAndSamePitch(string written, int pitch, int steps, AccidentalPreference? preference, NotationOptions output)
    {
        var input = written.StartsWith("E:", StringComparison.Ordinal) ? NotationOptions.English : NotationOptions.Italian;
        var key = written[2..];
        var options = new ChordProRenderOptions { TransposeSteps = steps, Accidentals = preference, InputNotation = input, OutputNotation = output };
        var result = RenderKey(key, options);
        var value = result["{key:".Length..^1];

        Assert.All(value, c => Assert.True(c >= ' ' && !char.IsControl(c), $"'{key}' gave a control character in '{value}'"));
        var back = RenderKey(value, new ChordProRenderOptions { TransposeSteps = (12 - ((pitch + steps) % 12)) % 12, Accidentals = AccidentalPreference.Sharps, InputNotation = output });
        var words = back["{key:".Length..^1].Split(' ');
        Assert.True(words[0].ToUpperInvariant() == "C" && words.Length == 2, $"'{key}' +{steps} gave '{value}' which reads back as '{back}'");
    }

    private static IEnumerable<(string Written, int Pitch)> WrittenKeys()
    {
        var english = new[] { ('C', 0, "C"), ('D', 2, "D"), ('E', 4, "E"), ('F', 5, "F"), ('G', 7, "G"), ('A', 9, "A"), ('B', 11, "B") };
        var italian = new[] { ("Do", 0), ("Re", 2), ("Mi", 4), ("Fa", 5), ("Sol", 7), ("La", 9), ("Si", 11) };
        var names = english.Select(e => ("E:", e.Item3, e.Item2, "minor", "major"))
            .Concat(italian.Select(i => ("I:", i.Item1, i.Item2, "minore", "maggiore")));
        foreach (var (prefix, name, pitch, minor, major) in names)
        {
            foreach (var accidental in new[] { "", "#", "b", "♯", "♭", " sharp", " flat", " diesis", " bemolle" })
            {
                var sign = accidental.Contains('#') || accidental.Contains('♯') || accidental.Contains("sharp") || accidental.Contains("diesis") ? 1
                    : accidental.Length == 0 ? 0 : -1;
                var isEnglishWord = accidental is " sharp" or " flat";
                if (isEnglishWord != (prefix == "E:") && accidental.StartsWith(' '))
                    continue;

                foreach (var quality in new[] { minor, major })
                {
                    foreach (var casing in new[] { 0, 1, 2 })
                    {
                        var text = Cased(name + accidental + " " + quality, casing);
                        yield return (prefix + text, ((pitch + sign) % 12 + 12) % 12);
                    }
                }
            }
        }
    }

    private static string Cased(string text, int casing) => casing switch
    {
        0 => text.ToLowerInvariant(),
        1 => char.ToUpperInvariant(text[0]) + text[1..].ToLowerInvariant(),
        _ => text.ToUpperInvariant()
    };
}
