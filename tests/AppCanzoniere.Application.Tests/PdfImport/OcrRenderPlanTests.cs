using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public class OcrRenderPlanTests
{
    [Fact]
    public void Compute_RendersA4At200Dpi()
    {
        var size = OcrRenderPlan.ComputeOrNull(595, 842);

        Assert.NotNull(size);
        Assert.Equal(1653, size.Width);
        Assert.Equal(2339, size.Height);
        Assert.False(size.Capped);
    }

    [Fact]
    public void Compute_ShrinksAPageThatExceedsTheSideCap_KeepingTheAspectRatio()
    {
        var size = OcrRenderPlan.ComputeOrNull(2000, 1000);

        Assert.NotNull(size);
        Assert.Equal(OcrRenderPlan.MaxSidePixels, size.Width);
        Assert.Equal(OcrRenderPlan.MaxSidePixels / 2, size.Height);
        Assert.True(size.Capped);
    }

    [Fact]
    public void Compute_ShrinksAPageThatExceedsThePixelBudget()
    {
        var size = OcrRenderPlan.ComputeOrNull(1500, 1500);

        Assert.NotNull(size);
        Assert.True((long)size.Width * size.Height <= OcrRenderPlan.MaxPixels);
        Assert.True(size.Capped);
        Assert.InRange(size.Width - size.Height, -1, 1);
    }

    [Theory]
    [InlineData(0, 842)]
    [InlineData(595, -1)]
    [InlineData(double.NaN, 842)]
    [InlineData(595, double.PositiveInfinity)]
    public void Compute_ReturnsNull_ForUnusablePageSizes(double width, double height) =>
        Assert.Null(OcrRenderPlan.ComputeOrNull(width, height));

    [Fact]
    public void Compute_NeverReturnsAZeroSide_ForAVeryThinPage()
    {
        var size = OcrRenderPlan.ComputeOrNull(1, 10_000);

        Assert.NotNull(size);
        Assert.True(size.Width >= 1);
        Assert.True(size.Height >= 1);
    }

    [Fact]
    public void ScaleWord_MapsRecognisedPixelsBackToTheRenderedPage()
    {
        var word = OcrRenderPlan.ScaleWord(new OcrWord("Do", 10, 20, 30, 40), 2);

        Assert.Equal(new OcrWord("Do", 20, 40, 60, 80), word);
    }

    [Theory]
    [InlineData(1654, 2339, 1)]
    [InlineData(0, 100, 1)]
    [InlineData(8000, 11000, 2)]
    [InlineData(16000, 22000, 4)]
    public void SampleSizeFor_ReducesOnlyAsFarAsTheCappedSizeAllows(int width, int height, int expected)
    {
        var sample = OcrRenderPlan.SampleSizeFor(width, height);

        Assert.Equal(expected, sample);
        if (width > 0 && OcrRenderPlan.ComputeOrNull(width * 72.0 / OcrRenderPlan.Dpi, height * 72.0 / OcrRenderPlan.Dpi) is { Capped: true } target)
        {
            Assert.True(width / sample >= target.Width);
            Assert.True(height / sample >= target.Height);
        }
    }
}
