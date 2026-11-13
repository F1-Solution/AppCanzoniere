using System.Globalization;

namespace AppCanzoniere.Application.Import;

/// <summary>Columns are counted in text elements: a surrogate pair or a letter with its accents is one column.</summary>
internal static class TextElements
{
    public static string[] Split(string text)
    {
        var elements = new List<string>(text.Length);
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
            elements.Add((string)enumerator.Current);
        return elements.ToArray();
    }

    /// <summary>For every UTF-16 index (and the end), the number of text elements before it.</summary>
    public static int[] ColumnMap(string text)
    {
        var map = new int[text.Length + 1];
        var starts = StringInfo.ParseCombiningCharacters(text);
        for (var element = 0; element < starts.Length; element++)
        {
            var end = element + 1 < starts.Length ? starts[element + 1] : text.Length;
            for (var i = starts[element]; i < end; i++)
                map[i] = element;
            map[end] = element + 1;
        }

        return map;
    }
}
