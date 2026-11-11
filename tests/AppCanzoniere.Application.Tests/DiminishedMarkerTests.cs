using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>The bare "o" diminished marker must not turn words or Italian chords into chords.</summary>
public class DiminishedMarkerTests
{
    private static bool IsChord(string text, Notation notation) => ChordSymbolParser.TryParse(text, notation, out _);

    private readonly ChordTransposer _transposer = new();
    private readonly ChordProParser _parser = new();

    [Theory]
    [InlineData("Bo")]
    [InlineData("Bo7")]
    [InlineData("Go7")]
    [InlineData("Go")] // Accepted: a lone "o" after a letter other than D is read as diminished.
    [InlineData("Bo7/D")]
    [InlineData("B°")]
    [InlineData("Dø")]
    [InlineData("Ddim")]
    [InlineData("D°")]
    [InlineData("D°7")]
    [InlineData("Dm")]
    [InlineData("D7")]
    public void English_LowerCaseSingleDiminishedMarkers_AreChords(string text)
    {
        Assert.True(IsChord(text, Notation.English));
    }

    [Theory]
    [InlineData("Do")]
    [InlineData("DO")]
    [InlineData("do")]
    [InlineData("Do7")]
    [InlineData("Do-")]
    [InlineData("Dom")]
    [InlineData("Dom7")]
    [InlineData("Domaj7")]
    [InlineData("Do/Mi")]
    [InlineData("Do#m")]
    [InlineData("bob")]
    [InlineData("Bob")]
    [InlineData("Goo")]
    [InlineData("GO")]
    [InlineData("Boo7")]
    [InlineData("Aom")]
    [InlineData("Bo7o")]
    [InlineData("BO7")]
    [InlineData("Bom")]
    public void English_ItalianDoAndWordsWithO_AreNotChordsAndStayUnchanged(string text)
    {
        Assert.False(IsChord(text, Notation.English));
        Assert.Equal(text, _transposer.TransposeChord(text, 2, false));
    }

    [Theory]
    [InlineData("Solo")]
    [InlineData("SOLO")]
    [InlineData("Mio")]
    [InlineData("Lao")]
    [InlineData("Domo")]
    [InlineData("Reo")]
    [InlineData("Dao")]
    public void Italian_BareOSuffix_IsNotAChord(string text)
    {
        Assert.False(IsChord(text, Notation.Italian));
    }

    [Theory]
    [InlineData("Soldim")]
    [InlineData("Sol°7")]
    [InlineData("Do°")]
    [InlineData("Dom")]
    [InlineData("Sol7")]
    public void Italian_DiminishedSpelledOutOrWithADegreeSign_AreChords(string text)
    {
        Assert.True(IsChord(text, Notation.Italian));
    }

    [Theory]
    [InlineData("Coro")]
    [InlineData("Fine")]
    [InlineData("Amen")]
    [InlineData("Ad lib")]
    [InlineData("Bis")]
    [InlineData("Coda")]
    [InlineData("Fade")]
    [InlineData("Bass")]
    [InlineData("Gesù")]
    [InlineData("Rit")]
    [InlineData("Intro")]
    [InlineData("Godot")]
    [InlineData("Bridge")]
    [InlineData("Chorus")]
    [InlineData("Solo")]
    [InlineData("bob")]
    public void CommonWords_AreNeverChordsInEitherNotation(string text)
    {
        Assert.False(IsChord(text, Notation.English));
        Assert.False(IsChord(text, Notation.Italian));
    }

    [Theory]
    [InlineData("Fa")]
    [InlineData("Fa7")]
    [InlineData("Fam")]
    [InlineData("Lam")]
    [InlineData("La")]
    [InlineData("Sol7")]
    [InlineData("Re")]
    [InlineData("Mi")]
    [InlineData("Si")]
    [InlineData("Solb")]
    [InlineData("Lab")]
    [InlineData("Fa#")]
    public void English_ItalianNames_AreNotChords(string text)
    {
        Assert.False(IsChord(text, Notation.English));
    }

    [Fact]
    public void English_Fadd9_IsStillFAdd9()
    {
        Assert.True(IsChord("Fadd9", Notation.English));
        Assert.Equal("Gadd9", _transposer.TransposeChord("Fadd9", 2, false));
    }

    [Fact]
    public void Render_ItalianSongReadAsEnglish_LeavesAllChordsUnchanged()
    {
        var doc = _parser.Parse("[Do]a [Lam]b [Fa]c [Sol7]d");

        Assert.Equal("[Do]a [Lam]b [Fa]c [Sol7]d\n", _parser.Render(doc, 2));
    }

    [Fact]
    public void Render_ItalianSongWithWordsInBrackets_LeavesThemUnchanged()
    {
        var doc = _parser.Parse("[Solo]a [Dom]b [Mio]c");

        Assert.Equal("[Solo]a [Dom]b [Mio]c\n", _parser.Render(doc, 2));
    }
}
