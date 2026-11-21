using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Layout;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class SongPreviewServiceTests
{
    private sealed class UnitMeasurer : ITextMeasurer
    {
        public TextMetrics Measure(string text, TextRole role) => new(text.Length * 8, 14, 11);
    }

    private static SongPreviewService Service() => new(new SongDisplayService(new ChordProParser()));

    [Fact]
    public void Valid_chordpro_gives_a_drawing()
    {
        var preview = Service().Create("{title: Luna}\n[Do]La luna", 300, new UnitMeasurer());

        Assert.True(preview.IsValid);
        Assert.NotEmpty(preview.Drawing!.Rows);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{title: unclosed\n[Do")]
    [InlineData("[[[[]]]] {{{{ }}}} [Zz#9]")]
    [InlineData("{start_of_chorus}\n[")]
    public void Bad_text_never_throws(string? text)
    {
        var preview = Service().Create(text, 300, new UnitMeasurer());

        Assert.True(preview.IsValid ^ preview.Error is not null);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Unusable_width_returns_an_error(double width)
    {
        var preview = Service().Create("[Do]La", width, new UnitMeasurer());

        Assert.False(preview.IsValid);
        Assert.NotNull(preview.Error);
    }

    [Fact]
    public void Null_measurer_returns_an_error()
    {
        Assert.NotNull(Service().Create("[Do]La", 300, null!).Error);
    }
}
