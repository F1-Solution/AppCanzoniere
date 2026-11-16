using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class CapoGuessedKeyMixedAccidentalsTests
{
    [Fact]
    public void CapoWithoutKey_MayMixFlatsAndSharps()
    {
        // DEC 27: with a capo and no {key}, mixing sharps and flats is allowed.
        var parser = new ChordProParser();
        var song = parser.Parse("[Am]a [Dm]b [G]c [C]d [Ab]e");

        var result = parser.Render(song, new ChordProRenderOptions { Capo = 2, Accidentals = AccidentalPreference.Auto });

        Assert.Equal("{capo:2}\n[Gm]a [Cm]b [F]c [Bb]d [F#]e\n", result);
    }
}
