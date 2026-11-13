using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

/// <summary>A comment between two explicit sections labels the second one; a comment inside an open section stays in it.</summary>
public class CommentBetweenSectionsTests
{
    [Fact]
    public void TwoChorusesSeparatedOnlyByAComment_AreTwoSections_TheSecondLabelled()
    {
        var flow = Flow("{soc}\n[C]uno\n{eoc}\n{c: Rit. 2}\n{soc}\n[C]due\n{eoc}");

        Assert.Equal(new[] { SectionKind.Chorus, SectionKind.Chorus }, flow.Sections.Select(s => s.Kind));
        Assert.Equal(new string?[] { null, "Rit. 2" }, flow.Sections.Select(s => s.Label));
        Assert.All(flow.Sections, s => Assert.DoesNotContain(s.Lines, l => l.IsComment));
    }

    [Fact]
    public void TwoBridgesSeparatedOnlyByAComment_AreTwoSections_TheSecondLabelled()
    {
        var flow = Flow("{sob}\nuno\n{eob}\n{c: Ponte 2}\n{sob}\ndue\n{eob}");

        Assert.Equal(new[] { SectionKind.Bridge, SectionKind.Bridge }, flow.Sections.Select(s => s.Kind));
        Assert.Equal("Ponte 2", flow.Sections[1].Label);
    }

    [Fact]
    public void ACommentAsTheLastLineOfAChorus_StaysInsideAsItsLastLine()
    {
        var section = OnlySection(Flow("{soc}\n[C]uno\n{c: x2}\n{eoc}"));

        Assert.Equal(SectionKind.Chorus, section.Kind);
        Assert.Equal(new[] { false, true }, section.Lines.Select(l => l.IsComment));
    }

    [Fact]
    public void ACommentAsTheLastLineOfAChorus_IsNotTakenAsTheLabelOfTheVerseThatFollows()
    {
        var flow = Flow("{soc}\n[C]uno\n{c: x2}\n{eoc}\nverso");

        Assert.Equal(new[] { SectionKind.Chorus, SectionKind.Verse }, flow.Sections.Select(s => s.Kind));
        Assert.Equal(2, flow.Sections[0].Lines.Count);
        Assert.Null(flow.Sections[1].Label);
    }

    [Fact]
    public void TwoAdjacentCommentsInsideAChorus_BothStayInside()
    {
        var section = OnlySection(Flow("{soc}\n[C]uno\n{c: a}\n{c: b}\n[C]due\n{eoc}"));

        Assert.Equal(new[] { false, true, true, false }, section.Lines.Select(l => l.IsComment));
    }

    [Fact]
    public void ACommentRightAfterTheStartOfAChorus_LabelsIt()
    {
        var flow = Flow("{soc}\n{c: Rit}\n[C]uno\n{eoc}");

        var section = OnlySection(flow);
        Assert.Equal("Rit", section.Label);
        Assert.Single(section.Lines);
    }

    [Fact]
    public void HandBuiltLinesWithoutFlags_KeepTheNeighbourRule()
    {
        var lines = new[]
        {
            new LayoutSourceLine(Application.Interfaces.ChordProLineType.Chorus, "uno", Array.Empty<LayoutChord>(), false, string.Empty),
            new LayoutSourceLine(Application.Interfaces.ChordProLineType.Comment, "x", Array.Empty<LayoutChord>(), false, string.Empty),
            new LayoutSourceLine(Application.Interfaces.ChordProLineType.Chorus, "due", Array.Empty<LayoutChord>(), false, string.Empty)
        };

        var flow = SongLayoutEngine.Flow(new LayoutSource(lines), Options(), new FixedAdvanceMeasurer());

        Assert.Equal(3, OnlySection(flow).Lines.Count);
    }
}
