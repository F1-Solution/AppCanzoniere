using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>Square-bracket tokens of a line: inline chords and annotations are kept, anything else is plain text.</summary>
internal static class BracketedText
{
    private static readonly Regex Pair = new(@"\[([^\[\]]*)\]", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex Parenthesised = new(@"\(([^()\s]+)\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static IEnumerable<Match> Matches(string text) => Pair.Matches(text);

    /// <summary>True for a chord written without spaces inside the brackets: of <paramref name="only"/> when given, else of either notation.</summary>
    public static bool IsInlineChord(string inner, Notation? only = null) =>
        inner.Length > 0 && !char.IsWhiteSpace(inner[0]) && !char.IsWhiteSpace(inner[^1]) &&
        (only is { } n
            ? ChordWords.IsChord(inner, n)
            : ChordWords.IsChord(inner, Notation.Italian) || ChordWords.IsChord(inner, Notation.English));

    /// <summary>True for the converter's own neutral mark, such as the text of <c>[*N.C.]</c> read as "*N.C.".</summary>
    public static bool IsAnnotation(string inner) => inner.Length > 1 && inner[0] == '*';

    public static bool IsKept(string inner, Notation? only = null) => IsAnnotation(inner) || IsInlineChord(inner, only);

    public static bool ContainsInlineChord(string text) => Matches(text).Any(m => IsInlineChord(m.Groups[1].Value));

    /// <summary>True when every bracketed chord of only the other notation has a space or the line start before it and one space and a word after it.</summary>
    public static bool OtherNotationStandsBetweenWords(string text, int start, Notation notation) =>
        Matches(text).Where(m => m.Index >= start && IsInlineChord(m.Groups[1].Value) && !IsInlineChord(m.Groups[1].Value, notation))
            .All(m => StandsBetweenWords(text, start, m.Index, m.Index + m.Length));

    /// <summary>True when a chord of <paramref name="notation"/> in parentheses stands between words, as a bracket written as text does.</summary>
    public static bool HasParenthesisedChordBetweenWords(string text, Notation notation) =>
        Parenthesised.Matches(text).Any(m => IsInlineChord(m.Groups[1].Value, notation) && StandsBetweenWords(text, 0, m.Index, m.Index + m.Length));

    // The converter writes a chord that misses a letter after the lyric or before a gap of three spaces, never like this.
    private static bool StandsBetweenWords(string text, int start, int index, int end) =>
        (index == start || text[index - 1] == ' ') &&
        end + 1 < text.Length && text[end] == ' ' && text[end + 1] is not (' ' or '[');

    /// <summary>True when a chord sits directly against a letter: the evidence that a line is already ChordPro.</summary>
    public static bool HasAttachedChord(string text) =>
        Matches(text).Any(m => IsInlineChord(m.Groups[1].Value) &&
            ((m.Index > 0 && char.IsLetter(text[m.Index - 1])) ||
             (m.Index + m.Length < text.Length && char.IsLetter(text[m.Index + m.Length]))));
}
