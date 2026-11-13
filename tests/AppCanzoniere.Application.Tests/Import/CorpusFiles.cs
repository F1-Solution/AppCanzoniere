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
    public static IReadOnlyList<int> PhysicalLines(string text)
    {
        var map = new List<int> { 1 };
        var physical = 1;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            var crlf = c == '\r' && i + 1 < text.Length && text[i + 1] == '\n';
            if (crlf)
                continue;

            if (c is '\n' or '\r')
                physical++;
            else if (c != (char)0x85 && c != (char)0x2028 && c != (char)0x2029)
                continue;

            map.Add(physical);
        }

        return map;
    }

    public static IEnumerable<string> All() =>
        TextFolder() is { } folder ? Directory.EnumerateFiles(folder, "*.txt") : Enumerable.Empty<string>();
}
