using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.Services;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// A parenthesised group of chords is split into chords; a chord never carries an unbalanced parenthesis.
public class ParenthesisedGroupTests
{
    [Theory]
    [InlineData("(Do Lam)", "[Do] [Lam]")]
    [InlineData("Re (Do Lam)", "[Re] [Do] [Lam]")]
    [InlineData("(Lam   Sol", "[Lam] [Sol]")]
    [InlineData("Do  Re)", "[Do] [Re]")]
    [InlineData("Do ( Lam )", "[Do] [Lam]")]
    [InlineData("Lam [ Do ]", "[Lam] [Do]")]
    public void GroupsAndStrayParentheses_BecomeChordLines(string line, string expected)
    {
        var result = Run(line);

        Assert.Equal(Out(expected), result.ChordPro);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
    }

    [Fact]
    public void ParenthesisedGroup_IsWarnedAboutLikeASingleParenthesisedChord() =>
        Assert.Contains(SheetWarning.ParenthesesRemoved, Run("(Do Lam)").Diag(1).Warnings);

    [Fact]
    public void ChordWithAnUnbalancedParenthesis_IsNeverEmitted()
    {
        var result = Run("Sol7  Do7(  Lam");

        Assert.DoesNotContain("[Do7(]", result.ChordPro);
    }

    [Fact]
    public void CorpusOutput_HasNoChordWithAnUnbalancedParenthesis()
    {
        foreach (var file in CorpusFiles.All())
        {
            var parsed = new ChordProParser().Parse(ChordSheetConverter.Convert(File.ReadAllText(file)).ChordPro);
            foreach (var chord in parsed.Lines.Where(l => l.Type != Application.Interfaces.ChordProLineType.Tab).SelectMany(l => l.Chords))
                Assert.True(chord.Chord.Count(c => c == '(') == chord.Chord.Count(c => c == ')'), $"{Path.GetFileName(file)}: {chord.Chord}");
        }
    }
}
