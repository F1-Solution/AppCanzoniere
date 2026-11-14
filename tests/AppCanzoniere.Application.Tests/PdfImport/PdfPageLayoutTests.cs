using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public class PdfPageLayoutTests
{
    private static readonly string[] Left =
    {
        "^Cammina con me per la ^strada",
        "^dove il sole ^nasce al mattino",
        "^porta la tua ^luce nel cuore",
    };

    private static readonly string[] Right =
    {
        "^Vento che soffia sul ^mare",
        "^onde che corrono ^verso la riva",
        "^voce che chiama il ^nome di tutti",
    };

    [Fact]
    public void Lay_ReportsAPageWithImagesAndNoText_AsScanned()
    {
        var page = new PdfPageWords(3, 595, 842, Array.Empty<PositionedWord>(), HasImages: true);

        var text = PdfPageLayout.Lay(page);

        Assert.True(text.IsScanned);
        Assert.Equal(3, text.PageNumber);
        Assert.Equal(0, text.ColumnCount);
        Assert.Empty(text.Blocks);
        Assert.Equal(string.Empty, text.Text);
    }

    [Fact]
    public void Lay_ReportsABlankPage_AsNeitherScannedNorText()
    {
        var text = PdfPageLayout.Lay(new PdfPageWords(1, 595, 842, Array.Empty<PositionedWord>(), HasImages: false));

        Assert.False(text.IsScanned);
        Assert.Empty(text.Blocks);
    }

    [Fact]
    public void Lay_GivesTheLeftColumnsTextThenTheRightOnes_SeparatedByABlankLine()
    {
        var builder = new PageBuilder();
        builder.Song(Left, new[] { "DO", "SOL" }, 40, 760);
        builder.Song(Right, new[] { "RE", "LA" }, 310, 760);

        var text = PdfPageLayout.Lay(builder.Page(5));

        Assert.Equal(2, text.ColumnCount);
        Assert.Equal(2, text.Blocks.Count);
        var lines = text.Text.Split('\n');
        Assert.Equal(13, lines.Length);
        Assert.Equal("Cammina con me per la strada", lines[1]);
        Assert.Equal(string.Empty, lines[6]);
        Assert.Equal("Vento che soffia sul mare", lines[8]);
        Assert.StartsWith("RE", lines[7], StringComparison.Ordinal);
    }

    [Fact]
    public void Lay_ReportsOneColumn_ForASingleColumnPage()
    {
        var builder = new PageBuilder();
        builder.Song(Left, new[] { "DO", "SOL" }, 40, 760);

        var text = PdfPageLayout.Lay(builder.Page());

        Assert.Equal(1, text.ColumnCount);
        Assert.Single(text.Blocks);
        Assert.False(text.IsScanned);
        Assert.Equal(6, text.Text.Split('\n').Length);
    }
}
