using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>A line break inside a header value never starts a directive of its own.</summary>
public class ChordProHeaderLineBreakTests
{
    private const string Song = "{title: T}\n{artist: A}\n{key: C}\n{capo: 1}\n{tempo: 9}\n{time: 4/4}\n[C]x\n";

    private static readonly string[] Properties = { "Title", "Artist", "Key", "Capo", "Tempo", "Time" };

    // Code points: line feed, carriage return, CR LF, next line, line separator, paragraph separator.
    private static readonly string[] Breaks =
    {
        char.ConvertFromUtf32(10),
        char.ConvertFromUtf32(13),
        char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10),
        char.ConvertFromUtf32(0x85),
        char.ConvertFromUtf32(0x2028),
        char.ConvertFromUtf32(0x2029)
    };

    public static IEnumerable<object[]> Cases() =>
        from property in Properties
        from lineBreak in Breaks
        from rebuilt in new[] { false, true }
        select new object[] { property, lineBreak, rebuilt };

    private readonly ChordProParser _parser = new();

    private static string Get(ChordProDocument document, string property) =>
        (string)typeof(ChordProDocument).GetProperty(property)!.GetValue(document)!;

    private static void Set(ChordProDocument document, string property, string value) =>
        typeof(ChordProDocument).GetProperty(property)!.SetValue(document, value);

    [Theory]
    [MemberData(nameof(Cases))]
    public void ABreakInAHeaderValue_BecomesASpace_AndInjectsNoDirective(string property, string lineBreak, bool rebuilt)
    {
        var inject = property == "Key" ? "{title: X" : "{key: G";
        var document = _parser.Parse(Song);
        if (rebuilt)
            document.Lines[^1].Text += "!"; // Makes the document rebuild from its typed members.

        Set(document, property, "A}" + lineBreak + inject + lineBreak);
        var again = _parser.Parse(_parser.Render(document));

        Assert.Equal(property == "Key" ? "T" : "C", property == "Key" ? again.Title.Trim() : again.Key.Trim());
        Assert.Equal("A} " + inject, Get(again, property).Trim());
    }

    [Fact]
    public void ABreakInAHandBuiltHeader_IsWrittenAsASpace()
    {
        var document = new ChordProDocument { Title = "A}\n{key: G", Artist = "B" + char.ConvertFromUtf32(0x2028) + "C" };

        var again = _parser.Parse(_parser.Render(document));

        Assert.Equal(string.Empty, again.Key);
        Assert.Equal("A} {key: G", again.Title);
        Assert.Equal("B C", again.Artist);
    }
}
