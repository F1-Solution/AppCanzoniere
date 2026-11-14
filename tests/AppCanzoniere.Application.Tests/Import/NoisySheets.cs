using System.Text;

namespace AppCanzoniere.Application.Tests.Import;

/// <summary>Seeded generator of very noisy pasted documents; its tables are its own and not shared with the converter.</summary>
internal static class NoisySheets
{
    private static readonly string[] Words =
    {
        "Signore", "canta", "forte", "gloria", "pace", "terra", "cielo", "mondo", "luce", "vieni", "notte", "stella", "Santo",
        "popolo", "sempre", "amore", "casa", "mare", "pane", "vino", "gioia", "Lord", "hear", "love", "light", "song"
    };

    private static readonly string[] NoteWords = { "A", "E", "a", "e", "Do", "Re", "Mi", "Fa", "Sol", "La", "Si", "do", "re", "mi", "fa", "sol", "la", "si", "Go", "Ami", "Remi", "Mimi", "Domi", "Fami", "Dono", "B", "C", "D", "G" };

    private static readonly string[] ItalianChords = { "Lam", "Sol7", "Do", "Fa", "Re-", "Mi7", "Sib", "Si-", "Solm", "Fa#", "Rem", "La", "Re", "Mi", "Si", "Sol", "Fa/La", "Do7+", "Lab" };
    private static readonly string[] EnglishChords = { "Am", "G7", "C", "F", "Dm", "E7", "Bb", "F#m", "A", "D", "E", "G", "B", "Dmi", "Gmi", "Ami", "Cmaj7", "D/F#", "Em" };
    private static readonly string[] LowerChords = { "do", "re-", "sol7", "lam", "fa", "mi7", "la", "si", "do/mi", "fa#", "re", "mi", "sol" };
    private static readonly string[] Neutrals = { "x2", "|", "N.C.", "%", "NC", "(bis)", "(2 v.)", "-", "/", "*", "->", "||", "x3", "3x" };
    private static readonly string[] Annotations = { "[*N.C.]", "[*x2]", "[*bis]", "[*]", "[**]", "[*(a)]", "[* ]", "[*[x]]" };
    private static readonly string[] BracketNoise = { "[forte]", "[Rit.]", "[1.]", "[ 3 x ]", "{x2}", "{", "}", "[", "]", "[bis]", "[Do]", "[Am]", "[Lam]", "[ A7 ]", "[]", "[ ]", "[C", "D]" };
    private static readonly string[] ParenNoise = { "(bis)", "(2 v.)", "(forte)", "(", ")", "(Do)", "(Lam)", "(do)", "(A)", "(Re", "Re)", "( Lam )", "(Do Lam)", "()", "(a b)" };
    private static readonly string[] Labels = { "Rit.", "Rit. 2", "1.", "2) ", "Strofa 2", "Bridge", "Intro: Do Sol", "Coro:", "Finale", "Ponte", "Rit. Santo", "3. SANTO SANTO", "12. INSIEME" };
    private static readonly string[] Metadata = { "Titolo: Canto", "Tonalità: Re", "Capo 2", "Tempo: 4/4", "Tonalità: boh", "Capotasto: II", "Key: G" };
    private static readonly string[] Misc = { "N.C.", "  N.C.   x2", "%", "e|---3---0---|----", "x2", "| Do | Sol |", "Do", "A", "Re (do)", "Am", "Ami", "[Do]", "[*N.C.]", "(Do)", "12", "--" };

    public static IEnumerable<string> Many(int seed, int count)
    {
        var random = new Random(seed);
        for (var i = 0; i < count; i++)
            yield return Create(random);
    }

    public static string Create(Random r)
    {
        var lines = new List<string>();
        var style = r.Next(4);
        var blocks = r.Next(1, 9);
        for (var b = 0; b < blocks; b++)
        {
            switch (r.Next(12))
            {
                case 0 or 1 or 2 or 3:
                    lines.Add(ChordLine(r, style));
                    lines.Add(LyricLine(r));
                    break;
                case 4 or 5:
                    lines.Add(LyricLine(r));
                    break;
                case 6:
                    lines.Add(ChordLine(r, style));
                    break;
                case 7:
                    lines.Add(string.Empty);
                    break;
                case 8:
                    lines.Add(Labels[r.Next(Labels.Length)]);
                    break;
                case 9:
                    lines.Add(Metadata[r.Next(Metadata.Length)]);
                    break;
                default:
                    lines.Add(Misc[r.Next(Misc.Length)]);
                    break;
            }
        }

        var ending = r.Next(3) == 0 ? "\r\n" : "\n";
        return string.Join(ending, lines) + (r.Next(2) == 0 ? ending : string.Empty);
    }

    private static string[] Chords(Random r, int style) => style switch
    {
        0 => ItalianChords,
        1 => EnglishChords,
        2 => LowerChords,
        _ => r.Next(2) == 0 ? ItalianChords : EnglishChords
    };

    private static string ChordLine(Random r, int style)
    {
        var pool = Chords(r, style);
        var builder = new StringBuilder(new string(' ', r.Next(0, 4)));
        var count = r.Next(1, 7);
        for (var i = 0; i < count; i++)
        {
            builder.Append(Token(r, pool));
            if (i < count - 1)
                builder.Append(r.Next(10) == 0 ? "\t" : new string(' ', r.Next(1, 9)));
        }

        return builder.ToString();
    }

    private static string Token(Random r, string[] pool)
    {
        var roll = r.Next(20);
        var chord = pool[r.Next(pool.Length)];
        return roll switch
        {
            0 => Neutrals[r.Next(Neutrals.Length)],
            1 => $"({chord})",
            2 => $"[{chord}]",
            3 => $"[ {chord} ]",
            4 => $"({chord} {pool[r.Next(pool.Length)]})",
            5 => "(" + chord,
            6 => chord + ")",
            7 => Annotations[r.Next(Annotations.Length)],
            8 => NoteWords[r.Next(NoteWords.Length)],
            9 => $"[{chord}]-[{pool[r.Next(pool.Length)]}]",
            10 => $"{chord}-{pool[r.Next(pool.Length)]}",
            11 => Words[r.Next(Words.Length)],
            _ => chord
        };
    }

    private static string LyricLine(Random r)
    {
        var builder = new StringBuilder(r.Next(4) == 0 ? new string(' ', r.Next(1, 5)) : string.Empty);
        var count = r.Next(1, 8);
        for (var i = 0; i < count; i++)
        {
            builder.Append(LyricPiece(r));
            if (i < count - 1)
                builder.Append(r.Next(8) == 0 ? "   " : " ");
        }

        return builder.ToString();
    }

    private static string LyricPiece(Random r)
    {
        var word = Words[r.Next(Words.Length)];
        return r.Next(12) switch
        {
            0 => NoteWords[r.Next(NoteWords.Length)],
            1 => BracketNoise[r.Next(BracketNoise.Length)] + word,
            2 => word + BracketNoise[r.Next(BracketNoise.Length)],
            3 => ParenNoise[r.Next(ParenNoise.Length)],
            4 => Annotations[r.Next(Annotations.Length)],
            5 => Annotations[r.Next(Annotations.Length)] + word,
            6 => word + ",",
            7 => NoteWords[r.Next(NoteWords.Length)] + " " + word,
            8 => BracketNoise[r.Next(BracketNoise.Length)],
            _ => word
        };
    }
}
