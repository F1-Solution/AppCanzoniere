using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// A lyric made only of note names ("La La La", "Do Re Mi Fa") still takes the chords above it when every chord lands
// on a letter: merged, each chord is glued to its word, so a second conversion reads the line as ChordPro again.
// Only a chord that would stand in a gap or after the lyric keeps the chord line apart (see IdempotenceTests).
public class NoteNameLyricMergeTests
{
    [Theory]
    [InlineData("Do  Sol Do\nLa  La  La", "[Do]La  [Sol]La  [Do]La")]
    [InlineData("C   G   C\nLa  La  La", "[C]La  [G]La  [C]La")]
    [InlineData("Do  Sol Do\nLA  LA  LA", "[Do]LA  [Sol]LA  [Do]LA")]
    [InlineData("Do   Sol   Do\nMi   Mi    Mi", "[Do]Mi   [Sol]Mi    [Do]Mi")]
    [InlineData("Do  Sol\nRe  Re", "[Do]Re  [Sol]Re")]
    [InlineData(
        "C       G       Am      F\nDoe, a deer, a female deer\nC   D   E   F\nDo  Re  Mi  Fa",
        "[C]Doe, a d[G]eer, a f[Am]emale de[F]er\n[C]Do  [D]Re  [E]Mi  [F]Fa")]
    public void ChordsOverEveryNoteNameOfALyric_AreMergedAndStable(string input, string expected)
    {
        var first = ChordSheetConverter.Convert(input).ChordPro;

        Assert.Equal(Out(expected), first);
        Assert.Equal(first, ChordSheetConverter.Convert(first).ChordPro);
    }

    // A chord line that already holds a bracketed chord is kept apart: merged, its bracket would no longer be checked.
    [Fact]
    public void BracketedChordOverANoteNameLyric_KeepsTheChordLineApartAndIsStable()
    {
        var first = Run("[Do]  Sol", "Re  Re").ChordPro;

        Assert.Equal(first, ChordSheetConverter.Convert(first).ChordPro);
    }
}
