using System.Text;

namespace AppCanzoniere.Application.Tests.Layout;

/// <summary>Builds random ChordPro songs from a seeded generator: lyrics, chords, instrumentals, comments, sections and tabs.</summary>
internal static class SongGenerator
{
    private static readonly string[] Chords = { "Do", "Sol7", "Re/Fa#", "La-", "Mim", "*x2", "Sib", "Fa#m7" };
    private static readonly string[] Words = { "Signore", "pietà", "Alleluia !", "e", "il", "cuore", "à", "Cristo", "ci", "salverà" };

    public static string Song(Random random)
    {
        var text = new StringBuilder();
        if (random.Next(4) == 0)
            text.Append("{title: Titolo}\n");

        for (var lines = random.Next(1, 14); lines > 0; lines--)
            AppendBlock(text, random);

        return text.ToString();
    }

    private static void AppendBlock(StringBuilder text, Random random)
    {
        switch (random.Next(20))
        {
            case < 10:
                text.Append(Lyric(random)).Append('\n');
                break;
            case < 12:
                text.Append(Instrumental(random)).Append('\n');
                break;
            case < 14:
                text.Append("{c: ").Append(Phrase(random, 25)).Append("}\n");
                break;
            case < 16:
                text.Append('\n');
                break;
            case < 18:
                Section(text, random, random.Next(2) == 0 ? "soc" : "sob", random.Next(2) == 0 ? "eoc" : "eob");
                break;
            default:
                Tab(text, random);
                break;
        }
    }

    private static void Section(StringBuilder text, Random random, string open, string close)
    {
        text.Append('{').Append(open).Append("}\n");
        for (var lines = random.Next(1, 5); lines > 0; lines--)
            text.Append(random.Next(6) == 0 ? "{c: " + Phrase(random, 4) + "}" : Lyric(random)).Append('\n');
        text.Append('{').Append(close).Append("}\n");
    }

    private static void Tab(StringBuilder text, Random random)
    {
        text.Append("{sot}\n");
        for (var lines = random.Next(2, 4); lines > 0; lines--)
            text.Append("e|").Append(new string('-', random.Next(4, 60))).Append("|\n");
        text.Append("{eot}\n");
    }

    private static string Lyric(Random random)
    {
        var line = new StringBuilder();
        for (var words = random.Next(1, 16); words > 0; words--)
        {
            if (random.Next(10) < 4)
                line.Append('[').Append(Chords[random.Next(Chords.Length)]).Append(']');
            line.Append(Word(random));
            if (random.Next(10) < 2)
                line.Append('[').Append(Chords[random.Next(Chords.Length)]).Append(']');
            line.Append(' ');
        }

        return line.ToString();
    }

    private static string Word(Random random) =>
        random.Next(25) == 0 ? new string('w', random.Next(20, 50)) : Words[random.Next(Words.Length)];

    private static string Instrumental(Random random) =>
        string.Join(' ', Enumerable.Range(0, random.Next(1, 25)).Select(_ => "[" + Chords[random.Next(Chords.Length)] + "]"));

    private static string Phrase(Random random, int maxWords) =>
        string.Join(' ', Enumerable.Range(0, random.Next(1, maxWords)).Select(_ => Word(random)));
}
