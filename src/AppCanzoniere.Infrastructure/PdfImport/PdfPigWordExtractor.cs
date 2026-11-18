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

    private readonly PdfExtractionLimits _limits;

    public PdfPigWordExtractor(PdfExtractionLimits? limits = null)
    {
        _limits = limits ?? new PdfExtractionLimits();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_limits.MaxBytes, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_limits.MaxPages, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_limits.MaxLettersPerPage, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_limits.MaxContentBytesPerPage, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_limits.MaxDecodedStreamBytes, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_limits.MaxDecodedBytesPerDocument, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_limits.MaxContentBytesPerDocument, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_limits.MaxLettersPerDocument, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfLessThan(_limits.MaxDuration, TimeSpan.Zero, nameof(limits));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(_limits.MaxDuration, PdfTextImporter.MaxSupportedDuration, nameof(limits));
    }

    public async Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdf);
        cancellationToken.ThrowIfCancellationRequested();
        var bytes = await ReadBoundedAsync(pdf, cancellationToken).ConfigureAwait(false);
        if (bytes is not { } content)
            return PdfWordsResult.Failure(PdfExtractionStatus.TooLarge, $"The file is larger than the limit of {_limits.MaxBytes} bytes.");
        if (content.Length == 0)
            return PdfWordsResult.Failure(PdfExtractionStatus.Empty, "The file is empty.");

        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var work = Task.Run(() => Read(content, stop.Token), stop.Token);
        try
        {
            return await work.WaitAsync(_limits.MaxDuration, cancellationToken).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            // PdfPig cannot be interrupted inside one page, so the background work stops at its next check, not at once.
            await stop.CancelAsync().ConfigureAwait(false);
            return TimedOut();
        }
    }

    private PdfWordsResult TimedOut() =>
        PdfWordsResult.Failure(PdfExtractionStatus.TimedOut, PdfTextImporter.TimedOutReason(_limits.MaxDuration));

    private PdfWordsResult TooLarge(string reason) => PdfWordsResult.Failure(PdfExtractionStatus.TooLarge, reason);

    private PdfWordsResult DecodeTooLarge(DecodeRefusal refusal) => refusal == DecodeRefusal.Document
        ? TooLarge($"The file unpacks to more than {_limits.MaxDecodedBytesPerDocument} bytes in total, the limit.")
        : TooLarge($"A part of the file unpacks to more than {_limits.MaxDecodedStreamBytes} bytes, the limit.");

    // Throws when the caller cancels; a result when the time limit is used up.
    private PdfWordsResult? Checkpoint(Stopwatch clock, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return clock.Elapsed >= _limits.MaxDuration ? TimedOut() : null;
    }

    // Reads at most MaxBytes into a single buffer; null means the input is larger than that.
    private async Task<ReadOnlyMemory<byte>?> ReadBoundedAsync(Stream pdf, CancellationToken cancellationToken)
    {
        if (pdf.CanSeek)
        {
            var remaining = pdf.Length - pdf.Position;
            if (remaining > _limits.MaxBytes || remaining > Array.MaxLength)
                return null;

            var exact = new byte[remaining];
            var count = await pdf.ReadAtLeastAsync(exact, exact.Length, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(false);
            return exact.AsMemory(0, count);
        }

        using var copy = new MemoryStream();
        var buffer = new byte[CopyBufferBytes];
        int read;
        while ((read = await pdf.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
        {
            if (copy.Length + read > _limits.MaxBytes)
                return null;
            copy.Write(buffer, 0, read);
        }

        return copy.GetBuffer().AsMemory(0, (int)copy.Length);
    }

    private PdfWordsResult Read(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken)
    {
        // A provider per file: its refusals are tracked for this file only, and PdfPig may catch the exception it throws.
        var filters = new BoundedFilterProvider(_limits.MaxDecodedStreamBytes, _limits.MaxDecodedBytesPerDocument);
        try
        {
            using var document = PdfDocument.Open(bytes, new ParsingOptions { UseLenientParsing = true, SkipMissingFonts = true, FilterProvider = filters });
            var result = ReadPages(document, filters, cancellationToken);
            return filters.Refusal != DecodeRefusal.None ? DecodeTooLarge(filters.Refusal) : result;
        }
        catch (Exception exception) when (exception is not OperationCanceledException && (filters.Refusal != DecodeRefusal.None || exception is DecodeLimitExceededException))
        {
            return DecodeTooLarge(filters.Refusal);
        }
        catch (PdfDocumentEncryptedException)
        {
            return PdfWordsResult.Failure(PdfExtractionStatus.Encrypted, "The document is protected by a password.");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (OutOfMemoryException exception)
        {
            return TooLarge("The file needs more memory than the device can give.") with { Detail = exception.Message };
        }
        catch (Exception exception)
        {
            // Untrusted input: any parser failure is reported, never thrown; the parser's words are for logs only.
            return PdfWordsResult.Failure(PdfExtractionStatus.Damaged, "The file is not a readable PDF.") with { Detail = $"{exception.GetType().Name}: {exception.Message}" };
        }
    }

    private PdfWordsResult ReadPages(PdfDocument document, BoundedFilterProvider filters, CancellationToken cancellationToken)
    {
        var clock = Stopwatch.StartNew();
        var count = document.NumberOfPages;
        if (count == 0)
            return PdfWordsResult.Failure(PdfExtractionStatus.Empty, "The document has no pages.");
        if (count > _limits.MaxPages)
            return TooLarge($"The document has {count} pages, more than the limit of {_limits.MaxPages}.");

        if (ContentBudget.Check(document, _limits, count, () => Checkpoint(clock, cancellationToken)) is { } refusal)
            return refusal;

        var pages = new List<PdfPageWords>(count);
        long letters = 0;
        for (var number = 1; number <= count; number++)
        {
            if (Checkpoint(clock, cancellationToken) is { } stop)
                return stop;

            var page = document.GetPage(number);
            if (filters.Refusal != DecodeRefusal.None)
                return DecodeTooLarge(filters.Refusal);
            if (Refuse(page) is { } tooBig)
                return tooBig;
            letters += page.Letters.Count;
            if (letters > _limits.MaxLettersPerDocument)
                return TooLarge($"The document has more than {_limits.MaxLettersPerDocument} characters, the limit.");

            cancellationToken.ThrowIfCancellationRequested();
            pages.Add(PdfPigPage.Words(page));
        }

        return Classify(pages) with { Title = filters.WithoutRefusal(() => PdfMetadataTitle.Read(document)) };
    }

    // A parsed page too big to lay out: a page box larger than PDF allows, or more characters than the limit.
    private PdfWordsResult? Refuse(Page page)
    {
        if (page.Width > MaxPageSidePoints || page.Height > MaxPageSidePoints)
            return PdfWordsResult.Failure(PdfExtractionStatus.Damaged, $"Page {page.Number} is larger than the PDF format allows.");

        return page.Letters.Count > _limits.MaxLettersPerPage
            ? TooLarge($"Page {page.Number} has more than {_limits.MaxLettersPerPage} characters, the limit.")
            : null;
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
