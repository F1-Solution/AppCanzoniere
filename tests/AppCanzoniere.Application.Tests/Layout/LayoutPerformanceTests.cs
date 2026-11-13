using System.Diagnostics;
using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class LayoutPerformanceTests
{
    private const int Warmups = 3;
    private const int Samples = 9;
    private const double GenerousBoundMilliseconds = 50;

    private static string HundredLineSong()
    {
        var line = "[Do]Signore [Sol]pietà di [Re]noi che [Mi]siamo qui [La]davanti a [Si]te";
        return Lines(Enumerable.Repeat(line, 100).ToArray());
    }

    [Fact]
    public void LayingOutAHundredLineSong_IsFarBelowTheBound_SoNothingIsQuadratic()
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

    [Fact]
    public void ADoubledSong_DoesNotMoreThanAboutDoubleTheMeasurements()
    {
        var single = new FixedAdvanceMeasurer();
        SongLayoutEngine.Layout(Source(HundredLineSong()), Options(300, 600), single);
        var doubled = new FixedAdvanceMeasurer();
        SongLayoutEngine.Layout(Source(HundredLineSong() + "\n" + HundredLineSong()), Options(300, 600), doubled);

        Assert.True(doubled.TotalCalls <= 2 * single.TotalCalls + 16);
    }
}
