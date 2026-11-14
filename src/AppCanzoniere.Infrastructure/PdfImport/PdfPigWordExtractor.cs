using System.Diagnostics;
using AppCanzoniere.Application.PdfImport;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Exceptions;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Reads positioned words from a PDF with PdfPig, entirely on the device.</summary>
public sealed class PdfPigWordExtractor : IPdfWordExtractor
{
    private const int CopyBufferBytes = 81920;

    // The PDF format limits a page side to 14,400 default user units (ISO 32000-1, annex C).
    private const double MaxPageSidePoints = 14400;

    private static readonly ParsingOptions Options = new() { UseLenientParsing = true, SkipMissingFonts = true };

    private readonly PdfExtractionLimits _limits;

    public PdfPigWordExtractor(PdfExtractionLimits? limits = null)
    {
        _limits = limits ?? new PdfExtractionLimits();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_limits.MaxBytes, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_limits.MaxPages, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfLessThan(_limits.MaxDuration, TimeSpan.Zero, nameof(limits));
    }

    public async Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdf);
        cancellationToken.ThrowIfCancellationRequested();
        var bytes = await ReadBoundedAsync(pdf, cancellationToken).ConfigureAwait(false);
        if (bytes is null)
            return PdfWordsResult.Failure(PdfExtractionStatus.TooLarge, $"The file is larger than the limit of {_limits.MaxBytes} bytes.");
        if (bytes.Length == 0)
            return PdfWordsResult.Failure(PdfExtractionStatus.Empty, "The file is empty.");

        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var work = Task.Run(() => Read(bytes, stop.Token), stop.Token);
        try
        {
            return await work.WaitAsync(_limits.MaxDuration, cancellationToken).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            // The parser cannot be aborted mid-page; cancelling makes it stop at the next page boundary.
            await stop.CancelAsync().ConfigureAwait(false);
            return TimedOut();
        }
    }

    private PdfWordsResult TimedOut() =>
        PdfWordsResult.Failure(PdfExtractionStatus.TimedOut, PdfTextImporter.TimedOutReason(_limits.MaxDuration));

    // Copies at most MaxBytes; null means the input is larger than that.
    private async Task<byte[]?> ReadBoundedAsync(Stream pdf, CancellationToken cancellationToken)
    {
        if (pdf.CanSeek && pdf.Length - pdf.Position > _limits.MaxBytes)
            return null;

        using var copy = new MemoryStream();
        var buffer = new byte[CopyBufferBytes];
        int read;
        while ((read = await pdf.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
        {
            if (copy.Length + read > _limits.MaxBytes)
                return null;
            copy.Write(buffer, 0, read);
        }

        return copy.ToArray();
    }

    private PdfWordsResult Read(byte[] bytes, CancellationToken cancellationToken)
    {
        var clock = Stopwatch.StartNew();
        try
        {
            using var document = PdfDocument.Open(bytes, Options);
            var count = document.NumberOfPages;
            if (count == 0)
                return PdfWordsResult.Failure(PdfExtractionStatus.Empty, "The document has no pages.");
            if (count > _limits.MaxPages)
                return PdfWordsResult.Failure(PdfExtractionStatus.TooLarge, $"The document has {count} pages, more than the limit of {_limits.MaxPages}.");

            var pages = new List<PdfPageWords>(count);
            for (var number = 1; number <= count; number++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (clock.Elapsed >= _limits.MaxDuration)
                    return TimedOut();

                var page = document.GetPage(number);
                if (page.Width > MaxPageSidePoints || page.Height > MaxPageSidePoints)
                    return PdfWordsResult.Failure(PdfExtractionStatus.Damaged, $"Page {number} is larger than the PDF format allows.");
                pages.Add(PdfPigPage.Words(page));
            }

            return Classify(pages);
        }
        catch (PdfDocumentEncryptedException)
        {
            return PdfWordsResult.Failure(PdfExtractionStatus.Encrypted, "The document is protected by a password.");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            // Untrusted input: any parser failure is reported, never thrown.
            return PdfWordsResult.Failure(PdfExtractionStatus.Damaged, $"The file is not a readable PDF ({exception.Message}).");
        }
    }

    private static PdfWordsResult Classify(IReadOnlyList<PdfPageWords> pages)
    {
        if (pages.Any(p => p.HasText))
            return PdfWordsResult.Success(pages);

        var scanned = pages.Count(p => p.HasImages);
        var reason = $"No page has a text layer; {scanned} of {pages.Count} pages carry images, as a scan does.";
        return PdfWordsResult.Failure(PdfExtractionStatus.NoTextLayer, reason, pages);
    }
}
