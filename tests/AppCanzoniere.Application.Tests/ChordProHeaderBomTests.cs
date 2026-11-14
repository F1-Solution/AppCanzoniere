using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>A header edit keeps any number of leading byte order marks and never writes a line that is not a directive.</summary>
public class ChordProHeaderBomTests
{
    private const char Bom = (char)0xFEFF;

    private readonly ChordProParser _parser = new();

    private static string Marks(int count) => new(Bom, count);

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void EditedTitle_KeepsTheLeadingMarksAndTheDirective(int marks)
    {
        var document = _parser.Parse(Marks(marks) + "{title: Old}\n[C]x\n");
        document.Title = "New";

        var written = _parser.Render(document);

        Assert.Equal(Marks(marks) + "{title: New}\n[C]x\n", written);
        Assert.Equal("New", _parser.Parse(written).Title.Trim());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void EditedKey_KeepsTheLeadingMarksAndTheDirective(int marks)
    {
        var document = _parser.Parse(Marks(marks) + "{key: C}\n[C]x\n");
        document.Key = "G";

        var written = _parser.Render(document);

        Assert.Equal(Marks(marks) + "{key: G}\n[C]x\n", written);
        Assert.Equal("G", _parser.Parse(written).Key.Trim());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ATitleAddedBeforeALyricLine_KeepsTheLeadingMarksInFront(int marks)
    {
        var document = _parser.Parse(Marks(marks) + "[C]x\n");
        document.Title = "New";

        Assert.Equal(Marks(marks) + "{title:New}\n[C]x\n", _parser.Render(document));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TransposingAFileWithLeadingMarks_RewritesTheKeyLineAndKeepsTheMarks(int marks)
    {
        var document = _parser.Parse(Marks(marks) + "{key: C}\n[C]x\n");

        Assert.Equal(Marks(marks) + "{key: D}\n[D]x\n", _parser.Render(document, 2));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void RequestingACapoOnAFileWithLeadingMarks_RewritesTheCapoLineAndKeepsTheMarks(int marks)
    {
        var document = _parser.Parse(Marks(marks) + "{capo: 2}\n[C]x\n");

        var written = _parser.Render(document, new ChordProRenderOptions { Capo = 3 });

        Assert.StartsWith(Marks(marks) + "{capo: 3}\n", written);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ACapoInsertedBeforeALyricLine_KeepsAllTheLeadingMarksInFront(int marks)
    {
        var document = _parser.Parse(Marks(marks) + "[C]x\n");

        var written = _parser.Render(document, new ChordProRenderOptions { Capo = 3 });

        Assert.StartsWith(Marks(marks) + "{capo:3}\n", written);
        Assert.Equal(marks, written.Count(c => c == Bom));
    }

    [Fact]
    public void ADirectiveLineThatCannotBeRewritten_IsLeftAloneAndANewDirectiveIsInserted()
    {
        const string odd = "{title Old";
        var document = _parser.Parse("{title: Old}\n[C]x\n");
        var sourceLines = (System.Collections.IList)typeof(ChordProDocument)
            .GetProperty("SourceLines", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(document)!;
        var first = sourceLines[0]!;
        first.GetType().GetProperty("Raw")!.SetValue(first, odd);
        document.Title = "New";

        Assert.Equal(odd + "\n{title:New}\n[C]x\n", _parser.Render(document));
    }
}
