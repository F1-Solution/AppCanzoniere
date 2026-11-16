using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

/// <summary>
/// Builds synthetic positioned words the way a PDF with a proportional font would place them.
/// Narrow letters (i, l, t, punctuation) and wide letters (m, w, capitals) have different widths,
/// so a renderer that assumes one width per character drifts and the tests notice it.
/// </summary>
internal sealed class PageBuilder
{
    public const double PageWidth = 595;
    public const double PageHeight = 842;
    public const double TextHeight = 8;
    public const double SpaceWidth = 3;

    private readonly List<PositionedWord> _words = new();

    /// <summary>When true, every word carries the x of each of its letters, as words read from a real PDF do.</summary>
    public bool WithLetters { get; init; }

    public IReadOnlyList<PositionedWord> Words => _words;

    public static double Width(char c) => c switch
    {
        'i' or 'l' or 'j' or 't' or 'f' or 'r' or '\'' or ',' or '.' or '-' or ':' or '!' => 3,
        'm' or 'w' => 9,
        >= 'A' and <= 'Z' => 8,
        _ => 6
    };

    public static double Width(string text) => text.Sum(Width);

    /// <summary>Adds a line of text whose words are separated by single spaces, starting at x.</summary>
    public PageBuilder Text(string text, double x, double baseline)
    {
        foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            Word(word, x, baseline);
            x += Width(word) + SpaceWidth;
        }

        return this;
    }

    /// <summary>Adds one word with its left edge at x.</summary>
    public PageBuilder Word(string text, double x, double baseline)
    {
        var letters = WithLetters ? text.Select((_, i) => x + Width(text[..i])).ToList() : null;
        _words.Add(new PositionedWord(text, new WordBox(x, baseline, x + Width(text), baseline + TextHeight), letters));
        return this;
    }

    /// <summary>X of the character at the given index of a line written with Text at x.</summary>
    public static double XOf(string line, int index, double x) => x + Width(line[..index].Replace(" ", string.Empty)) + line[..index].Count(c => c == ' ') * SpaceWidth;

    /// <summary>Adds a chord line above a lyric line: each chord sits over the character that follows its marker '^' in the lyric.</summary>
    public PageBuilder ChordsOver(string markedLyric, string[] chords, double x, double baseline, double pitch = 12)
    {
        var lyric = markedLyric.Replace("^", string.Empty);
        var chord = 0;
        for (int i = 0, plain = 0; i < markedLyric.Length; i++)
        {
            if (markedLyric[i] != '^')
            {
                plain++;
                continue;
            }

            Word(chords[chord++], XOf(lyric, plain, x), baseline + pitch);
        }

        return Text(lyric, x, baseline);
    }

    /// <summary>Adds a song of chord and lyric pairs going down from the top baseline; returns the next free baseline.</summary>
    public double Song(IEnumerable<string> markedLyrics, string[] chords, double x, double top, double pitch = 12)
    {
        var baseline = top;
        foreach (var lyric in markedLyrics)
        {
            var count = lyric.Count(c => c == '^');
            ChordsOver(lyric, chords.Take(count).ToArray(), x, baseline - pitch, pitch);
            baseline -= 2 * pitch;
        }

        return baseline;
    }

    public PdfPageWords Page(int number = 1, bool hasImages = false) =>
        new(number, PageWidth, PageHeight, _words.ToList(), hasImages);
}
