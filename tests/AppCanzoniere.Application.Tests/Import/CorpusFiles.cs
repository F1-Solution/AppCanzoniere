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

    public static IEnumerable<string> All() =>
        TextFolder() is { } folder ? Directory.EnumerateFiles(folder, "*.txt") : Enumerable.Empty<string>();
}
