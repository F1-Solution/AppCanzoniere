using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.Application.Tests.Layout;

public class SpreadCalculatorTests
{
    [Fact]
    public void Case49_FullSpread_PairsPagesFromTheFirst()
    {
        Assert.Equal((0, (int?)1), SpreadCalculator.At(0, 3, SpreadMode.Full));
        Assert.Equal((2, (int?)null), SpreadCalculator.At(1, 3, SpreadMode.Full));
        Assert.Equal(2, SpreadCalculator.TurnCount(3, SpreadMode.Full));
    }

    [Fact]
    public void Case49_HalfTurn_ReplacesOnlyTheHalfAlreadyRead()
    {
        Assert.Equal(2, SpreadCalculator.TurnCount(3, SpreadMode.HalfTurn));
        Assert.Equal((2, (int?)1), SpreadCalculator.At(1, 3, SpreadMode.HalfTurn));
        Assert.Equal((2, (int?)3), SpreadCalculator.At(2, 4, SpreadMode.HalfTurn));
        Assert.Equal((0, (int?)null), SpreadCalculator.At(0, 1, SpreadMode.HalfTurn));
    }

    [Fact]
    public void HalfTurn_WalksZeroOneTwoOneTwoThreeFourThree()
    {
        var shown = Enumerable.Range(0, SpreadCalculator.TurnCount(5, SpreadMode.HalfTurn))
            .Select(t => SpreadCalculator.At(t, 5, SpreadMode.HalfTurn))
            .ToArray();

        Assert.Equal(new (int, int?)[] { (0, 1), (2, 1), (2, 3), (4, 3) }, shown);
    }

    [Theory]
    [InlineData(1, SpreadMode.Full, 1)]
    [InlineData(1, SpreadMode.HalfTurn, 1)]
    [InlineData(2, SpreadMode.Full, 1)]
    [InlineData(2, SpreadMode.HalfTurn, 1)]
    [InlineData(4, SpreadMode.Full, 2)]
    public void TurnCount_IsAtLeastOne(int pages, SpreadMode mode, int expected) =>
        Assert.Equal(expected, SpreadCalculator.TurnCount(pages, mode));

    [Fact]
    public void InvalidArguments_Throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SpreadCalculator.TurnCount(0, SpreadMode.Full));
        Assert.Throws<ArgumentOutOfRangeException>(() => SpreadCalculator.At(-1, 3, SpreadMode.Full));
        Assert.Throws<ArgumentOutOfRangeException>(() => SpreadCalculator.At(2, 3, SpreadMode.Full));
    }
}
