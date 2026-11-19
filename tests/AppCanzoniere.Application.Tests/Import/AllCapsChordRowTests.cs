using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Scanned songbooks print the lyrics in capitals and OCR may read chord names in capitals too; the lyrics here are invented.
public class AllCapsChordRowTests
{
    private static readonly string[] CapsLyrics =
    {
        "IL SOLE ILLUMINA IL MONDO",
        "CANTIAMO UNA NUOVA CANZONE",
        "TUTTI INSIEME NELLA LUCE",
        "VIENE LA SERA SUL MARE"
    };

    private static string[] Song(params string[] chordRows) =>
        chordRows.Zip(CapsLyrics, (chords, lyric) => new[] { chords, lyric }).SelectMany(x => x).ToArray();

    [Fact]
    public void AllCapsChordNames_AreWrittenInNormalSpelling()
    {
        var result = Run(Song("SOL        LAM", "DO7      RE/FA#", "MI-         SIM", "FA#M7     SOL"));

        Assert.All(new[] { 1, 3, 5, 7 }, n => Assert.Equal(SheetLineKind.ChordLine, result.Diag(n).Kind));
        Assert.Contains("[Sol]", result.ChordPro);
        Assert.Contains("[Lam]", result.ChordPro);
        Assert.Contains("[Do7]", result.ChordPro);
        Assert.Contains("[Re/Fa#]", result.ChordPro);
        Assert.Contains("[Mi-]", result.ChordPro);
        Assert.Contains("[Sim]", result.ChordPro);
        Assert.Contains("[Fa#m7]", result.ChordPro);
        Assert.DoesNotContain("[SOL]", result.ChordPro);
        Assert.DoesNotContain("[LAM]", result.ChordPro);
    }

    [Fact]
    public void MinorMarkerSeparatedByOcr_JoinsTheChordBeforeIt()
    {
        var result = Run(Song("SOL           SI m", "DO      LA m", "SOL        DO", "RE         SOL"));

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Contains("[Sim]", result.ChordPro);
        Assert.Contains("[Lam]", result.ChordPro);
        Assert.DoesNotContain("[SI]", result.ChordPro);
    }

    [Fact]
    public void OcrConfusionsInChordNames_AreRepaired()
    {
        var result = Run(Song("SOL         Slm", "DO       S17", "D07      SOL", "RE         SO1"));

        Assert.All(new[] { 1, 3, 5, 7 }, n => Assert.Equal(SheetLineKind.ChordLine, result.Diag(n).Kind));
        Assert.Contains("[Sim]", result.ChordPro);
        Assert.Contains("[Si7]", result.ChordPro);
        Assert.Contains("[Do7]", result.ChordPro);
    }

    [Fact]
    public void LoneNoteNames_BecomeChordRows_OnceTheSongHasAChordRow()
    {
        var result = Run(Song("SOL           SI m", "DO", "LA m", "SOL"));

        Assert.All(new[] { 1, 3, 5, 7 }, n => Assert.Equal(SheetLineKind.ChordLine, result.Diag(n).Kind));
        Assert.Equal(4, result.ChordLineCount);
    }

    [Theory]
    [InlineData("DO RE MI FA SOLO")]
    [InlineData("SI SOLO PER TE")]
    [InlineData("LA LA LA LA")]
    [InlineData("DO RE MI")]
    [InlineData("SOL LA SI")]
    [InlineData("FA SI LA")]
    [InlineData("SE TI PIACE SI SI SI")]
    public void AllCapsLyricRows_StayLyrics_EvenWithNoteNameWords(string lyric)
    {
        var result = Run("SOL           SI m", "IL SOLE ILLUMINA IL MONDO", "DO        LAM", "CANTIAMO UNA NUOVA CANZONE", lyric, "TUTTI INSIEME NELLA LUCE");

        Assert.Equal(SheetLineKind.Lyric, result.Diag(5).Kind);
        Assert.DoesNotContain("[", result.ChordPro.Split('\n').First(l => l.Contains(lyric, StringComparison.Ordinal)));
    }

    [Fact]
    public void NoteNamesAfterASectionLabel_StayChords_InACapsSong()
    {
        var result = Run(Song("SOL           SI m", "DO        LAM", "SOL        DO", "RE         SOL").Append("Intro: RE SOL RE LA RE").ToArray());

        Assert.Equal(SheetLineKind.SectionLabel, result.Diag(9).Kind);
        Assert.True(result.Diag(9).CarriesChords);
    }

    [Fact]
    public void MixedCaseLyricsUnderCapsTitles_KeepTightCapsChordRows()
    {
        var result = Run(
            "CANTO DELLA SERA DEL PAESE", "RE LA RE", "Insieme abbiamo cantato un giorno", "LA RE", "per strade non battute",
            "SOL LA", "insieme abbiamo raccolto un fiore", "ALTRO CANTO DELLA NOTTE", "SOL LA RE LA", "Insieme nel bene crediamo");

        Assert.All(new[] { 2, 4, 6, 9 }, n => Assert.Equal(SheetLineKind.ChordLine, result.Diag(n).Kind));
    }

    [Fact]
    public void CapsLyricsWithoutAnyChordRow_YieldNoChords()
    {
        var result = Run(CapsLyrics[0], "DO RE MI FA", CapsLyrics[1], CapsLyrics[2]);

        Assert.Equal(0, result.ChordLineCount);
        Assert.DoesNotContain("[", result.ChordPro);
    }

    [Fact]
    public void MixedCaseSong_KeepsItsReadingAndSpelling()
    {
        var result = Run("SOL        SI m", "Il sole illumina il mondo", "DO LAm", "Cantiamo una nuova canzone", "Do", "Tutti insieme nella luce");

        Assert.Equal(SheetLineKind.Lyric, result.Diag(1).Kind);
        Assert.Equal(SheetLineKind.ChordLine, result.Diag(3).Kind);
        Assert.Contains("[DO]", result.ChordPro);
        Assert.Contains("[LAm]", result.ChordPro);
    }

    [Fact]
    public void AllCapsEnglishChordNames_AreRecognised_IncludingACapitalFlatInTheFirstRow()
    {
        var result = Run(Song("AM        BB", "CM7      G", "DM         F", "AM7     E"));

        // "CM7" and "AM7" keep their capitals, as "M7" is a major seventh.
        Assert.All(new[] { 1, 3, 5, 7 }, n => Assert.Equal(SheetLineKind.ChordLine, result.Diag(n).Kind));
        Assert.Contains("[Am]", result.ChordPro);
        Assert.Contains("[Bb]", result.ChordPro);
        Assert.Contains("[CM7]", result.ChordPro);
        Assert.Contains("[Dm]", result.ChordPro);
        Assert.Contains("[AM7]", result.ChordPro);
    }

    [Theory]
    [InlineData("BB")]
    [InlineData("BB KING")]
    public void CapitalFlatWithoutChordCompany_StaysALyric_InAnEnglishCapsSong(string lyric)
    {
        var result = Run("CM7      G", CapsLyrics[0], "DM         F", CapsLyrics[1], lyric, CapsLyrics[2], "AM7     E", CapsLyrics[3]);

        Assert.Equal(SheetLineKind.Lyric, result.Diag(5).Kind);
    }

    [Fact]
    public void BarMarksStuckToChords_DoNotHideAChordRow()
    {
        // OCR reads the bars between chords as slashes, attaches one to a minor marker and one to a name as a leading I.
        var result = Run(
            "SOL / m/ DO IRE", string.Empty, "SOL           SI m", CapsLyrics[0], "DO        LAM", CapsLyrics[1],
            "SOL        DO", CapsLyrics[2], "RE         SOL", CapsLyrics[3]);

        Assert.Equal(SheetLineKind.ChordLine, result.Diag(1).Kind);
        Assert.Contains("[Re]", result.ChordPro);
        Assert.DoesNotContain("IRE", result.ChordPro);
    }

    [Fact]
    public void CapsLyricWithLeadingIWords_IsNotTurnedIntoAChordRow()
    {
        var result = Run("SOL           SI m", CapsLyrics[0], "DO        LAM", CapsLyrics[1], "ILA IRE", CapsLyrics[2]);

        Assert.Equal(SheetLineKind.Lyric, result.Diag(5).Kind);
    }
}
