using System.Text.RegularExpressions;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

/// <summary>Keeps square brackets and braces out of text that is not a chord, so ChordPro never misreads it.</summary>
internal static class TextSafety
{
    private static readonly Regex Bracketed = new(@"\[([^\[\]]+)\]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>Replaces <c>[ ] { }</c> by parentheses; the length never changes.</summary>
    public static string Replace(string text) =>
        text.IndexOfAny(Special) < 0 ? text : Map(text, text.Length, _ => false);

    /// <summary>Like <see cref="Replace"/>, but a bracketed chord of the notation stays an inline chord.</summary>
    public static string ReplaceKeepingChords(string text, Notation notation)
    {
        if (text.IndexOfAny(Special) < 0)
            return text;

        var kept = new HashSet<int>();
        foreach (Match m in Bracketed.Matches(text))
        {
            if (ChordSymbolParser.TryParse(m.Groups[1].Value, notation, out _))
            {
                kept.Add(m.Index);
                kept.Add(m.Index + m.Length - 1);
            }
        }

        return Map(text, text.Length, kept.Contains);
    }

    private static readonly char[] Special = { '[', ']', '{', '}' };

    private static string Map(string text, int length, Func<int, bool> keep)
    {
        var characters = text.ToCharArray();
        for (var i = 0; i < length; i++)
        {
            if (!keep(i))
                characters[i] = characters[i] switch { '[' or '{' => '(', ']' or '}' => ')', var c => c };
        }

        return new string(characters);
    }
}
