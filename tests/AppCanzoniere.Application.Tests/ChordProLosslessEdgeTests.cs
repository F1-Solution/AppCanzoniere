using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;
using static AppCanzoniere.Application.Tests.LosslessAssert;

namespace AppCanzoniere.Application.Tests;

public class ChordProLosslessEdgeTests
{
    private readonly ChordProParser _parser = new();

    [Theory]
    [InlineData("# a ChordPro comment\n[C]Line")]
    [InlineData("# [C] chords in a comment\n[C]Line")]
    [InlineData("#no space\n#\n[C]Line")]
    [InlineData("[C]Line\n# trailing comment")]
    [InlineData("  [C]  leading and trailing spaces  ")]
    [InlineData("\t[C]tab\tseparated\t")]
    [InlineData("[ C ]spaced [C ]odd [ C]chords")]
    [InlineData("unclosed [ bracket and [C]chord")]
    [InlineData("empty [] brackets")]
    [InlineData("Text before the first directive\n{title: T}\n[C]Line")]
    [InlineData("﻿{title: T}\n[C]Line")]
    [InlineData("﻿[C]Line")]
    [InlineData("{title:}\n{key:}\n{capo:}\n{tempo:}")]
    [InlineData("{title}\n{t:}")]
    [InlineData("{title: One}\n{title: Two}\n[C]Line")]
    [InlineData("{key: C}\n{key: G}\n[C]Line")]
    [InlineData("[C]Line\n{title: After}\n{key: G}\n{capo: 2}\n{tempo: 80}")]
    [InlineData("[C]Line  \n  \n\t\n[G]Line")]
    public void Oddities_ComeBackAsWritten(string song) => AssertLossless(song);

    [Theory]
    [InlineData("[C]Only lyrics\nSecond line")]
    [InlineData("{title: T}\n{artist: A}\n{key: G}")]
    [InlineData("")]
    [InlineData("\n")]
    [InlineData("\n\n\n")]
    [InlineData("   ")]
    [InlineData("  \n\t\n   ")]
    [InlineData("[N.C.]a [%] [Intro] [x]")]
    [InlineData("{capo: two}\n[C]x")]
    [InlineData("{capo: 15}\n[C]x")]
    [InlineData("{capo: -1}\n[C]x")]
    public void SpecialInputs_ComeBackAsWritten(string song) => AssertLossless(song);

    [Fact]
    public void VeryLongLines_ComeBackAsWritten()
    {
        var lyric = string.Concat(Enumerable.Repeat("[C]word [G]more ", 20_000));
        var directive = "{comment: " + new string('x', 200_000) + "}";

        AssertLossless(lyric + "\n" + directive + "\n" + lyric);
    }

    [Fact]
    public void LineEndings_AreNormalisedToLineFeed()
    {
        Assert.Equal("{title: T}\n[C]a\n\n[G]b\n", RoundTrip("{title: T}\r\n[C]a\r\n\r\n[G]b\r\n"));
        Assert.Equal("[C]a\n[G]b\n", RoundTrip("[C]a\r[G]b"));
    }

    [Fact]
    public void Render_IsIdempotent_OnAWeirdFile()
    {
        const string weird = "﻿# c\n{ Title : X }\n  [ C ]a {b}\r\n{SOC: R.}\n\n{sot}\n{eot}\n{x_y:z}\n[";

        var once = RoundTrip(weird);

        Assert.Equal(once, RoundTrip(once));
    }

    [Fact]
    public void ByteOrderMark_IsIgnoredWhenReadingTheFirstDirective()
    {
        var document = _parser.Parse("﻿{title: T}\n{key: G}\n[G]x");

        Assert.Equal(" T", document.Title);
        Assert.Equal(" G", document.Key);
    }

    [Fact]
    public void ByteOrderMark_StaysFirstWhenACapoLineIsInsertedAtTheTop()
    {
        var result = Render("﻿[N.C.]x", new ChordProRenderOptions { Capo = 2 });

        Assert.Equal("﻿{capo:2}\n[N.C.]x\n", result);
    }

    [Fact]
    public void FileComments_AreNotLyricLines()
    {
        var document = _parser.Parse("# note\n[C]Line\n#more");

        Assert.Single(document.Lines);
        Assert.Equal("Line", document.Lines[0].Text);
    }

    [Fact]
    public void LyricLineWithBracesInTheMiddle_IsALyricLineWithItsChords()
    {
        var document = _parser.Parse("[C]Hello {world} there");

        Assert.Equal("Hello {world} there", document.Lines.Single().Text);
        Assert.Equal("C", document.Lines.Single().Chords.Single().Chord);
    }

    [Fact]
    public void DuplicateDirectives_TypedPropertiesKeepTheLastNonEmptyValue()
    {
        var document = _parser.Parse("{title: One}\n{title: Two}\n{key: C}\n{key: G}\n{capo: 1}\n{capo: 3}");

        Assert.Equal(" Two", document.Title);
        Assert.Equal(" G", document.Key);
        Assert.Equal("3", document.Capo);
    }

    [Fact]
    public void CapoWithANonNumericValue_HasNoFret()
    {
        var document = _parser.Parse("{capo: two}\n[C]x");

        Assert.Equal("two", document.Capo);
        Assert.Null(document.CapoFret);
    }

    [Fact]
    public void NonChords_AreNeverTransposed()
    {
        Assert.Equal("[N.C.]a [%] [Intro] [D]b\n", Render("[N.C.]a [%] [Intro] [C]b", new ChordProRenderOptions { TransposeSteps = 2 }));
    }

    [Fact]
    public void ADocumentBuiltByHand_StillRendersFromItsLines()
    {
        var document = new ChordProDocument
        {
            Title = "T",
            Lines = new List<ChordProLine> { new() { Type = ChordProLineType.Verse, Text = "Hi", Chords = new List<ChordPosition> { new() { Chord = "C", Position = 0 } } } }
        };

        Assert.Equal("{title:T}\n\n[C]Hi\n", _parser.Render(document, 0));
    }
}
