using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class CommentWrapTests
{
    private static readonly string TwentyWords = string.Join(' ', Enumerable.Repeat("word", 20));

    private static LayoutLine CommentLineOf(string comment, double width) =>
        Flow($"{{c: {comment}}}", Options(width)).Sections[0].Lines[0];

    [Fact]
    public void ALongComment_WrapsAtWordBoundaries_FourWordsPerRow()
    {
        var line = CommentLineOf(TwentyWords, 200);

        Assert.Equal(5, line.Rows.Count);
        Assert.All(line.Rows, r => Assert.Equal("word word word word", Single(r, RunKind.Comment).Text));
        Assert.Equal(new[] { 1, 21, 41, 61, 81 }, line.Rows.Select(r => Single(r, RunKind.Comment).SourceOffset));
    }

    [Fact]
    public void CommentRows_UseTheCommentRole_TheContinuationIndent_AndTheLineSpacing()
    {
        var line = CommentLineOf(TwentyWords, 200);

        Assert.Equal(new[] { 0d, 26, 52, 78, 104 }, line.Rows.Select(r => r.Y));
        Assert.Equal(new[] { 0d, 20, 20, 20, 20 }, line.Rows.Select(r => Single(r, RunKind.Comment).X));
        Assert.All(line.Rows, r => Assert.Equal(TextRole.Comment, Single(r, RunKind.Comment).Role));
        AssertBox(Single(line.Rows[1], RunKind.Comment), 20, 171, 26, 22, 43);
        Assert.Equal(126, line.Height);
        Assert.All(line.Rows, r => Assert.False(r.Overflow));
    }

    [Fact]
    public void TheSectionAfterAWrappedComment_StartsBelowAllItsRows()
    {
        var flow = Flow($"{{c: {TwentyWords}}}\n\nSanto", Options(200));

        Assert.Equal(126, flow.Sections[0].Height);
        Assert.Equal(142, flow.Sections[1].Y);
    }

    [Fact]
    public void AWordWiderThanTheWidth_IsHardBrokenLikeLyrics()
    {
        var line = CommentLineOf("abcdefghijkl", 50);

        Assert.Equal(new[] { "abcde", "fgh", "ijk", "l" }, line.Rows.Select(r => Single(r, RunKind.Comment).Text));
        Assert.All(line.Rows, r => Assert.False(r.Overflow));
    }

    [Fact]
    public void ASingleCharacterWiderThanTheWidth_IsPlacedAndFlagged()
    {
        var line = CommentLineOf("W", 5);

        Assert.True(OnlyRow(line).Overflow);
    }

    [Fact]
    public void AWideLabel_WrapsIntoOneRunPerRow_AndTheFirstLineStartsBelowIt()
    {
        var section = OnlySection(Flow("{c: Ritornello finale lento}\nSanto", Options(100)));

        Assert.Equal("Ritornello finale lento", section.Label);
        Assert.Equal(new[] { "Ritornello", "finale", "lento" }, section.LabelRuns.Select(r => r.Text));
        Assert.Equal(new[] { 0d, 22, 44 }, section.LabelRuns.Select(r => r.Y));
        Assert.Equal(new[] { 0d, 20, 20 }, section.LabelRuns.Select(r => r.X));
        Assert.All(section.LabelRuns, r => Assert.Equal((RunKind.SectionLabel, TextRole.SectionLabel), (r.Kind, r.Role)));
        Assert.Same(section.LabelRuns[0], section.LabelRun);
        Assert.Equal(66, section.Lines[0].Y);
        Assert.Equal(90, section.Height);
    }

    [Fact]
    public void ALabelThatFits_StaysOneRun()
    {
        var section = OnlySection(Flow("{c: Rit.}\nSanto"));

        AssertBox(Assert.Single(section.LabelRuns), 0, 32, 0, 18, 14);
    }

    [Fact]
    public void ALabelAcrossAPageBreak_NeverSeparatesItsRowsFromTheFirstLine()
    {
        var layout = LayoutKit.Layout("Uno\n\n{c: Ritornello finale lento}\nDue", Options(100, 100));

        Assert.Equal(2, layout.Pages.Count);
        var section = Assert.Single(layout.Pages[1].Sections);
        Assert.Equal(3, section.LabelRuns.Count);
        Assert.Equal(new[] { 0d, 22, 44 }, section.LabelRuns.Select(r => r.Y));
        Assert.Equal(66, section.Lines[0].Y);
    }
}
