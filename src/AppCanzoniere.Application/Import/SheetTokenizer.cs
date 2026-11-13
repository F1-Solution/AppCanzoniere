using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

internal enum TokenKind
{
    Neutral,
    Word,
    Chord
}

/// <summary>A run of non-space characters with its source column; Chord is the chord text without parentheses.</summary>
internal sealed record SheetToken(
    string Text,
    int Column,
    TokenKind Kind,
    string Chord = "",
    bool Ambiguous = false,
    bool Parenthesised = false,
    bool LowerChord = false,
    bool Foreign = false);

/// <summary>Splits a line into neutral marks, words and chords (spec 3.2).</summary>
internal static class SheetTokenizer
{
    private static readonly Regex RepeatMarks = new(
        @"\(\s*\d+\s*v(?:olte)?\.?\s*\)|\(\s*[xX×]\s*\d+\s*\)|\(\s*(?:bis|ter)\s*\)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex Bars = new(@"^[|:]*\|[|:]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex BarRuns = new(@"([|:]*\|[|:]*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RepeatCount = new(@"^(?:[xX×]\d{1,2}|\d{1,2}[xX×])$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex ShortNumber = new(@"^\d{1,2}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly HashSet<string> NeutralWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "/", "%", "-", "–", "—", "*", "...", "bis", "ter", "N.C.", "N.C", "NC"
    };

    private static readonly HashSet<string> ItalianBareNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Do", "Re", "Mi", "Fa", "Sol", "La", "Si"
    };

    private static readonly HashSet<string> EnglishAmbiguous = new(StringComparer.Ordinal) { "A", "Am", "Go" };

    /// <summary>Tokens of <paramref name="text"/> from index <paramref name="start"/>, with absolute columns.</summary>
    public static IReadOnlyList<SheetToken> Tokenize(string text, int start, Notation notation)
    {
        var tokens = new List<SheetToken>();
        var marks = RepeatMarks.Matches(text);
        var i = start;
        while (i < text.Length)
        {
            if (text[i] == ' ')
            {
                i++;
                continue;
            }

            var mark = marks.FirstOrDefault(m => m.Index == i);
            if (mark is not null)
            {
                tokens.Add(new SheetToken(mark.Value, i, TokenKind.Neutral));
                i += mark.Length;
                continue;
            }

            var end = i;
            while (end < text.Length && text[end] != ' ' && (end == i || marks.All(m => m.Index != end)))
                end++;
            tokens.AddRange(ClassifyRaw(text[i..end], i, notation));
            i = end;
        }

        return tokens;
    }

    public static bool IsNoChord(SheetToken token) =>
        token.Kind == TokenKind.Neutral &&
        (token.Text.Equals("%", StringComparison.Ordinal) ||
         token.Text.Replace(".", string.Empty, StringComparison.Ordinal).Equals("NC", StringComparison.OrdinalIgnoreCase));

    /// <summary>True when the text is a chord of the notation and starts with an upper-case letter.</summary>
    public static bool TryChord(string text, Notation notation) =>
        text.Length > 0 && char.IsUpper(text[0]) && ChordSymbolParser.TryParse(text, notation, out _);

    public static bool IsAmbiguous(string chord, Notation notation) =>
        notation == Notation.Italian ? ItalianBareNames.Contains(chord) : EnglishAmbiguous.Contains(chord);

    private static IEnumerable<SheetToken> ClassifyRaw(string raw, int column, Notation notation)
    {
        if (IsNeutral(raw))
            return new[] { new SheetToken(raw, column, TokenKind.Neutral) };

        if (!raw.Contains('|'))
            return ClassifyCore(raw, column, notation);

        var tokens = new List<SheetToken>();
        var offset = column;
        foreach (var piece in BarRuns.Split(raw).Where(p => p.Length > 0))
        {
            tokens.AddRange(Bars.IsMatch(piece)
                ? new[] { new SheetToken(piece, offset, TokenKind.Neutral) }
                : ClassifyCore(piece, offset, notation));
            offset += piece.Length;
        }

        return tokens;
    }

    private static bool IsNeutral(string text) =>
        NeutralWords.Contains(text) || Bars.IsMatch(text) || RepeatCount.IsMatch(text) || ShortNumber.IsMatch(text);

    private static IEnumerable<SheetToken> ClassifyCore(string core, int column, Notation notation)
    {
        if (IsNeutral(core))
            return new[] { new SheetToken(core, column, TokenKind.Neutral) };

        if (",;:!?.".Contains(core[^1]) || core.Contains('\'') || core.Contains('’'))
            return new[] { Word(core, column) };

        if (core.Length > 2 && core[0] == '(' && core[^1] == ')')
        {
            var inner = core[1..^1];
            return new[] { TryChord(inner, notation) ? Chord(core, column, inner, notation, true) : Word(core, column) };
        }

        if (TryChord(core, notation))
            return new[] { Chord(core, column, core, notation, false) };

        return Sequence(core, column, notation) ?? new[] { WordOrLooseChord(core, column, notation) };
    }

    private static IEnumerable<SheetToken>? Sequence(string core, int column, Notation notation)
    {
        var parts = core.Split('-', '–');
        if (parts.Length < 2 || parts.Any(p => !TryChord(p, notation)))
            return null;

        var tokens = new List<SheetToken>();
        var offset = column;
        for (var i = 0; i < parts.Length; i++)
        {
            tokens.Add(Chord(parts[i], offset, parts[i], notation, false));
            offset += parts[i].Length;
            if (i < parts.Length - 1)
                tokens.Add(new SheetToken(core[offset - column].ToString(), offset++, TokenKind.Neutral));
        }

        return tokens;
    }

    // Lower-case chords and chords of the other notation stay words unless the whole line turns out to be a chord line.
    private static SheetToken WordOrLooseChord(string core, int column, Notation notation)
    {
        var other = notation == Notation.Italian ? Notation.English : Notation.Italian;
        if (char.IsLower(core[0]) && ChordSymbolParser.TryParse(core, notation, out _))
            return new SheetToken(core, column, TokenKind.Word, LowerChord: true);

        return char.IsUpper(core[0]) && ChordSymbolParser.TryParse(core, other, out _)
            ? new SheetToken(core, column, TokenKind.Word, Foreign: true)
            : Word(core, column);
    }

    private static SheetToken Word(string text, int column) => new(text, column, TokenKind.Word);

    private static SheetToken Chord(string text, int column, string chord, Notation notation, bool parenthesised) =>
        new(text, column, TokenKind.Chord, chord, IsAmbiguous(chord, notation), parenthesised);
}
