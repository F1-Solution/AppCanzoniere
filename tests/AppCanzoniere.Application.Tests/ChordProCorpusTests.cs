using System.Text;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Opt-in: round-trips the real files of the local corpus when the folder exists; passes silently otherwise (CI has no corpus).</summary>
public class ChordProCorpusTests
{
    [Fact]
    public void LocalCorpus_RoundTripsEveryFileExactly()
    {
        var folder = FindCorpusFolder();
        if (folder is null)
            return;

        var files = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).OrderBy(f => f, StringComparer.Ordinal).ToList();
        var parser = new ChordProParser();
        var failures = new List<string>();
        foreach (var file in files)
        {
            var text = File.ReadAllText(file, Encoding.UTF8);
            var expected = LosslessAssert.Expected(text);
            var actual = parser.Render(parser.Parse(text));
            if (actual != expected)
                failures.Add($"{Path.GetFileName(file)}: {FirstDifference(expected, actual)}");
        }

        Assert.True(failures.Count == 0, "Files that do not round-trip:\n" + string.Join("\n", failures));
    }

    private static string? FindCorpusFolder()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "corpus-locale", "chordpro");
            if (Directory.Exists(candidate))
                return Directory.EnumerateFiles(candidate, "*", SearchOption.AllDirectories).Any() ? candidate : null;
        }

        return null;
    }

    private static string FirstDifference(string expected, string actual)
    {
        var left = expected.Split('\n');
        var right = actual.Split('\n');
        for (var i = 0; i < Math.Max(left.Length, right.Length); i++)
        {
            var a = i < left.Length ? left[i] : "<missing>";
            var b = i < right.Length ? right[i] : "<missing>";
            if (a != b)
                return $"line {i + 1}: expected '{Show(a)}' but got '{Show(b)}'";
        }

        return "no differing line found";
    }

    private static string Show(string line) => line.Length > 80 ? line[..80] + "..." : line;
}
