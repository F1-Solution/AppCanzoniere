using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Import;

internal enum MetaKind
{
    Title,
    Artist,
    Lyricist,
    Composer,
    Key,
    Capo,
    Tempo,
    Time,
    Comment
}

/// <summary>A header line turned into a directive; Unreadable ones are written as comments.</summary>
internal sealed record MetadataItem(MetaKind Kind, string Value, bool Unreadable);

internal static class MetadataParser
{
    public static MetadataItem? TryParse(string trimmed, Notation notation) => null;
}
