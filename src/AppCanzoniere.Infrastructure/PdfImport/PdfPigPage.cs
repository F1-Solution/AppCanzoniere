using AppCanzoniere.Application.PdfImport;
using UglyToad.PdfPig.Content;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Converts one PdfPig page into the positioned words of the application model.</summary>
internal static class PdfPigPage
{
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
        var words = page.GetWords()
            .Where(w => w.TextOrientation == TextOrientation.Horizontal && !string.IsNullOrWhiteSpace(w.Text))
            .Select(w => ToWord(w, origin.Left, origin.Bottom))
            .ToList();
        return new PdfPageWords(page.Number, page.Width, page.Height, words, HasImages(page));
    }

    private static PositionedWord ToWord(Word word, double originLeft, double originBottom)
    {
        var text = FoldLigatures(word.Text);
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
            var text = FoldLigatures(letter.Value);
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
}
