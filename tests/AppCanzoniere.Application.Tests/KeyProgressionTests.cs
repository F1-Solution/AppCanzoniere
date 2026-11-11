using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Every key renders a progression (I IV V vi bVII, or i iv V bVI) with a capo; the expected shapes are written by hand.</summary>
public class KeyProgressionTests
{
    private readonly ChordProParser _parser = new();

    // Sounding key, sounding progression, hand-written shapes with capo 2.
    // Shapes come out in the key lowered by two semitones; F#/Gb major is spelled with sharps.
    private static readonly string[][] Rows =
    {
        new[] { "C", "C F G Am Bb", "Bb Eb F Gm Ab" },
        new[] { "Db", "Db Gb Ab Bbm Cb", "B E F# G#m A" },
        new[] { "D", "D G A Bm C", "C F G Am Bb" },
        new[] { "Eb", "Eb Ab Bb Cm Db", "Db Gb Ab Bbm B" },
        new[] { "E", "E A B C#m D", "D G A Bm C" },
        new[] { "F", "F Bb C Dm Eb", "Eb Ab Bb Cm Db" },
        new[] { "F#", "F# B C# D#m E", "E A B C#m D" },
        new[] { "G", "G C D Em F", "F Bb C Dm Eb" },
        new[] { "Ab", "Ab Db Eb Fm Gb", "F# B C# D#m E" },
        new[] { "A", "A D E F#m G", "G C D Em F" },
        new[] { "Bb", "Bb Eb F Gm Ab", "Ab Db Eb Fm Gb" },
        new[] { "B", "B E F# G#m A", "A D E F#m G" },
        new[] { "Cm", "Cm Fm G Ab", "Bbm Ebm F Gb" },
        new[] { "C#m", "C#m F#m G# A", "Bm Em F# G" },
        new[] { "Dm", "Dm Gm A Bb", "Cm Fm G Ab" },
        new[] { "Ebm", "Ebm Abm Bb Cb", "C#m F#m G# A" },
        new[] { "Em", "Em Am B C", "Dm Gm A Bb" },
        new[] { "Fm", "Fm Bbm C Db", "Ebm Abm Bb B" },
        new[] { "F#m", "F#m Bm C# D", "Em Am B C" },
        new[] { "Gm", "Gm Cm D Eb", "Fm Bbm C Db" },
        new[] { "G#m", "G#m C#m D# E", "F#m Bm C# D" },
        new[] { "Am", "Am Dm E F", "Gm Cm D Eb" },
        new[] { "Bbm", "Bbm Ebm F Gb", "G#m C#m D# E" },
        new[] { "Bm", "Bm Em F# G", "Am Dm E F" }
    };

    public static IEnumerable<object[]> Keys => Rows.Select(r => new object[] { r[0], r[1], r[2] });

    private static string Chords(string progression) =>
        string.Join(" ", progression.Split(' ').Select(c => $"[{c}]x"));

    private static string Song(string key, string progression) => $"{{key:{key}}}\n{Chords(progression)}";

    private static string Expected(string key, string capo, string progression) =>
        $"{{key:{key}}}\n{capo}\n\n{Chords(progression)}\n";

    // Test-local solfege writer so the Italian expectations do not depend on the code under test.
    private static string Solfege(string text) =>
        Regex.Replace(text, "[A-G]", m => m.Value switch
        {
            "C" => "Do",
            "D" => "Re",
            "E" => "Mi",
            "F" => "Fa",
            "G" => "Sol",
            "A" => "La",
            _ => "Si"
        });

    private string Render(string song, ChordProRenderOptions options) =>
        _parser.Render(_parser.Parse(song), options with { Accidentals = AccidentalPreference.Auto });

    [Theory]
    [MemberData(nameof(Keys))]
    public void English_CapoTwo_ShowsTheHandWrittenShapes(string key, string progression, string shapes)
    {
        var result = Render(Song(key, progression), new ChordProRenderOptions { Capo = 2 });

        Assert.Equal(Expected(key, "{capo:2}", shapes), result);
    }

    [Theory]
    [MemberData(nameof(Keys))]
    public void Italian_CapoTwo_ShowsTheHandWrittenShapesInSolfege(string key, string progression, string shapes)
    {
        var options = new ChordProRenderOptions { Capo = 2, OutputNotation = NotationOptions.Italian };

        var result = Render(Song(key, progression), options);

        Assert.Equal(Solfege(Expected(key, "{capo:2}", shapes)), result);
    }
}
