using static AppCanzoniere.Application.Tests.LosslessAssert;

namespace AppCanzoniere.Application.Tests;

public class ChordProLosslessTests
{
    [Theory]
    [InlineData("{tempo: 120}")]
    [InlineData("{time: 3/4}")]
    [InlineData("{subtitle: A sub}")]
    [InlineData("{st: A sub}")]
    [InlineData("{composer: Someone}")]
    [InlineData("{lyricist: Someone}")]
    [InlineData("{album: Some album}")]
    [InlineData("{year: 1999}")]
    [InlineData("{copyright: (c) 2000}")]
    [InlineData("{duration: 3:45}")]
    [InlineData("{meta: x y}")]
    [InlineData("{define: Am base-fret 1 frets x 0 2 2 1 0}")]
    [InlineData("{chord: Am}")]
    [InlineData("{new_page}")]
    [InlineData("{np}")]
    [InlineData("{column_break}")]
    [InlineData("{colb}")]
    [InlineData("{columns: 2}")]
    [InlineData("{textsize: 12}")]
    [InlineData("{textfont: Courier}")]
    [InlineData("{chordcolour: red}")]
    [InlineData("{start_of_verse}\n[C]One\n{end_of_verse}")]
    [InlineData("{sov}\n[C]One\n{eov}")]
    [InlineData("{start_of_bridge}\n[C]One\n{end_of_bridge}")]
    [InlineData("{sob}\n[C]One\n{eob}")]
    [InlineData("{start_of_grid}\n| Am . . . | C . . . |\n{end_of_grid}")]
    [InlineData("{chorus}")]
    [InlineData("{comment_italic: Soft}")]
    [InlineData("{ci: Soft}")]
    [InlineData("{comment_box: Boxed}")]
    [InlineData("{cb: Boxed}")]
    [InlineData("{highlight: Look}")]
    [InlineData("{x_custom: anything}")]
    [InlineData("{comment-guitar: x}")]
    [InlineData("{c: Hello}")]
    [InlineData("{comment: Hello}")]
    public void Directive_IsWrittenBackAsWritten(string song) => AssertLossless(song);

    [Theory]
    [InlineData("[C]Hello {world} there")]
    [InlineData("  [C]Hello {world}")]
    [InlineData("{world}")]
    [InlineData("Say {hi} and {bye}")]
    public void LyricLineWithBraces_IsKept(string song) => AssertLossless(song);

    [Theory]
    [InlineData("{start_of_chorus: Rit.}\n[C]Chorus\n{end_of_chorus}")]
    [InlineData("{soc: Chorus 2}\n[C]Chorus\n{eoc}")]
    [InlineData("{sov: Verse 1}\n[C]Verse\n{eov}")]
    [InlineData("{soc}\n[C]Chorus\n{end_of_chorus}")]
    [InlineData("{start_of_chorus}\n[C]Chorus\n{eoc}")]
    [InlineData("{SOC}\n[C]Chorus\n{EOC}")]
    [InlineData("{ title : X }")]
    [InlineData("{ Soc }\n[C]Chorus\n{ eoc }")]
    [InlineData("{T:X}\n{C:Y}")]
    public void SectionDirectives_KeepLabelAliasCaseAndSpacing(string song) => AssertLossless(song);

    [Theory]
    [InlineData("{soc}\n[C]One\n\n[G]Two\n{eoc}")]
    [InlineData("{soc}\n[C]One\n{comment: Slowly}\n[G]Two\n{eoc}")]
    [InlineData("{soc}\n\n[C]One\n{eoc}")]
    [InlineData("{soc}\n[C]One\n\n{eoc}")]
    [InlineData("{sob}\n[C]One\n\n{comment: x}\n{eob}")]
    public void BlankLineAndCommentInsideSection_StayInside(string song) => AssertLossless(song);

    [Theory]
    [InlineData("{soc}\n[C]One\n{sot}\ne|--0--\n{eot}\n[G]Two\n{eoc}")]
    [InlineData("{sot: Riff}\n{eot}")]
    [InlineData("{start_of_tab: Riff}\n{end_of_tab}\n[C]After")]
    [InlineData("{sot}\ne|--0--\n\ne|--1--\n{eot}")]
    [InlineData("{sot}\ne|--0--\n{eot}\n{sot}\ne|--1--\n{eot}")]
    public void TabBlocks_KeepStructure(string song) => AssertLossless(song);

    [Theory]
    [InlineData("{title: T}\n{artist: A}\n[C]Line")]
    [InlineData("{key: G}\n{title: T}\n[C]Line")]
    [InlineData("{title: T}\n\n[C]Line")]
    [InlineData("{title: T}\n\n\n[C]Line")]
    [InlineData("{artist: A}\n{capo: 2}\n{key: G}\n{title: T}\n{tempo: 90}\n[C]Line")]
    [InlineData("{title: T}\n{key: C}\n{capo: 1}\n[C]Line")]
    public void HeaderBlankLineAndOrder_AreAsWritten(string song) => AssertLossless(song);

    [Fact]
    public void FinalNewlineRule_EveryRenderedLineEndsWithOneLineFeed()
    {
        Assert.Equal("[C]Line\n", RoundTrip("[C]Line"));
        Assert.Equal("[C]Line\n", RoundTrip("[C]Line\n"));
        Assert.Equal("[C]Line\n\n", RoundTrip("[C]Line\n\n"));
        Assert.Equal("", RoundTrip(""));
        Assert.Equal("[C]Line\n\n", RoundTrip("[C]Line\r\n\r\n"));
        Assert.Equal("a\nb\n", RoundTrip("a\rb"));
    }
}
