using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class MeasurementCallTests
{
    private const string TwoChordLine = "[Do]Signore [Sol]pietà";

    [Fact]
    public void CachingMeasurer_MemoisesByTextAndRoleOrdinally()
    {
        var inner = new FixedAdvanceMeasurer();
        var cache = new CachingTextMeasurer(inner);

        var first = cache.Measure("Do", TextRole.Chord);
        var second = cache.Measure("Do", TextRole.Chord);
        cache.Measure("do", TextRole.Chord);
        cache.Measure("Do", TextRole.Lyric);

        Assert.Equal(first, second);
        Assert.Equal(3, inner.TotalCalls);
    }

    [Fact]
    public void CachingMeasurer_RejectsNulls()
    {
        Assert.Throws<ArgumentNullException>(() => new CachingTextMeasurer(null!));
        Assert.Throws<ArgumentNullException>(() => new CachingTextMeasurer(new FixedAdvanceMeasurer()).Measure(null!, TextRole.Lyric));
    }

    [Fact]
    public void Case37_EveryDistinctTextAndRole_IsMeasuredOnce_AndAWidthChangeAddsNoCalls()
    {
        var inner = new FixedAdvanceMeasurer();
        var cache = new CachingTextMeasurer(inner);

        SongLayoutEngine.Flow(Source(TwoChordLine), Options(400), cache);

        Assert.All(inner.Calls.Values, calls => Assert.Equal(1, calls));
        var calls = inner.TotalCalls;

        SongLayoutEngine.Flow(Source(TwoChordLine), Options(500), cache);

        Assert.Equal(calls, inner.TotalCalls);
    }

    [Fact]
    public void Case37_AfterAWrapChange_NothingIsMeasuredTwice()
    {
        var inner = new FixedAdvanceMeasurer();
        var cache = new CachingTextMeasurer(inner);

        SongLayoutEngine.Flow(Source(TwoChordLine), Options(400), cache);
        SongLayoutEngine.Flow(Source(TwoChordLine), Options(100), cache);

        Assert.All(inner.Calls.Values, calls => Assert.Equal(1, calls));
    }

    [Fact]
    public void Case38_CallsGrowLinearlyWithoutACache_AndStayConstantWithOne()
    {
        var oneLine = new FixedAdvanceMeasurer();
        SongLayoutEngine.Flow(Source(TwoChordLine), Options(), oneLine);

        var plain = new FixedAdvanceMeasurer();
        SongLayoutEngine.Flow(Source(Lines(Enumerable.Repeat(TwoChordLine, 50).ToArray())), Options(), plain);
        Assert.True(plain.TotalCalls <= 50 * oneLine.TotalCalls, $"{plain.TotalCalls} calls for 50 lines, {oneLine.TotalCalls} for one");

        var inner = new FixedAdvanceMeasurer();
        SongLayoutEngine.Flow(Source(Lines(Enumerable.Repeat(TwoChordLine, 50).ToArray())), Options(), new CachingTextMeasurer(inner));
        Assert.True(inner.TotalCalls <= oneLine.TotalCalls, $"{inner.TotalCalls} cached calls, {oneLine.TotalCalls} for one line");
    }

    [Fact]
    public void WrappingALongLine_MeasuresEachAtomAFixedNumberOfTimes()
    {
        var text = string.Join(' ', Enumerable.Range(0, 40).Select(i => $"[Do]parola{i}"));
        var small = new FixedAdvanceMeasurer();
        SongLayoutEngine.Flow(Source(text), Options(300), small);

        var big = new FixedAdvanceMeasurer();
        SongLayoutEngine.Flow(Source(text + " " + text), Options(300), big);

        Assert.True(big.TotalCalls <= 2 * small.TotalCalls + 40, $"{big.TotalCalls} versus {small.TotalCalls}");
    }
}
