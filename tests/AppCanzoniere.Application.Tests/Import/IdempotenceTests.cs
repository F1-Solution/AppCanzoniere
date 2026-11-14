using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Converting the converter's own output returns it byte for byte.
public class IdempotenceTests
{
    [Fact]
    public void NoChordMarkLine_IsWrittenAsAnAnnotation() =>
        Assert.Equal(Out("[*N.C.]"), Run("N.C.").ChordPro);

    [Fact]
    public void NeutralOnlyChordLine_IsWrittenAsAnnotations() =>
        Assert.Equal(Out("[*N.C.] [*x2]"), Run("N.C.   x2").ChordPro);

    [Fact]
    public void ChordLinesKeepTheirNeutralMarksAsWritten() =>
        Assert.Equal(Out("| [Do] | [Fa] | x2"), Run("| Do | Fa | x2").ChordPro);

    [Theory]
    [InlineData("N.C.", "Do       Sol", "Signore, ascolta")]
    [InlineData("N.C.", "Taci")]
    [InlineData("Do  Sol", "Taci, poi canta", "", "Fa  Re", "Vieni")]
    [InlineData("Do  Sol", "Amen", "Fa")]
    public void OutputWithoutDirectives_IsStable(params string[] input)
    {
        var first = Run(input).ChordPro;

        Assert.Equal(first, ChordSheetConverter.Convert(first).ChordPro);
    }

    // A lyric made only of note names ("Re Re") would read as a chord line once a chord sits over a gap or after it,
    // so the chord line above it is written on its own line.
    [Theory]
    [InlineData("   Sib", "Re\tRe", "[Sib]", "Re      Re")]
    [InlineData("              Sib", "Re\tRe", "[Sib]", "Re      Re")]
    [InlineData("     (F#m)        (Am)", "Mi     Mi     Mi", "[F#m] [Am]", "Mi     Mi     Mi")]
    public void LyricOfRepeatedNoteNames_KeepsTheChordLineAboveItAndIsStable(string chords, string lyric, string chordOut, string lyricOut)
    {
        var first = Run(chords, lyric).ChordPro;

        Assert.Equal(Out(chordOut, lyricOut), first);
        Assert.Equal(first, ChordSheetConverter.Convert(first).ChordPro);
    }

    // The chord of the other notation is written after the lyric, next to another chord: it stays a chord when read again.
    [Fact]
    public void ChordOfTheOtherNotationAfterALyricStartingWithADigit_IsStable()
    {
        var first = Run("Sib     [G]      (Lam)", "2 Gloria").ChordPro;

        Assert.Equal(Out("[Sib]2 Gloria [G] [Lam]"), first);
        Assert.Equal(first, ChordSheetConverter.Convert(first).ChordPro);
    }

    // The lyric below a lower-case chord line is judged as it will be written: a bracketed word that becomes text
    // must not hide the alignment on the first conversion and reveal it on the second.
    [Theory]
    [InlineData("Vieni [e] la luce")]
    [InlineData("Vieni (e) la luce")]
    public void LowerCaseChordLineOverALyricWithABracketedWord_MergesTheSameWayAndIsStable(string lyric)
    {
        var first = Run("Do      Sol", "Canta il Signore", "", "do    fa  sol", lyric).ChordPro;

        Assert.Equal(Out("[Do]Canta il [Sol]Signore", "", "[do]Vieni [fa](e) [sol]la luce"), first);
        Assert.Equal(first, ChordSheetConverter.Convert(first).ChordPro);
    }

    // "[Bb]  Re" in an Italian reading has one chord of each notation: as a chord line it would count for the notation,
    // merged it would not, and the tie between notations would break the other way on the second conversion.
    // "(A)  (A)" is written back as "[A]  [A]", so a repeated name in brackets or parentheses must read the same way.
    [Theory]
    [InlineData("C   G   Am   F", "", "Dm   G7   C", "", "   [Bb]  Re", "pace la", "balla [F#m] va luce, [Fa] [Fa]")]
    [InlineData("C   G   Am   F", "", "Dm   G7   C", "", "     (A)\t(A)", "house cielo [E] il balla    sing")]
    public void ChordLinesWithBracketsOfBothNotations_AreStable(params string[] input)
    {
        var first = Run(input).ChordPro;

        Assert.Equal(first, ChordSheetConverter.Convert(first).ChordPro);
    }

    private const int MixedDocuments = 20000;

    [Fact]
    public void MixedNotationDocuments_AreStableWhenConvertedAgain()
    {
        var failures = new List<string>();
        var unstable = 0;
        foreach (var text in MixedSheets.Many(7, MixedDocuments))
        {
            var first = ChordSheetConverter.Convert(text).ChordPro;
            var second = ChordSheetConverter.Convert(first).ChordPro;
            if (first == second)
                continue;

            unstable++;
            if (failures.Count < ReportedFailures)
                failures.Add($"INPUT:\n{text}\nFIRST:\n{first}\nSECOND:\n{second}");
        }

        _output.WriteLine($"{unstable} of {MixedDocuments} mixed documents changed on a second conversion.");
        Assert.True(unstable == 0, $"{unstable} unstable.\n" + string.Join("\n=====\n", failures));
    }

    private const int NoisyDocuments = 40000;
    private const int ReportedFailures = 4;
    private readonly Xunit.Abstractions.ITestOutputHelper _output;

    public IdempotenceTests(Xunit.Abstractions.ITestOutputHelper output) => _output = output;

    [Fact]
    public void NoisyDocuments_AreStableWhenConvertedAgain()
    {
        var failures = new List<string>();
        var unstable = 0;
        foreach (var text in NoisySheets.Many(20260101, NoisyDocuments))
        {
            var first = ChordSheetConverter.Convert(text).ChordPro;
            var second = ChordSheetConverter.Convert(first).ChordPro;
            if (first == second)
                continue;

            unstable++;
            if (failures.Count < ReportedFailures)
                failures.Add($"INPUT:\n{text.Replace("\r", "\\r")}\nFIRST:\n{first}\nSECOND:\n{second}");
        }

        _output.WriteLine($"{unstable} of {NoisyDocuments} noisy documents changed on a second conversion.");
        Assert.True(unstable == 0, $"{unstable} unstable.\n" + string.Join("\n=====\n", failures));
    }

    [Fact]
    public void RandomDocuments_AreStableWhenConvertedAgain()
    {
        var checkedDocuments = 0;
        foreach (var sheet in RandomSheets.Many(99, 3000, SheetFeatures.All))
        {
            var first = ChordSheetConverter.Convert(sheet.Text).ChordPro;
            var second = ChordSheetConverter.Convert(first).ChordPro;
            Assert.True(first == second, $"Unstable.\nInput: {sheet.Text.Replace("\n", "\n\n")}\nFirst: {first}\nSecond: {second}");
            checkedDocuments++;
        }

        Assert.Equal(3000, checkedDocuments);
    }
}
