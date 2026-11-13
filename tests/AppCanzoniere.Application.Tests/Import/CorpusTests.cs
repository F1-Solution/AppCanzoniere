using AppCanzoniere.Application.Import;
using Xunit.Abstractions;

namespace AppCanzoniere.Application.Tests.Import;

// Spec 8: recall of hand-labelled chord lines on the local, git-ignored corpus; passes silently when it is missing.
public class CorpusTests
{
    private const double MinimumRecall = 0.90;
    private const string CorpusFolderName = "corpus-locale";
    private readonly ITestOutputHelper _output;

    public CorpusTests(ITestOutputHelper output) => _output = output;

    private static string? FindTextFolder()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("APPCANZONIERE_CORPUS");
        var candidates = new List<string>();
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            candidates.Add(fromEnvironment);

        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            candidates.Add(Path.Combine(dir.FullName, CorpusFolderName));

        // Linked worktrees live under the main checkout, so the walk finds its corpus too.
        return candidates.Select(c => Path.Combine(c, "testo")).FirstOrDefault(Directory.Exists);
    }

    private static HashSet<int> ReadLabels(string path) =>
        File.ReadAllText(path)
            .Split(new[] { ',', ' ', '\r', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(token => int.TryParse(token, out var n) ? n : 0)
            .Where(n => n > 0)
            .ToHashSet();

    // A label line such as "Intro: Do Sol" counts as a chord line when its text carries chords.
    private static HashSet<int> ChordLineNumbers(ChordSheetResult result)
    {
        var output = result.ChordPro.Split('\n');
        return result.Lines
            .Where(l => l.Kind == SheetLineKind.ChordLine ||
                        (l.Kind == SheetLineKind.SectionLabel && l.OutputLine < output.Length && output[l.OutputLine].Contains('[')))
            .Select(l => l.LineNumber)
            .ToHashSet();
    }

    [Fact]
    public void LabelledChordLines_AreFoundWithRecallOfAtLeastNinetyPercent()
    {
        var folder = FindTextFolder();
        if (folder is null)
            return;

        int labelled = 0, found = 0, classified = 0;
        foreach (var textFile in Directory.EnumerateFiles(folder, "*.txt"))
        {
            var labelFile = Path.ChangeExtension(textFile, ".chordlines");
            if (!File.Exists(labelFile))
                continue;

            var truth = ReadLabels(labelFile);
            var predicted = ChordLineNumbers(ChordSheetConverter.Convert(File.ReadAllText(textFile)));
            labelled += truth.Count;
            classified += predicted.Count;
            found += truth.Count(predicted.Contains);
        }

        if (labelled == 0)
            return;

        var recall = (double)found / labelled;
        _output.WriteLine($"Recall {recall:P1} ({found}/{labelled}); precision {(classified == 0 ? 0 : (double)found / classified):P1}");
        Assert.True(recall >= MinimumRecall, $"Recall {recall:P1} is below {MinimumRecall:P0}.");
    }

    [Fact]
    public void EveryCorpusFile_ConvertsWithOneDiagnosticPerLine_AndIsStableWhenConvertedAgain()
    {
        var folder = FindTextFolder();
        if (folder is null)
            return;

        foreach (var textFile in Directory.EnumerateFiles(folder, "*.txt"))
        {
            var result = ChordSheetConverter.Convert(File.ReadAllText(textFile));
            var name = Path.GetFileName(textFile);

            Assert.True(result.Lines.Count > 0 || new FileInfo(textFile).Length == 0, name);
            Assert.True(result.Lines.All(l => l.OutputLine >= 0), name);
            Assert.True(ChordSheetConverter.Convert(result.ChordPro).ChordPro == result.ChordPro, $"{name} is not stable");
        }
    }
}
