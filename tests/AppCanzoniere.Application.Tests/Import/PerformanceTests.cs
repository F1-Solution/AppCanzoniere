using System.Diagnostics;
using AppCanzoniere.Application.Import;

namespace AppCanzoniere.Application.Tests.Import;

// Pathological pastes must stay linear; the bounds are generous so slow machines do not fail them.
public class PerformanceTests
{
    private const int GenerousSeconds = 3;

    [Fact]
    public void ThousandsOfRepeatMarksOnOneLine_AreTokenisedInLinearTime()
    {
        var line = string.Join(" ", Enumerable.Repeat("(2 v.)", 30_000));
        var watch = Stopwatch.StartNew();

        var result = ChordSheetConverter.Convert(line);

        Assert.True(watch.Elapsed < TimeSpan.FromSeconds(GenerousSeconds), $"Took {watch.Elapsed.TotalSeconds:0.0} s.");
        Assert.Equal(line + "\n", result.ChordPro);
    }

    [Fact]
    public void HugeChordLineOverAHugeLyric_IsMergedInLinearTime()
    {
        var chords = string.Join(" ", Enumerable.Repeat("Do (2 v.) Sol", 6_000));
        var lyric = string.Join(" ", Enumerable.Repeat("canto", 12_000));
        var watch = Stopwatch.StartNew();

        var result = ChordSheetConverter.Convert(chords + "\n" + lyric);

        Assert.True(watch.Elapsed < TimeSpan.FromSeconds(GenerousSeconds), $"Took {watch.Elapsed.TotalSeconds:0.0} s.");
        Assert.Equal(1, result.MergedChordLineCount);
    }
}
