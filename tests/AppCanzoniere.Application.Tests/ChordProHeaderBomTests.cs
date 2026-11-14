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
        Assert.Equal("New", _parser.Parse(written).Title);
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

    [Fact]
    public void ADirectiveLineThatCannotBeRewritten_IsLeftAloneAndANewDirectiveIsInserted()
    {
        var odd = ((char)0x1C).ToString() + "{title: Old}";
        var document = _parser.Parse(odd + "\n[C]x\n");
        document.Title = "New";

        var written = _parser.Render(document);

        Assert.Equal(odd + "\n{title:New}\n[C]x\n", written);
        Assert.Equal("New", _parser.Parse(written).Title);
    }
}
