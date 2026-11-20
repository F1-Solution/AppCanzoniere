using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public class MlKitTextMapperTests
{
    private static RawTextElement E(string text, int l, int t, int r, int b) => new(text, l, t, r, b);

    [Fact]
    public void Map_TurnsEdgesIntoPositionAndSize()
    {
        var result = MlKitTextMapper.Map(1000, 2000, new[] { new RawTextLine("Do Re", new[] { E("Do", 10, 20, 50, 60), E("Re", 70, 20, 120, 62) }) });

        var words = Assert.Single(result.Lines).Words;
        Assert.Equal(new OcrWord("Do", 10, 20, 40, 40), words[0]);
        Assert.Equal(new OcrWord("Re", 70, 20, 50, 42), words[1]);
        Assert.Equal(1000, result.Width);
        Assert.Equal(2000, result.Height);
    }

    [Fact]
    public void Map_UsesTheWholeLineAsOneWord_WhenTheEngineGivesNoElements()
    {
        var result = MlKitTextMapper.Map(100, 100, new[] { new RawTextLine("Alleluia", Array.Empty<RawTextElement>(), 5, 6, 85, 26) });

        var word = Assert.Single(Assert.Single(result.Lines).Words);
        Assert.Equal(new OcrWord("Alleluia", 5, 6, 80, 20), word);
    }

    [Fact]
    public void Map_SkipsBlankTextAndEmptyOrInvertedBoxes()
    {
        var result = MlKitTextMapper.Map(100, 100, new[]
        {
            new RawTextLine("x", new[] { E(" ", 1, 1, 9, 9), E("a", 10, 10, 10, 20), E("b", 30, 30, 20, 40), E("ok", 1, 1, 9, 9) }),
        });

        var word = Assert.Single(Assert.Single(result.Lines).Words);
        Assert.Equal("ok", word.Text);
    }

    [Fact]
    public void Map_DropsALineWithoutUsableWords()
    {
        var result = MlKitTextMapper.Map(100, 100, new[] { new RawTextLine("", Array.Empty<RawTextElement>()) });

        Assert.Empty(result.Lines);
    }

    [Fact]
    public void Map_FeedsTheSameLayoutAsTheWindowsTrial_KeepingAChordOverItsSyllable()
    {
        var result = MlKitTextMapper.Map(1190, 1684, new[]
        {
            new RawTextLine("DO SOL", new[] { E("DO", 100, 400, 140, 420), E("SOL", 360, 400, 420, 420) }),
            new RawTextLine("canto la gioia", new[] { E("canto", 100, 430, 200, 450), E("la", 220, 430, 250, 450), E("gioia", 360, 430, 460, 450) }),
        });

        var text = PdfPageLayout.Lay(OcrWordAdapter.ToPageWords(result, 1).Page).Text.Split('\n');

        Assert.Equal(2, text.Length);
        Assert.Equal(text[0].IndexOf("SOL", StringComparison.Ordinal), text[1].IndexOf("gioia", StringComparison.Ordinal));
    }
}
