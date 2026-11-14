using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.Import;

internal enum SectionKind
{
    Chorus,
    Verse,
    Bridge,
    CommentOnly
}

/// <summary>A section label line; Value is the directive value, RestStart the index of the text after the label or -1.</summary>
internal sealed record SectionLabel(SectionKind Kind, string Text, string? Value, int RestStart)
{
    private static readonly HashSet<string> InstrumentalNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "intro", "introduzione", "finale", "fine", "coda", "interludio", "intermezzo", "strumentale", "outro", "inizio", "ponte strumentale"
    };

    /// <summary>True for a label of a part without lyrics (Intro, Coda...), which can carry bare chords on its own line.</summary>
    public bool IsInstrumental => InstrumentalNames.Contains(string.Join(' ', Text.Trim().TrimEnd('.', ':').Split(' ', StringSplitOptions.RemoveEmptyEntries)));
}

/// <summary>Recognises Rit., Strofa 2, 1., Intro: and friends (spec 6.1).</summary>
internal static class SectionLabelParser
{
    private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

    private static readonly Regex Keyword = new(
        @"^(?<kw>ritornello|rit|coro|chorus|refrain|strofa|str|verse|stanza|bridge|ponte\s+strumentale|ponte|special|introduzione|intro|interludio|strumentale|finale|coda|outro|fine)(?!\p{L})",
        Options);

    private static readonly Regex NumberOnly = new(@"^\d{1,2}[.)](?=\s|$)", Options);
    private static readonly Regex Number = new(@"\G\s*\d{1,2}(?!\d)", Options);

    public static SectionLabel? TryParse(string text)
    {
        var trimmed = text.TrimStart();
        var lead = text.Length - trimmed.Length;
        var keyword = Keyword.Match(trimmed);
        var position = keyword.Success ? keyword.Length : NumberOnly.Match(trimmed).Length;
        if (position == 0)
            return null;

        var kind = keyword.Success ? KindOf(keyword.Groups["kw"].Value) : SectionKind.Verse;
        var bare = keyword.Success;
        if (keyword.Success)
            position = ReadSuffix(trimmed, position, ref bare);

        var label = trimmed[..position];
        var rest = trimmed[position..];
        if (rest.Trim().Length == 0)
            return Create(kind, label, bare, -1);

        var endsWithPunctuation = IsPunctuation(label[^1]);
        if (!endsWithPunctuation || !char.IsWhiteSpace(rest[0]))
            return null;

        return Create(kind, label, bare, lead + position + (rest.Length - rest.TrimStart().Length));
    }

    private static int ReadSuffix(string trimmed, int position, ref bool bare)
    {
        position = SkipPunctuation(trimmed, position);

        var number = Number.Match(trimmed, position);
        if (!number.Success)
            return position;

        bare = false;
        position = number.Index + number.Length;
        return SkipPunctuation(trimmed, position);
    }

    private static int SkipPunctuation(string text, int position)
    {
        while (position < text.Length && IsPunctuation(text[position]))
            position++;
        return position;
    }

    private static SectionLabel Create(SectionKind kind, string label, bool bare, int restStart)
    {
        var value = kind == SectionKind.CommentOnly || bare ? null : label.TrimEnd(':');
        return new SectionLabel(kind, label, value, restStart);
    }

    private static bool IsPunctuation(char c) => c is '.' or ':' or ')';

    private static SectionKind KindOf(string keyword) => keyword.ToLowerInvariant() switch
    {
        "rit" or "ritornello" or "coro" or "chorus" or "refrain" => SectionKind.Chorus,
        "strofa" or "str" or "verse" or "stanza" => SectionKind.Verse,
        "bridge" or "ponte" or "special" => SectionKind.Bridge,
        _ => SectionKind.CommentOnly
    };
}
