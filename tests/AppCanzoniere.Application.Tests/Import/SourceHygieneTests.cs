using System.Globalization;

namespace AppCanzoniere.Application.Tests.Import;

// Invisible characters must be written as escapes: a literal one in source cannot be reviewed and breaks under editors and shells.
public class SourceHygieneTests
{
    private static readonly string[] Folders =
    {
        "src/AppCanzoniere.Application/Import",
        "tests/AppCanzoniere.Application.Tests/Import",
        "tools/AppCanzoniere.CorpusReport"
    };

    [Fact]
    public void ImportSources_HaveNoLiteralControlFormatOrSeparatorCharactersOutsideComments()
    {
        var root = FindRoot();
        var problems = new List<string>();
        var scanned = 0;
        foreach (var folder in Folders)
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(root, folder), "*.cs", SearchOption.AllDirectories)
                         .Where(f => !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar) &&
                                     !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar)))
            {
                scanned++;
                problems.AddRange(Scan(file));
            }
        }

        Assert.True(scanned > 20, $"Only {scanned} source files were found.");
        Assert.True(problems.Count == 0, string.Join("\n", problems));
    }

    private static string FindRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, Folders[0])))
                return dir.FullName;
        }

        throw new DirectoryNotFoundException("The source folders were not found above the test assembly.");
    }

    private static IEnumerable<string> Scan(string file)
    {
        var text = File.ReadAllText(file);
        var inLineComment = false;
        var inBlockComment = false;
        var inString = false;
        var line = 1;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            var next = i + 1 < text.Length ? text[i + 1] : '\0';
            if (c == '\n')
            {
                line++;
                inLineComment = false;
                inString = false;
            }
            else if (inBlockComment)
                inBlockComment = !(c == '*' && next == '/');
            else if (!inLineComment && !inString && c == '/' && next == '/')
                inLineComment = true;
            else if (!inLineComment && !inString && c == '/' && next == '*')
                inBlockComment = true;
            else if (!inLineComment && c == '"' && (i == 0 || text[i - 1] != '\\'))
                inString = !inString;

            if (!inLineComment && !inBlockComment && IsHidden(c))
                yield return $"{Path.GetFileName(file)}:{line} contains U+{(int)c:X4}";
        }
    }

    private static bool IsHidden(char c)
    {
        if (c < 0x80)
            return (c < 0x20 && c is not ('\t' or '\n' or '\r')) || c == 0x7F;

        return CharUnicodeInfo.GetUnicodeCategory(c) is UnicodeCategory.Control or UnicodeCategory.Format
            or UnicodeCategory.SpaceSeparator or UnicodeCategory.LineSeparator or UnicodeCategory.ParagraphSeparator
            or UnicodeCategory.NonSpacingMark or UnicodeCategory.EnclosingMark or UnicodeCategory.SpacingCombiningMark;
    }
}
