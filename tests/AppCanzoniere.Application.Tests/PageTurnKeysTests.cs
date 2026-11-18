using AppCanzoniere.Application.PageTurning;

namespace AppCanzoniere.Application.Tests;

public class PageTurnKeysTests
{
    [Theory]
    [InlineData("PageDown")]
    [InlineData("ArrowDown")]
    [InlineData("ArrowRight")]
    [InlineData("Space")]
    [InlineData("VolumeDown")]
    public void Forward_keys_turn_forward(string key) =>
        Assert.Equal(PageTurnDirection.Forward, PageTurnKeys.Map(key, ctrl: false, alt: false));

    [Theory]
    [InlineData("PageUp")]
    [InlineData("ArrowUp")]
    [InlineData("ArrowLeft")]
    [InlineData("VolumeUp")]
    public void Back_keys_turn_back(string key) =>
        Assert.Equal(PageTurnDirection.Back, PageTurnKeys.Map(key, ctrl: false, alt: false));

    [Theory]
    [InlineData("Enter")]
    [InlineData("A")]
    [InlineData("Home")]
    [InlineData("")]
    [InlineData(null)]
    public void Unrelated_keys_do_nothing(string? key) =>
        Assert.Equal(PageTurnDirection.None, PageTurnKeys.Map(key, ctrl: false, alt: false));

    [Theory]
    [InlineData("PageDown", true, false)]
    [InlineData("PageUp", false, true)]
    [InlineData("Space", true, true)]
    public void Ctrl_or_alt_combinations_do_nothing(string key, bool ctrl, bool alt) =>
        Assert.Equal(PageTurnDirection.None, PageTurnKeys.Map(key, ctrl, alt));
}
