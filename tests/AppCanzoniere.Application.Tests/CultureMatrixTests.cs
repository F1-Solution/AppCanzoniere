using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Notation and directive reading is culture independent; only these tests change the culture, and they restore it.</summary>
public class CultureMatrixTests
{
    private readonly ChordProParser _parser = new();
    private readonly ChordTransposer _transposer = new();

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
    [InlineData("tr-TR")]
    [InlineData("it-IT")]
    [InlineData("en-US")]
    [InlineData("")]
    public void NotationAndDirectives_ReadTheSameUnderEveryCulture(string culture)
    {
        UnderCulture(culture, () =>
        {
            Assert.Equal("B", ChordNotation.Convert("SI", NotationOptions.Italian, NotationOptions.English));
            Assert.Equal("Em", ChordNotation.Convert("MIm", NotationOptions.Italian, NotationOptions.English));
            Assert.Equal("DoDIM", ChordNotation.Convert("CDIM", NotationOptions.English, NotationOptions.Italian));
            Assert.Equal("Dom7", ChordNotation.Convert("CMIN7", NotationOptions.English, NotationOptions.Italian));
            Assert.Equal("D", _transposer.TransposeChord("C", 2, false));

            var document = _parser.Parse("{TITLE:Song}\n{KEY:C}\n{SOT}\ne|--0--\n{EOT}");
            Assert.Equal("Song", document.Title);
            Assert.Equal("C", document.Key);
            Assert.Equal(ChordProLineType.Tab, document.Lines.Last().Type);
        });
    }

    [Theory]
    [InlineData(typeof(ChordSymbolParser))]
    [InlineData(typeof(ChordProParser))]
    public void EveryStaticPattern_IsCultureInvariant(Type type)
    {
        // Static patterns capture the culture when first built, so the option itself is what keeps them stable.
        var patterns = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(f => f.FieldType == typeof(Regex))
            .Select(f => (Regex)f.GetValue(null)!)
            .ToList();

        Assert.NotEmpty(patterns);
        Assert.All(patterns, r => Assert.True(r.Options.HasFlag(RegexOptions.CultureInvariant), r.ToString()));
    }
}
