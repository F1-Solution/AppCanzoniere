using System.Globalization;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;
using Xunit.Abstractions;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

// Regression checks on the local, git-ignored corpus of real booklets; they pass with a message when it is missing.
public class PdfCorpusTests
{
    private const string CorpusFolderName = "corpus-locale";

    // Booklets the corpus description lists as two-column, and booklets that are single-column throughout.
    private static readonly string[] TwoColumnBooklets = { "03-", "13-", "15-", "18-" };
    private static readonly string[] SingleColumnBooklets = { "14-", "16-" };

    // Share of text pages that must be read as two columns, or may be, for the two groups.
    private const double MinTwoColumnShare = 0.9;
    private const double MaxFalseTwoColumnShare = 0.1;

    private readonly ITestOutputHelper _output;

    public PdfCorpusTests(ITestOutputHelper output) => _output = output;

    private static string? PdfFolder()
    {
        var candidates = new List<string>();
        var fromEnvironment = Environment.GetEnvironmentVariable("APPCANZONIERE_CORPUS");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            candidates.Add(fromEnvironment);

        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            candidates.Add(Path.Combine(dir.FullName, CorpusFolderName));

        return candidates.Select(c => Path.Combine(c, "pdf")).FirstOrDefault(Directory.Exists);
    }

    private static async Task<PdfTextResult> Import(string path)
    {
        await using var stream = File.OpenRead(path);
        return await new PdfTextImporter(new PdfPigWordExtractor()).ImportAsync(stream);
    }

    private static IEnumerable<string> Pdfs(string folder, IEnumerable<string> prefixes) =>
        Directory.EnumerateFiles(folder, "*.pdf").Where(p => prefixes.Any(Path.GetFileName(p).StartsWith));

    private static double TwoColumnShare(PdfTextResult result)
    {
        var text = result.Pages.Where(p => p.ColumnCount > 0).ToList();
        return text.Count == 0 ? 0 : (double)text.Count(p => p.ColumnCount == 2) / text.Count;
    }

    [Fact]
    public async Task EveryCorpusPdf_ComesBackTyped_AndScansAreReportedAsScans()
    {
        var folder = PdfFolder();
        if (folder is null)
        {
            _output.WriteLine("corpus not found");
            return;
        }

        foreach (var pdf in Directory.EnumerateFiles(folder, "*.pdf"))
        {
            var result = await Import(pdf);
            var name = Path.GetFileName(pdf);
            var expected = name.Contains("-scan-", StringComparison.Ordinal) ? PdfExtractionStatus.NoTextLayer : PdfExtractionStatus.Success;
            Assert.True(expected == result.Status, $"{name}: {result.Status} {result.Reason}");
            Assert.NotEmpty(result.Pages);
        }
    }

    [Fact]
    public async Task TwoColumnBooklets_AreReadColumnByColumn_OnNearlyEveryTextPage()
    {
        var folder = PdfFolder();
        if (folder is null)
        {
            _output.WriteLine("corpus not found");
            return;
        }

        foreach (var pdf in Pdfs(folder, TwoColumnBooklets))
        {
            var share = TwoColumnShare(await Import(pdf));
            _output.WriteLine(string.Create(CultureInfo.InvariantCulture, $"{Path.GetFileName(pdf)}: {share:P1} two-column pages"));
            Assert.True(share >= MinTwoColumnShare, $"{Path.GetFileName(pdf)}: only {share:P1} of text pages read as two columns");
        }
    }

    [Fact]
    public async Task SingleColumnBooklets_AreRarelySplit()
    {
        var folder = PdfFolder();
        if (folder is null)
        {
            _output.WriteLine("corpus not found");
            return;
        }

        foreach (var pdf in Pdfs(folder, SingleColumnBooklets))
        {
            var share = TwoColumnShare(await Import(pdf));
            _output.WriteLine(string.Create(CultureInfo.InvariantCulture, $"{Path.GetFileName(pdf)}: {share:P1} two-column pages"));
            Assert.True(share <= MaxFalseTwoColumnShare, $"{Path.GetFileName(pdf)}: {share:P1} of text pages split into columns");
        }
    }
}
