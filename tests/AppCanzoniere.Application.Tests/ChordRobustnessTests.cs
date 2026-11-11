using System.Globalization;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class ChordRobustnessTests
{
    private readonly ChordTransposer _transposer = new();
    private readonly ChordProParser _parser = new();

    private static void UnderCulture(string name, Action action)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(name);
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Theory]
    [InlineData("SI", "B")]
    [InlineData("MI", "E")]
    [InlineData("MIm", "Em")]
    [InlineData("SIM", "BM")]
    [InlineData("FA", "F")]
    [InlineData("Mi-", "Em")]
    public void Italian_UpperCaseNames_AreReadTheSameInTurkishCulture(string italian, string english)
    {
        UnderCulture("tr-TR", () =>
            Assert.Equal(english, ChordNotation.Convert(italian, NotationOptions.Italian, NotationOptions.English)));
    }

    [Theory]
    [InlineData("CMIN7", "Dom7")]
    [InlineData("CMI7", "Dom7")]
    [InlineData("CDIM", "DoDIM")]
    [InlineData("CMAJ7", "DoMAJ7")]
    [InlineData("CSUS4", "DoSUS4")]
    public void English_UpperCaseQualities_AreReadTheSameInTurkishCulture(string english, string italian)
    {
        UnderCulture("tr-TR", () =>
            Assert.Equal(italian, ChordNotation.Convert(english, NotationOptions.English, NotationOptions.Italian)));
    }

    [Fact]
    public void Directives_AreReadTheSameInTurkishCulture()
    {
        UnderCulture("tr-TR", () =>
        {
            var document = _parser.Parse("{TITLE:Song}\n{SOT}\ne|--0--\n{EOT}");

            Assert.Equal("Song", document.Title);
            Assert.Equal(ChordProLineType.Tab, document.Lines.Last().Type);
        });
    }

    [Theory]
    [InlineData(int.MaxValue, "G")]
    [InlineData(int.MinValue, "E")]
    [InlineData(12, "C")]
    [InlineData(-13, "B")]
    public void Steps_AreReducedModuloTwelve(int steps, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord("C", steps, AccidentalPreference.Sharps));
        Assert.Equal(expected, _transposer.TransposeChord("C", steps, AccidentalPreference.Auto, "C"));
        Assert.Equal(expected, _transposer.TransposeKey("C", steps, AccidentalPreference.Sharps));
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Render_ExtremeTransposition_DoesNotOverflow(int steps)
    {
        var options = new ChordProRenderOptions { TransposeSteps = steps, Capo = 3, Accidentals = AccidentalPreference.Auto };

        var rendered = _parser.Render(_parser.Parse("{key:C}\n[C]a"), options);

        Assert.StartsWith("{key:", rendered);
        Assert.DoesNotContain("[C]", rendered);
    }

    [Theory]
    [InlineData("Bbb")]
    [InlineData("C##")]
    [InlineData("C♯♯")]
    [InlineData("Dbb7")]
    [InlineData("C##m")]
    public void DoubleAccidentals_AreNotChordsAndStayUnchanged(string text)
    {
        Assert.Equal(text, _transposer.TransposeChord(text, 2, AccidentalPreference.Sharps));
        Assert.Equal(text, _transposer.TransposeChord(text, 2, AccidentalPreference.Auto));
        Assert.False(ChordSymbolParser.TryParse(text, Notation.English, out _));
    }

    [Theory]
    [InlineData("Sibb")]
    [InlineData("Do##")]
    public void DoubleAccidentals_InItalian_StayUnchanged(string text)
    {
        Assert.Equal(text, ChordNotation.Convert(text, NotationOptions.Italian, NotationOptions.English));
    }

    [Fact]
    public void Render_NullDocument_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _parser.Render(null!, new ChordProRenderOptions()));
        Assert.Throws<ArgumentNullException>(() => _parser.Render(null!, 2));
    }

    [Fact]
    public void Render_NullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _parser.Render(new ChordProDocument(), (ChordProRenderOptions)null!));
    }
}
