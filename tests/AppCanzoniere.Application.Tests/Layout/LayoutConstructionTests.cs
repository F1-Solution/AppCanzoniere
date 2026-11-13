using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class LayoutConstructionTests
{
    private static LayoutSourceLine Line(string text, IReadOnlyList<LayoutChord>? chords = null) =>
        new(ChordProLineType.Verse, text, chords ?? new List<LayoutChord>(), false, string.Empty);

    private static LayoutRun Run() => new(RunKind.Lyric, "a", TextRole.Lyric, 0, 0, 10, 24, 0, 19, 0, false);

    private static LayoutRow Row() => new(0, 0, 24, new[] { Run() }, false);

    private static LayoutLine OneLine() => new(0, null, 0, 24, new[] { Row() });

    private static LayoutSection Section(IReadOnlyList<LayoutLine>? lines = null) =>
        new(SectionKind.Verse, null, Array.Empty<LayoutRun>(), 0, 24, 1, false, false, false, lines ?? new[] { OneLine() });

    [Fact]
    public void AHandBuiltLine_SortsAndClampsItsChords()
    {
        var chords = new List<LayoutChord> { new("B", 99), new("A", 2), new("C", 2), new("D", -3) };

        var line = Line("Amen", chords);

        Assert.Equal(new[] { "D", "A", "C", "B" }, line.Chords.Select(c => c.Text));
        Assert.Equal(new[] { 0, 2, 2, 4 }, line.Chords.Select(c => c.Position));
    }

    [Fact]
    public void AHandBuiltSource_WithAChordPastTheEnd_LaysOutWithoutThrowing()
    {
        var source = new LayoutSource(new[] { Line("Amen", new[] { new LayoutChord("G", 99) }) });

        var flow = SongLayoutEngine.Flow(source, Options(), new FixedAdvanceMeasurer());

        var chord = Single(OnlyRow(OnlySection(flow).Lines[0]), RunKind.Chord);
        Assert.Equal((4, 40d), (chord.SourceOffset, chord.X));
    }

    [Fact]
    public void ASourceLine_RejectsNullTextChordsAndTabLabel()
    {
        Assert.Throws<ArgumentNullException>(() => new LayoutSourceLine(ChordProLineType.Verse, null!, new List<LayoutChord>(), false, ""));
        Assert.Throws<ArgumentNullException>(() => new LayoutSourceLine(ChordProLineType.Verse, "a", null!, false, ""));
        Assert.Throws<ArgumentNullException>(() => new LayoutSourceLine(ChordProLineType.Verse, "a", new List<LayoutChord>(), false, null!));
    }

    [Fact]
    public void ASourceLine_RejectsAChordWithoutText()
    {
        Assert.Throws<ArgumentException>(() => Line("a", new[] { default(LayoutChord) }));
    }

    [Fact]
    public void ASourceLine_DoesNotShareTheCallersChordList()
    {
        var chords = new List<LayoutChord> { new("G", 0) };
        var line = Line("a", chords);

        chords.Add(new LayoutChord("D", 1));
        chords[0] = new LayoutChord("X", 0);

        Assert.Equal("G", Assert.Single(line.Chords).Text);
    }

    [Fact]
    public void ASource_DoesNotShareTheCallersLineList_AndRejectsNulls()
    {
        var lines = new List<LayoutSourceLine> { Line("a") };
        var source = new LayoutSource(lines);

        lines.Add(Line("b"));

        Assert.Single(source.Lines);
        Assert.Throws<ArgumentNullException>(() => new LayoutSource(null!));
        Assert.Throws<ArgumentException>(() => new LayoutSource(new LayoutSourceLine[] { null! }));
    }

    [Fact]
    public void LayoutRecords_DoNotShareTheCallersLists()
    {
        var runs = new List<LayoutRun> { Run() };
        var row = new LayoutRow(0, 0, 24, runs, false);
        var rows = new List<LayoutRow> { row };
        var line = new LayoutLine(0, null, 0, 24, rows);
        var lines = new List<LayoutLine> { line };
        var labels = new List<LayoutRun> { Run() };
        var section = new LayoutSection(SectionKind.Verse, "x", labels, 0, 24, 1, false, false, false, lines);
        var sections = new List<LayoutSection> { section };
        var flow = new FlowLayout(100, 0, 24, 1, sections);
        var pages = new List<LayoutPage> { new(0, 24, false, sections) };
        var song = new SongLayout(pages, 0, 1);

        runs.Clear();
        rows.Clear();
        lines.Clear();
        labels.Clear();
        sections.Clear();
        pages.Clear();

        Assert.Single(row.Runs);
        Assert.Single(line.Rows);
        Assert.Single(section.Lines);
        Assert.Single(section.LabelRuns);
        Assert.Single(flow.Sections);
        Assert.Single(song.Pages);
        Assert.Single(song.Pages[0].Sections);
    }

    [Fact]
    public void LayoutRecords_RejectNullListsAndNullElements()
    {
        Assert.Throws<ArgumentNullException>(() => new LayoutRow(0, 0, 24, null!, false));
        Assert.Throws<ArgumentNullException>(() => new LayoutLine(0, null, 0, 24, null!));
        Assert.Throws<ArgumentNullException>(() => new LayoutSection(SectionKind.Verse, null, null!, 0, 24, 1, false, false, false, Array.Empty<LayoutLine>()));
        Assert.Throws<ArgumentNullException>(() => new LayoutSection(SectionKind.Verse, null, Array.Empty<LayoutRun>(), 0, 24, 1, false, false, false, null!));
        Assert.Throws<ArgumentNullException>(() => new FlowLayout(100, 0, 24, 1, null!));
        Assert.Throws<ArgumentNullException>(() => new LayoutPage(0, 24, false, null!));
        Assert.Throws<ArgumentNullException>(() => new SongLayout(null!, 0, 1));
        Assert.Throws<ArgumentException>(() => new LayoutRow(0, 0, 24, new LayoutRun[] { null! }, false));
        Assert.Throws<ArgumentException>(() => new FlowLayout(100, 0, 24, 1, new LayoutSection[] { null! }));
        Assert.Throws<ArgumentNullException>(() => Row() with { Runs = null! });
    }

    [Fact]
    public void PaginatingAHandBuiltFlow_WithAnEmptySection_KeepsTheSectionAndDoesNotThrow()
    {
        var empty = Section(Array.Empty<LayoutLine>());
        var full = Section() with { Y = 40 };
        var flow = new FlowLayout(100, 0, 64, 1, new[] { empty, full });

        var layout = SongLayoutEngine.Paginate(flow, Options(100, 30));

        Assert.Equal(2, layout.Pages.Count);
        Assert.Equal(new[] { 1, 1 }, layout.Pages.Select(p => p.Sections.Count));
        Assert.Empty(layout.Pages[0].Sections[0].Lines);
    }
}
