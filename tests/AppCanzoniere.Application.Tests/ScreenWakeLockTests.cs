using AppCanzoniere.Application.PageTurning;

namespace AppCanzoniere.Application.Tests;

public class ScreenWakeLockTests
{
    private readonly List<bool> _calls = new();

    private ScreenWakeLock Create() => new(_calls.Add);

    [Fact]
    public void The_screen_stays_on_until_the_last_holder_lets_go()
    {
        var wake = Create();
        var first = wake.Acquire();
        var second = wake.Acquire();

        first.Dispose();
        Assert.Equal(new[] { true }, _calls);

        second.Dispose();
        Assert.Equal(new[] { true, false }, _calls);
    }

    [Fact]
    public void Letting_go_in_either_order_gives_the_same_result()
    {
        var wake = Create();
        var first = wake.Acquire();
        var second = wake.Acquire();

        second.Dispose();
        first.Dispose();

        Assert.Equal(new[] { true, false }, _calls);
    }

    [Fact]
    public void Disposing_the_same_lease_twice_does_not_release_another_holder()
    {
        var wake = Create();
        var first = wake.Acquire();
        var second = wake.Acquire();

        first.Dispose();
        first.Dispose();

        Assert.Equal(new[] { true }, _calls);
        second.Dispose();
        Assert.Equal(new[] { true, false }, _calls);
    }

    [Fact]
    public void Acquiring_again_after_release_turns_the_screen_on_again()
    {
        var wake = Create();
        wake.Acquire().Dispose();
        wake.Acquire();

        Assert.Equal(new[] { true, false, true }, _calls);
    }
}
