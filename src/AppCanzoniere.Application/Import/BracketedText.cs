using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>Square-bracket tokens of a line: inline chords and annotations are kept, anything else is plain text.</summary>
internal static class BracketedText
{
    private static readonly Regex Pair = new(@"\[([^\[\]]*)\]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static IEnumerable<Match> Matches(string text) => Pair.Matches(text);

    /// <summary>True for a chord of either notation written without spaces inside the brackets.</summary>
    public static bool IsInlineChord(string inner) =>
        inner.Length > 0 && !char.IsWhiteSpace(inner[0]) && !char.IsWhiteSpace(inner[^1]) &&
        (ChordWords.IsChord(inner, Notation.Italian) || ChordWords.IsChord(inner, Notation.English));

    /// <summary>True for the converter's own neutral mark, such as the text of <c>[*N.C.]</c> read as "*N.C.".</summary>
    public static bool IsAnnotation(string inner) => inner.Length > 1 && inner[0] == '*';

    public static bool IsKept(string inner) => IsAnnotation(inner) || IsInlineChord(inner);

    public static bool ContainsInlineChord(string text) => Matches(text).Any(m => IsInlineChord(m.Groups[1].Value));

    /// <summary>True when a chord sits directly against a letter: the evidence that a line is already ChordPro.</summary>
    public static bool HasAttachedChord(string text) =>
        Matches(text).Any(m => IsInlineChord(m.Groups[1].Value) &&
            ((m.Index > 0 && char.IsLetter(text[m.Index - 1])) ||
             (m.Index + m.Length < text.Length && char.IsLetter(text[m.Index + m.Length]))));
}
