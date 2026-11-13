using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class WrapLimitsTests
{
    private const int LongLine = 20_000;
    private const int MeasuredCharactersPerCharacterBound = 5;

    private static long CharactersMeasured(FixedAdvanceMeasurer measurer) =>
        measurer.Calls.Sum(call => (long)call.Key.Text.Length * call.Value);

    private static IReadOnlyList<LayoutRow> RowsOf(string text, double width, FixedAdvanceMeasurer measurer) =>
        OnlySection(SongLayoutEngine.Flow(Source(text), Options(width), measurer)).Lines[0].Rows;

    [Fact]
    public void HardBreakingALongUnbrokenLine_MeasuresAFewTimesItsLength()
    {
        var measurer = new FixedAdvanceMeasurer();

        var rows = RowsOf(new string('a', LongLine), 400, measurer);

        Assert.True(
            CharactersMeasured(measurer) <= MeasuredCharactersPerCharacterBound * LongLine,
            $"{CharactersMeasured(measurer)} characters measured");
        Assert.Equal(527, rows.Count);
        Assert.Equal(new[] { 40, 38, 10 }, new[] { RowText(rows[0]).Length, RowText(rows[1]).Length, RowText(rows[^1]).Length });
        Assert.Equal(LongLine, rows.Sum(r => RowText(r).Length));
    }

    [Fact]
    public void DoublingALongUnbrokenLine_DoesNotMoreThanDoubleTheMeasuredCharacters()
    {
        var single = new FixedAdvanceMeasurer();
        RowsOf(new string('a', LongLine), 400, single);
        var doubled = new FixedAdvanceMeasurer();
        RowsOf(new string('a', 2 * LongLine), 400, doubled);

        Assert.True(CharactersMeasured(doubled) <= 2.2 * CharactersMeasured(single));
    }

    [Fact]
    public void HardBreakingWithAChordInsideTheLongWord_StillBreaksAtTheWidth()
    {
        var rows = RowsOf("ab[Do]" + new string('c', 100), 200, new FixedAdvanceMeasurer());

        Assert.All(rows, r => Assert.False(r.Overflow));
        Assert.Equal(102, rows.Sum(r => Of(r, RunKind.Lyric).Sum(l => l.Text.Length)));
    }

    [Theory]
    [InlineData(' ')]
    [InlineData(' ')]
    [InlineData(' ')]
    public void ANonBreakingSpace_IsNotABreakPoint(char space)
    {
        var rows = RowsOf($"Ciao Alleluia{space}!", 130, new FixedAdvanceMeasurer());

        Assert.Equal(new[] { "Ciao", $"Alleluia{space}!" }, rows.Select(RowText));
    }

    [Fact]
    public void ABreakingSpace_StillBreaks()
    {
        var rows = RowsOf("Ciao Alleluia !", 130, new FixedAdvanceMeasurer());

        Assert.Equal(new[] { "Ciao Alleluia", "!" }, rows.Select(RowText));
    }

    [Fact]
    public void AChainOfNonBreakingSpaces_NeverLeavesAnExclamationMarkAlone()
    {
        var text = "Alleluia ! Alleluia !";

        foreach (var width in new[] { 100d, 120, 150, 215, 300 })
        {
            var rows = RowsOf(text, width, new FixedAdvanceMeasurer());
            Assert.DoesNotContain(rows, r => RowText(r) == "!");
        }
    }

    private static string RowText(LayoutRow row) => string.Concat(Of(row, RunKind.Lyric).Select(r => r.Text));
}
