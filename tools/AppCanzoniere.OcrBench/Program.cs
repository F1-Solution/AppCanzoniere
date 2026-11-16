using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.OcrBench;

// Usage: dotnet run --project tools/AppCanzoniere.OcrBench -c Release -- <corpus folder> [name filter, default "scan"]
// Reads <corpus>\pdf\*scan*.pdf and writes <corpus>\report-ocr\index.md; the report stays out of the repository.
if (args.Length is < 1 or > 2 || !Directory.Exists(Path.Combine(args[0], "pdf")))
{
    Console.Error.WriteLine("Usage: AppCanzoniere.OcrBench <corpus folder containing pdf\\> [name filter]");
    return 1;
}

var engine = WindowsOcrEngine.Create();
if (engine is null)
{
    Console.Error.WriteLine("No Windows OCR language is installed. Installed: " + WindowsOcrEngine.InstalledLanguages());
    return 2;
}

var corpus = Path.GetFullPath(args[0]);
var filter = args.Length == 2 ? args[1] : "scan";
var report = new StringBuilder();
report.Append(CultureInfo.InvariantCulture, $"# OCR report\n\nGenerated {DateTimeOffset.UtcNow:u}. OCR language: **{engine.Language}** (installed: {WindowsOcrEngine.InstalledLanguages()}).\n\n");
foreach (var pdf in Directory.EnumerateFiles(Path.Combine(corpus, "pdf"), "*.pdf").Where(p => Path.GetFileName(p).Contains(filter, StringComparison.OrdinalIgnoreCase)).Order())
{
    report.Append(await RunAsync(engine, pdf));
}

var folder = Path.Combine(corpus, "report-ocr");
Directory.CreateDirectory(folder);
var path = Path.Combine(folder, "index.md");
File.WriteAllText(path, report.ToString(), Encoding.UTF8);
Console.WriteLine($"Report written to {path}");
return 0;

static async Task<string> RunAsync(WindowsOcrEngine engine, string pdf)
{
    var md = new StringBuilder();
    md.Append(CultureInfo.InvariantCulture, $"## {Path.GetFileName(pdf)}\n\n");
    foreach (var scanned in ScanImages.Extract(File.ReadAllBytes(pdf)))
    {
        md.Append(CultureInfo.InvariantCulture, $"### Page {scanned.PageNumber}: {scanned.Note}\n\n");
        if (scanned.Image is null)
            continue;

        var clock = Stopwatch.StartNew();
        var recognised = await engine.RecognizeAsync(scanned.Image);
        clock.Stop();
        if (engine.LastFailure is { } failure)
        {
            md.Append(CultureInfo.InvariantCulture, $"- Page failed: {failure}\n\n");
            continue;
        }

        var adapted = OcrWordAdapter.ToPageWords(recognised, scanned.PageNumber);
        var text = PdfPageLayout.Lay(adapted.Page).Text;
        var converted = ChordSheetConverter.Convert(text);
        var italian = ChordSheetConverter.Convert(text, new ChordSheetOptions { Notation = AppCanzoniere.Application.Music.Notation.Italian });
        md.Append(CultureInfo.InvariantCulture, $"- With Italian notation forced: chord lines {italian.ChordLineCount}, merged {italian.MergedChordLineCount}\n");
        md.Append("- Line kinds: ").Append(string.Join(", ", converted.Lines.GroupBy(l => l.Kind).Select(g => $"{g.Key} {g.Count()}"))).Append('\n');
        var words = recognised.Lines.Sum(l => l.Words.Count);
        md.Append(CultureInfo.InvariantCulture, $"- Language: {recognised.Language}; image read as {recognised.Width}x{recognised.Height}; time {clock.ElapsedMilliseconds} ms\n");
        md.Append(CultureInfo.InvariantCulture, $"- OCR lines {recognised.Lines.Count}, words {words}, rejected {adapted.RejectedWords}\n");
        md.Append(CultureInfo.InvariantCulture, $"- Chord lines found {converted.ChordLineCount}, merged into lyrics {converted.MergedChordLineCount}, doubtful lines {converted.DoubtfulLines.Count()}\n");
        md.Append(CultureInfo.InvariantCulture, $"- Quality note (automatic): {Suspects(text)}\n\n");
        md.Append("Aligned text:\n\n```text\n").Append(text).Append("\n```\n\n");
        md.Append("ChordPro:\n\n```text\n").Append(converted.ChordPro).Append("\n```\n\n");
    }

    return md.ToString();
}

// Counts tokens that mix letters with digits or symbols, the typical misreads such as "So1" or "D0".
static string Suspects(string text)
{
    var tokens = Regex.Matches(text, @"\S+").Select(m => m.Value).ToList();
    var mixed = tokens.Where(t => Regex.IsMatch(t, @"\p{L}") && Regex.IsMatch(t, @"[0-9|\\/@#$%^*_=~]")).Distinct().Take(15).ToList();
    return $"{tokens.Count} tokens, {mixed.Count} with digits or odd symbols inside words: {string.Join(' ', mixed)}";
}
