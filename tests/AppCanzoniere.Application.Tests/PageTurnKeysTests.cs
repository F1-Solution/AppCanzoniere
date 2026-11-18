using AppCanzoniere.Application.PageTurning;
using FluentAssertions;

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
        PageTurnKeys.Map(key, ctrl: false, alt: false).Should().Be(PageTurnDirection.Forward);

    [Theory]
    [InlineData("PageUp")]
    [InlineData("ArrowUp")]
    [InlineData("ArrowLeft")]
    [InlineData("VolumeUp")]
    public void Back_keys_turn_back(string key) =>
        PageTurnKeys.Map(key, ctrl: false, alt: false).Should().Be(PageTurnDirection.Back);

    [Theory]
    [InlineData("Enter")]
    [InlineData("A")]
    [InlineData("Home")]
    [InlineData("")]
    [InlineData(null)]
    public void Unrelated_keys_do_nothing(string? key) =>
        PageTurnKeys.Map(key, ctrl: false, alt: false).Should().Be(PageTurnDirection.None);

    [Theory]
    [InlineData("PageDown", true, false)]
    [InlineData("PageUp", false, true)]
    [InlineData("Space", true, true)]
    public void Ctrl_or_alt_combinations_do_nothing(string key, bool ctrl, bool alt) =>
        PageTurnKeys.Map(key, ctrl, alt).Should().Be(PageTurnDirection.None);
}
