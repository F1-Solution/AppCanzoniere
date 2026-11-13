using System.Text;
using AppCanzoniere.Application.Import;
using AppCanzoniere.CorpusReport;

const string CorpusFolderName = "corpus-locale";

var corpus = args.Length > 0 ? args[0] : FindCorpus(Directory.GetCurrentDirectory());
if (corpus is null || !Directory.Exists(Path.Combine(corpus, "testo")))
{
    Console.Error.WriteLine($"Cartella '{CorpusFolderName}/testo' non trovata. Uso: dotnet run --project tools/AppCanzoniere.CorpusReport -- <cartella {CorpusFolderName}>");
    return 1;
}

var reportFolder = Path.Combine(corpus, "report");
Directory.CreateDirectory(reportFolder);
var utf8 = new UTF8Encoding(false);
var songs = new List<SongSummary>();
foreach (var file in Directory.EnumerateFiles(Path.Combine(corpus, "testo"), "*.txt").Order(StringComparer.Ordinal))
{
    var name = Path.GetFileNameWithoutExtension(file);
    var result = ChordSheetConverter.Convert(File.ReadAllText(file));
    var summary = new SongSummary(name, name + ".html", result);
    File.WriteAllText(Path.Combine(reportFolder, summary.PageName), SongPage.Render(name, result), utf8);
    songs.Add(summary);
}

var index = Path.Combine(reportFolder, "index.html");
File.WriteAllText(index, IndexPage.Render(songs), utf8);
Console.WriteLine($"{songs.Count} brani convertiti. Rapporto: {index}");
return 0;

static string? FindCorpus(string start)
{
    for (var dir = new DirectoryInfo(start); dir is not null; dir = dir.Parent)
    {
        var candidate = Path.Combine(dir.FullName, CorpusFolderName);
        if (Directory.Exists(candidate))
            return candidate;
    }

    return null;
}
