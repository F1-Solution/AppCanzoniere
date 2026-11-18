using AppCanzoniere.Application.PageTurning;

namespace AppCanzoniere.Application.Tests;

public class PageTurnScrollTests
{
    [Fact]
    public void Forward_from_top_moves_one_viewport_minus_a_row() =>
        Assert.Equal(480, PageTurnScroll.NextOffset(500, 2000, 0, 20, PageTurnDirection.Forward));

    [Fact]
    public void Forward_in_the_middle_moves_by_the_same_step() =>
        Assert.Equal(960, PageTurnScroll.NextOffset(500, 2000, 480, 20, PageTurnDirection.Forward));

    [Fact]
    public void Forward_near_the_bottom_stops_at_the_last_position() =>
        Assert.Equal(1500, PageTurnScroll.NextOffset(500, 2000, 1300, 20, PageTurnDirection.Forward));

    [Fact]
    public void Forward_at_the_bottom_stays() =>
        Assert.Equal(1500, PageTurnScroll.NextOffset(500, 2000, 1500, 20, PageTurnDirection.Forward));

    [Fact]
    public void Back_in_the_middle_moves_up_by_the_same_step() =>
        Assert.Equal(480, PageTurnScroll.NextOffset(500, 2000, 960, 20, PageTurnDirection.Back));

    [Fact]
    public void Back_near_the_top_stops_at_zero() =>
        Assert.Equal(0, PageTurnScroll.NextOffset(500, 2000, 100, 20, PageTurnDirection.Back));

    [Fact]
    public void Back_at_the_top_stays() =>
        Assert.Equal(0, PageTurnScroll.NextOffset(500, 2000, 0, 20, PageTurnDirection.Back));

    [Fact]
    public void Content_shorter_than_the_viewport_never_scrolls()
    {
        Assert.Equal(0, PageTurnScroll.NextOffset(500, 300, 0, 20, PageTurnDirection.Forward));
        Assert.Equal(0, PageTurnScroll.NextOffset(500, 300, 0, 20, PageTurnDirection.Back));
    }

    [Fact]
    public void Viewport_smaller_than_a_row_still_moves_by_the_viewport() =>
        Assert.Equal(10, PageTurnScroll.NextOffset(10, 2000, 0, 20, PageTurnDirection.Forward));

    [Fact]
    public void No_direction_keeps_the_offset() =>
        Assert.Equal(300, PageTurnScroll.NextOffset(500, 2000, 300, 20, PageTurnDirection.None));

    [Fact]
    public void Offset_beyond_the_end_is_clamped() =>
        Assert.Equal(1020, PageTurnScroll.NextOffset(500, 2000, 1800, 20, PageTurnDirection.Back));

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
        Assert.Equal(0, PageTurnScroll.NextOffset(viewport, content, offset, row, PageTurnDirection.Forward));
}
