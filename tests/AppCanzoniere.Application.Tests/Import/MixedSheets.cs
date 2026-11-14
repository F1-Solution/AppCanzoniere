using System.Text;

namespace AppCanzoniere.Application.Tests.Import;

/// <summary>
/// Seeded generator of short documents that mix bracketed and plain chords of both notations, lyrics made of
/// note-name words, short words and leading digits, and chords placed over gaps or past the end of the lyric.
/// Documents are short so that few chords end up glued to a letter and a second conversion reads them again.
/// Its tables are its own and not shared with the converter.
/// </summary>
internal static class MixedSheets
{
    private static readonly string[] Words =
    {
        "Vieni", "canta", "luce", "pace", "gloria", "Signore", "casa", "balla", "cielo", "terra", "Lord", "home", "come", "now",
        "house", "sing", "light"
    };

    private static readonly string[] ShortWords = { "e", "a", "o", "è", "la", "il", "di", "va", "to", "I", "E", "Go", "La", "Re", "do", "mi" };
    private static readonly string[] Punctuation = { "…", "«", "»", "“", "”", ",", ".", "!", "...", "-" };
    private static readonly string[] NoteNames = { "Re", "Do", "La", "Mi", "Si", "Sol", "Fa", "A", "Am", "Go" };
    private static readonly string[] ItalianChords = { "Sib", "Lam", "Do", "Re", "Sol7", "Mi-", "Fa#", "La", "Rem" };
    private static readonly string[] EnglishChords = { "G", "Am", "C", "D7", "Bb", "E", "F#m", "A", "Em" };
    private static readonly string[] OtherWords = { "e", "a", "E", "Am", "la", "si", "Do", "mi", "Re", "G", "C", "Fa" };
    private static readonly string[] LongWords = { "Vieni", "Signore", "gloria", "Cantate", "Alleluia" };
    private static readonly string[] LowerNotes = { "do", "fa", "sol", "re", "la", "mi" };
    private static readonly string[] Leads = { "2", "1.", "«", "(bis)", "x2", "3)" };

    public static IEnumerable<string> Many(int seed, int count)
    {
        var random = new Random(seed);
        for (var i = 0; i < count; i++)
            yield return Create(random);
    }

    public static string Create(Random r)
    {
        var italian = r.Next(2) == 0;
        var lines = new List<string>(Anchors(italian));
        var blocks = r.Next(1, 4);
        for (var b = 0; b < blocks; b++)
        {
            if (b > 0 && r.Next(2) == 0)
                lines.Add(string.Empty);
            AddBlock(r, italian, lines);
        }

        return string.Join("\n", lines) + (r.Next(2) == 0 ? "\n" : string.Empty);
    }

    // Two instrumental lines in the document's notation, each followed by a blank line, so the notation is clear
    // without gluing any chord to a letter. A document whose notation rests on one or two chord lines can still change
    // notation on a second conversion when those lines are merged; that is a separate problem, not generated here.
    private static IEnumerable<string> Anchors(bool italian) =>
        italian
            ? new[] { "Do   Sol   Lam   Fa", string.Empty, "Rem   Sol7   Do", string.Empty }
            : new[] { "C   G   Am   F", string.Empty, "Dm   G7   C", string.Empty };

    private static void AddBlock(Random r, bool italian, List<string> lines)
    {
        switch (r.Next(6))
        {
            case 0:
                lines.Add(ChordLine(r, italian));
                lines.Add(NoteNameLyric(r));
                break;
            case 1:
                lines.Add(ChordLine(r, italian));
                lines.Add(Leads[r.Next(Leads.Length)] + " " + Lyric(r, italian));
                break;
            case 2:
                lines.Add(Lyric(r, italian));
                break;
            case 3 when italian:
                AddAlignedLowerCase(r, lines);
                break;
            default:
                lines.Add(ChordLine(r, italian));
                lines.Add(Lyric(r, italian));
                break;
        }
    }

    // One to four tokens: plain or bracketed chords of the document, a bracketed or parenthesised chord of the other notation.
    private static string ChordLine(Random r, bool italian)
    {
        var own = italian ? ItalianChords : EnglishChords;
        var other = italian ? EnglishChords : ItalianChords;
        var builder = new StringBuilder(new string(' ', r.Next(0, 6)));
        var count = r.Next(1, 5);
        for (var i = 0; i < count; i++)
        {
            var chord = own[r.Next(own.Length)];
            builder.Append(r.Next(8) switch
            {
                0 or 1 => $"[{chord}]",
                2 => $"[{other[r.Next(other.Length)]}]",
                3 => $"({chord})",
                4 => NoteNames[r.Next(NoteNames.Length)],
                _ => chord
            });
            if (i < count - 1)
                builder.Append(r.Next(6) == 0 ? "\t" : new string(' ', r.Next(1, 9)));
        }

        return builder.ToString();
    }

    // Lower-case notes over the word starts of a lyric that holds a bracketed word of the other notation, with one wide gap.
    private static void AddAlignedLowerCase(Random r, List<string> lines)
    {
        var words = new List<string> { LongWords[r.Next(LongWords.Length)] };
        var count = r.Next(2, 5);
        for (var i = 1; i < count; i++)
            words.Add(r.Next(3) == 0 ? $"[{OtherWords[r.Next(OtherWords.Length)]}]" : ShortWords[r.Next(ShortWords.Length)]);

        var chords = new StringBuilder();
        var column = 0;
        foreach (var word in words)
        {
            var note = LowerNotes[r.Next(LowerNotes.Length)];
            if (chords.Length > 0 && chords.Length >= column)
                break;
            chords.Append(' ', column - chords.Length).Append(note);
            column += word.Length + 1;
        }

        lines.Add(chords.ToString());
        lines.Add(string.Join(" ", words));
    }

    // The same note name repeated, separated by tabs or wide gaps, so a chord placed over it lands in a gap.
    private static string NoteNameLyric(Random r)
    {
        var name = NoteNames[r.Next(NoteNames.Length)];
        var count = r.Next(1, 4);
        return string.Join(r.Next(2) == 0 ? "\t" : "     ", Enumerable.Repeat(name, count));
    }

    // Plain words, short words, typographic punctuation, wide gaps and bracketed words of either notation.
    private static string Lyric(Random r, bool italian)
    {
        var own = italian ? ItalianChords : EnglishChords;
        var builder = new StringBuilder();
        var count = r.Next(2, 7);
        for (var i = 0; i < count; i++)
        {
            builder.Append(r.Next(10) switch
            {
                0 or 1 => ShortWords[r.Next(ShortWords.Length)],
                2 => $"[{OtherWords[r.Next(OtherWords.Length)]}]",
                3 => $"[{own[r.Next(own.Length)]}]",
                4 => Punctuation[r.Next(Punctuation.Length)] + Words[r.Next(Words.Length)],
                5 => Words[r.Next(Words.Length)] + Punctuation[r.Next(Punctuation.Length)],
                _ => Words[r.Next(Words.Length)]
            });
            if (i < count - 1)
                builder.Append(r.Next(6) == 0 ? "    " : " ");
        }

        return builder.ToString();
    }
}
