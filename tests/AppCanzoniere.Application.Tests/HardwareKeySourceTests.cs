using AppCanzoniere.Application.PageTurning;

namespace AppCanzoniere.Application.Tests;

public class HardwareKeySourceTests
{
    [Fact]
    public void Without_subscribers_no_key_is_consumed() =>
        Assert.False(new HardwareKeySource().OnKey("VolumeDown", false, false));

    [Fact]
    public void A_page_turn_key_is_consumed_and_raised()
    {
        var source = new HardwareKeySource();
        PageTurnDirection? seen = null;
        source.PageTurnRequested += (_, d) => seen = d;

        Assert.True(source.OnKey("PageUp", false, false));
        Assert.Equal(PageTurnDirection.Back, seen);
    }

    [Fact]
    public void Unrelated_keys_are_not_consumed()
    {
        var source = new HardwareKeySource();
        source.PageTurnRequested += (_, _) => { };

        Assert.False(source.OnKey("Enter", false, false));
    }

    [Fact]
    public void After_unsubscribing_keys_pass_through()
    {
        var source = new HardwareKeySource();
        EventHandler<PageTurnDirection> handler = (_, _) => { };
        source.PageTurnRequested += handler;
        source.PageTurnRequested -= handler;

        Assert.False(source.OnKey("VolumeDown", false, false));
    }

    [Fact]
    public void A_repeat_or_release_without_subscribers_is_not_consumed()
    {
        var source = new HardwareKeySource();

        Assert.False(source.OnKey("VolumeDown", false, false, KeyPhase.Repeat));
        Assert.False(source.OnKey("VolumeDown", false, false, KeyPhase.Release));
    }

    [Theory]
    [InlineData(KeyPhase.Repeat)]
    [InlineData(KeyPhase.Release)]
    public void A_repeat_or_release_of_a_mapped_key_is_consumed_without_turning_the_page(KeyPhase phase)
    {
        var source = new HardwareKeySource();
        var turns = 0;
        source.PageTurnRequested += (_, _) => turns++;

        Assert.True(source.OnKey("ArrowDown", false, false, phase));
        Assert.Equal(0, turns);
    }

    [Theory]
    [InlineData(KeyPhase.Repeat)]
    [InlineData(KeyPhase.Release)]
    public void A_repeat_or_release_of_an_unmapped_key_is_not_consumed(KeyPhase phase)
    {
        var source = new HardwareKeySource();
        source.PageTurnRequested += (_, _) => { };

        Assert.False(source.OnKey("Enter", false, false, phase));
    }
}
