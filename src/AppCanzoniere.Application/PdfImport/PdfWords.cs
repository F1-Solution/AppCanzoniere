namespace AppCanzoniere.Application.PdfImport;

/// <summary>Axis-aligned box in PDF points, with the y axis pointing up as in PDF user space.</summary>
public sealed record WordBox(double Left, double Bottom, double Right, double Top)
{
    public double Width => Right - Left;

    public double Height => Top - Bottom;
}

/// <summary>A word as the PDF places it: its text and its bounding box.</summary>
public sealed record PositionedWord(string Text, WordBox Box);

/// <summary>The positioned words of one page; HasImages tells a scanned page from a blank one.</summary>
public sealed record PdfPageWords(int PageNumber, double Width, double Height, IReadOnlyList<PositionedWord> Words, bool HasImages)
{
    public bool HasText => Words.Count > 0;
}
