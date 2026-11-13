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

    /// <summary>Number of text elements before the UTF-16 index.</summary>
    public static int ColumnOf(string text, int index) =>
        index <= 0 ? 0 : new StringInfo(text[..Math.Min(index, text.Length)]).LengthInTextElements;
}
