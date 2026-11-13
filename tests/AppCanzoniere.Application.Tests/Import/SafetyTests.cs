using System.Text.RegularExpressions;
using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Spec 9.1 cases 44-47, 52-54, 56: already-ChordPro input, lyrics-only input, escaping and global properties.
public class SafetyTests
{
    [Fact]
    public void Case44_ChordProWithDirectives_IsReturnedExactly()
    {
        const string input = "{title:Santo}\r\n[Do]Santo";

        var result = ChordSheetConverter.Convert(input);

        Assert.Equal(input, result.ChordPro);
        Assert.True(result.WasChordPro);
        Assert.All(result.Lines, l => Assert.Equal(SheetLineKind.ChordPro, l.Kind));
        Assert.Equal(new[] { 0, 1 }, result.Lines.Select(l => l.OutputLine));
    }

    [Fact]
    public void Case45_ChordProByBrackets_IsReturnedExactly()
    {
        var input = "[Do]Ti lodo\n[Sol]Signore";

        var result = ChordSheetConverter.Convert(input);

        Assert.Equal(input, result.ChordPro);
        Assert.True(result.WasChordPro);
    }

    [Fact]
    public void Case46_InlineParentheses_BecomeBrackets()
    {
        var result = Run("(Do)Ti lodiamo, (Sol)Signore");

        Assert.Equal(Out("[Do]Ti lodiamo, [Sol]Signore"), result.ChordPro);
        Assert.Equal(SheetLineKind.InlineChordLyric, result.Diag(1).Kind);
    }

    [Fact]
    public void Case47_RepeatMarkInALyric_IsNotAChord()
    {
        var result = Run("Ti lodiamo (2 v.)");

        Assert.Equal(Out("Ti lodiamo (2 v.)"), result.ChordPro);
        Assert.Equal(SheetLineKind.Lyric, result.Diag(1).Kind);
    }

    [Fact]
    public void SingleBracketedLine_IsKeptAsWritten()
    {
        var result = Run("Ti lodiamo", "Signore", "[Do]Ti lodo");

        Assert.Equal(Out("Ti lodiamo", "Signore", "[Do]Ti lodo"), result.ChordPro);
        Assert.Equal(SheetLineKind.InlineChordLyric, result.Diag(3).Kind);
        Assert.False(result.WasChordPro);
    }

    [Fact]
    public void Case52_LyricStartingWithHash_GetsALeadingSpace()
    {
        var result = Run("Do       Sol", "Signore, ascolta", "#1 hit");

        Assert.Equal(Out("[Do]Signore, [Sol]ascolta", " #1 hit"), result.ChordPro);
        Assert.Contains(SheetWarning.EscapedLyric, result.Diag(3).Warnings);
    }

    // The spec shows "Do" over "#1 hit" alone, but a lone bare note is a lyric (case 13), so a chord line gives it context.
    [Fact]
    public void Case52_MergedLineStartingWithABracket_IsNeverEscaped()
    {
        var result = Run("Do Sol", "Ti lodiamo", "Do", "#1 hit");

        Assert.Equal(Out("[Do]Ti [Sol]lodiamo", "[Do]#1 hit"), result.ChordPro);
        Assert.DoesNotContain(SheetWarning.EscapedLyric, result.Diag(4).Warnings);
    }

    [Fact]
    public void Case53_BlankRuns_AreCollapsedAndEdgesDropped() =>
        Assert.Equal(
            Out("Santo", "Dio", "", "Benedetto"),
            Run("Santo", "Dio", "", "", "", "Benedetto", "").ChordPro);

    [Fact]
    public void Case54_EmptyInput()
    {
        var result = ChordSheetConverter.Convert(string.Empty);

        Assert.Equal(string.Empty, result.ChordPro);
        Assert.Empty(result.Lines);
        Assert.Null(result.DetectedNotation);
    }

    [Fact]
    public void LeadingAndTrailingBlankLines_AreDropped() =>
        Assert.Equal(Out("Santo", "Dio"), Run("", "", "Santo", "Dio", "", "").ChordPro);

    public static IEnumerable<object[]> PlanInputs()
    {
        var inputs = new List<string[]>
        {
            new[] { "Do       Sol", "Signore, ascolta" }, new[] { "C     G", "Lord, hear our prayer" }, new[] { "Sol Re", "Alleluia" },
            new[] { "Do          Sol", "Amen" }, new[] { "Do    Fa  Sol", "Amen" },
            new[] { "Do  Sol  Lam  Fa", "Do       Sol", "Signore, ascolta" }, new[] { "Re  La", "", "Ti lodiamo" }, new[] { "Ti lodiamo" },
            new[] { "Re     La", "La mia vita è tua" }, new[] { "Do Sol", "Ti lodiamo", "Re", "Amen" }, new[] { "Si", "lo voglio" },
            new[] { "| Do | Sol | Lam | Fa |" }, new[] { "Do       Sol       x2", "Signore, ascolta" }, new[] { "Do  Sol  (2 v.)" },
            new[] { "Re  La  %" }, new[] { "N.C.  Do", "Taci, poi canta" }, new[] { "N.C.", "Taci" },
            new[] { "Do     (Lam)", "Vieni, Signore" }, new[] { "Do - Lam - Fa - Sol" }, new[] { "Do-Lam-Fa-Sol" },
            new[] { "La-    Re", "Vieni, Signore" }, new[] { "Do\tSol", "Ti lodo e canto" }, new[] { "Do Sol", "Ti lodiamo" },
            new[] { "Do      Sol", "Gloria a te" }, new[] { "Do    Sol", "Amen,     amen" }, new[] { "     Re   Sol", "     Il Signore" },
            new[] { "Rit.", "Do Sol", "Ti lodiamo", "Signore" }, new[] { "Rit. 2", "Santo" },
            new[] { "Rit.", "Santo", "", "Strofa 2", "Benedetto", "", "Rit." }, new[] { "Benedetto", "il Signore", "", "Rit." },
            new[] { "1.", "Santo" }, new[] { "   Do       Sol", "1. Signore, ascolta" },
            new[] { "Rit.", "Santo", "Strofa 2", "Benedetto" }, new[] { "Ponte", "Santo" }, new[] { "Intro: Do Sol Lam Fa" },
            new[] { "Finale" }, new[] { "Stanza mia" }, new[] { "Ritorna Signore" },
            new[] { "Titolo: Canto della sera", "Tonalità: Re", "Capo 2", "Tempo: 4/4", "", "Do Sol", "Ti lodiamo" },
            new[] { "Capotasto: II" }, new[] { "Capo 12" }, new[] { "Tonalità: boh" }, new[] { "Tonalità: La minore" },
            new[] { "CANTO DELLA SERA", "", "Do Sol", "Ti lodiamo" }, new[] { "Ti lodiamo", "Signore" },
            new[] { "(Do)Ti lodiamo, (Sol)Signore" }, new[] { "Ti lodiamo (2 v.)" }, new[] { "e|-----0-----|", "B|---1---1---|" },
            new[] { "Do       Sol", "Signore, ascolta", "C  G", "E sia la pace" }, new[] { "DO LAm", "Ti lodiamo" },
            new[] { "Do  Sol  Lam  Fa  lento" }, new[] { "Do       Sol", "Signore, ascolta", "#1 hit" },
            new[] { "Santo", "Dio", "", "", "", "Benedetto", "" }, new[] { "La mia vita" }, new[] { "A child is born" },
        };
        inputs.AddRange(WholeSongTests.AllSongs);
        return inputs.Select(i => new object[] { string.Join("\n", i) });
    }

    [Theory]
    [MemberData(nameof(PlanInputs))]
    public void Case56_OneDiagnosticPerLine_WithValidOutputLines(string input)
    {
        var result = ChordSheetConverter.Convert(input);
        var outputLineCount = result.ChordPro.Split('\n').Length - 1;

        var expectedLines = input.Split('\n').Length - (input.EndsWith('\n') ? 1 : 0);
        Assert.Equal(expectedLines, result.Lines.Count);
        Assert.All(result.Lines, l => Assert.InRange(l.OutputLine, 0, Math.Max(outputLineCount - 1, 0)));
    }

    [Theory]
    [MemberData(nameof(PlanInputs))]
    public void Case56_LyricLinesAppearUnchanged_AndChordTokensAreBracketed(string input)
    {
        var result = ChordSheetConverter.Convert(input);
        var output = result.ChordPro.Split('\n');
        var inputLines = input.Split('\n');

        foreach (var diagnostic in result.Lines.Where(l => l.Kind == SheetLineKind.Lyric))
        {
            var carried = Regex.Replace(output[diagnostic.OutputLine], @"\[[^\]]*\]", string.Empty).TrimEnd();
            Assert.Equal(Normalise(inputLines[diagnostic.LineNumber - 1]).TrimStart(), carried.TrimStart());
        }

        foreach (var diagnostic in result.Lines.Where(l => l.Kind == SheetLineKind.ChordLine))
            Assert.Matches(@"\[\*?[^\]]+\]", output[diagnostic.OutputLine]);
    }

    [Theory]
    [MemberData(nameof(PlanInputs))]
    public void Case56_ConvertingTheOutputAgain_ChangesNothing(string input)
    {
        var first = ChordSheetConverter.Convert(input).ChordPro;

        Assert.Equal(first, ChordSheetConverter.Convert(first).ChordPro);
    }

    private static string Normalise(string line) =>
        line.Replace("\t", "        ").Replace(' ', ' ').TrimEnd();
}
