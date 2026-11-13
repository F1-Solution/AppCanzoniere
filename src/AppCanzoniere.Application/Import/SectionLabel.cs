namespace AppCanzoniere.Application.Import;

internal enum SectionKind
{
    Chorus,
    Verse,
    Bridge,
    CommentOnly
}

/// <summary>A section label line; Value is the directive value, RestStart the index of the text after the label or -1.</summary>
internal sealed record SectionLabel(SectionKind Kind, string Text, string? Value, int RestStart);

internal static class SectionLabelParser
{
    public static SectionLabel? TryParse(string text) => null;
}
