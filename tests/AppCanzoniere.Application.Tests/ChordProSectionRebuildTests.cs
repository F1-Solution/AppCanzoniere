using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Text rebuilt from the typed lines keeps tab blocks whole and writes each section with the kind and extent its lines indicate.</summary>
public class ChordProSectionRebuildTests
{
    private readonly ChordProParser _parser = new();

    private string RenderEditingLastLine(string song, string text)
    {
        var document = _parser.Parse(song);
        document.Lines[^1].Text = text;
        return _parser.Render(document);
    }

    [Fact]
    public void ATabLineWithDefaultFlagsInsideATabBlockOfAChorus_DoesNotSplitTheBlock()
    {
        var document = _parser.Parse("{soc}\n[C]a\n{sot}\n-1-\n-2-\n-3-\n{eot}\n[G]b\n{eoc}\n");
        var before = document.Lines.Select(l => l.Type).ToList();
        document.Lines[2] = new ChordProLine { Type = ChordProLineType.Tab, Text = "-9-" };

        var written = _parser.Render(document);
        var again = _parser.Parse(written);

        Assert.Equal(before, again.Lines.Select(l => l.Type).ToList());
        Assert.Equal("{start_of_chorus}\n[C]a\n{start_of_tab}\n-1-\n-9-\n-3-\n{end_of_tab}\n[G]b\n{end_of_chorus}\n", written);
        Assert.Single(written.Split('\n'), l => l == "{start_of_chorus}");
    }

    [Fact]
    public void ASectionOpenedByABlankLine_IsNotReopenedByItsFirstLyricLine()
    {
        var written = RenderEditingLastLine("{start_of_chorus: Rit}\n\n[C]a\n{eoc}\n", "b");

        Assert.Equal("{start_of_chorus}\n\n[C]b\n{end_of_chorus}\n", written);
    }

    [Fact]
    public void ASectionOpenedByATabBlock_IsNotReopenedByItsFirstLyricLine()
    {
        var written = RenderEditingLastLine("{sob}\n{sot}\n-1-\n{eot}\n[G]a\n{eob}\n", "b");

        Assert.Equal("{start_of_bridge}\n{start_of_tab}\n-1-\n{end_of_tab}\nb\n{end_of_bridge}\n", written.Replace("[G]", string.Empty));
    }

    [Fact]
    public void ATabAtTheStartOfABridgeThatFollowsAChorus_StaysInTheBridge()
    {
        var written = RenderEditingLastLine("{soc}\n[C]a\n{eoc}\n{sob}\n{sot}\n-1-\n{eot}\n[G]b\n{eob}\n", "c");

        Assert.Equal(
            "{start_of_chorus}\n[C]a\n{end_of_chorus}\n{start_of_bridge}\n{start_of_tab}\n-1-\n{end_of_tab}\n[G]c\n{end_of_bridge}\n",
            written);
    }

    [Fact]
    public void ABridgeOpenedByAComment_IsWrittenAsABridge()
    {
        var written = RenderEditingLastLine("{sob}\n{c: solo}\n[C]a\n{eob}\n", "b");

        Assert.Equal("{start_of_bridge}\n{comment: solo}\n[C]b\n{end_of_bridge}\n", written);
    }
}
