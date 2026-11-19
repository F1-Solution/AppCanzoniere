namespace AppCanzoniere.Application.PdfImport;

/// <summary>A recognised element (word) as the engine reports it, with edges in pixels.</summary>
public sealed record RawTextElement(string? Text, int Left, int Top, int Right, int Bottom);

/// <summary>A recognised line as the engine reports it; the line box is used only when it has no elements.</summary>
public sealed record RawTextLine(string? Text, IReadOnlyList<RawTextElement> Elements, int Left = 0, int Top = 0, int Right = 0, int Bottom = 0);

/// <summary>Maps the lines of the on-device engine of Android to the OCR contract, with no platform types.</summary>
public static class MlKitTextMapper
{
    /// <summary>Builds the page result; blank words and empty boxes are skipped and lines left empty are dropped.</summary>
    public static OcrPageResult Map(int width, int height, IEnumerable<RawTextLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        var mapped = new List<OcrLine>();
        foreach (var line in lines)
        {
            var source = line.Elements.Count > 0
                ? line.Elements
                : new[] { new RawTextElement(line.Text, line.Left, line.Top, line.Right, line.Bottom) };
            var words = source.Select(ToWord).OfType<OcrWord>().ToList();
            if (words.Count > 0)
                mapped.Add(new OcrLine(words));
        }

        return new OcrPageResult(width, height, mapped);
    }

    private static OcrWord? ToWord(RawTextElement element)
    {
        var text = element.Text?.Trim();
        if (string.IsNullOrEmpty(text) || element.Right <= element.Left || element.Bottom <= element.Top)
            return null;
        return new OcrWord(text, element.Left, element.Top, element.Right - element.Left, element.Bottom - element.Top);
    }
}
