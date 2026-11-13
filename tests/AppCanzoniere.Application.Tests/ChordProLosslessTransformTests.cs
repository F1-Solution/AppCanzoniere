using AppCanzoniere.Application.Music;
using static AppCanzoniere.Application.Tests.LosslessAssert;

namespace AppCanzoniere.Application.Tests;

public class ChordProLosslessTransformTests
{
    private static readonly ChordProRenderOptions UpTwo = new() { TransposeSteps = 2 };

    [Fact]
    public void Transposition_ChangesOnlyChordsAndKey()
    {
        const string song =
            "# [C] stays in a file comment\n" +
            "{ title : Rit. Song }\n" +
            "{tempo: 90}\n" +
            "{ key : C }\n" +
            "{c: [C] in a comment}\n" +
            "{soc: Chorus 2}\n" +
            "  [C]Hello {world} [ Am ]there  \n" +
            "\n" +
            "[N.C.]Rest [%] [Intro] [] [F\n" +
            "{eoc}\n" +
            "{sot: Riff}\n" +
            "  e|--[C]--\n" +
            "{eot}\n" +
            "\ttab\t[G7]x";

        const string expected =
            "# [C] stays in a file comment\n" +
            "{ title : Rit. Song }\n" +
            "{tempo: 90}\n" +
            "{ key : D }\n" +
            "{c: [C] in a comment}\n" +
            "{soc: Chorus 2}\n" +
            "  [D]Hello {world} [ Bm ]there  \n" +
            "\n" +
            "[N.C.]Rest [%] [Intro] [] [F\n" +
            "{eoc}\n" +
            "{sot: Riff}\n" +
            "  e|--[C]--\n" +
            "{eot}\n" +
            "\ttab\t[A7]x\n";

        Assert.Equal(expected, Render(song, UpTwo));
    }

    [Fact]
    public void Transposition_RewritesEveryKeyDirective()
    {
        Assert.Equal("{key:D}\n[D]x\n{key: Em }\n", Render("{key:C}\n[C]x\n{key: Dm }", UpTwo));
    }

    [Fact]
    public void Transposition_EmptyKeyValue_IsKept()
    {
        Assert.Equal("{key:}\n{key}\n[D]x\n", Render("{key:}\n{key}\n[C]x", UpTwo));
    }

    [Fact]
    public void Notation_ChangesChordsAndKeyOnly()
    {
        var options = new ChordProRenderOptions { OutputNotation = NotationOptions.Italian };

        Assert.Equal("{subtitle: x}\n{key: Do}\n[Sol]a\n# [C]\n", Render("{subtitle: x}\n{key: C}\n[G]a\n# [C]", options));
    }

    [Fact]
    public void Capo_ReplacesTheValueInPlace()
    {
        var options = new ChordProRenderOptions { Capo = 4 };

        Assert.Equal("{title: T}\n{ capo : 4 }\n{key:A}\n[F]x\n", Render("{title: T}\n{ capo : 2 }\n{key:A}\n[G]x", options));
    }

    [Fact]
    public void Capo_Zero_RemovesTheCapoLine()
    {
        Assert.Equal("{key:A}\n[A]x\n", Render("{key:A}\n{capo:2}\n[G]x", new ChordProRenderOptions { Capo = 0 }));
    }

    [Fact]
    public void Capo_SameFretAsWritten_KeepsTheLineAsWritten()
    {
        Assert.Equal("{ capo : 2 }\n[G]x\n", Render("{ capo : 2 }\n[G]x", new ChordProRenderOptions { Capo = 2 }));
    }

    [Theory]
    [InlineData("{key:A}\n[A]x", "{key:A}\n{capo:2}\n")]
    [InlineData("{title:T}\n[A]x", "{title:T}\n{capo:2}\n")]
    [InlineData("[A]x", "{capo:2}\n")]
    public void Capo_AddedWhenMissing_GoesAfterTheKeyOrTitleOtherwiseFirst(string song, string expectedStart)
    {
        var result = Render(song, new ChordProRenderOptions { Capo = 2 });

        Assert.StartsWith(expectedStart, result);
        Assert.Equal(1, result.Split("{capo:2}").Length - 1);
    }

    [Fact]
    public void Capo_NonNumericValueAsWritten_IsKeptWhenNothingAsksForACapo()
    {
        Assert.Equal("{capo: two}\n[C]x\n", Render("{capo: two}\n[C]x", new ChordProRenderOptions()));
        Assert.Equal("{capo: two}\n[D]x\n", Render("{capo: two}\n[C]x", UpTwo));
    }
}
