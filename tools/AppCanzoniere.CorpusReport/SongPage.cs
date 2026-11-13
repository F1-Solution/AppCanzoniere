using System.Text;
using AppCanzoniere.Application.Import;

namespace AppCanzoniere.CorpusReport;

/// <summary>One song: original text, chords-above-lyrics redrawn from the result, and the ChordPro.</summary>
internal static class SongPage
{
    public static string Render(string name, ChordSheetResult result)
    {
        var doubtful = result.DoubtfulLines.ToDictionary(l => l.LineNumber);
        var body = new StringBuilder();
        body.Append("<p><a href=\"index.html\">&larr; Elenco dei brani</a></p>\n");
        body.Append($"<h1>{Html.Escape(name)}</h1>\n");
        body.Append("<div class=\"cols\">\n");
        body.Append(Column("1. Testo originale", OriginalRows(result, doubtful)));
        body.Append(Column("2. Risultato come accordi sopra il testo", PlainRows(ChordsAboveText.Render(result.ChordPro))));
        body.Append(Column("3. ChordPro prodotto", PlainRows(result.ChordPro)));
        body.Append("</div>\n");
        body.Append(DoubtfulTable(doubtful.Values));
        return Html.Page(name, body.ToString());
    }

    private static string Column(string title, string rows) =>
        $"<div class=\"col\"><h3>{Html.Escape(title)}</h3><pre>{rows}</pre></div>\n";

    private static string OriginalRows(ChordSheetResult result, Dictionary<int, SheetLineDiagnostic> doubtful)
    {
        var rows = new StringBuilder();
        foreach (var line in result.Lines)
        {
            var isDoubtful = doubtful.TryGetValue(line.LineNumber, out var diagnostic);
            var cssClass = Html.KindClass(line.Kind) + (isDoubtful ? " doubt" : string.Empty);
            var title = Html.Escape($"{Html.KindLabel(line.Kind)}, affidabilità {Html.Score(line.Confidence)}" +
                (isDoubtful ? ": " + Describe(diagnostic!) : string.Empty));
            rows.Append($"<span class=\"row {cssClass}\" title=\"{title}\"><span class=\"no\">{Html.Number(line.LineNumber)}</span>{Html.Escape(line.Source)}");
            if (isDoubtful)
                rows.Append($"<span class=\"warn\">⚠ {Html.Escape(Describe(diagnostic!))}</span>");
            rows.Append("</span>");
        }

        return rows.ToString();
    }

    private static string PlainRows(string text)
    {
        var rows = new StringBuilder();
        foreach (var line in text.TrimEnd('\n').Split('\n'))
            rows.Append($"<span class=\"row\">{Html.Escape(line)}</span>");
        return rows.ToString();
    }

    private static string Describe(SheetLineDiagnostic line) =>
        line.Warnings.Count > 0
            ? string.Join(", ", line.Warnings.Select(Html.WarningLabel))
            : "affidabilità bassa";

    private static string DoubtfulTable(IEnumerable<SheetLineDiagnostic> lines)
    {
        var rows = lines.OrderBy(l => l.LineNumber).ToList();
        if (rows.Count == 0)
            return "<h2>Righe dubbie</h2>\n<p class=\"muted\">Nessuna.</p>\n";

        var table = new StringBuilder("<h2>Righe dubbie</h2>\n<table><tr><th class=\"n\">Riga</th><th>Tipo</th><th class=\"n\">Affidabilità</th><th>Motivo</th></tr>\n");
        foreach (var line in rows)
        {
            table.Append($"<tr><td class=\"n\">{Html.Number(line.LineNumber)}</td><td>{Html.Escape(Html.KindLabel(line.Kind))}</td>");
            table.Append($"<td class=\"n\">{Html.Score(line.Confidence)}</td><td>{Html.Escape(Describe(line))}</td></tr>\n");
        }

        return table.Append("</table>\n").ToString();
    }
}
