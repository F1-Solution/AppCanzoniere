using System.Diagnostics;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Documents created with <c>new ChordProDocument</c> render from their typed members alone.</summary>
public class ChordProHandBuiltTests
{
    private readonly ChordProParser _parser = new();

    private static ChordProLine Line(ChordProLineType type, string text, params (string Chord, int Position)[] chords) =>
        new() { Type = type, Text = text, Chords = chords.Select(c => new ChordPosition { Chord = c.Chord, Position = c.Position }).ToList() };

    private static ChordProLine Tab(string text, bool starts = false, string label = "") =>
        new() { Type = ChordProLineType.Tab, Text = text, StartsTabBlock = starts, TabLabel = label };

    private string Render(ChordProDocument document, ChordProRenderOptions? options = null) =>
        _parser.Render(document, options ?? new ChordProRenderOptions());

    [Fact]
    public void Header_WritesTitleArtistKeyCapoTempoAndTimeInThatOrder()
    {
        var document = new ChordProDocument
        {
            Title = "Song",
            Artist = "Art",
            Key = "G",
            Capo = "2",
            Tempo = "90",
            Time = "4/4",
            Lines = { Line(ChordProLineType.Verse, "Hello", ("G", 0)) }
        };

        Assert.Equal("{title:Song}\n{artist:Art}\n{key:G}\n{capo:2}\n{tempo:90}\n{time:4/4}\n\n[G]Hello\n", Render(document));
    }

    [Fact]
    public void Header_OmitsEmptyMembers()
    {
        var document = new ChordProDocument { Tempo = "120", Lines = { Line(ChordProLineType.Verse, "Hi") } };

        Assert.Equal("{tempo:120}\n\nHi\n", Render(document));
    }

    [Fact]
    public void Header_WithoutLines_IsJustTheHeader()
    {
        Assert.Equal("{title:T}\n{time:3/4}\n", Render(new ChordProDocument { Title = "T", Time = "3/4" }));
    }

    [Fact]
    public void Verse_IsWrittenWithItsChordsAtTheirPositions()
    {
        var document = new ChordProDocument { Lines = { Line(ChordProLineType.Verse, "Hello world", ("C", 0), ("G", 6)) } };

        Assert.Equal("[C]Hello [G]world\n", Render(document));
    }

    [Fact]
    public void ChorusThenBridge_AreWrappedInTheirSections()
    {
        var document = new ChordProDocument
        {
            Lines =
            {
                Line(ChordProLineType.Verse, "One", ("C", 0)),
                Line(ChordProLineType.Chorus, "Two", ("F", 0)),
                Line(ChordProLineType.Chorus, "Three", ("G", 0)),
                Line(ChordProLineType.Bridge, "Four", ("Am", 0)),
                Line(ChordProLineType.Verse, "Five", ("C", 0))
            }
        };

        Assert.Equal(
            "[C]One\n{start_of_chorus}\n[F]Two\n[G]Three\n{end_of_chorus}\n{start_of_bridge}\n[Am]Four\n{end_of_bridge}\n[C]Five\n",
            Render(document));
    }

    [Fact]
    public void SectionAtTheEnd_IsClosed()
    {
        var document = new ChordProDocument { Lines = { Line(ChordProLineType.Chorus, "Two", ("F", 0)) } };

        Assert.Equal("{start_of_chorus}\n[F]Two\n{end_of_chorus}\n", Render(document));
    }

    [Fact]
    public void TabBlock_IsWrittenVerbatimWithItsLabel()
    {
        var document = new ChordProDocument
        {
            Lines =
            {
                Tab("e|-0-", starts: true, label: "Riff"),
                Tab("B|-1-"),
                Line(ChordProLineType.Verse, "After")
            }
        };

        Assert.Equal("{start_of_tab:Riff}\ne|-0-\nB|-1-\n{end_of_tab}\nAfter\n", Render(document));
    }

    [Fact]
    public void TabBlocks_WithoutALabelAndBackToBack_AreSeparate()
    {
        var document = new ChordProDocument { Lines = { Tab("e|-0-", starts: true), Tab("e|-1-", starts: true) } };

        Assert.Equal("{start_of_tab}\ne|-0-\n{end_of_tab}\n{start_of_tab}\ne|-1-\n{end_of_tab}\n", Render(document));
    }

    [Fact]
    public void TabLines_AreNeverTransposedOrConverted()
    {
        var document = new ChordProDocument { Lines = { Tab("Lam [C] Do|-3-", starts: true) } };
        var options = new ChordProRenderOptions { TransposeSteps = 2, OutputNotation = NotationOptions.Italian };

        Assert.Equal("{start_of_tab}\nLam [C] Do|-3-\n{end_of_tab}\n", Render(document, options));
    }

    [Fact]
    public void CommentAndEmptyLines_AreWrittenAsTheyAre()
    {
        var document = new ChordProDocument
        {
            Lines =
            {
                Line(ChordProLineType.Comment, "Slowly"),
                new ChordProLine { Type = ChordProLineType.Empty },
                Line(ChordProLineType.Verse, "x"),
                new ChordProLine { Type = ChordProLineType.Empty }
            }
        };

        Assert.Equal("{comment:Slowly}\n\nx\n\n", Render(document));
    }

    [Fact]
    public void TitleLines_AreWrittenOnlyThroughTheHeader()
    {
        var document = new ChordProDocument
        {
            Title = "T",
            Lines = { Line(ChordProLineType.Title, "T"), Line(ChordProLineType.Verse, "x") }
        };

        Assert.Equal("{title:T}\n\nx\n", Render(document));
    }

    [Fact]
    public void Transposition_MovesChordsAndKey()
    {
        var document = new ChordProDocument
        {
            Key = "G",
            Lines = { Line(ChordProLineType.Verse, "Hello", ("G", 0), ("Em", 3)) }
        };

        Assert.Equal("{key:A}\n\n[A]Hel[F#m]lo\n", Render(document, new ChordProRenderOptions { TransposeSteps = 2 }));
    }

    [Fact]
    public void RequestedCapo_ShiftsTheChordsAndIsWrittenInTheHeader()
    {
        var document = new ChordProDocument { Lines = { Line(ChordProLineType.Verse, "Hello", ("G", 0), ("C", 3)) } };

        Assert.Equal("{capo:3}\n\n[E]Hel[A]lo\n", Render(document, new ChordProRenderOptions { Capo = 3 }));
    }

    [Fact]
    public void RequestedCapoZero_LeavesNoCapoLine()
    {
        var document = new ChordProDocument { Capo = "2", Lines = { Line(ChordProLineType.Verse, "Hello", ("G", 0)) } };

        Assert.Equal("\n[A]Hello\n", Render(document, new ChordProRenderOptions { Capo = 0 }));
    }

    [Fact]
    public void Notation_IsConvertedForChords()
    {
        var document = new ChordProDocument { Lines = { Line(ChordProLineType.Verse, "Casa", ("Am", 0)) } };

        Assert.Equal("[Lam]Casa\n", Render(document, new ChordProRenderOptions { OutputNotation = NotationOptions.Italian }));
    }

    [Fact]
    public void ChordPastTheEndOfTheText_IsClampedToTheEnd()
    {
        var document = new ChordProDocument { Lines = { Line(ChordProLineType.Verse, "Hi", ("C", 99)) } };

        Assert.Equal("Hi[C]\n", Render(document));
    }

    [Fact]
    public void ChordsOutOfOrder_AreWrittenInTextOrder()
    {
        var document = new ChordProDocument { Lines = { Line(ChordProLineType.Verse, "Hello", ("G", 3), ("C", 0)) } };

        Assert.Equal("[C]Hel[G]lo\n", Render(document));
    }

    [Fact]
    public void LargeDocument_RendersInLinearTime()
    {
        var document = new ChordProDocument();
        for (var i = 0; i < 100_000; i++)
            document.Lines.Add(Line(ChordProLineType.Verse, "Some words here", ("C", 0), ("G", 5)));

        var watch = Stopwatch.StartNew();
        var text = Render(document);
        watch.Stop();

        Assert.True(text.Length > 1_000_000);
        Assert.True(watch.Elapsed < TimeSpan.FromSeconds(10), $"Rendering took {watch.Elapsed}.");
    }
}
