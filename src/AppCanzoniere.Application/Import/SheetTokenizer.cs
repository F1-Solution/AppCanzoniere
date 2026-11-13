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
    bool Foreign = false,
    bool Doubtful = false,
    bool HasInlineChord = false);

/// <summary>Splits a line into neutral marks, words and chords (spec 3.2).</summary>
internal static class SheetTokenizer
{
    private static readonly Regex RepeatMarks = new(
        @"\(\s*\d+\s*v(?:olte)?\.?\s*\)|\(\s*[xX×]\s*\d+\s*\)|\(\s*(?:bis|ter)\s*\)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex Bars = new(@"^[|:]*\|[|:]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex BarRuns = new(@"([|:]*\|[|:]*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RepeatCount = new(@"^(?:[xX×]\d{1,2}|\d{1,2}[xX×])$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex ShortNumber = new(@"^\d{1,3}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly HashSet<string> NeutralWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "/", "%", "-", "–", "—", "*", "...", "bis", "ter", "N.C.", "N.C", "NC", "(", ")", "→", "->"
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
        var marks = RepeatMarks.Matches(text).ToDictionary(m => m.Index);
        var groups = TokenGroups.Find(text, marks, notation);
        var i = start;
        while (i < text.Length)
        {
            if (text[i] == ' ')
            {
                i++;
                continue;
            }

            if (marks.TryGetValue(i, out var mark))
            {
                tokens.Add(new SheetToken(mark.Value, i, TokenKind.Neutral));
                i += mark.Length;
                continue;
            }

            if (groups.TryGetValue(i, out var group))
            {
                tokens.AddRange(group.Tokens);
                i += group.Length;
                continue;
            }

            var end = i;
            while (end < text.Length && text[end] != ' ' && (end == i || !EndsRun(text, end, marks, groups)))
                end++;
            tokens.AddRange(ClassifyRaw(text[i..end], i, notation));
            i = end;
        }

        return tokens.Select(t => t.Kind == TokenKind.Word && BracketedText.ContainsInlineChord(t.Text) ? t with { HasInlineChord = true } : t).ToList();
    }

    // A run also ends before a chord group that follows a dash, as in "[Re]-[Sol]".
    private static bool EndsRun(string text, int end, Dictionary<int, Match> marks, Dictionary<int, TokenGroup> groups) =>
        marks.ContainsKey(end) || (groups.ContainsKey(end) && text[end - 1] is '-' or '–');

    public static bool IsNoChord(SheetToken token) =>
        token.Kind == TokenKind.Neutral &&
        (token.Text.Equals("%", StringComparison.Ordinal) ||
         token.Text.Replace(".", string.Empty, StringComparison.Ordinal).Equals("NC", StringComparison.OrdinalIgnoreCase));

    /// <summary>True when the text is a chord of the notation and starts with an upper-case letter.</summary>
    public static bool TryChord(string text, Notation notation) =>
        text.Length > 0 && char.IsUpper(text[0]) && ChordWords.TryParseDefinite(text, notation, out _);

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
        NeutralWords.Contains(text) || Bars.IsMatch(text) || RepeatCount.IsMatch(text) || ShortNumber.IsMatch(text) || IsBassOnly(text);

    // A bare bass note such as "/do" annotates the previous chord.
    private static bool IsBassOnly(string text) =>
        text.Length > 1 && text[0] == '/' &&
        (ChordSymbolParser.TryParseNote(text[1..], Notation.Italian, out _) || ChordSymbolParser.TryParseNote(text[1..], Notation.English, out _));

    private static IEnumerable<SheetToken> ClassifyCore(string core, int column, Notation notation)
    {
        if (IsNeutral(core))
            return new[] { new SheetToken(core, column, TokenKind.Neutral) };

        if (",;:!?.".Contains(core[^1]) || core.Contains('\'') || core.Contains('’'))
            return new[] { Word(core, column) };

        if (core.Length > 2 && ((core[0] == '(' && core[^1] == ')') || (core[0] == '[' && core[^1] == ']')))
        {
            var inner = core[1..^1];
            return new[] { ParenthesisedToken(core, column, inner, notation) };
        }

        if (StrayParenthesis(core, notation) is { } stray)
            return new[] { Chord(core, column, stray, notation, true) };

        if (TryChord(core, notation))
            return new[] { Chord(core, column, core, notation, false) };

        return Sequence(core, column, notation) ?? new[] { WordOrLooseChord(core, column, notation) };
    }

    // A chord with one unmatched parenthesis ("(Lam", "Re)") is the chord; the stray parenthesis is dropped.
    private static string? StrayParenthesis(string core, Notation notation)
    {
        var chord = core.Length > 1 && core[0] == '(' && !core.Contains(')') ? core[1..]
            : core.Length > 1 && core[^1] == ')' && !core.Contains('(') ? core[..^1]
            : null;
        return chord is not null && TryChord(chord, notation) ? chord : null;
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

    private static SheetToken ParenthesisedToken(string core, int column, string inner, Notation notation)
    {
        if (TryChord(inner, notation))
            return Chord(core, column, inner, notation, true);

        if (char.IsLower(inner[0]) && ChordWords.TryParseDefinite(inner, notation, out _))
            return new SheetToken(core, column, TokenKind.Word, Parenthesised: true, LowerChord: true);

        var other = notation == Notation.Italian ? Notation.English : Notation.Italian;
        return char.IsUpper(inner[0]) && ChordWords.TryParseDefinite(inner, other, out _)
            ? new SheetToken(core, column, TokenKind.Word, Parenthesised: true, Foreign: true)
            : Word(core, column);
    }

    // Lower-case chords and chords of the other notation stay words unless the whole line turns out to be a chord line.
    private static SheetToken WordOrLooseChord(string core, int column, Notation notation)
    {
        var other = notation == Notation.Italian ? Notation.English : Notation.Italian;
        if (char.IsLower(core[0]) && ChordWords.TryParseDefinite(core, notation, out _))
            return new SheetToken(core, column, TokenKind.Word, LowerChord: true);

        if (!char.IsUpper(core[0]))
            return Word(core, column);

        if (ChordWords.TryParseDefinite(core, other, out _))
            return new SheetToken(core, column, TokenKind.Word, Foreign: true);

        return ChordWords.IsDoubtful(core, notation) ? new SheetToken(core, column, TokenKind.Word, Doubtful: true) : Word(core, column);
    }

    private static SheetToken Word(string text, int column) => new(text, column, TokenKind.Word);

    internal static SheetToken Chord(string text, int column, string chord, Notation notation, bool parenthesised) =>
        new(text, column, TokenKind.Chord, chord, IsAmbiguous(chord, notation), parenthesised);
}
