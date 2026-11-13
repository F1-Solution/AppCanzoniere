using System.Globalization;
using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Lyric text with brackets or braces must reach the ChordPro parser unchanged in meaning.
public class BracketSafetyTests
{
    [Fact]
    public void BracketedWordInALyric_BecomesParentheses_AndChordsDoNotMove()
    {
        var result = Run("Do      Sol", "Canta [forte] Signore");

        Assert.Equal(Out("[Do]Canta (f[Sol]orte) Signore"), result.ChordPro);
        Assert.Contains(SheetWarning.BracketsReplaced, result.Diag(2).Warnings);
        Assert.DoesNotContain(SheetWarning.BracketsReplaced, result.Diag(1).Warnings);
    }

    [Fact]
    public void BracketedRepeatMark_IsNotTurnedIntoAChord() =>
        Assert.Equal(Out("Santo", "Gloria a te (bis)"), Run("Santo", "Gloria a te [bis]").ChordPro);

    [Theory]
    [InlineData("[Rit.] Santo", "(Rit.) Santo")]
    [InlineData("Santo [1.]", "Santo (1.)")]
    [InlineData("Santo [ 3 x ]", "Santo ( 3 x )")]
    public void CommonSongbookMarks_BecomeParentheses(string input, string expected)
    {
        var result = Run("Benedetto", input);

        Assert.Equal(Out("Benedetto", expected), result.ChordPro);
        Assert.Contains(SheetWarning.BracketsReplaced, result.Diag(2).Warnings);
    }

    [Fact]
    public void LyricLineMadeOfBraces_DoesNotVanish()
    {
        var result = Run("Santo", "{x2}");
        var parsed = new ChordProParser().Parse(result.ChordPro);

        Assert.Equal(Out("Santo", "(x2)"), result.ChordPro);
        Assert.Equal(new[] { "Santo", "(x2)" }, parsed.Lines.Select(l => l.Text));
    }

    [Fact]
    public void BracketedChordInsideALyric_StaysAnInlineChord()
    {
        var result = Run("Santo", "Signore [Lam] pietà [forte]");

        Assert.Equal(Out("Santo", "Signore [Lam] pietà (forte)"), result.ChordPro);
        Assert.Equal(SheetLineKind.InlineChordLyric, result.Diag(2).Kind);
        Assert.Contains(SheetWarning.BracketsReplaced, result.Diag(2).Warnings);
    }

    [Fact]
    public void LabelRest_IsSanitised()
    {
        var result = Run("Rit. Santo [bis]");

        Assert.Equal(Out("{start_of_chorus}", "Santo (bis)", "{end_of_chorus}"), result.ChordPro);
        Assert.Contains(SheetWarning.BracketsReplaced, result.Diag(1).Warnings);
    }

    [Fact]
    public void MetadataTitleAndUnreadableComment_AreSanitised()
    {
        var result = Run("Titolo: Gloria [bis] {x}", "Tonalità: [forse] }", "Do Sol", "Santo");

        Assert.Equal(Out("{title:Gloria (bis) (x)}", "{comment:Tonalità: (forse) )}", "", "[Do]San[Sol]to"), result.ChordPro);
        Assert.Contains(SheetWarning.BracketsReplaced, result.Diag(1).Warnings);
        Assert.Contains(SheetWarning.BracketsReplaced, result.Diag(2).Warnings);
    }

    [Fact]
    public void GuessedTitle_IsSanitised()
    {
        var result = Run("Gloria {a}", "", "Santo");

        Assert.Equal(Out("{title:Gloria (a)}", "", "Santo"), result.ChordPro);
        Assert.Contains(SheetWarning.BracketsReplaced, result.Diag(1).Warnings);
    }

    [Fact]
    public void StrayWordOnAChordLine_IsSanitised()
    {
        var result = Run("Do Sol Re Mi {x}", "Santo ascolta");

        Assert.Contains("[*(x)]", result.ChordPro);
    }

    [Fact]
    public void ChordFromAboveIsNeverPlacedInsideAnExistingBracket()
    {
        var chords = LineClassifier.BuildUnit("Do    Sol", 0, true, Notation.Italian);
        var lyric = LineClassifier.BuildUnit("Ti [Lam] lodo", 0, true, Notation.Italian);

        Assert.Equal("[Do]Ti [Sol][Lam] lodo", ChordMerger.Merge(chords, lyric, 2).Line);
    }

    [Fact]
    public void RandomDocuments_KeepEveryLyricLine_AndNeverInventChords()
    {
        var documents = 0;
        foreach (var sheet in RandomSheets.Many(20240601, 3000, SheetFeatures.Brackets))
        {
            var result = ChordSheetConverter.Convert(sheet.Text);
            if (result.WasChordPro)
                continue;

            documents++;
            AssertParsedFaithfully(sheet, result.ChordPro);
        }

        Assert.True(documents > 2000, $"Only {documents} documents were converted.");
    }

    internal static void AssertParsedFaithfully(GeneratedSheet sheet, string chordPro)
    {
        var parsed = new ChordProParser().Parse(chordPro);
        var texts = parsed.Lines
            .Where(l => l.Type is ChordProLineType.Verse or ChordProLineType.Chorus or ChordProLineType.Bridge or ChordProLineType.Title)
            .Select(l => l.Text.Trim())
            .Where(t => t.Length > 0)
            .ToList();

        var index = 0;
        foreach (var lyric in sheet.Lyrics)
        {
            while (index < texts.Count && texts[index] != lyric)
                index++;
            Assert.True(index < texts.Count, $"Lyric '{lyric}' lost.\nInput: {Escape(sheet.Text)}\nOutput: {Escape(chordPro)}");
            index++;
        }

        foreach (var line in parsed.Lines.Where(l => l.Type != ChordProLineType.Tab))
        {
            foreach (var chord in line.Chords)
            {
                var name = chord.Chord.TrimStart('*');
                Assert.True(
                    sheet.AllowedChords.Contains(name) && name.IndexOfAny(new[] { '[', ']', '{', '}' }) < 0,
                    $"Invented chord '{chord.Chord}'.\nInput: {Escape(sheet.Text)}\nOutput: {Escape(chordPro)}");
                var starts = StringInfo.ParseCombiningCharacters(line.Text);
                Assert.True(
                    chord.Position == line.Text.Length || Array.IndexOf(starts, chord.Position) >= 0,
                    $"Chord '{chord.Chord}' splits a character.\nInput: {Escape(sheet.Text)}\nOutput: {Escape(chordPro)}");
            }
        }
    }

    private static string Escape(string text) => text.Replace("\r", "\\r").Replace("\n", "\\n\n").Replace("\t", "\\t");
}
