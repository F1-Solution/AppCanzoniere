using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class CommentInSectionTests
{
    private const string ChorusWithComment = "{soc}\n[G]Uno\n{c: Ripetere}\n[C]Due\n{eoc}";

    [Fact]
    public void ACommentInsideAChorus_StaysInTheSection_AsACommentLine()
    {
        var section = OnlySection(Flow(ChorusWithComment));

        Assert.Equal(SectionKind.Chorus, section.Kind);
        Assert.Equal(new[] { false, true, false }, section.Lines.Select(l => l.IsComment));
        var run = Single(OnlyRow(section.Lines[1]), RunKind.Comment);
        Assert.Equal(("Ripetere", TextRole.Comment), (run.Text, run.Role));
        Assert.Equal(new[] { 0d, 48, 74 }, section.Lines.Select(l => l.Y));
        Assert.Equal(118, section.Height);
    }

    [Fact]
    public void ALyricLine_IsNotMarkedAsAComment()
    {
        var line = OnlySection(Flow("Signore")).Lines[0];

        Assert.False(line.IsComment);
    }

    [Fact]
    public void ACommentInsideABridge_StaysInTheSection()
    {
        var section = OnlySection(Flow("{sob}\nUno\n{c: x}\nDue\n{eob}"));

        Assert.Equal(SectionKind.Bridge, section.Kind);
        Assert.Equal(3, section.Lines.Count);
    }

    [Fact]
    public void ACommentInsideAChorus_IsNotNumbered_AndDoesNotCountAsANumberedLine()
    {
        var flow = Flow(ChorusWithComment, Options() with { LineNumbers = true });

        Assert.Equal(2, flow.NumberedLineCount);
        Assert.Equal(new int?[] { 1, null, 2 }, flow.Sections[0].Lines.Select(l => l.Number));
    }

    [Fact]
    public void AChorusWithAComment_IsKeptWholeOnTheNextPage()
    {
        var layout = LayoutKit.Layout("Uno\n\n" + ChorusWithComment, Options(400, 140));

        Assert.Equal(2, layout.Pages.Count);
        var chorus = Assert.Single(layout.Pages[1].Sections);
        Assert.Equal(3, chorus.Lines.Count);
        Assert.False(chorus.ContinuesOnNext);
    }

    [Fact]
    public void ACommentInsideAChorus_IsStillACommentLineInLyricsOnlyMode()
    {
        var section = OnlySection(Flow(ChorusWithComment, Options() with { DisplayMode = LayoutDisplayMode.LyricsOnly }));

        Assert.Equal(new[] { false, true, false }, section.Lines.Select(l => l.IsComment));
    }

    [Fact]
    public void ACommentBetweenImplicitVerseLines_StillSplitsTheVerse()
    {
        var flow = Flow("Uno\n{c: x}\nDue");

        Assert.Equal(2, flow.Sections.Count);
        Assert.Equal("x", flow.Sections[1].Label);
    }

    [Fact]
    public void ACommentRightAfterTheOpeningDirective_StillLabelsTheChorus()
    {
        var section = OnlySection(Flow("{soc}\n{c: Rit.}\nUno\n{eoc}"));

        Assert.Equal("Rit.", section.Label);
        Assert.Single(section.Lines);
    }
}
