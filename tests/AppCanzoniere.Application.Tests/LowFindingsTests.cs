using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Doubled accidentals before a tension, empty directive values, null chord lists and pinned directive behaviours.</summary>
public class LowFindingsTests
{
    private readonly ChordTransposer _transposer = new();
    private readonly ChordProParser _parser = new();

    [Theory]
    [InlineData("Bbb5", AccidentalPreference.Flats, "C(b5)")]
    [InlineData("Bbb9", AccidentalPreference.Flats, "C(b9)")]
    [InlineData("Abb5", AccidentalPreference.Flats, "Bbb5")]
    [InlineData("Cbb5", AccidentalPreference.Flats, "Dbb5")]
    [InlineData("F##5", AccidentalPreference.Sharps, "G##5")]
    [InlineData("F#b5", AccidentalPreference.Sharps, "G#b5")]
    public void DoubledAccidentalBeforeATension_IsOneAccidentalAndAnAlteredTension(string chord, AccidentalPreference preference, string expected)
    {
        Assert.Equal(expected, _transposer.TransposeChord(chord, 2, preference));
    }

    [Theory]
    [InlineData("Bbb")]
    [InlineData("C##")]
    [InlineData("Dbb7")]
    [InlineData("C##x")]
    public void PlainDoubledAccidentals_AreStillNotChords(string text)
    {
        Assert.Equal(text, _transposer.TransposeChord(text, 2, false));
    }

    [Fact]
    public void Parse_EmptyTitleOrKey_DoesNotEraseThePreviousValue()
    {
        var document = _parser.Parse("{title:Real}\n{key:G}\n{title:}\n{key:}\n{title}\n[G]x");

        Assert.Equal("Real", document.Title);
        Assert.Equal("G", document.Key);
        Assert.Equal("{title:Real}\n{key:G}\n{title:}\n{key:}\n{title}\n[G]x\n", _parser.Render(document, 0));
    }

    [Fact]
    public void Render_LineWithoutAChordList_IsRenderedAsPlainText()
    {
        var document = new ChordProDocument();
        document.Lines.Add(new ChordProLine { Type = ChordProLineType.Verse, Text = "plain", Chords = null! });

        Assert.Equal("plain\n", _parser.Render(document, 2));
    }

    [Fact]
    public void Render_TabSample_ComesBackAsWrittenAndReparsingKeepsIt()
    {
        const string sample = "{title:Intro}\n[C]Verse line\n\n{start_of_tab}\n  e|--0--[3]--|  \nB|---1-------|\n{end_of_tab}\n[G]After the tab";

        var rendered = _parser.Render(_parser.Parse(sample), 0);

        Assert.Equal(sample + "\n", rendered);
        Assert.Equal(rendered, _parser.Render(_parser.Parse(rendered), 0));
    }

    [Fact]
    public void Parse_DirectivesWithInnerSpaces_AreDirectives()
    {
        var document = _parser.Parse("{ title : X }\n{ soc }\n[C]a\n{ eoc }\n{ c : x }");

        Assert.Equal("X", document.Title.Trim());
        Assert.Equal(
            new[] { ChordProLineType.Title, ChordProLineType.Chorus, ChordProLineType.Comment },
            document.Lines.Select(l => l.Type));
        Assert.Equal("x", document.Lines[^1].Text.Trim());
    }

    [Fact]
    public void Render_TitleLineInsideAChorus_DoesNotSplitIt()
    {
        var rendered = _parser.Render(_parser.Parse("{soc}\n[C]a\n{title:Mid}\n[G]b\n{eoc}"), 0);

        Assert.Equal("{soc}\n[C]a\n{title:Mid}\n[G]b\n{eoc}\n", rendered);
    }
}
