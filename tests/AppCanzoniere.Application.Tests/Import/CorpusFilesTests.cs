using AppCanzoniere.Application.Import;

namespace AppCanzoniere.Application.Tests.Import;

// The recall test maps every converter line back to the physical line its label refers to; a wrong map would
// silently move labels, so the map is checked on its own and against the converter's line count.
public class CorpusFilesTests
{
    private static readonly string LineSeparator = ((char)0x2028).ToString();
    private static readonly string ParagraphSeparator = ((char)0x2029).ToString();
    private static readonly string NextLine = ((char)0x85).ToString();

    public static TheoryData<string, int[]> Cases => new()
    {
        { "a\nb", new[] { 1, 2 } },
        { "a\r\nb\rc", new[] { 1, 2, 3 } },
        { "a" + LineSeparator + "b\nc", new[] { 1, 1, 2 } },
        { "a" + LineSeparator + "\nb", new[] { 1, 2 } },
        { "a" + LineSeparator + ParagraphSeparator + "b", new[] { 1, 1, 1 } },
        { "a" + NextLine + LineSeparator + "\r\nb", new[] { 1, 2 } },
        { "a" + LineSeparator, new[] { 1 } },
        { "a\n\nb", new[] { 1, 2, 3 } },
        { "a\nb\n", new[] { 1, 2 } },
        { "a\n\n", new[] { 1, 2 } }
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void PhysicalLines_MapsEachConverterLineToItsNewlineTerminatedLine(string text, int[] expected) =>
        Assert.Equal(expected, CorpusFiles.PhysicalLines(text));

    // Random texts of letters, newlines and Unicode separators: the map has one entry per line the converter reports.
    [Fact]
    public void PhysicalLines_HasOneEntryPerConverterLine()
    {
        var pieces = new[] { "a", "b", "\n", "\r", "\r\n", LineSeparator, ParagraphSeparator, NextLine };
        var random = new Random(31);
        for (var i = 0; i < 3000; i++)
        {
            var text = "x" + string.Concat(Enumerable.Range(0, random.Next(1, 12)).Select(_ => pieces[random.Next(pieces.Length)]));
            var lines = ChordSheetConverter.Convert(text).Lines.Count;

            Assert.True(CorpusFiles.PhysicalLines(text).Count == lines, $"Map and converter disagree on {Escape(text)}");
        }
    }

    private static string Escape(string text) => string.Concat(text.Select(c => c < 0x20 || c > 0x7E ? $"\\u{(int)c:X4}" : c.ToString()));
}
