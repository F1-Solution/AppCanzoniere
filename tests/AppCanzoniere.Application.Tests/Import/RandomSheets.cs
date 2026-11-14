using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.Tests.Import;

/// <summary>Random-document features; each finding of the review adds one to the property tests.</summary>
[Flags]
internal enum SheetFeatures
{
    None = 0,
    Brackets = 1,
    Emoji = 2,
    Combining = 4,
    ControlChars = 8,
    WrappedChords = 16,
    All = 31
}

/// <summary>A random pasted document with the lyric lines it must keep and the chord tokens it may contain.</summary>
internal sealed record GeneratedSheet(string Text, IReadOnlyList<string> Lyrics, IReadOnlySet<string> AllowedChords);

/// <summary>Seeded generator of chords-above-lyrics documents with hostile lyric text (invented words only).</summary>
internal static class RandomSheets
{
    // The generator keeps its own character lists, so it stays an independent check of the converter's tables.
    private const string FormFeed = "\u000C";
    private const string VerticalTab = "\u000B";
    private static readonly string ZeroWidthSpace = ((char)0x200B).ToString();
    private static readonly string ByteOrderMark = ((char)0xFEFF).ToString();
    private static readonly string NoBreakSpace = ((char)0x00A0).ToString();
    private static readonly string CombiningAcute = ((char)0x0301).ToString();
    private static readonly string CombiningGrave = ((char)0x0300).ToString();
    private static readonly string CombiningDiaeresis = ((char)0x0308).ToString();
    private static readonly string CombiningTilde = ((char)0x0303).ToString();
    private const string FoldedHands = "\U0001F64F";
    private const string MusicalNotes = "\U0001F3B6";
    private const string MediumSkinTone = "\U0001F3FD";

    private static readonly char[] RemovedChars = { (char)0x000C, (char)0x000B, (char)0x200B, (char)0xFEFF };
    private static readonly char[] SpaceLikeChars = { (char)0x00A0, (char)0x2007, (char)0x202F };
    private const char FirstSpaceInRange = (char)0x2000;
    private const char LastSpaceInRange = (char)0x200A;

    private static readonly string[] Words =
    {
        "Signore", "canta", "forte", "gloria", "pace", "terra", "cielo", "mondo", "fratelli", "luce", "alleluia", "vieni",
        "notte", "stella", "Santo", "Benedetto", "popolo", "sempre", "amore", "casa", "mare", "pane", "vino", "gioia"
    };

    private static readonly string[] ItalianChords = { "Lam", "Sol7", "Do", "Fa", "Re-", "Mi7", "Sib", "Si-", "Solm", "Fa#" };
    private static readonly string[] EnglishChords = { "Am", "G7", "C", "F", "Dm", "E7", "Bb", "F#m" };
    private static readonly string[] Neutrals = { "x2", "|", "N.C.", "%" };

    private static readonly string[] BracketNoise = { "[forte]", "[Rit.]", "[1.]", "[ 3 x ]", "{x2}", "{", "}", "[", "]", "[bis]", "{", "]" };
    private static readonly string[] PlainNoise = { "(bis)", "(2 v.)", "(forte)", "(", ")", NoBreakSpace, "\t" };
    private static readonly string[] EmojiNoise = { FoldedHands, MusicalNotes, FoldedHands + MediumSkinTone };
    private static readonly string[] CombiningNoise =
    {
        "e" + CombiningAcute, "o" + CombiningGrave, "a" + CombiningDiaeresis + CombiningAcute, "n" + CombiningTilde
    };

    private static readonly string[] ControlNoise = { FormFeed, VerticalTab, ZeroWidthSpace, ByteOrderMark };

    public static IEnumerable<GeneratedSheet> Many(int seed, int count, SheetFeatures features)
    {
        var random = new Random(seed);
        for (var i = 0; i < count; i++)
            yield return Create(random, features);
    }

    public static GeneratedSheet Create(Random random, SheetFeatures features)
    {
        var italian = random.Next(2) == 0;
        var chords = italian ? ItalianChords : EnglishChords;
        var inlineChord = italian ? "Lam" : "Am";
        var lines = new List<string>();
        var lyrics = new List<string>();
        var allowed = new HashSet<string>(chords.Concat(Neutrals).Concat(new[] { inlineChord }));
        lines.Add(ChordLine(random, features, chords));
        AddLyric(random, features, inlineChord, lines, lyrics);
        var blocks = random.Next(2, 9);
        for (var b = 0; b < blocks; b++)
            AddBlock(random, features, chords, inlineChord, lines, lyrics);

        var ending = random.Next(2) == 0 ? "\n" : "\r\n";
        var text = string.Join(ending, lines) + (random.Next(2) == 0 ? ending : string.Empty);
        return new GeneratedSheet(text, lyrics, allowed);
    }

    private static void AddBlock(Random r, SheetFeatures f, string[] chords, string inlineChord, List<string> lines, List<string> lyrics)
    {
        switch (r.Next(10))
        {
            case 0 or 1 or 2:
                lines.Add(ChordLine(r, f, chords));
                AddLyric(r, f, inlineChord, lines, lyrics);
                break;
            case 3 or 4:
                AddLyric(r, f, inlineChord, lines, lyrics);
                break;
            case 5:
                lines.Add(BlankLine(r, f));
                break;
            case 6:
                lines.Add(new[] { "Rit.", "1.", "Strofa 2", "Bridge" }[r.Next(4)]);
                break;
            case 7:
                lines.Add(new[] { "N.C.", "  N.C.   x2", "%" }[r.Next(3)]);
                if (r.Next(2) == 0)
                    AddLyric(r, f, inlineChord, lines, lyrics);
                break;
            default:
                lines.Add("e|---3---0---|----");
                break;
        }
    }

    private static string BlankLine(Random r, SheetFeatures f)
    {
        var options = f.HasFlag(SheetFeatures.ControlChars)
            ? new[] { "", "   ", NoBreakSpace, FormFeed, VerticalTab + " " }
            : new[] { "", "   ", NoBreakSpace };
        return options[r.Next(options.Length)];
    }

    private static string ChordLine(Random r, SheetFeatures f, string[] chords)
    {
        var builder = new StringBuilder(new string(' ', r.Next(0, 4)));
        var count = r.Next(2, 6);
        for (var i = 0; i < count; i++)
        {
            var token = i == 0 ? chords[r.Next(chords.Length)] : r.Next(6) == 0 ? Neutrals[r.Next(Neutrals.Length)] : chords[r.Next(chords.Length)];
            builder.Append(Wrap(r, f, token)).Append(r.Next(8) == 0 ? "\t" : new string(' ', r.Next(1, 9)));
        }

        return builder.ToString() + Wrap(r, f, "Lam" == chords[0] ? "Sol7" : "G7");
    }

    private static string Wrap(Random r, SheetFeatures f, string token)
    {
        if (!f.HasFlag(SheetFeatures.WrappedChords) || Neutrals.Contains(token) || r.Next(4) != 0)
            return token;

        return r.Next(5) != 0 ? $"({token})" : $"[{token}]";
    }

    private static void AddLyric(Random r, SheetFeatures f, string inlineChord, List<string> lines, List<string> lyrics)
    {
        var text = LyricText(r, f, inlineChord);
        lines.Add(text);
        var expected = Expected(text, inlineChord);
        if (expected.Length > 0)
            lyrics.Add(expected);
    }

    private static string LyricText(Random r, SheetFeatures f, string inlineChord)
    {
        var noise = PlainNoise.ToList();
        if (f.HasFlag(SheetFeatures.Brackets))
            noise.AddRange(BracketNoise.Concat(new[] { $"[{inlineChord}]", $"[{inlineChord}]" }));
        if (f.HasFlag(SheetFeatures.Emoji))
            noise.AddRange(EmojiNoise);
        if (f.HasFlag(SheetFeatures.Combining))
            noise.AddRange(CombiningNoise);
        if (f.HasFlag(SheetFeatures.ControlChars))
            noise.AddRange(ControlNoise);

        var builder = new StringBuilder(r.Next(3) == 0 ? new string(' ', r.Next(1, 4)) : string.Empty);
        var count = r.Next(2, 7);
        for (var i = 0; i < count; i++)
        {
            if (r.Next(3) == 0)
                builder.Append(noise[r.Next(noise.Count)]);
            builder.Append(Words[r.Next(Words.Length)]);
            if (r.Next(4) == 0)
                builder.Append(noise[r.Next(noise.Count)]);
            builder.Append(' ');
        }

        return builder.ToString().TrimEnd(' ') + (r.Next(5) == 0 ? "," : string.Empty);
    }

    /// <summary>The lyric text the converter must keep: documented normalisation, bracket replacement, inline chords removed.</summary>
    public static string Expected(string line, string inlineChord)
    {
        var normalised = Normalise(line);
        var chord = new Regex($@"\[{Regex.Escape(inlineChord)}\]");
        var kept = new StringBuilder();
        var position = 0;
        foreach (Match m in chord.Matches(normalised))
        {
            kept.Append(ReplaceBrackets(normalised[position..m.Index]));
            position = m.Index + m.Length;
        }

        kept.Append(ReplaceBrackets(normalised[position..]));
        return kept.ToString().Trim();
    }

    private static string ReplaceBrackets(string text) => text.Replace('[', '(').Replace(']', ')').Replace('{', '(').Replace('}', ')');

    /// <summary>Reference of the documented pre-processing: NFC, invisible characters removed, spaces, tabs per text element.</summary>
    public static string Normalise(string line)
    {
        var builder = new StringBuilder();
        foreach (var c in line.Normalize(NormalizationForm.FormC))
        {
            if (c == '\t')
                builder.Append(' ', 8 - new StringInfo(builder.ToString()).LengthInTextElements % 8);
            else if (!RemovedChars.Contains(c))
                builder.Append(IsSpaceLike(c) ? ' ' : c);
        }

        return builder.ToString().TrimEnd();
    }

    private static bool IsSpaceLike(char c) =>
        c == ' ' || SpaceLikeChars.Contains(c) || (c >= FirstSpaceInRange && c <= LastSpaceInRange);
}
