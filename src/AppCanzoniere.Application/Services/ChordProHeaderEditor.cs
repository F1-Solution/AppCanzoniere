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

    public static IReadOnlyList<ChordProSourceLine> Apply(IReadOnlyList<ChordProSourceLine> source, IReadOnlyList<HeaderEdit> edits)
    {
        var lines = source.ToList();
        var missing = new List<HeaderEdit>();
        foreach (var edit in edits.OrderBy(e => e.Field))
        {
            if (!TryReplace(lines, edit))
                missing.Add(edit);
        }

        var cursor = Math.Max(0, IndexOfLine(lines, HeaderField.Title) + 1);
        foreach (var edit in missing.Where(e => e.Value.Trim().Length > 0))
            Insert(lines, cursor++, edit);

        return lines;
    }

    private static bool TryReplace(List<ChordProSourceLine> lines, HeaderEdit edit)
    {
        var index = IndexOfLine(lines, edit.Field);
        if (index < 0)
            return false;

        lines[index] = lines[index] with { Raw = Rewrite(lines[index].Raw, edit.Value.Trim()) };
        return true;
    }

    // Title and key lines with an empty value are ignored by the parser, so they are not the line to edit.
    private static int IndexOfLine(List<ChordProSourceLine> lines, HeaderField field)
    {
        for (var i = 0; i < lines.Count; i++)
        {
            if (lines[i].Kind is ChordProSourceKind.Lyrics or ChordProSourceKind.Tab)
                continue;

            var match = Directive.Match(lines[i].Raw);
            if (!match.Success || Array.IndexOf(Names[field], match.Groups["name"].Value.Trim().ToLowerInvariant()) < 0)
                continue;

            if (field is HeaderField.Title or HeaderField.Key && match.Groups["value"].Value.Trim().Length == 0)
                continue;

            return i;
        }

        return -1;
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
        var raw = "{" + name + ":" + edit.Value.Trim() + "}";
        if (index == 0 && lines.Count > 0 && lines[0].Raw.StartsWith(Bom))
        {
            raw = Bom + raw;
            lines[0] = lines[0] with { Raw = lines[0].Raw.TrimStart(Bom) };
        }

        lines.Insert(index, new ChordProSourceLine(kind, raw));
    }
}
