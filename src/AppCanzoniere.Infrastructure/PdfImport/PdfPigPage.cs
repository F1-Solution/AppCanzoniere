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
            .Select(w => new PositionedWord(
                FoldLigatures(w.Text),
                new WordBox(
                    w.BoundingBox.Left - origin.Left,
                    w.BoundingBox.Bottom - origin.Bottom,
                    w.BoundingBox.Right - origin.Left,
                    w.BoundingBox.Top - origin.Bottom)))
            .ToList();
        return new PdfPageWords(page.Number, page.Width, page.Height, words, HasImages(page));
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
