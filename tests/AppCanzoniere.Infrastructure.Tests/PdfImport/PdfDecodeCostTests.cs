using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;
using static AppCanzoniere.Infrastructure.Tests.PdfImport.CraftedPdf;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

/// <summary>Runs alone, so the bytes the process allocates while one file is read belong to that file.</summary>
[CollectionDefinition(nameof(AllocationCollection), DisableParallelization = true)]
public sealed class AllocationCollection;

/// <summary>
/// Streams the page measurement never sees (font programs here) are unpacked only by the capped decoders: where those
/// cannot read a stream, PdfPig's own filter must not run either, because it allocates by what the file claims. The
/// result is the same either way, so these tests measure what reading the file costs.
/// </summary>
[Collection(nameof(AllocationCollection))]
public class PdfDecodeCostTests
{
    // Reading a one-page crafted file allocates a few megabytes; the costs guarded here are hundreds.
    private const long ReadingAllocationBytes = 64L * 1024 * 1024;

    private static async Task<(PdfWordsResult Result, long Allocated)> ExtractMeasured(byte[] pdf, PdfExtractionLimits? limits = null)
    {
        var extractor = new PdfPigWordExtractor(limits ?? new PdfExtractionLimits { MaxDuration = TimeSpan.FromSeconds(30) });
        await extractor.ExtractAsync(new MemoryStream(OnePage(string.Empty, Ascii(Text))));
        var before = GC.GetTotalAllocatedBytes(precise: true);
        var result = await extractor.ExtractAsync(new MemoryStream(pdf));
        return (result, GC.GetTotalAllocatedBytes(precise: true) - before);
    }

    [Fact]
    public async Task ExtractAsync_DoesNotAllocatePredictorRows_ForAFontProgramWhoseFlateDataIsCorrupt()
    {
        // Rows of 268 MB: PdfPig's filter allocates two of them before it finds the data corrupt and returns it packed.
        var corrupt = Zlib(new byte[4096]);
        corrupt[corrupt.Length / 2] ^= 0xFF;
        corrupt[(corrupt.Length / 2) + 1] ^= 0xFF;
        var pdf = WithFontProgram("/Filter /FlateDecode /DecodeParms << /Predictor 12 /Colors 32 /BitsPerComponent 8 /Columns 8388607 >>", corrupt);

        var (result, allocated) = await ExtractMeasured(pdf);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Contains(result.Pages[0].Words, w => w.Text == "gioia");
        Assert.InRange(allocated, 0, ReadingAllocationBytes);
    }

    [Fact]
    public async Task ExtractAsync_DoesNotGrowAnLzwTable_ForAFontProgramWithoutAnEndCode()
    {
        // Three million literal codes and no end code: PdfPig's filter adds a table entry per code before it fails.
        var pdf = WithFontProgram("/Filter /LZWDecode", LzwLiterals(0, 3_000_000, endCode: false));

        var (result, allocated) = await ExtractMeasured(pdf);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Contains(result.Pages[0].Words, w => w.Text == "gioia");
        Assert.InRange(allocated, 0, ReadingAllocationBytes);
    }

    [Fact]
    public async Task ExtractAsync_DoesNotGrowAnLzwTable_ForAFontProgramWithAPredictor()
    {
        // Readable LZW of 3 MB with a predictor: running PdfPig's filter to apply the predictor would grow its table.
        var pdf = WithFontProgram("/Filter /LZWDecode /DecodeParms << /Predictor 2 /Columns 1 >>", LzwLiterals(0, 3_000_000, endCode: true));

        var (result, allocated) = await ExtractMeasured(pdf);

        Assert.Equal(PdfExtractionStatus.Success, result.Status);
        Assert.Contains(result.Pages[0].Words, w => w.Text == "gioia");
        Assert.InRange(allocated, 0, ReadingAllocationBytes);
    }
}
