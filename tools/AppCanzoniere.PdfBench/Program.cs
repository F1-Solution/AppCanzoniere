using System.Globalization;
using System.Text;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Infrastructure.PdfImport;
using AppCanzoniere.PdfBench;

// Usage: dotnet run --project tools/AppCanzoniere.PdfBench -c Release -- <corpus folder>
// Reads <corpus>\pdf\*.pdf and the labels in <corpus>\testo, writes <corpus>\report-pdf\index.md and one text file per PDF.
if (args.Length != 1 || !Directory.Exists(Path.Combine(args[0], "pdf")))
{
    Console.Error.WriteLine("Usage: AppCanzoniere.PdfBench <corpus folder containing pdf\\ and testo\\>");
    return 1;
}

var corpus = new CorpusFolder(args[0]);
var bench = new PdfBenchmark(new PdfTextImporter(new PdfPigWordExtractor()));
var results = new List<PdfBenchResult>();
foreach (var pdf in corpus.Pdfs())
{
    var result = await bench.RunAsync(pdf, corpus.LabelsFor(pdf), CancellationToken.None);
    results.Add(result);
    Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"{result.File}: {result.Text.Status}, {result.Text.Pages.Count} pages, {result.Elapsed.TotalMilliseconds:0} ms"));
}

var textFolder = Path.Combine(corpus.ReportFolder, "text");
Directory.CreateDirectory(textFolder);
foreach (var result in results)
    File.WriteAllText(Path.Combine(textFolder, Path.ChangeExtension(result.File, ".txt")), PageDump(result.Text), Encoding.UTF8);

var report = Path.Combine(corpus.ReportFolder, "index.md");
File.WriteAllText(report, MarkdownReport.Write(results, DateTimeOffset.UtcNow), Encoding.UTF8);
Console.WriteLine($"Report written to {report}");
return 0;

static string PageDump(PdfTextResult text)
{
    var dump = new StringBuilder();
    foreach (var page in text.Pages)
    {
        dump.Append(CultureInfo.InvariantCulture, $"===== page {page.PageNumber}, columns {page.ColumnCount}{(page.IsScanned ? ", scanned" : string.Empty)}\n");
        dump.Append(page.Text).Append('\n');
    }

    return dump.ToString();
}
