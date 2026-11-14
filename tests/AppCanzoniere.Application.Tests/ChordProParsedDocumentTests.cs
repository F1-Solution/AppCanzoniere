using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class ChordProParsedDocumentTests
{
    private readonly ChordProParser _parser = new();

    [Fact]
    public void SourceLineTypes_AreNotPartOfThePublicSurface()
    {
        var assembly = typeof(ChordProParser).Assembly;

        Assert.False(assembly.GetType("AppCanzoniere.Application.Interfaces.ChordProSourceLine")!.IsPublic);
        Assert.False(assembly.GetType("AppCanzoniere.Application.Interfaces.ChordProSourceKind")!.IsPublic);
        Assert.DoesNotContain(typeof(ChordProDocument).GetProperties(), p => p.Name.Contains("Source", StringComparison.Ordinal) && p.Name != "KeepsSourceText");
        Assert.DoesNotContain(typeof(ChordProDocument).GetProperties(), p => p.Name.Contains("Snapshot", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("{key: G} # in G\n[C]x", 2, "{key: G} # in G\n[D]x\n")]
    [InlineData("{title: T} trailing\n[C]x", 0, "{title: T} trailing\n[C]x\n")]
    public void DirectiveFollowedByTrailingText_StaysAPlainTextLine(string song, int steps, string expected)
    {
        var document = _parser.Parse(song);

        Assert.Equal(string.Empty, document.Key);
        Assert.Equal(string.Empty, document.Title);
        Assert.Equal(ChordProLineType.Verse, document.Lines[0].Type);
        Assert.StartsWith("{", document.Lines[0].Text, StringComparison.Ordinal);
        Assert.Equal(expected, _parser.Render(document, new ChordProRenderOptions { TransposeSteps = steps }));
    }

    [Theory]
    [InlineData("﻿Hello world", "Hello world")]
    [InlineData("﻿[C]Hello", "Hello")]
    [InlineData("﻿  Indented", "  Indented")]
    public void ByteOrderMarkBeforeTheFirstLyricLine_IsNotPartOfTheLyric(string song, string text)
    {
        var document = _parser.Parse(song);

        Assert.Equal(text, document.Lines[0].Text);
        Assert.Equal(LosslessAssert.Expected(song), _parser.Render(document));
    }
}
