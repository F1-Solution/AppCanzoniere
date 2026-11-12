using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Tests;

public class NotationOptionsTests
{
    [Theory]
    [InlineData("it")]
    [InlineData("it-IT")]
    [InlineData("IT")]
    [InlineData("it_IT")]
    [InlineData("it-CH")]
    [InlineData(" it-IT ")]
    public void ForLanguage_Italian_DefaultsToSolfegeWithLam(string tag)
    {
        var options = NotationOptions.ForLanguage(tag);

        Assert.Equal(Notation.Italian, options.Notation);
        Assert.Equal(MinorStyle.LetterM, options.MinorStyle);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("en-US")]
    [InlineData("en-GB")]
    [InlineData("fr")]
    [InlineData("de-DE")]
    [InlineData("es-ES")]
    [InlineData("ita")]
    [InlineData("zz")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ForLanguage_AnyOtherLanguage_DefaultsToLettersWithAm(string? tag)
    {
        var options = NotationOptions.ForLanguage(tag);

        Assert.Equal(Notation.English, options.Notation);
        Assert.Equal(MinorStyle.LetterM, options.MinorStyle);
    }

    [Fact]
    public void ForLanguage_Default_CanBeOverriddenWithoutChangingTheOriginal()
    {
        var italian = NotationOptions.ForLanguage("it");

        var dash = italian with { MinorStyle = MinorStyle.Dash };
        var letters = italian with { Notation = Notation.English };

        Assert.Equal(NotationOptions.ItalianDash, dash);
        Assert.Equal(NotationOptions.English, letters);
        Assert.Equal(NotationOptions.Italian, italian);
    }

    [Theory]
    [InlineData("it", "Am7", "Lam7")]
    [InlineData("it-IT", "C/E", "Do/Mi")]
    [InlineData("en-US", "Am7", "Am7")]
    [InlineData("fr", "Bbmaj7", "Bbmaj7")]
    public void ForLanguage_DrivesTheOutputOfAConversion(string tag, string english, string expected)
    {
        var result = ChordNotation.Convert(english, NotationOptions.English, NotationOptions.ForLanguage(tag));

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ExplicitOverride_WinsOverTheLanguageDefault()
    {
        var italianWithDash = NotationOptions.ForLanguage("it") with { MinorStyle = MinorStyle.Dash };

        Assert.Equal("La-7", ChordNotation.Convert("Am7", NotationOptions.English, italianWithDash));
    }
}
