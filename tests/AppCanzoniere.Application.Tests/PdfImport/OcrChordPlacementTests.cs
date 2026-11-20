using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

// Synthetic OCR words: a scanned proportional font has letters of different widths and chord rows that are slightly misaligned.
public class OcrChordPlacementTests
{
    private const int Width = 2380;
    private const int Height = 3300;
    private const double Em = 60;

    private static double GlyphWidth(char c) => c switch
    {
        'I' => 0.3,
        'M' or 'W' => 0.92,
        'T' or 'L' or 'E' or 'F' or 'Z' => 0.6,
        _ => 0.72
    };

    private static double TextWidth(string text) => text.Sum(GlyphWidth) * Em;

    private static OcrWord Word(string text, double left, double top) => new(text, left, top, TextWidth(text), 40);

    private static OcrLine Lyric(double top, params string[] words)
    {
        var x = 250.0;
        var result = new List<OcrWord>();
        foreach (var word in words)
        {
            result.Add(Word(word, x, top));
            x += TextWidth(word) + 20;
        }

        return new OcrLine(result);
    }

    private static string[] Lay(params OcrLine[] lines) =>
        PdfPageLayout.Lay(OcrWordAdapter.ToPageWords(new OcrPageResult(Width, Height, lines), 1).Page).Text.Split('\n');

    [Fact]
    public void Chord_LandsOverTheLetterItsBoxStartsOver_InAWordWithNarrowAndWideLetters()
    {
        // The chord starts over the fourth letter of "WIIIIIT", which is not at 3/7 of the word's width.
        var lyric = Lyric(400, "TIMIMI", "WIIIIIT", "ZEZE");
        var wordLeft = lyric.Words[1].Left;
        var chord = new OcrLine(new[] { Word("SOL", wordLeft + ((0.92 + (2 * 0.3)) * Em), 340) });

        var lines = Lay(chord, lyric);

        Assert.Equal(lines[1].IndexOf("WIIIIIT", StringComparison.Ordinal) + 3, lines[0].IndexOf("SOL", StringComparison.Ordinal));
    }

    [Fact]
    public void ChordRow_WithASeparateMinorMarker_IsStillMappedOntoTheLyricBelow()
    {
        // "SI" and "m" side by side look like two lyric words, which once made the whole chord row count as lyrics.
        var lyric = Lyric(400, "TIMIMI", "WIIIIIT", "ZEZE");
        var siLeft = lyric.Words[1].Left + ((0.92 + (2 * 0.3)) * Em);
        var chords = new OcrLine(new[] { Word("SOL", lyric.Words[0].Left, 340), Word("SI", siLeft, 340), Word("m", siLeft + TextWidth("SI") + 6, 340) });

        var lines = Lay(chords, lyric);

        Assert.Equal(lines[1].IndexOf("WIIIIIT", StringComparison.Ordinal) + 3, lines[0].IndexOf("SI", StringComparison.Ordinal));
        Assert.Equal(0, lines[0].IndexOf("SOL", StringComparison.Ordinal));
    }

    [Fact]
    public void Chord_SnapsToTheWordStart_WhenItIsSlightlyIndentedOrEarly()
    {
        var lyric = Lyric(400, "TIMIMI", "LIZEZI", "ZEZE");
        var lines = Lay(new OcrLine(new[] { Word("RE", lyric.Words[0].Left + 14, 340), Word("DO", lyric.Words[1].Left - 12, 340) }), lyric);

        Assert.Equal(lines[1].IndexOf("TIMIMI", StringComparison.Ordinal), lines[0].IndexOf("RE", StringComparison.Ordinal));
        Assert.Equal(lines[1].IndexOf("LIZEZI", StringComparison.Ordinal), lines[0].IndexOf("DO", StringComparison.Ordinal));
    }

    [Fact]
    public void Chord_StaysInsideTheWord_WhenItIsWellPastTheWordStart()
    {
        var lyric = Lyric(400, "TIMIMI", "ZEZEZEZE", "TI");
        var lines = Lay(new OcrLine(new[] { Word("MI", lyric.Words[1].Left + (3 * 0.66 * Em), 340) }), lyric);

        Assert.Equal(lines[1].IndexOf("ZEZEZEZE", StringComparison.Ordinal) + 3, lines[0].IndexOf("MI", StringComparison.Ordinal));
    }
}
