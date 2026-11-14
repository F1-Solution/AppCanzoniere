using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// A whole-line brace line is ChordPro evidence only when its name is a real ChordPro directive.
public class DirectiveListTests
{
    [Theory]
    [InlineData("{rit}", "(rit)")]
    [InlineData("{Rit}", "(Rit)")]
    [InlineData("{coro}", "(coro)")]
    [InlineData("{Coro: tutti}", "(Coro: tutti)")]
    [InlineData("{tutti}", "(tutti)")]
    [InlineData("{solo}", "(solo)")]
    [InlineData("{Amen}", "(Amen)")]
    [InlineData("{Alleluia}", "(Alleluia)")]
    [InlineData("{fine}", "(fine)")]
    [InlineData("{1}", "(1)")]
    [InlineData("{2}", "(2)")]
    [InlineData("{tris}", "(tris)")]
    [InlineData("{2v}", "(2v)")]
    [InlineData("{x100}", "(x100)")]
    [InlineData("{Re}", "(Re)")]
    [InlineData("{Do}", "(Do)")]
    [InlineData("{Lam}", "(Lam)")]
    [InlineData("{x2}", "(x2)")]
    [InlineData("{bis}", "(bis)")]
    public void SongbookBraceLine_DoesNotMarkTheTextAsChordPro(string braceLine, string expectedText)
    {
        var result = Run("Do   Sol", "Santo ascolta", "", braceLine, "", "Lam   Re", "Ti lodo Signore");

        Assert.False(result.WasChordPro);
        Assert.Contains(expectedText, result.ChordPro);
        Assert.DoesNotContain("{", result.ChordPro);
    }

    [Theory]
    [InlineData("{title: Canto nuovo}")]
    [InlineData("{soc}")]
    [InlineData("{ SOC }")]
    [InlineData("{album: Primo}")]
    [InlineData("{columns: 2}")]
    [InlineData("{x_custom: 1}")]
    [InlineData("{comment-guitar: piano}")]
    [InlineData("{c: piano}")]
    [InlineData("{textsize: 12}")]
    [InlineData("{textcolor: red}")]
    [InlineData("{new_physical_page}")]
    public void RealDirective_MarksTheTextAsChordPro(string directive)
    {
        var input = "Do   Sol\nSanto ascolta\n" + directive + "\nLam   Re\nTi lodo Signore";

        var result = ChordSheetConverter.Convert(input);

        Assert.True(result.WasChordPro);
        Assert.Equal(input, result.ChordPro);
    }
}
