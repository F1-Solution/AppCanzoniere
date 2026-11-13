using AppCanzoniere.Application.Layout;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>The parser marks where explicit sections start and which comments sit inside an open section.</summary>
public class ParsedSectionFlagsTests
{
    private readonly ChordProParser _parser = new();

    [Fact]
    public void FirstLineOfEachExplicitSection_IsMarked_AndCommentsInsideAreFlagged()
    {
        var document = _parser.Parse("{soc}\n[C]uno\n{c: x}\n[C]due\n{eoc}\nverso\n{sob}\n{c: lab}\nponte\n{eob}\n{c: fuori}");

        Assert.Equal(new[] { true, false, false, false, true, false, false }, document.Lines.Select(l => l.StartsSection));
        Assert.Equal(new[] { false, true, false, false, true, false, false }, document.Lines.Select(l => l.InOpenSection));
    }

    [Fact]
    public void LayoutSource_CarriesTheFlags()
    {
        var source = LayoutSource.From(_parser.Parse("{soc}\n[C]uno\n{c: x}\n{eoc}"));

        Assert.Equal(new[] { true, false, false }, source.Lines.Select(l => l.StartsSection));
        Assert.Equal(new[] { false, false, true }, source.Lines.Select(l => l.InOpenSection));
    }

    [Fact]
    public void ChangingAFlag_CountsAsAnEdit()
    {
        var document = _parser.Parse("# nota\n{soc}\n[C]uno\n{eoc}");
        Assert.Equal("# nota\n{soc}\n[C]uno\n{eoc}\n", _parser.Render(document));

        document.Lines[0].StartsSection = false;

        Assert.DoesNotContain("# nota", _parser.Render(document));
    }

    [Fact]
    public void RenderingAParsedDocument_IsLosslessWhateverTheFlags()
    {
        const string song = "{soc}\n[C]uno\n{c: x2}\n{eoc}\n{c: Rit. 2}\n{soc}\n[C]due\n{eoc}";

        LosslessAssert.AssertLossless(song);
    }
}
