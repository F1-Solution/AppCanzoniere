namespace AppCanzoniere.Application.DTOs;

public class OcrResult
{
    public string Text { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public string Language { get; set; } = "ita";
    public List<OcrLine> Lines { get; set; } = new();
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class OcrLine
{
    public string Text { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public int LineNumber { get; set; }
}
