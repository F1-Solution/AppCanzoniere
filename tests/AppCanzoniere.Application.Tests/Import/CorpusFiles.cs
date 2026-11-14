namespace AppCanzoniere.Application.Tests.Import;

/// <summary>Finds the local, git-ignored corpus text files; empty when the corpus is not there.</summary>
internal static class CorpusFiles
{
    private const string CorpusFolderName = "corpus-locale";

    public static string? TextFolder()
    {
        var candidates = new List<string>();
        var fromEnvironment = Environment.GetEnvironmentVariable("APPCANZONIERE_CORPUS");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            candidates.Add(fromEnvironment);

        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            candidates.Add(Path.Combine(dir.FullName, CorpusFolderName));

        return candidates.Select(c => Path.Combine(c, "testo")).FirstOrDefault(Directory.Exists);
    }

    /// <summary>For every converter line, the number of the physical (newline-terminated) line it belongs to; labels use that numbering.</summary>
    /// <remarks>A character walk written apart from the converter's own splitting, so a mistake there is not mirrored here.</remarks>
    public static IReadOnlyList<int> PhysicalLines(string text)
    {
        var map = new List<int> { 1 };
        var physical = 1;
        var endsWithBreak = false;
        for (var i = 0; i < text.Length; i++)
        {
            // Separators between "\r" and "\n" are dropped, which leaves one "\r\n" break.
            var c = text[i];
            if (c == '\r' && SkipSeparators(text, i + 1) is var next && next < text.Length && text[next] == '\n')
                continue;

            if (c is '\n' or '\r')
            {
                physical++;
                map.Add(physical);
                endsWithBreak = true;
            }
            else if (IsSeparator(c) && !OnlySeparatorsUntilBreak(text, i))
            {
                map.Add(physical);
                endsWithBreak = false;
            }
            else if (!IsSeparator(c))
            {
                endsWithBreak = false;
            }
        }

        // A terminating newline ends the last line; it does not start another.
        if (endsWithBreak && map.Count > 1)
            map.RemoveAt(map.Count - 1);
        return map;
    }

    private static bool IsSeparator(char c) => c is (char)0x85 or (char)0x2028 or (char)0x2029;

    // A run of separators right before a newline or the end of the text belongs to that break.
    private static bool OnlySeparatorsUntilBreak(string text, int index)
    {
        var end = SkipSeparators(text, index);
        return end == text.Length || text[end] is '\r' or '\n';
    }

    private static int SkipSeparators(string text, int index)
    {
        while (index < text.Length && IsSeparator(text[index]))
            index++;
        return index;
    }

    public static IEnumerable<string> All() =>
        TextFolder() is { } folder ? Directory.EnumerateFiles(folder, "*.txt") : Enumerable.Empty<string>();
}
