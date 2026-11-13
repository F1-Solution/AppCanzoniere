namespace AppCanzoniere.Application.Interfaces;

/// <summary>How a source line is treated when a document is rendered.</summary>
internal enum ChordProSourceKind
{
    /// <summary>Written back exactly as read.</summary>
    Verbatim,

    /// <summary>A lyric line whose <c>[chord]</c> markers follow transposition and notation changes.</summary>
    Lyrics,

    /// <summary>A <c>{key}</c> directive whose value follows transposition.</summary>
    Key,

    /// <summary>A <c>{capo}</c> directive whose value follows a requested capo.</summary>
    Capo,

    /// <summary>A line inside a tab block: written back exactly as read and never a place to insert a directive.</summary>
    Tab
}

/// <summary>One line of the parsed file, exactly as written but for its line ending.</summary>
internal sealed record ChordProSourceLine(ChordProSourceKind Kind, string Raw);
