using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;
using static AppCanzoniere.Application.Tests.LosslessAssert;

namespace AppCanzoniere.Application.Tests;

public class ChordProLosslessCapoTests
{
    private static readonly ChordProRenderOptions CapoThree = new() { Capo = 3 };
    private static readonly ChordProRenderOptions CapoTwo = new() { Capo = 2 };

    private static int? ReparsedCapo(string rendered) => new ChordProParser().Parse(rendered).CapoFret;

    [Theory]
    [InlineData("{capo:}\n[G]x", "{capo:3}\n[E]x\n")]
    [InlineData("{capo}\n[G]x", "{capo:3}\n[E]x\n")]
    [InlineData("{capo: x}\n[G]x", "{capo: 3}\n[E]x\n")]
    [InlineData("{capo: }\n[G]x", "{capo: 3}\n[E]x\n")]
    [InlineData("  {capo}\n[G]x", "  {capo:3}\n[E]x\n")]
    [InlineData("﻿  {capo:}\n[G]x", "﻿  {capo:3}\n[E]x\n")]
    [InlineData("{CAPO:}\n[G]x", "{CAPO:3}\n[E]x\n")]
    [InlineData("{ Capo }\n[G]x", "{ Capo :3}\n[E]x\n")]
    public void CapoLineWithoutAReadableNumber_GetsTheRequestedFret(string song, string expected)
    {
        var result = Render(song, CapoThree);

        Assert.Equal(expected, result);
        Assert.Equal(3, ReparsedCapo(result));
    }

    [Fact]
    public void CapoLineWithoutANumber_IsKeptWhenNoCapoIsRequested()
    {
        Assert.Equal("{capo}\n[G]x\n", Render("{capo}\n[G]x", new ChordProRenderOptions()));
        Assert.Equal("{capo:}\n[G]x\n", Render("{capo:}\n[G]x", new ChordProRenderOptions()));
    }

    [Fact]
    public void CapoLineWithoutANumber_IsRemovedForCapoZero()
    {
        Assert.Equal("[G]x\n", Render("{capo}\n[G]x", new ChordProRenderOptions { Capo = 0 }));
    }

    [Theory]
    [InlineData("{sot}\n{title: Riff}\ne|-0-\n{eot}\n[G]x", "{capo:2}\n{sot}\n{title: Riff}\ne|-0-\n{eot}\n[F]x\n")]
    [InlineData("{sot}\n{title: Riff}\ne|-0-\n[G]x", "{capo:2}\n{sot}\n{title: Riff}\ne|-0-\n[G]x\n")]
    [InlineData("{title: Real}\n{sot}\n{title: Riff}\n{eot}\n[G]x", "{title: Real}\n{capo:2}\n{sot}\n{title: Riff}\n{eot}\n[F]x\n")]
    [InlineData("{sot}\n{title: Riff}\n{eot}\n{title: Real}\n[G]x", "{sot}\n{title: Riff}\n{eot}\n{title: Real}\n{capo:2}\n[F]x\n")]
    public void CapoInserted_NeverLandsInsideATabBlock(string song, string expected)
    {
        var result = Render(song, CapoTwo);

        Assert.Equal(expected, result);
        Assert.Equal(2, ReparsedCapo(result));
    }
}
