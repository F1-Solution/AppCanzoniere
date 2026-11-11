using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class ChordProIdempotenceTests
{
    private readonly ChordProParser _parser = new();

    private string Cycle(string text) => _parser.Render(_parser.Parse(text), 0);

    public static IEnumerable<object[]> Songs() => new[]
    {
        new object[] { "[C]Line" },
        new object[] { "[C]Line\n" },
        new object[] { "[C]Line\n\n" },
        new object[] { "{title:T}\n{key:G}\n[G]One\n[D]Two" },
        new object[] { "{title:T}\n\n[G]One\n" },
        new object[] { "{title:T}\n\n\n[G]One\n" },
        new object[] { "{key:G}\n[G]One" },
        new object[] { "{title:T}\r\n[C]One\r\n\r\n[G]Two\r\n" },
        new object[] { "[C]Verse\n{soc}\n[G]Chorus\n\n[D]More\n{eoc}\n[Am]After" },
        new object[] { "{soc}\nA\n{title:Late}\nB\n{eoc}" },
        new object[] { "[C]Verse\n{sot}\ne|--0--\n\n{eot}\n[G]After\n" },
        new object[] { "{sot: Riff}\ne|--0--" },
        new object[] { "{sot}\ne|--0--\n{eot}\n{sot}\ne|--1--\n{eot}" },
        new object[] { "{comment:Hi}\n{sob}\n[C]Bridge\n{eob}\n" },
        new object[] { "\n\n[C]Leading blanks" }
    };

    [Theory]
    [MemberData(nameof(Songs))]
    public void RenderOfParse_IsStableAfterTheFirstCycle(string song)
    {
        var first = Cycle(song);

        Assert.Equal(first, Cycle(first));
        Assert.Equal(first, Cycle(Cycle(first)));
    }

    [Theory]
    [InlineData("[C]Line")]
    [InlineData("{title:T}\n\n[C]Line\n")]
    [InlineData("{start_of_chorus}\n[C]Chorus\n{end_of_chorus}\n")]
    public void CanonicalText_ComesBackUnchanged(string canonical)
    {
        Assert.Equal(canonical.EndsWith('\n') ? canonical : canonical + "\n", Cycle(canonical));
    }

    [Fact]
    public void Render_WithoutMetadata_HasNoLeadingBlankLine()
    {
        Assert.Equal("[C]Line\n", Cycle("[C]Line"));
    }

    [Fact]
    public void Render_SingleTerminatingNewline_IsNotAnExtraEmptyLine()
    {
        Assert.Equal("[C]Line\n", Cycle("[C]Line\n"));
        Assert.Equal("[C]Line\n\n", Cycle("[C]Line\n\n"));
    }
}
