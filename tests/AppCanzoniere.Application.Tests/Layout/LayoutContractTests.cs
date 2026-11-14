using System.Text.Json;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Layout;
using AppCanzoniere.Application.Services;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class LayoutContractTests
{
    private const string Song = "[Do]Signore [Sol]pietà\n\n{soc}\n[Re]Santo\n{eoc}\n\n{sot}\ne|--0--|\n{eot}";

    [Fact]
    public void Case51_EqualInputs_GiveEqualSerialisations()
    {
        var options = Options(100, 150);

        var first = JsonSerializer.Serialize(LayoutKit.Layout(Song, options));
        var second = JsonSerializer.Serialize(LayoutKit.Layout(Song, options));

        Assert.Equal(first, second);
    }

    [Fact]
    public void Case51_MutatingTheDocumentAfterTheSnapshot_DoesNotChangeALaterLayout()
    {
        var document = new ChordProParser().Parse(Song);
        var source = LayoutSource.From(document);
        var before = JsonSerializer.Serialize(SongLayoutEngine.Layout(source, Options(), new FixedAdvanceMeasurer()));

        document.Lines[0].Text = "Cambiato";
        document.Lines[0].Chords.Clear();
        document.Lines.RemoveAt(1);
        var after = JsonSerializer.Serialize(SongLayoutEngine.Layout(source, Options(), new FixedAdvanceMeasurer()));

        Assert.Equal(before, after);
    }

    [Fact]
    public void TheLayoutNeverChangesItsInputs()
    {
        var source = Source(Song);
        var flow = SongLayoutEngine.Flow(source, Options(100), new FixedAdvanceMeasurer());
        var flowBefore = JsonSerializer.Serialize(flow);

        SongLayoutEngine.Paginate(flow, Options(100, 150));

        Assert.Equal(flowBefore, JsonSerializer.Serialize(flow));
    }

    [Fact]
    public void OutputLists_CannotBeMutatedThroughACast()
    {
        var flow = Flow(Song);
        var line = flow.Sections[0].Lines[0];

        Assert.Throws<NotSupportedException>(() => ((IList<LayoutSection>)flow.Sections).Add(flow.Sections[0]));
        Assert.Throws<NotSupportedException>(() => ((IList<LayoutRow>)line.Rows).Clear());
        Assert.Throws<NotSupportedException>(() => ((IList<LayoutRun>)line.Rows[0].Runs).RemoveAt(0));
        Assert.IsNotType<LayoutRun[]>(line.Rows[0].Runs);
    }

    [Fact]
    public void LayoutSource_SortsChordsStablyAndClampsPositions()
    {
        var document = new ChordProDocument();
        document.Lines.Add(new ChordProLine
        {
            Type = ChordProLineType.Verse,
            Text = "Amen",
            Chords =
            {
                new ChordPosition { Chord = "B", Position = 99 },
                new ChordPosition { Chord = "A", Position = 2 },
                new ChordPosition { Chord = "C", Position = 2 },
                new ChordPosition { Chord = "D", Position = -3 },
            },
        });
        document.Lines.Add(new ChordProLine { Type = ChordProLineType.Title, Text = "T" });

        var line = Assert.Single(LayoutSource.From(document).Lines);

        Assert.Equal(new[] { "D", "A", "C", "B" }, line.Chords.Select(c => c.Text));
        Assert.Equal(new[] { 0, 2, 2, 4 }, line.Chords.Select(c => c.Position));
    }

    [Fact]
    public void NullArguments_Throw()
    {
        var measurer = new FixedAdvanceMeasurer();
        var source = Source(Song);
        var flow = Flow(Song);

        Assert.Throws<ArgumentNullException>(() => LayoutSource.From(null!));
        Assert.Throws<ArgumentNullException>(() => SongLayoutEngine.Flow(null!, Options(), measurer));
        Assert.Throws<ArgumentNullException>(() => SongLayoutEngine.Flow(source, null!, measurer));
        Assert.Throws<ArgumentNullException>(() => SongLayoutEngine.Flow(source, Options(), null!));
        Assert.Throws<ArgumentNullException>(() => SongLayoutEngine.Paginate(null!, Options()));
        Assert.Throws<ArgumentNullException>(() => SongLayoutEngine.Paginate(flow, null!));
    }

    [Fact]
    public void ParserAdaptation_FileCommentsAndUnsupportedDirectivesAreNotLines()
    {
        var plain = Flow("Signore\n\n{soc}\nSanto\n{eoc}");
        var noisy = Flow("# una nota del file\nSignore\n{start_of_verse: Strofa}\n\n{soc}\nSanto\n{eoc}\n{chorus}\n# altro");

        Assert.Equal(JsonSerializer.Serialize(plain.Sections.Select(s => (s.Kind, s.Y, s.Height))),
            JsonSerializer.Serialize(noisy.Sections.Select(s => (s.Kind, s.Y, s.Height))));
        Assert.All(noisy.Sections, s => Assert.Null(s.Label));
    }

    [Fact]
    public void ParserAdaptation_ABlankLineInsideATabBlockIsATabLine_NotASeparator()
    {
        var flow = Flow("Signore\n\n{sot}\ne|--0--|\n\nB|--1--|\n{eot}\n\nPietà");

        Assert.Equal(new[] { SectionKind.Verse, SectionKind.Tab, SectionKind.Verse }, flow.Sections.Select(s => s.Kind));
        Assert.Equal(3, flow.Sections[1].Lines.Count);
    }
}
