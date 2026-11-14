using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// A line made only of chords (plain, parenthesised or bracketed) and marks is a chord line, brackets or not.
public class BracketedChordLineTests
{
    [Fact]
    public void BracketedChordBesidePlainChords_StillMergesIntoTheLyric()
    {
        var result = Run("[Do]      Lam      Re", "Canta il Signore oggi");

        Assert.Equal(Out("[Do]Canta il S[Lam]ignore og[Re]gi"), result.ChordPro);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
    }

    [Fact]
    public void BracketedChordInTheMiddle_StillMerges()
    {
        var result = Run("Lam [Re] Sol", "Canta il Signore");

        Assert.Equal(Out("[Lam]Cant[Re]a il [Sol]Signore"), result.ChordPro);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
    }

    [Fact]
    public void BracketedChordFirstWithTwoPlainChords_StillMerges()
    {
        var result = Run("[Re] Lam Sol", "Canta il Signore");

        Assert.Equal(Out("[Re]Cant[Lam]a il [Sol]Signore"), result.ChordPro);
    }

    [Fact]
    public void BracketedAmbiguousNoteFirst_StillMerges()
    {
        var result = Run("[A] Lam Re", "Canta il Signore");

        Assert.Equal(Out("[A]Cant[Lam]a il [Re]Signore"), result.ChordPro);
    }

    [Fact]
    public void BracketedChordBesideAWord_IsStillKeptAsWritten()
    {
        var result = Run("Do      Sol", "Canta il Signore", "", "[Do] Santo Dio");

        Assert.Equal(Out("[Do]Canta il [Sol]Signore", "", "[Do] Santo Dio"), result.ChordPro);
        Assert.Equal(SheetLineKind.InlineChordLyric, result.Diag(4).Kind);
    }
}
