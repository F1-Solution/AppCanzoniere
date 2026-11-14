using System.Text.RegularExpressions;
using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Services;

/// <summary>Writes edited header values into the first matching directive of the source lines, or adds the directive.</summary>
internal static class ChordProHeaderEditor
{
    private const char Bom = '﻿';

    private static readonly Regex Directive = new(
        @"^(?<lead>﻿?\s*\{(?<name>[^:}]*))(?::(?<value>.*))?(?<tail>\}\s*)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Dictionary<HeaderField, string[]> Names = new()
    {
        [HeaderField.Title] = new[] { "title", "t" },
        [HeaderField.Artist] = new[] { "artist", "subtitle", "st" },
        [HeaderField.Key] = new[] { "key" },
        [HeaderField.Capo] = new[] { "capo" },
        [HeaderField.Tempo] = new[] { "tempo" },
        [HeaderField.Time] = new[] { "time" }
    };

    /// <param name="snapshot">Tells which source line the parser kept each value from.</param>
    public static IReadOnlyList<ChordProSourceLine> Apply(IReadOnlyList<ChordProSourceLine> source, IReadOnlyList<HeaderEdit> edits, ChordProDocumentSnapshot snapshot)
    {
        var lines = source.ToList();
        var missing = new List<HeaderEdit>();
        foreach (var edit in edits.OrderBy(e => e.Field))
        {
            if (!TryReplace(lines, edit, snapshot.SourceLineOf(edit.Field)))
                missing.Add(edit);
        }

        var cursor = snapshot.SourceLineOf(HeaderField.Title) + 1;
        foreach (var edit in missing.Where(e => HeaderText.Clean(e.Value).Length > 0))
            Insert(lines, cursor++, edit);

        return lines;
    }

    private static bool TryReplace(List<ChordProSourceLine> lines, HeaderEdit edit, int index)
    {
        if (index < 0)
            return false;

        lines[index] = lines[index] with { Raw = Rewrite(lines[index].Raw, HeaderText.Clean(edit.Value)) };
        return true;
    }

    private static string Rewrite(string raw, string value)
    {
        var match = Directive.Match(raw);
        var old = match.Groups["value"];
        var core = old.Value.Trim();
        var start = core.Length == 0 ? old.Value.Length : old.Value.IndexOf(core, StringComparison.Ordinal);
        var written = !old.Success
            ? ":" + value
            : ":" + old.Value[..start] + value + old.Value[(start + core.Length)..];
        return match.Groups["lead"].Value + written + match.Groups["tail"].Value;
    }

    private static void Insert(List<ChordProSourceLine> lines, int index, HeaderEdit edit)
    {
        var name = Names[edit.Field][0];
        var kind = edit.Field switch
        {
            HeaderField.Key => ChordProSourceKind.Key,
            HeaderField.Capo => ChordProSourceKind.Capo,
            _ => ChordProSourceKind.Verbatim
        };
        var raw = "{" + name + ":" + HeaderText.Clean(edit.Value) + "}";
        if (index == 0 && lines.Count > 0 && lines[0].Raw.StartsWith(Bom))
        {
            raw = Bom + raw;
            lines[0] = lines[0] with { Raw = lines[0].Raw.TrimStart(Bom) };
        }

        lines.Insert(index, new ChordProSourceLine(kind, raw));
    }
}
