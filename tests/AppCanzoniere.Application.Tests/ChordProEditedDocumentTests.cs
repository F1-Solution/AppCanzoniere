using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>A parsed document renders byte for byte until a caller edits it; then it renders from its typed members.</summary>
public class ChordProEditedDocumentTests
{
    private const string Song = "{title:T}\n{artist:A}\n{key:C}\n{capo:1}\n{tempo:90}\n{time:3/4}\n[C]Line\n";
    private const string Head = "{title:T}\n{artist:A}\n{key:C}\n{capo:1}\n{tempo:90}\n{time:3/4}\n";

    private readonly ChordProParser _parser = new();

    private static readonly string[] HeaderCases = { "title", "artist", "key", "capo", "tempo", "time" };

    public static IEnumerable<object[]> BodyEdits() => Edits().Where(e => !HeaderCases.Contains((string)e[0]));

    public static IEnumerable<object[]> Edits() => new[]
    {
        Case("title", d => d.Title = "New", "{title:New}\n{artist:A}\n{key:C}\n{capo:1}\n{tempo:90}\n{time:3/4}\n[C]Line\n"),
        Case("artist", d => d.Artist = "B", "{title:T}\n{artist:B}\n{key:C}\n{capo:1}\n{tempo:90}\n{time:3/4}\n[C]Line\n"),
        Case("key", d => d.Key = "G", "{title:T}\n{artist:A}\n{key:G}\n{capo:1}\n{tempo:90}\n{time:3/4}\n[C]Line\n"),
        Case("capo", d => d.Capo = "2", "{title:T}\n{artist:A}\n{key:C}\n{capo:2}\n{tempo:90}\n{time:3/4}\n[C]Line\n"),
        Case("tempo", d => d.Tempo = "100", "{title:T}\n{artist:A}\n{key:C}\n{capo:1}\n{tempo:100}\n{time:3/4}\n[C]Line\n"),
        Case("time", d => d.Time = "4/4", "{title:T}\n{artist:A}\n{key:C}\n{capo:1}\n{tempo:90}\n{time:4/4}\n[C]Line\n"),
        Case("line text", d => d.Lines[1].Text = "Other", Head + "\n[C]Other\n"),
        Case("line type", d => d.Lines[1].Type = ChordProLineType.Chorus, Head + "\n{start_of_chorus}\n[C]Line\n{end_of_chorus}\n"),
        Case("chord name", d => d.Lines[1].Chords[0].Chord = "G", Head + "\n[G]Line\n"),
        Case("chord position", d => d.Lines[1].Chords[0].Position = 2, Head + "\nLi[C]ne\n"),
        Case("added chord", d => d.Lines[1].Chords.Add(new ChordPosition { Chord = "G", Position = 2 }), Head + "\n[C]Li[G]ne\n"),
        Case("removed chord", d => d.Lines[1].Chords.Clear(), Head + "\nLine\n"),
        Case("added line", d => d.Lines.Add(new ChordProLine { Type = ChordProLineType.Verse, Text = "More" }), Head + "\n[C]Line\nMore\n"),
        Case("removed line", d => d.Lines.RemoveAt(1), Head),
        Case("replaced lines", d => d.Lines = new List<ChordProLine> { new() { Type = ChordProLineType.Verse, Text = "New" } }, Head + "\nNew\n")
    };

    private static object[] Case(string name, Action<ChordProDocument> edit, string expected) => new object[] { name, edit, expected };

    [Theory]
    [MemberData(nameof(Edits))]
    public void EditedDocument_ShowsTheEditInTheRenderedText(string name, Action<ChordProDocument> edit, string expected)
    {
        var document = _parser.Parse(Song);

        edit(document);

        Assert.True(Song != expected, name);
        Assert.Equal(expected, _parser.Render(document));
    }

    [Theory]
    [MemberData(nameof(BodyEdits))]
    public void EditedDocument_RendersLikeAHandBuiltOne(string name, Action<ChordProDocument> edit, string expected)
    {
        var document = _parser.Parse(Song);
        edit(document);

        var handBuilt = new ChordProDocument
        {
            Title = document.Title,
            Artist = document.Artist,
            Key = document.Key,
            Capo = document.Capo,
            Tempo = document.Tempo,
            Time = document.Time,
            Lines = document.Lines
        };

        Assert.Equal(_parser.Render(handBuilt), _parser.Render(document));
        Assert.Equal(expected, _parser.Render(handBuilt));
        Assert.NotEmpty(name);
    }

    [Fact]
    public void EditedDocument_StillFollowsTheRenderOptions()
    {
        var document = _parser.Parse(Song);
        document.Title = "New";

        var result = _parser.Render(document, new ChordProRenderOptions { TransposeSteps = 2 });

        Assert.Equal("{title:New}\n{artist:A}\n{key:D}\n{capo:1}\n{tempo:90}\n{time:3/4}\n[D]Line\n", result);
    }

    [Fact]
    public void UneditedDocument_IsStillByteForByte()
    {
        const string weird = "﻿# c\n{ Title : X }\n  [ C ]a {b}\r\n{SOC: R.}\n\n{sot}\n{eot}\n{x_y:z}\n[";
        var document = _parser.Parse(weird);

        Assert.Equal(LosslessAssert.Expected(weird), _parser.Render(document));
        Assert.Equal(LosslessAssert.Expected(weird), _parser.Render(document));
    }

    [Fact]
    public void EditThenRestore_IsUneditedAgain()
    {
        var document = _parser.Parse("{ title : X }\n[C]a");
        document.Title = "Other";
        document.Title = " X ";

        Assert.Equal("{ title : X }\n[C]a\n", _parser.Render(document));
    }

    [Fact]
    public void EditingOneDocument_DoesNotAffectAnotherParsedFromTheSameText()
    {
        var edited = _parser.Parse(Song);
        var untouched = _parser.Parse(Song);

        edited.Title = "New";

        Assert.Equal(Song, _parser.Render(untouched));
    }

    [Fact]
    public void RenderingAnEditedDocument_DoesNotChangeIt()
    {
        var document = _parser.Parse(Song);
        document.Key = "D";

        var first = _parser.Render(document);

        Assert.Equal(first, _parser.Render(document));
        Assert.Equal("D", document.Key);
    }

    [Fact]
    public void ChecksOnALargeDocument_StayLinear()
    {
        // Quadratic checks would cost 16x for 4x the lines, above the 8x allowed by the scaling check.
        ScalingAssert.StaysLinear(
            lines =>
            {
                var song = string.Concat(Enumerable.Repeat("[C]Some [G]words here\n", lines));
                var document = _parser.Parse(song);
                return () =>
                {
                    var last = document.Lines[^1];
                    var unedited = _parser.Render(document);
                    last.Text += "!";
                    var edited = _parser.Render(document);
                    last.Text = last.Text[..^1];
                    Assert.Equal(song, unedited);
                    Assert.NotEqual(song, edited);
                };
            },
            10_000);
    }
}
