using AppCanzoniere.Infrastructure.PdfImport;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;

namespace AppCanzoniere.Windows.UiTests;

/// <summary>Checks the built Windows app: the PDF reader must ship with it and read a PDF from there.</summary>
public class PdfWorkerPackageTests
{
    [WindowsFact]
    public void WindowsOutput_ContainsThePdfReader()
    {
        var launch = PdfWorkerLaunch.LocateForApp(AppSession.AppDirectory);

        Assert.NotNull(launch);
        Assert.True(File.Exists(launch!.FileName), $"missing {launch.FileName}");
        Assert.StartsWith(Path.Combine(AppSession.AppDirectory, PdfWorkerLaunch.AppFolder), launch.FileName, StringComparison.OrdinalIgnoreCase);
    }

    [WindowsFact]
    public async Task ReaderStartedFromTheAppOutput_ReturnsTheTextOfASyntheticPdf()
    {
        var launch = PdfWorkerLaunch.LocateForApp(AppSession.AppDirectory) ?? throw new FileNotFoundException("The PDF reader is missing from the app output.");
        using var extractor = new IsolatedPdfWordExtractor(launch);

        var result = await extractor.ExtractAsync(new MemoryStream(SyntheticPdf()));

        Assert.True(result.IsSuccess, $"{result.Status}: {result.Reason} {result.Detail}");
        var words = result.Pages.SelectMany(page => page.Words).Select(word => word.Text).ToList();
        Assert.Contains("Cammina", words);
        Assert.Contains("strada", words);
    }

    private static byte[] SyntheticPdf()
    {
        var builder = new PdfDocumentBuilder();
        var font = builder.AddStandard14Font(Standard14Font.Helvetica);
        var page = builder.AddPage(PageSize.A4);
        page.AddText("Cammina con me per la strada", 12, new PdfPoint(40, 700), font);
        return builder.Build();
    }
}
