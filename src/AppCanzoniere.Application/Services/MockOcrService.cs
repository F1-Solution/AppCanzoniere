using AppCanzoniere.Application.DTOs;
using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Services;

public class MockOcrService : IOcrService
{
    public string ServiceName => "Mock OCR (Demo)";

    public Task<OcrResult> ExtractTextFromImageAsync(
        string imagePath,
        string language = "ita",
        CancellationToken cancellationToken = default)
    {
        // Return sample ChordPro content
        var sampleText = @"{title: Amazing Grace}
{artist: John Newton}
{key: G}

[G]Amazing [G7]grace how [C]sweet the [G]sound
That saved a wretch like [D]me
[G]I once was [G7]lost but [C]now I'm [G]found
Was [D]blind but now I [G]see

{start_of_chorus}
Was [G]grace that taught my [C]heart to [G]fear
And grace my fears re[D]lieved
How [G]precious did that [C]grace ap[G]pear
The [D]hour I first be[G]lieved
{end_of_chorus}";

        var lines = sampleText.Split('\n')
            .Select((text, index) => new OcrLine
            {
                Text = text,
                Confidence = 0.95f,
                LineNumber = index + 1
            }).ToList();

        var result = new OcrResult
        {
            Success = true,
            Text = sampleText,
            Confidence = 0.95f,
            Language = language,
            Lines = lines
        };

        return Task.FromResult(result);
    }

    public Task<OcrResult> ExtractTextFromImageAsync(
        byte[] imageBytes,
        string language = "ita",
        CancellationToken cancellationToken = default)
    {
        // Same as file-based version
        return ExtractTextFromImageAsync(string.Empty, language, cancellationToken);
    }

    public Task<OcrResult> ExtractTextFromPdfAsync(
        string pdfPath,
        string language = "ita",
        CancellationToken cancellationToken = default)
    {
        // Return multiple pages of sample content
        var sampleText = @"{title: Sample Song from PDF}
{artist: Demo Artist}
{key: C}

[C]This is a sample song extracted from PDF
[G]With multiple chords and [Am]lines of [F]text
[C]The OCR service would extract [G]each page
And com[Am]bine them into [F]one [C]result";

        var lines = sampleText.Split('\n')
            .Select((text, index) => new OcrLine
            {
                Text = text,
                Confidence = 0.92f,
                LineNumber = index + 1
            }).ToList();

        var result = new OcrResult
        {
            Success = true,
            Text = sampleText,
            Confidence = 0.92f,
            Language = language,
            Lines = lines
        };

        return Task.FromResult(result);
    }

    public Task<bool> IsAvailableAsync()
    {
        return Task.FromResult(true);
    }
}
