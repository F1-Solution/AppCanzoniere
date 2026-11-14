using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Services;

/// <summary>Writes a parsed document back from its source lines, changing only what a requested transformation touches.</summary>
internal static class ChordProSourceRenderer
{
    private const char Bom = (char)0xFEFF;

    private static readonly Regex ChordToken = new(@"\[([^\]]*)\]", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex DirectiveValue = new("^(?<head>" + Bom + @"*\s*\{[^:}]*:)(?<value>.*)(?<tail>\}\s*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex CapoDirective = new("^(?<lead>" + Bom + @"*\s*\{[^:}]*)(?::(?<value>.*))?(?<tail>\}\s*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex TitleDirective = new("^" + Bom + @"*\s*\{\s*(?:title|t)\s*[:}]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static string Render(IReadOnlyList<ChordProSourceLine> lines, ChordTransformer transformer, int? capo)
    {
        var insertAt = capo > 0 && !lines.Any(l => l.Kind == ChordProSourceKind.Capo) ? CapoInsertIndex(lines) : -1;
        var sb = new StringBuilder();
        for (var i = 0; i < lines.Count; i++)
        {
            var stripBom = false;
            if (i == insertAt)
                stripBom = AppendInsertedCapo(sb, lines, capo!.Value);

            AppendLine(sb, stripBom ? lines[i] with { Raw = lines[i].Raw.TrimStart(Bom) } : lines[i], transformer, capo);
        }

        if (insertAt == lines.Count)
            sb.Append("{capo:").Append(capo!.Value.ToString(CultureInfo.InvariantCulture)).Append("}\n");

        return sb.ToString();
    }

    private static int CapoInsertIndex(IReadOnlyList<ChordProSourceLine> lines)
    {
        var key = IndexOf(lines, l => l.Kind == ChordProSourceKind.Key);
        if (key >= 0)
            return key + 1;

        var title = IndexOf(lines, l => l.Kind == ChordProSourceKind.Verbatim && TitleDirective.IsMatch(l.Raw));
        return title >= 0 ? title + 1 : 0;
    }

    private static int IndexOf(IReadOnlyList<ChordProSourceLine> lines, Func<ChordProSourceLine, bool> match)
    {
        for (var i = 0; i < lines.Count; i++)
        {
            if (match(lines[i]))
                return i;
        }

        return -1;
    }

    // A byte order mark belongs at the very start, so it moves in front of a capo line inserted there.
    private static bool AppendInsertedCapo(StringBuilder sb, IReadOnlyList<ChordProSourceLine> lines, int fret)
    {
        var marks = sb.Length == 0 ? lines[0].Raw.Length - lines[0].Raw.TrimStart(Bom).Length : 0;
        sb.Append(Bom, marks);
        sb.Append("{capo:").Append(fret.ToString(CultureInfo.InvariantCulture)).Append("}\n");
        return marks > 0;
    }

    private static void AppendLine(StringBuilder sb, ChordProSourceLine line, ChordTransformer transformer, int? capo)
    {
        switch (line.Kind)
        {
            case ChordProSourceKind.Lyrics:
                sb.Append(ChordToken.Replace(line.Raw, m => "[" + KeepPadding(m.Groups[1].Value, transformer.Chord) + "]"));
                break;
            case ChordProSourceKind.Key:
                sb.Append(RewriteValue(line.Raw, v => KeepPadding(v, transformer.Key)));
                break;
            case ChordProSourceKind.Capo when capo is { } fret:
                if (fret == 0)
                    return; // The line is removed together with its line feed.

                sb.Append(RewriteCapo(line.Raw, fret));
                break;
            default:
                sb.Append(line.Raw);
                break;
        }

        sb.Append('\n');
    }

    private static int? CurrentFret(string text) =>
        int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var fret) ? fret : null;

    private static string RewriteCapo(string raw, int fret)
    {
        var match = CapoDirective.Match(raw);
        if (!match.Success)
            return raw;

        var value = match.Groups["value"].Value;
        var core = value.Trim();
        if (CurrentFret(core) == fret)
            return raw;

        var written = fret.ToString(CultureInfo.InvariantCulture);
        var newValue = core.Length == 0 ? value + written : KeepPadding(value, _ => written);
        return match.Groups["lead"].Value + ":" + newValue + match.Groups["tail"].Value;
    }

    private static string RewriteValue(string raw, Func<string, string> rewrite)
    {
        var match = DirectiveValue.Match(raw);
        return match.Success
            ? match.Groups["head"].Value + rewrite(match.Groups["value"].Value) + match.Groups["tail"].Value
            : raw;
    }

    private static string KeepPadding(string value, Func<string, string> rewrite)
    {
        var core = value.Trim();
        if (core.Length == 0)
            return value;

        var start = value.IndexOf(core, StringComparison.Ordinal);
        return value[..start] + rewrite(core) + value[(start + core.Length)..];
    }
}
