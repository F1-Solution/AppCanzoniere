using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class LayoutPerformanceTests
{
    private const string SongLine = "[Do]Signore [Sol]pietà di [Re]noi che [Mi]siamo qui [La]davanti a [Si]te";

    [Theory]
    [InlineData(100, 200)]
    [InlineData(200, 400)]
    public void DoublingTheLines_NeverMoreThanDoublesTheMeasurerCalls(int smaller, int larger)
    {
        Assert.True(CallsFor(larger) <= 2 * CallsFor(smaller) + 16, $"{CallsFor(larger)} calls for {larger} lines, {CallsFor(smaller)} for {smaller}");
    }

    // Measured characters are a deterministic proxy for work: quadratic layout would quadruple them when the lines double.
    [Theory]
    [InlineData(100, 200)]
    [InlineData(200, 400)]
    public void DoublingTheLines_NeverMoreThanDoublesTheMeasuredCharacters(int smaller, int larger)
    {
        var small = CharactersFor(smaller);
        var large = CharactersFor(larger);

        Assert.True(large <= 2 * small + 1000, $"{large} characters for {larger} lines, {small} for {smaller}");
    }

    private static int CallsFor(int lines)
    {
        var measurer = LayOut(lines);
        return measurer.TotalCalls;
    }

    private static long CharactersFor(int lines) => LayOut(lines).TotalCharacters;

    private static FixedAdvanceMeasurer LayOut(int lines)
    {
        var measurer = new FixedAdvanceMeasurer();
        var song = Lines(Enumerable.Repeat(SongLine, lines).ToArray());
        SongLayoutEngine.Layout(Source(song), Options(300, 600), measurer);
        return measurer;
    }
}
