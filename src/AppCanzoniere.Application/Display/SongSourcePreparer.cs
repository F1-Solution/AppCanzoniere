using System.Text;
using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.Display;

/// <summary>Rewrites stored ChordPro for the text screen: tabs become spaces and chorus directives become visible labels.</summary>
internal static class SongSourcePreparer
{
    private const int TabWidth = 4;

    private static readonly Regex Directive = new(
        @"^\s*\{\s*(?<name>[A-Za-z_]+)\s*(?::\s*(?<arg>.*?))?\s*\}\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex QuotedLabel = new(
        "^label\\s*=\\s*\"(?<text>.*)\"$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly HashSet<string> ChorusStart = new(StringComparer.OrdinalIgnoreCase) { "start_of_chorus", "soc" };

    private static readonly HashSet<string> TabStart = new(StringComparer.OrdinalIgnoreCase) { "start_of_tab", "sot" };

    private static readonly HashSet<string> TabEnd = new(StringComparer.OrdinalIgnoreCase) { "end_of_tab", "eot" };

    private static readonly HashSet<string> Comments = new(StringComparer.OrdinalIgnoreCase)
    {
        "comment", "c", "comment_italic", "ci", "comment_box", "cb"
    };

    /// <summary>The song text ready to render; <paramref name="chorusWord"/> labels a chorus or recall that has no label.</summary>
    public static string Prepare(string chordPro, string chorusWord)
    {
        var output = new List<string>();
        var inTab = false;
        foreach (var raw in chordPro.Split('\n'))
        {
            var ending = raw.EndsWith('\r') ? "\r" : string.Empty;
            var line = ExpandTabs(ending.Length > 0 ? raw[..^1] : raw);
            var directive = Directive.Match(line);
            var name = directive.Success ? directive.Groups["name"].Value : string.Empty;

            if (TabStart.Contains(name))
                inTab = true;
            else if (TabEnd.Contains(name))
                inTab = false;

            if (!inTab && directive.Success && TryMark(name, directive, output, chorusWord, ending, out var replacement))
                output.AddRange(replacement);
            else
                output.Add(line + ending);
        }

        return string.Join('\n', output);
    }

    private static bool TryMark(string name, Match directive, List<string> output, string chorusWord, string ending, out string[] lines)
    {
        lines = Array.Empty<string>();
        var label = LabelOf(directive.Groups["arg"].Value, chorusWord);
        if (name.Equals("chorus", StringComparison.OrdinalIgnoreCase))
        {
            lines = new[] { Comment(label, ending) };
            return true;
        }

        if (!ChorusStart.Contains(name) || PreviousLineIsComment(output))
            return false;

        lines = new[] { Comment(label, ending), directive.Value + ending };
        return true;
    }

    private static string Comment(string label, string ending) => "{comment: " + label + "}" + ending;

    private static string LabelOf(string argument, string fallback)
    {
        var quoted = QuotedLabel.Match(argument.Trim());
        var text = quoted.Success ? quoted.Groups["text"].Value : argument;
        text = text.Replace("{", string.Empty).Replace("}", string.Empty).Trim();
        return text.Length > 0 ? text : fallback;
    }

    private static bool PreviousLineIsComment(List<string> output)
    {
        for (var i = output.Count - 1; i >= 0; i--)
        {
            if (output[i].Trim().Length == 0)
                continue;

            var match = Directive.Match(output[i]);
            return match.Success && Comments.Contains(match.Groups["name"].Value);
        }

        return false;
    }

    // Chord brackets take no column, so the tab stops count only the lyric characters.
    private static string ExpandTabs(string line)
    {
        if (!line.Contains('\t'))
            return line;

        var sb = new StringBuilder();
        var column = 0;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            var close = c == '[' ? line.IndexOf(']', i) : -1;
            if (close >= 0)
            {
                sb.Append(line, i, close - i + 1);
                i = close;
            }
            else if (c == '\t')
            {
                var spaces = TabWidth - column % TabWidth;
                sb.Append(' ', spaces);
                column += spaces;
            }
            else
            {
                sb.Append(c);
                column++;
            }
        }

        return sb.ToString();
    }
}
