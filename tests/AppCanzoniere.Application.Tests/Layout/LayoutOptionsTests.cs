using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.Application.Tests.Layout;

public class LayoutOptionsTests
{
    [Fact]
    public void Case52_InvalidValues_ThrowArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LayoutOptions { Width = 0 });
        Assert.Throws<ArgumentOutOfRangeException>(() => new LayoutOptions { Width = 10, PageHeight = -1 });
        Assert.Throws<ArgumentOutOfRangeException>(() => new LayoutOptions { Width = 10, MinTabScale = 0 });
    }

    public static TheoryData<string, Func<LayoutOptions>> NegativeSpacings => new()
    {
        { "LineSpacing", () => new LayoutOptions { Width = 10, LineSpacing = -1 } },
        { "SectionSpacing", () => new LayoutOptions { Width = 10, SectionSpacing = -1 } },
        { "ChordLyricSpacing", () => new LayoutOptions { Width = 10, ChordLyricSpacing = -1 } },
        { "MinChordGap", () => new LayoutOptions { Width = 10, MinChordGap = -1 } },
        { "PillPaddingX", () => new LayoutOptions { Width = 10, PillPaddingX = -1 } },
        { "PillPaddingY", () => new LayoutOptions { Width = 10, PillPaddingY = -1 } },
        { "ContinuationIndent", () => new LayoutOptions { Width = 10, ContinuationIndent = -1 } },
        { "GutterGap", () => new LayoutOptions { Width = 10, GutterGap = -1 } },
        { "LabelSpacing", () => new LayoutOptions { Width = 10, LabelSpacing = -1 } },
    };

    [Theory]
    [MemberData(nameof(NegativeSpacings))]
    public void NegativeSpacing_Throws(string member, Func<LayoutOptions> create)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(create);
        Assert.Equal(member, ex.ParamName);
    }

    [Fact]
    public void MinLinesAtBreakBelowOne_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new LayoutOptions { Width = 10, MinLinesAtBreak = 0 });

    [Fact]
    public void MinTabScaleAboveOne_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new LayoutOptions { Width = 10, MinTabScale = 1.5 });

    [Fact]
    public void NaNWidth_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new LayoutOptions { Width = double.NaN });

    [Fact]
    public void Defaults_MatchTheSpecification()
    {
        var o = new LayoutOptions { Width = 400 };

        Assert.Null(o.PageHeight);
        Assert.Equal(LayoutDisplayMode.ChordsAndLyrics, o.DisplayMode);
        Assert.Equal(ChordBoxStyle.Plain, o.ChordStyle);
        Assert.False(o.LineNumbers);
        Assert.Equal(
            new[] { 4d, 16, 0, 8, 4, 2, 20, 8, 4, 0.5 },
            new[] { o.LineSpacing, o.SectionSpacing, o.ChordLyricSpacing, o.MinChordGap, o.PillPaddingX, o.PillPaddingY, o.ContinuationIndent, o.GutterGap, o.LabelSpacing, o.MinTabScale });
        Assert.Equal(2, o.MinLinesAtBreak);
        Assert.Equal(6, o.KeepWholeMaxLines);
    }

    [Fact]
    public void With_CopiesAndKeepsValidation()
    {
        var o = new LayoutOptions { Width = 400 } with { LineSpacing = 9 };

        Assert.Equal(9, o.LineSpacing);
        Assert.Throws<ArgumentOutOfRangeException>(() => o with { Width = -3 });
    }
}
