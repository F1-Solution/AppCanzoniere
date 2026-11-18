using AppCanzoniere.Application.PageTurning;
using FluentAssertions;

namespace AppCanzoniere.Application.Tests;

public class PageTurnScrollTests
{
    [Fact]
    public void Forward_from_top_moves_one_viewport_minus_a_row() =>
        PageTurnScroll.NextOffset(500, 2000, 0, 20, PageTurnDirection.Forward).Should().Be(480);

    [Fact]
    public void Forward_in_the_middle_moves_by_the_same_step() =>
        PageTurnScroll.NextOffset(500, 2000, 480, 20, PageTurnDirection.Forward).Should().Be(960);

    [Fact]
    public void Forward_near_the_bottom_stops_at_the_last_position() =>
        PageTurnScroll.NextOffset(500, 2000, 1300, 20, PageTurnDirection.Forward).Should().Be(1500);

    [Fact]
    public void Forward_at_the_bottom_stays() =>
        PageTurnScroll.NextOffset(500, 2000, 1500, 20, PageTurnDirection.Forward).Should().Be(1500);

    [Fact]
    public void Back_in_the_middle_moves_up_by_the_same_step() =>
        PageTurnScroll.NextOffset(500, 2000, 960, 20, PageTurnDirection.Back).Should().Be(480);

    [Fact]
    public void Back_near_the_top_stops_at_zero() =>
        PageTurnScroll.NextOffset(500, 2000, 100, 20, PageTurnDirection.Back).Should().Be(0);

    [Fact]
    public void Back_at_the_top_stays() =>
        PageTurnScroll.NextOffset(500, 2000, 0, 20, PageTurnDirection.Back).Should().Be(0);

    [Fact]
    public void Content_shorter_than_the_viewport_never_scrolls()
    {
        PageTurnScroll.NextOffset(500, 300, 0, 20, PageTurnDirection.Forward).Should().Be(0);
        PageTurnScroll.NextOffset(500, 300, 0, 20, PageTurnDirection.Back).Should().Be(0);
    }

    [Fact]
    public void Viewport_smaller_than_a_row_still_moves_by_the_viewport() =>
        PageTurnScroll.NextOffset(10, 2000, 0, 20, PageTurnDirection.Forward).Should().Be(10);

    [Fact]
    public void No_direction_keeps_the_offset() =>
        PageTurnScroll.NextOffset(500, 2000, 300, 20, PageTurnDirection.None).Should().Be(300);

    [Fact]
    public void Offset_beyond_the_end_is_clamped() =>
        PageTurnScroll.NextOffset(500, 2000, 1800, 20, PageTurnDirection.Back).Should().Be(1500);

    [Theory]
    [InlineData(double.NaN, 2000, 0, 20)]
    [InlineData(500, double.NaN, 0, 20)]
    [InlineData(500, 2000, double.NaN, 20)]
    [InlineData(500, 2000, 0, double.NaN)]
    [InlineData(-5, 2000, 0, 20)]
    [InlineData(500, -2000, 0, 20)]
    [InlineData(500, 2000, -10, 20)]
    [InlineData(500, 2000, 0, -20)]
    [InlineData(double.PositiveInfinity, 2000, 0, 20)]
    public void Invalid_inputs_do_not_move_the_page(double viewport, double content, double offset, double row) =>
        PageTurnScroll.NextOffset(viewport, content, offset, row, PageTurnDirection.Forward).Should().Be(0);
}
