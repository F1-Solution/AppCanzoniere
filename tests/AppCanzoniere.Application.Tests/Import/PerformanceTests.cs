using AppCanzoniere.Application.Import;

namespace AppCanzoniere.Application.Tests.Import;

[Collection(TimingCollection.Name)]
public class PerformanceTests
{
    // Quadratic tokenising would cost 64x for 8x the marks, above the 24x allowed by the scaling check.
    [Fact]
    public void ThousandsOfRepeatMarksOnOneLine_AreTokenisedInLinearTime()
    {
        ScalingAssert.StaysLinear(
            marks =>
            {
                var line = string.Join(" ", Enumerable.Repeat("(2 v.)", marks));
                return () => Assert.Equal(line + "\n", ChordSheetConverter.Convert(line).ChordPro);
            },
            5_000);
    }

    // Quadratic merging would cost 64x for 8x the chords, above the 24x allowed by the scaling check.
    [Fact]
    public void HugeChordLineOverAHugeLyric_IsMergedInLinearTime()
    {
        ScalingAssert.StaysLinear(
            groups =>
            {
                var chords = string.Join(" ", Enumerable.Repeat("Do (2 v.) Sol", groups));
                var lyric = string.Join(" ", Enumerable.Repeat("canto", groups * 2));
                var text = chords + "\n" + lyric;
                return () => Assert.Equal(1, ChordSheetConverter.Convert(text).MergedChordLineCount);
            },
            1_000);
    }
}
