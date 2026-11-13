using System.Text.RegularExpressions;
using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Chords that are already written in brackets are never turned into text.
public class ExistingChordTests
{
    private static readonly Regex Bracket = new(@"\[([^\[\]]+)\]", RegexOptions.CultureInvariant);

    [Theory]
    [InlineData("lo[Do]dia[Sol]mo [Lam] [Fa] [Do] [Sol]")]
    [InlineData("[Do]Amen [Sol] [Lam] [Fa] [Do]")]
    [InlineData("[C]Amen [G] [Am] [F] [C]")]
    public void LineWithInlineChordsAndMoreChords_IsKeptAsWritten(string line)
    {
        var result = Run(line);

        Assert.Equal(Out(line), result.ChordPro);
        Assert.Equal(SheetLineKind.InlineChordLyric, result.Diag(1).Kind);
    }

    [Fact]
    public void NoChordMarkBesideBracketedChords_KeepsTheChords()
    {
        var result = Run("N.C. [Re]-[Sol]");

        Assert.Equal(Out("N.C. [Re]-[Sol]"), result.ChordPro);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
    }

    [Fact]
    public void ConvertedChordLines_KeepAllTheirChordsWhenConvertedAgainOnTheirOwn()
    {
        var lines = 0;
        foreach (var sheet in RandomSheets.Many(77, 3000, SheetFeatures.All))
        {
            var first = ChordSheetConverter.Convert(sheet.Text);
            foreach (var line in first.WasChordPro ? Enumerable.Empty<string>() : ChordLines(first.ChordPro))
            {
                lines++;
                var again = ChordSheetConverter.Convert(line).ChordPro.TrimEnd('\n');
                Assert.True(Chords(line).SequenceEqual(Chords(again)), $"Chords changed.\nBefore: {line}\nAfter:  {again}");
            }
        }

        Assert.True(lines > 1000, $"Only {lines} chord lines were checked.");
    }

    private static IEnumerable<string> ChordLines(string chordPro) =>
        chordPro.Split('\n').Where(l => !l.StartsWith('{') && Chords(l).Any());

    private static List<string> Chords(string line) =>
        Bracket.Matches(line).Select(m => m.Groups[1].Value).Where(c => !c.StartsWith('*')).ToList();
}
