using System.Diagnostics;
using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class LayoutPerformanceTests
{
    private const int Warmups = 3;
    private const int Samples = 9;
    private const double GenerousBoundMilliseconds = 2000;

    private const string SongLine = "[Do]Signore [Sol]pietà di [Re]noi che [Mi]siamo qui [La]davanti a [Si]te";

    private static string HundredLineSong() => Lines(Enumerable.Repeat(SongLine, 100).ToArray());

    [Fact]
    public void LayingOutAHundredLineSong_FinishesWithinAVeryGenerousWallClockBound()
    {
        var source = Source(HundredLineSong());
        var options = Options(300, 600);
        var measurer = new FixedAdvanceMeasurer();
        for (var i = 0; i < Warmups; i++)
            SongLayoutEngine.Layout(source, options, measurer);

        var timings = new List<double>();
        for (var i = 0; i < Samples; i++)
        {
            var watch = Stopwatch.StartNew();
            SongLayoutEngine.Layout(source, options, measurer);
            timings.Add(watch.Elapsed.TotalMilliseconds);
        }

        timings.Sort();
        var median = timings[Samples / 2];
        Assert.True(median < GenerousBoundMilliseconds, $"median {median:F2} ms over {Samples} runs");
    }

    [Theory]
    [InlineData(100, 200)]
    [InlineData(200, 400)]
    public void DoublingTheLines_NeverMoreThanDoublesTheMeasurerCalls(int smaller, int larger)
    {
        Assert.True(CallsFor(larger) <= 2 * CallsFor(smaller) + 16, $"{CallsFor(larger)} calls for {larger} lines, {CallsFor(smaller)} for {smaller}");
    }

    private static int CallsFor(int lines)
    {
        var measurer = new FixedAdvanceMeasurer();
        var song = Lines(Enumerable.Repeat(SongLine, lines).ToArray());
        SongLayoutEngine.Layout(Source(song), Options(300, 600), measurer);
        return measurer.TotalCalls;
    }
}
