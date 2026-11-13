using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class ChordPlacementTests
{
    private const string TwoChordLine = "[Do]Signore [Sol]pietà";

    private static LayoutRow RowOf(string chordPro, LayoutOptions? options = null) =>
        OnlyRow(OnlySection(Flow(chordPro, options)).Lines[0]);

    [Fact]
    public void Case01_PlainLine_IsOneSectionOneRowOneLyricRun()
    {
        var flow = Flow("Signore pietà");

        var section = OnlySection(flow);
        Assert.Equal(SectionKind.Verse, section.Kind);
        var line = Assert.Single(section.Lines);
        var row = OnlyRow(line);
        Assert.Equal(24, row.Height);
        var run = Assert.Single(row.Runs);
        Assert.Equal(RunKind.Lyric, run.Kind);
        Assert.Equal("Signore pietà", run.Text);
        AssertBox(run, 0, 130, 0, 24, 19);
        Assert.Equal(24, flow.Height);

        var layout = SongLayoutEngine.Paginate(flow, Options());
        var page = Assert.Single(layout.Pages);
        Assert.Equal(24, page.Height);
    }

    [Fact]
    public void Case02_TwoChords_SitOverTheirSyllables()
    {
        var row = RowOf(TwoChordLine);

        AssertBox(Run(row, RunKind.Chord, "Do"), 0, 16, 0, 20, 16);
        AssertBox(Run(row, RunKind.Chord, "Sol"), 80, 24);
        AssertBox(Run(row, RunKind.Lyric, "Signore "), 0, 80, 20, 24, 39);
        AssertBox(Run(row, RunKind.Lyric, "pietà"), 80, 50);
        Assert.Equal(44, row.Height);
        Assert.All(Of(row, RunKind.Lyric), r => Assert.False(r.HyphenAfter));
    }

    [Fact]
    public void Case03_CollisionMidWord_StretchesTheLyricAndHyphenates()
    {
        var row = RowOf("[Sol]a[Re]men");

        var a = Run(row, RunKind.Lyric, "a");
        AssertBox(a, 0, 10);
        Assert.True(a.HyphenAfter);
        AssertBox(Run(row, RunKind.Chord, "Sol"), 0, 24);
        var men = Run(row, RunKind.Lyric, "men");
        AssertBox(men, 32, 30);
        Assert.False(men.HyphenAfter);
        AssertBox(Run(row, RunKind.Chord, "Re"), 32, 16);
    }

    [Fact]
    public void Case04_CollisionAtAWordBoundary_StretchesWithoutHyphen()
    {
        var row = RowOf("[Sol]Tu [Re]sei");

        var tu = Run(row, RunKind.Lyric, "Tu ");
        AssertBox(tu, 0, 30);
        Assert.False(tu.HyphenAfter);
        AssertBox(Run(row, RunKind.Lyric, "sei"), 32, 30);
        AssertBox(Run(row, RunKind.Chord, "Re"), 32, 16);
    }

    [Fact]
    public void Case05_WiderLyric_NeedsNoStretch()
    {
        var row = RowOf(TwoChordLine);

        AssertBox(Run(row, RunKind.Lyric, "pietà"), 80, 50);
        Assert.DoesNotContain(row.Runs, r => r.HyphenAfter);
    }

    [Fact]
    public void Case06_PillStyle_PadsTheChordBoxes()
    {
        var row = RowOf(TwoChordLine, Options() with { ChordStyle = ChordBoxStyle.Pill });

        var doRun = Run(row, RunKind.Chord, "Do");
        AssertBox(doRun, 0, 24, 0, 24, 18);
        Assert.Equal(4, doRun.TextX);
        var sol = Run(row, RunKind.Chord, "Sol");
        AssertBox(sol, 80, 32);
        Assert.Equal(84, sol.TextX);
        AssertBox(Run(row, RunKind.Lyric, "Signore "), 0, 80, 24, 24, 43);
        Assert.Equal(48, row.Height);
    }

    [Fact]
    public void Case07_PillCollision_UsesThePaddedChordWidth()
    {
        var row = RowOf("[Sol]a[Re]men", Options() with { ChordStyle = ChordBoxStyle.Pill });

        AssertBox(Run(row, RunKind.Chord, "Sol"), 0, 32);
        AssertBox(Run(row, RunKind.Lyric, "men"), 40, 30);
        var re = Run(row, RunKind.Chord, "Re");
        AssertBox(re, 40, 24);
        Assert.Equal(44, re.TextX);
    }

    [Fact]
    public void Case08_ChordAtTheEnd_HasNoLyricRunOfItsOwn()
    {
        var row = RowOf("Gloria[Re]");

        AssertBox(Single(row, RunKind.Lyric), 0, 60);
        AssertBox(Single(row, RunKind.Chord), 60, 16);
    }

    [Fact]
    public void Case09_TwoChordsPastTheEnd_AreSeparatedByTheChordGap()
    {
        var row = RowOf("Amen[Re][Sol]");

        AssertBox(Run(row, RunKind.Chord, "Re"), 40, 16);
        AssertBox(Run(row, RunKind.Chord, "Sol"), 64, 24);
        Assert.Single(Of(row, RunKind.Lyric));
    }

    [Fact]
    public void Case10_TextBeforeTheFirstChord_IsALyricRunWithoutChord()
    {
        var row = RowOf("Ti [Do]lodo");

        AssertBox(Run(row, RunKind.Lyric, "Ti "), 0, 30);
        AssertBox(Run(row, RunKind.Lyric, "lodo"), 30, 40);
        AssertBox(Single(row, RunKind.Chord), 30, 16);
    }

    [Fact]
    public void Case11_InstrumentalLine_IsAChordRowOnly()
    {
        var line = OnlySection(Flow("[Do] [Sol] [Do]")).Lines[0];
        var row = OnlyRow(line);

        var chords = Of(row, RunKind.Chord);
        Assert.Equal(new[] { 0d, 24, 56 }, chords.Select(c => c.X));
        Assert.Equal(new[] { "Do", "Sol", "Do" }, chords.Select(c => c.Text));
        Assert.Equal(20, row.Height);
        Assert.Empty(Of(row, RunKind.Lyric));
        Assert.Null(line.Number);
    }

    [Fact]
    public void Case12_ChorusLyrics_UseTheChorusRole()
    {
        var section = OnlySection(Flow("{soc}\n[Do]Santo\n{eoc}"));

        Assert.Equal(SectionKind.Chorus, section.Kind);
        var row = OnlyRow(section.Lines[0]);
        var lyric = Single(row, RunKind.Lyric);
        Assert.Equal(TextRole.ChorusLyric, lyric.Role);
        AssertBox(lyric, 0, 55);
        AssertBox(Single(row, RunKind.Chord), 0, 16);
    }

    [Fact]
    public void Case13_Annotation_IsPlacedLikeAChordWithoutTheStar()
    {
        var row = RowOf("Amen[*x2]");

        var run = Single(row, RunKind.Annotation);
        Assert.Equal("x2", run.Text);
        Assert.Equal(TextRole.Annotation, run.Role);
        AssertBox(run, 40, 14);
        Assert.Empty(Of(row, RunKind.Chord));
    }

    [Fact]
    public void ChordRuns_RememberTheirSourcePosition()
    {
        var row = RowOf(TwoChordLine);

        Assert.Equal(0, Run(row, RunKind.Chord, "Do").SourceOffset);
        Assert.Equal(8, Run(row, RunKind.Chord, "Sol").SourceOffset);
        Assert.Equal(8, Run(row, RunKind.Lyric, "pietà").SourceOffset);
    }

    [Fact]
    public void ChordLyricSpacing_PushesTheLyricRowDown()
    {
        var row = RowOf(TwoChordLine, Options() with { ChordLyricSpacing = 3 });

        Assert.Equal(47, row.Height);
        Assert.Equal(23, Run(row, RunKind.Lyric, "Signore ").Y);
    }
}
