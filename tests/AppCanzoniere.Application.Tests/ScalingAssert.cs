using System.Diagnostics;

namespace AppCanzoniere.Application.Tests;

/// <summary>Timing tests share one collection so xUnit never runs them in parallel with each other.</summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class TimingCollection
{
    public const string Name = "Timing";
}

// Compares two sizes measured in the same run, so machine load cancels out; no absolute time bound.
internal static class ScalingAssert
{
    private const int Runs = 7;
    private const int SizeStep = 8;
    private const double SlackMilliseconds = 50;
    private const double AllowedFactor = 24;

    // Eight times the input may cost at most 24x (linear is 8x, quadratic is 64x) plus a fixed slack.
    public static void StaysLinear(Func<int, Action> prepare, int size)
    {
        var small = prepare(size);
        var large = prepare(size * SizeStep);
        small();
        large();

        var (smallBest, largeBest) = InterleavedBest(small, large);

        Assert.True(
            largeBest <= AllowedFactor * smallBest + SlackMilliseconds,
            $"{size} took {smallBest:F1} ms but {size * SizeStep} took {largeBest:F1} ms.");
    }

    private static (double Small, double Large) InterleavedBest(Action small, Action large)
    {
        var smallBest = double.MaxValue;
        var largeBest = double.MaxValue;
        for (var i = 0; i < Runs; i++)
        {
            smallBest = Math.Min(smallBest, Time(small));
            largeBest = Math.Min(largeBest, Time(large));
        }

        return (smallBest, largeBest);
    }

    private static double Time(Action work)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var watch = Stopwatch.StartNew();
        work();
        return watch.Elapsed.TotalMilliseconds;
    }
}
