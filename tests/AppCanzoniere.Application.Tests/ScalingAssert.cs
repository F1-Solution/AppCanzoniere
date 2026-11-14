using System.Diagnostics;

namespace AppCanzoniere.Application.Tests;

// Compares two sizes measured in the same run, so machine load cancels out; no absolute time bound.
internal static class ScalingAssert
{
    private const int Runs = 5;
    private const double SlackMilliseconds = 50;
    private const double AllowedFactor = 8;

    // Four times the input may cost at most 8x (linear is 4x, quadratic is 16x) plus a fixed slack.
    public static void StaysLinear(Func<int, Action> prepare, int size)
    {
        prepare(size / 4)();
        var small = BestOf(prepare(size));
        var large = BestOf(prepare(size * 4));

        Assert.True(
            large <= AllowedFactor * small + SlackMilliseconds,
            $"{size} took {small:F1} ms but {size * 4} took {large:F1} ms.");
    }

    private static double BestOf(Action work)
    {
        var best = double.MaxValue;
        for (var i = 0; i < Runs; i++)
        {
            var watch = Stopwatch.StartNew();
            work();
            best = Math.Min(best, watch.Elapsed.TotalMilliseconds);
        }

        return best;
    }
}
