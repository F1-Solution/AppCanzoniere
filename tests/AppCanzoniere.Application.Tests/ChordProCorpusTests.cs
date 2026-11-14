using System.Text;
using AppCanzoniere.Application.Services;
using Xunit.Abstractions;

namespace AppCanzoniere.Application.Tests;

/// <summary>Opt-in: round-trips the real files of the local corpus when the folder exists; passes with a visible note otherwise (CI has no corpus).</summary>
public class ChordProCorpusTests
{
    private readonly ITestOutputHelper _output;

    public ChordProCorpusTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void LocalCorpus_RoundTripsEveryFileExactly()
    {
        var folder = FindCorpusFolder();
        if (folder is null)
        {
            _output.WriteLine("Local corpus not found (corpus-locale/chordpro is missing or empty): nothing was round-tripped.");
            return;
        }

        var failures = RoundTripFailures(folder);

        Assert.True(failures.Count == 0, "Files that do not round-trip:\n" + string.Join("\n", failures));
    }

    [Fact]
    public void ReadExactly_KeepsTheByteOrderMark()
    {
        WithTempFolder(folder =>
        {
            var path = Path.Combine(folder, "bom.cho");
            File.WriteAllBytes(path, WithBom("{title: T}\n[C]x"));

            Assert.Equal("﻿{title: T}\n[C]x", ReadExactly(path));
        });
    }

    [Fact]
    public void RoundTripFailures_ComparesBomFilesExactly()
    {
        WithTempFolder(folder =>
        {
            File.WriteAllBytes(Path.Combine(folder, "bom.cho"), WithBom("{title: T}\r\n[C]x\r\n"));
            File.WriteAllText(Path.Combine(folder, "plain.cho"), "[G]y");

            Assert.Empty(RoundTripFailures(folder));
        });
    }

    private static byte[] WithBom(string text) => new byte[] { 0xEF, 0xBB, 0xBF }.Concat(Encoding.UTF8.GetBytes(text)).ToArray();

    private static void WithTempFolder(Action<string> body)
    {
        var folder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "corpus-" + Guid.NewGuid().ToString("N"))).FullName;
        try
        {
            body(folder);
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    // GetString keeps a leading byte order mark, unlike reading the file as text.
    private static string ReadExactly(string path) => new UTF8Encoding(false).GetString(File.ReadAllBytes(path));

    private static List<string> RoundTripFailures(string folder)
    {
        var parser = new ChordProParser();
        var failures = new List<string>();
        foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).OrderBy(f => f, StringComparer.Ordinal))
        {
            var text = ReadExactly(file);
            var expected = LosslessAssert.Expected(text);
            var actual = parser.Render(parser.Parse(text));
            if (actual != expected)
                failures.Add($"{Path.GetFileName(file)}: {FirstDifference(expected, actual)}");
        }

        return failures;
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
