namespace AppCanzoniere.Application.Import;

/// <summary>Keeps square brackets and braces out of text that is not a chord, so ChordPro never misreads it.</summary>
internal static class TextSafety
{
    /// <summary>Replaces <c>[ ] { }</c> by parentheses; the length never changes.</summary>
    public static string Replace(string text) =>
        text.IndexOfAny(Special) < 0 ? text : Map(text, text.Length, _ => false);

    /// <summary>Like <see cref="Replace"/>, but a bracketed chord of either notation or a star annotation stays as written.</summary>
    public static string ReplaceKeepingChords(string text)
    {
        if (text.IndexOfAny(Special) < 0)
            return text;

        var kept = new HashSet<int>();
        foreach (var m in BracketedText.Matches(text))
        {
            if (BracketedText.IsKept(m.Groups[1].Value))
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
