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
}
