using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Layout;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Tests.Display;

// The projector's collision rule cannot be reached through the layout engine, so it is checked on hand-made layouts.
public class ProjectorAndDetectorTests
{
    private static LayoutRun Run(RunKind kind, string text, double x) =>
        new(kind, text, TextRole.Lyric, x, 0, text.Length, 1, x, 0, -1, false);

    private static string Project(params LayoutRun[] runs)
    {
        var row = new LayoutRow(0, 0, 1, runs, false);
        var line = new LayoutLine(0, null, 0, 1, new[] { row });
        var section = new LayoutSection(SectionKind.Verse, null, Array.Empty<LayoutRun>(), 0, 1, 1, false, false, false, new[] { line });
        return LayoutTextProjector.Project(new FlowLayout(100, 0, 1, 0, new[] { section }));
    }

    [Fact]
    public void ChordsThatOverlap_AreSeparatedByOneSpace()
    {
        Assert.Equal("Am D", Project(Run(RunKind.Chord, "Am", 0), Run(RunKind.Chord, "D", 1)).Split('\n')[0]);
    }

    [Fact]
    public void ChordsThatTouch_AreSeparatedByOneSpace()
    {
        Assert.Equal("Am D", Project(Run(RunKind.Chord, "Am", 0), Run(RunKind.Chord, "D", 2)).Split('\n')[0]);
    }

    [Fact]
    public void AnAnnotationThatTouchesAChord_IsSeparatedByOneSpace()
    {
        Assert.Equal("Am rit.", Project(Run(RunKind.Chord, "Am", 0), Run(RunKind.Annotation, "rit.", 2)).Split('\n')[0]);
    }

    [Fact]
    public void ChordsWithRoom_KeepTheirColumns()
    {
        Assert.Equal("Am   D", Project(Run(RunKind.Chord, "Am", 0), Run(RunKind.Chord, "D", 5)).Split('\n')[0]);
    }

    [Fact]
    public void LyricsThatTouch_AreNotSeparated()
    {
        var text = Project(Run(RunKind.Lyric, "ab", 0), Run(RunKind.Lyric, "cd", 2));

        Assert.Equal("abcd", text);
    }

    [Fact]
    public void LyricsThatOverlap_AreNotSeparatedEither()
    {
        Assert.Equal("abcd", Project(Run(RunKind.Lyric, "ab", 0), Run(RunKind.Lyric, "cd", 1)));
    }

    [Fact]
    public void RunsAreDrawnLeftToRight_WhateverTheirOrder()
    {
        Assert.Equal("Am   D", Project(Run(RunKind.Chord, "D", 5), Run(RunKind.Chord, "Am", 0)).Split('\n')[0]);
    }

    [Fact]
    public void ANullLayout_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => LayoutTextProjector.Project(null!));
    }

    [Theory]
    [InlineData(new[] { "Re", "La", "E", "A" }, Notation.Italian)]
    [InlineData(new[] { "E", "A", "Re", "La" }, Notation.English)]
    [InlineData(new[] { "Re", "E" }, Notation.Italian)]
    [InlineData(new[] { "E", "Re" }, Notation.English)]
    [InlineData(new[] { "E", "A", "Re" }, Notation.English)]
    [InlineData(new[] { "Re", "La", "E" }, Notation.Italian)]
    [InlineData(new[] { "x", "Re" }, Notation.Italian)]
    [InlineData(new string[0], Notation.English)]
    [InlineData(new[] { "not", "chords" }, Notation.English)]
    public void TheNotationIsTheOneMoreChordsReadIn_AndOnATieTheFirstDecisiveChordWins(string[] chords, Notation expected)
    {
        Assert.Equal(expected, ChordNotationDetector.Detect(chords, MinorStyle.LetterM).Notation);
    }

    [Theory]
    [InlineData("Am", MinorStyle.LetterM, MinorStyle.LetterM)]
    [InlineData("A-", MinorStyle.LetterM, MinorStyle.Dash)]
    [InlineData("Am", MinorStyle.Dash, MinorStyle.LetterM)]
    [InlineData("A-", MinorStyle.Dash, MinorStyle.Dash)]
    [InlineData("A", MinorStyle.Dash, MinorStyle.Dash)]
    [InlineData("La-", MinorStyle.LetterM, MinorStyle.Dash)]
    [InlineData("Amin", MinorStyle.LetterM, MinorStyle.LetterM)]
    public void TheInputMinorStyleDiffersFromTheWantedOneOnlyWhenAChordContradictsIt(string chord, MinorStyle wanted, MinorStyle expected)
    {
        Assert.Equal(expected, ChordNotationDetector.Detect(new[] { chord }, wanted).MinorStyle);
    }
}
