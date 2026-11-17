using AppCanzoniere.Application.PdfImport;
using UglyToad.PdfPig.Content;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Converts one PdfPig page into the positioned words of the application model.</summary>
internal static class PdfPigPage
{
    // Words closer than this share of the smaller one's height are one word split by a change of size...
    private const double TouchingGapShare = 0.1;

    // ...when their bottoms differ by at most this share of the larger one's height.
    private const double SameBaselineShare = 0.3;

    // Words reaching no closer than this to the page box are invisible and dropped; the rest are clamped to it.
    private const double OffPageMarginPoints = 36;

    // Typographic ligatures folded back to their letters, so that "ﬁne" reads "fine".
    private static readonly Dictionary<char, string> Ligatures = new()
    {
        ['ﬀ'] = "ff",
        ['ﬁ'] = "fi",
        ['ﬂ'] = "fl",
        ['ﬃ'] = "ffi",
        ['ﬄ'] = "ffl",
        ['ﬅ'] = "st",
        ['ﬆ'] = "st",
    };

    public static PdfPageWords Words(Page page)
    {
        var origin = page.CropBox.Bounds;
        var visible = new WordBox(-OffPageMarginPoints, -OffPageMarginPoints, origin.Width + OffPageMarginPoints, origin.Height + OffPageMarginPoints);
        var words = page.GetWords()
            .Where(w => w.TextOrientation == TextOrientation.Horizontal && !string.IsNullOrWhiteSpace(w.Text))
            .Select(w => (Word: ClampedTo(ToWord(w, origin.Left, origin.Bottom), visible), Pen: Pen(w, origin.Left)))
            .Where(w => w.Word is not null)
            .Select(w => new PenWord(w.Word!, w.Pen.Start, w.Pen.End));
        return new PdfPageWords(page.Number, page.Width, page.Height, JoinTouching(words), HasImages(page));
    }

    // PdfPig splits a word where the font size changes, as in small capitals; touching letters on one baseline are rejoined.
    private static List<PositionedWord> JoinTouching(IEnumerable<PenWord> words)
    {
        var joined = new List<PenWord>();
        foreach (var word in words)
        {
            if (joined.Count > 0 && Touches(joined[^1], word))
                joined[^1] = new PenWord(Join(joined[^1].Word, word.Word), joined[^1].PenStart, word.PenEnd);
            else
                joined.Add(word);
        }

        return joined.Select(w => w.Word).ToList();
    }

    // The pen decides, not the glyph boxes: an italic f overhangs its advance and nearly reaches a word a space away.
    private static bool Touches(PenWord left, PenWord right)
    {
        var gap = right.PenStart - left.PenEnd;
        var smaller = Math.Min(left.Word.Box.Height, right.Word.Box.Height);
        var larger = Math.Max(left.Word.Box.Height, right.Word.Box.Height);
        return char.IsLetter(left.Word.Text[^1]) && char.IsLetter(right.Word.Text[0])
            && right.Word.Box.Left > left.Word.Box.Left
            && Math.Abs(gap) <= TouchingGapShare * smaller
            && Math.Abs(right.Word.Box.Bottom - left.Word.Box.Bottom) <= SameBaselineShare * larger;
    }

    // Where the pen starts the first letter and ends the last one, in page coordinates from the crop box's left edge.
    private static (double Start, double End) Pen(Word word, double originLeft) =>
        (word.Letters[0].StartBaseLine.X - originLeft, word.Letters[^1].EndBaseLine.X - originLeft);

    private static PositionedWord Join(PositionedWord left, PositionedWord right) => new(
        left.Text + right.Text,
        new WordBox(left.Box.Left, Math.Min(left.Box.Bottom, right.Box.Bottom), Math.Max(left.Box.Right, right.Box.Right), Math.Max(left.Box.Top, right.Box.Top)),
        left.LetterLefts is { } a && right.LetterLefts is { } b ? a.Concat(b).ToList() : null);

    // The word cut to the visible area, or null when it lies wholly outside it or its box is not a number.
    private static PositionedWord? ClampedTo(PositionedWord word, WordBox visible)
    {
        var box = word.Box;
        var finite = double.IsFinite(box.Left) && double.IsFinite(box.Right) && double.IsFinite(box.Bottom) && double.IsFinite(box.Top);
        if (!finite || box.Right < visible.Left || box.Left > visible.Right || box.Top < visible.Bottom || box.Bottom > visible.Top)
            return null;

        var clamped = new WordBox(
            Math.Max(box.Left, visible.Left),
            Math.Max(box.Bottom, visible.Bottom),
            Math.Min(box.Right, visible.Right),
            Math.Min(box.Top, visible.Top));
        if (clamped == box)
            return word;
        var lefts = word.LetterLefts?.Select(x => double.IsFinite(x) ? Math.Clamp(x, clamped.Left, clamped.Right) : clamped.Left).ToList();
        return new PositionedWord(word.Text, clamped, lefts);
    }

    private static PositionedWord ToWord(Word word, double originLeft, double originBottom)
    {
        var text = FoldLigatures(string.Concat(word.Letters.Select(TexGlyphs.TextOf)));
        var box = new WordBox(
            word.BoundingBox.Left - originLeft,
            word.BoundingBox.Bottom - originBottom,
            word.BoundingBox.Right - originLeft,
            word.BoundingBox.Top - originBottom);
        return new PositionedWord(text, box, LetterLefts(word, originLeft, text.Length));
    }

    // Pen position of each character of the folded text, as the word box uses; a ligature's letters share its advance.
    private static IReadOnlyList<double>? LetterLefts(Word word, double originLeft, int length)
    {
        var lefts = new List<double>(length);
        foreach (var letter in word.Letters)
        {
            var text = FoldLigatures(TexGlyphs.TextOf(letter));
            var start = letter.StartBaseLine.X;
            var advance = letter.EndBaseLine.X - start;
            for (var k = 0; k < text.Length; k++)
                lefts.Add(start - originLeft + (k * advance / text.Length));
        }

        return lefts.Count == length ? lefts : null;
    }

    private static bool HasImages(Page page)
    {
        try
        {
            return page.GetImages().Any();
        }
        catch (Exception)
        {
            // An image the parser cannot decode is still an image on the page.
            return true;
        }
    }

    private static string FoldLigatures(string text) =>
        text.Any(Ligatures.ContainsKey) ? string.Concat(text.Select(c => Ligatures.TryGetValue(c, out var s) ? s : c.ToString())) : text;

    // A word and its pen positions, kept while touching words are rejoined.
    private sealed record PenWord(PositionedWord Word, double PenStart, double PenEnd);
}
