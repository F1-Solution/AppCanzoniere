using AppCanzoniere.Application.Import;

namespace AppCanzoniere.Application.Tests.Import;

// A Unicode line separator right before a newline is part of that one line break (pdftotext output).
public class LineSeparatorTests
{
    private static string Ls => ((char)0x2028).ToString();

    private static string Ps => ((char)0x2029).ToString();

    private static string Nel => ((char)0x0085).ToString();

    [Fact]
    public void SeparatorBeforeCrLf_KeepsTheChordLineMergedWithTheLyric()
    {
        var result = ChordSheetConverter.Convert("Do      Sol" + Ls + "\r\nCanta il Signore" + Ls + "\r\n");

        Assert.Equal("[Do]Canta il [Sol]Signore\n", result.ChordPro);
        Assert.Equal(2, result.Lines.Count);
    }

    [Theory]
    [InlineData("\r\n")]
    [InlineData("\n")]
    [InlineData("\r")]
    public void EverySeparatorBeforeANewline_IsOneBreak(string newline)
    {
        foreach (var separator in new[] { Ls, Ps, Nel })
            Assert.Equal("a\nb\n", ChordSheetConverter.Convert("a" + separator + newline + "b").ChordPro);
    }

    [Fact]
    public void SeparatorInTheMiddleOfALine_StillBreaksTheLine()
    {
        var result = ChordSheetConverter.Convert("a" + Ls + "b");

        Assert.Equal(2, result.Lines.Count);
        Assert.Equal("a\nb\n", result.ChordPro);
    }

    [Fact]
    public void SeparatorBeforeABlankLine_KeepsExactlyOneBlankLine()
    {
        var result = ChordSheetConverter.Convert("a," + Ls + "\n\nb");

        Assert.Equal("a,\n\nb\n", result.ChordPro);
        Assert.Equal(3, result.Lines.Count);
    }

    [Fact]
    public void RunOfSeparatorsBeforeANewline_IsOneBreak() =>
        Assert.Equal("a\nb\n", ChordSheetConverter.Convert("a" + Ls + Ps + Nel + Ls + "\r\nb").ChordPro);

    [Fact]
    public void SeparatorAtTheEndOfTheText_AddsNoLine()
    {
        var result = ChordSheetConverter.Convert("a\nb" + Ls);

        Assert.Equal(2, result.Lines.Count);
        Assert.Equal("a\nb\n", result.ChordPro);
    }

    [Fact]
    public void Diagnostics_StayOnePerPhysicalLine_AndKeepTheSourceWithoutTheBreak()
    {
        var result = ChordSheetConverter.Convert("Do      Sol" + Ls + "\r\nCanta il Signore" + Ls + "\r\n\r\nFa  Re\r\nVieni");

        Assert.Equal(5, result.Lines.Count);
        Assert.Equal("Do      Sol", result.Lines[0].Source);
        Assert.Equal("Canta il Signore", result.Lines[1].Source);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result.Lines.Select(l => l.LineNumber));
    }
}
