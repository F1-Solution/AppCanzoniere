using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>A document says whether rendering it will write the source text back or rebuild it from the typed members.</summary>
public class ChordProKeepsSourceTextTests
{
    private const string Song = "# note\n{title:T}\n{key:C}\n[C]uno\n";

    private readonly ChordProParser _parser = new();

    [Fact]
    public void UneditedParsedDocument_KeepsTheSourceText()
    {
        Assert.True(_parser.Parse(Song).KeepsSourceText);
    }

    [Fact]
    public void HandBuiltDocument_DoesNotKeepSourceText()
    {
        Assert.False(new ChordProDocument { Title = "T" }.KeepsSourceText);
    }

    [Fact]
    public void BodyEdit_LosesTheSourceText()
    {
        var document = _parser.Parse(Song);
        document.Lines[1].Text = "due";

        Assert.False(document.KeepsSourceText);
    }

    [Fact]
    public void RevertedBodyEdit_KeepsTheSourceTextAgain()
    {
        var document = _parser.Parse(Song);
        document.Lines[1].Text = "due";
        document.Lines[1].Text = string.Empty;
        document.Lines[1].Text = "uno";

        Assert.True(document.KeepsSourceText);
    }

    [Theory]
    [InlineData("Title")]
    [InlineData("Artist")]
    [InlineData("Key")]
    [InlineData("Capo")]
    [InlineData("Tempo")]
    [InlineData("Time")]
    public void HeaderOnlyEdit_KeepsTheSourceText(string field)
    {
        var document = _parser.Parse(Song);
        typeof(ChordProDocument).GetProperty(field)!.SetValue(document, "7");

        Assert.True(document.KeepsSourceText);
    }

    [Fact]
    public void HeaderEditTogetherWithABodyEdit_LosesTheSourceText()
    {
        var document = _parser.Parse(Song);
        document.Title = "U";
        document.Lines.Add(new ChordProLine { Type = ChordProLineType.Empty });

        Assert.False(document.KeepsSourceText);
    }
}
