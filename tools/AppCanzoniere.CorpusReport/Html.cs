using System.Net;
using AppCanzoniere.Application.Import;

namespace AppCanzoniere.CorpusReport;

/// <summary>Escaping, shared style and Italian labels of the report pages.</summary>
internal static class Html
{
    public const string Style = """
        :root { --bg:#fbfbf9; --fg:#1d1d1b; --muted:#6b6b66; --line:#d8d8d2; --panel:#ffffff;
          --chord:#d9ecff; --lyric:#ffffff; --label:#e9e0ff; --meta:#e2f4e2; --tab:#f1ead3; --inline:#d9f2f2; --other:#eee;
          --doubt:#e0a000; --doubtbg:#fff1c9; }
        @media (prefers-color-scheme: dark) { :root { --bg:#161615; --fg:#e8e8e3; --muted:#9a9a93; --line:#3a3a36; --panel:#1f1f1d;
          --chord:#1c3550; --lyric:#1f1f1d; --label:#34284f; --meta:#1f3a22; --tab:#3d361d; --inline:#183c3c; --other:#2a2a28;
          --doubt:#e0a000; --doubtbg:#4a3a0a; } }
        body { background:var(--bg); color:var(--fg); font:15px/1.4 system-ui,sans-serif; margin:16px; }
        a { color:inherit; } h1 { font-size:1.3rem; } h2 { font-size:1.05rem; margin-top:1.5rem; }
        table { border-collapse:collapse; width:100%; } th,td { border-bottom:1px solid var(--line); padding:4px 8px; text-align:left; }
        td.n,th.n { text-align:right; } .muted { color:var(--muted); }
        .cols { display:grid; grid-template-columns:repeat(3,minmax(0,1fr)); gap:10px; }
        @media (max-width:900px) { .cols { grid-template-columns:1fr; } }
        .col { background:var(--panel); border:1px solid var(--line); border-radius:6px; overflow-x:auto; }
        .col h3 { margin:0; padding:6px 10px; font-size:.85rem; border-bottom:1px solid var(--line); }
        pre { margin:0; padding:6px 0; font:13px/1.35 ui-monospace,Consolas,monospace; }
        .row { display:block; padding:0 10px; white-space:pre; }
        .no { display:inline-block; width:3.2em; color:var(--muted); user-select:none; }
        .chord { background:var(--chord); } .lyric { background:var(--lyric); } .label { background:var(--label); }
        .meta { background:var(--meta); } .tab { background:var(--tab); } .inline { background:var(--inline); }
        .other { background:var(--other); } .blank { background:transparent; }
        .doubt { outline:2px solid var(--doubt); outline-offset:-2px; background:var(--doubtbg); }
        .warn { color:var(--doubt); font-weight:600; margin-left:1em; }
        """;

    public static string Escape(string text) => WebUtility.HtmlEncode(text);

    public static string Page(string title, string body) =>
        $"<!doctype html>\n<html lang=\"it\">\n<head>\n<meta charset=\"utf-8\">\n" +
        $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n<title>{Escape(title)}</title>\n" +
        $"<style>\n{Style}\n</style>\n</head>\n<body>\n{body}\n</body>\n</html>\n";

    public static string KindClass(SheetLineKind kind) => kind switch
    {
        SheetLineKind.ChordLine => "chord",
        SheetLineKind.Lyric => "lyric",
        SheetLineKind.SectionLabel => "label",
        SheetLineKind.Metadata or SheetLineKind.Title => "meta",
        SheetLineKind.Tab => "tab",
        SheetLineKind.InlineChordLyric => "inline",
        SheetLineKind.Blank => "blank",
        _ => "other"
    };

    public static string KindLabel(SheetLineKind kind) => kind switch
    {
        SheetLineKind.ChordLine => "accordi",
        SheetLineKind.Lyric => "testo",
        SheetLineKind.SectionLabel => "etichetta",
        SheetLineKind.Metadata => "metadato",
        SheetLineKind.Title => "titolo",
        SheetLineKind.Tab => "tablatura",
        SheetLineKind.InlineChordLyric => "accordi nel testo",
        SheetLineKind.Blank => "vuota",
        _ => "altro"
    };

    public static string WarningLabel(SheetWarning warning) => warning switch
    {
        SheetWarning.AmbiguousSingleToken => "una sola nota isolata",
        SheetWarning.AmbiguousNoteWords => "parole che sembrano accordi",
        SheetWarning.StrayWordOnChordLine => "parola estranea tra gli accordi",
        SheetWarning.OtherNotation => "accordi dell'altra notazione",
        SheetWarning.ParenthesesRemoved => "parentesi tolte",
        SheetWarning.SnappedColumn => "accordo spostato a inizio parola",
        SheetWarning.ChordInsideWord => "accordo dentro la parola",
        SheetWarning.TabExpanded => "tabulazioni espanse",
        SheetWarning.ChorusRecallWithoutChorus => "ritornello richiamato ma mai scritto",
        SheetWarning.KeyAndCapo => "tonalità e capotasto insieme",
        SheetWarning.UnreadableMetadata => "metadato illeggibile",
        SheetWarning.EscapedLyric => "riga di testo con cancelletto",
        SheetWarning.BracketsReplaced => "parentesi quadre o graffe sostituite",
        SheetWarning.ChordLineKeptAlone => "accordi lasciati soli sopra un testo con accordi",
        _ => warning.ToString()
    };

    public static string NotationLabel(ChordSheetResult result) => result.WasChordPro
        ? "già ChordPro"
        : result.DetectedNotation?.ToString() switch
        {
            "Italian" => "italiana",
            "English" => "inglese",
            _ => "nessuna"
        };
}
