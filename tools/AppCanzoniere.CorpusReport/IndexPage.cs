using System.Text;
using AppCanzoniere.Application.Import;

namespace AppCanzoniere.CorpusReport;

/// <summary>Summary of one converted song, as listed in the index.</summary>
internal sealed record SongSummary(string Name, string PageName, ChordSheetResult Result)
{
    public int Lines => Result.Lines.Count;

    public int LyricLines => Result.Lines.Count(l => l.Kind == SheetLineKind.Lyric);

    public int Warnings => Result.Lines.Sum(l => l.Warnings.Count);

    public int DoubtfulLines => Result.DoubtfulLines.Count();
}

/// <summary>Index of all songs, the most doubtful first.</summary>
internal static class IndexPage
{
    public static string Render(IReadOnlyList<SongSummary> songs)
    {
        var body = new StringBuilder();
        body.Append("<h1>Corpus: conversione degli accordi sopra il testo</h1>\n");
        body.Append($"<p class=\"muted\">{Html.Number(songs.Count)} brani, i più dubbi in alto.</p>\n");
        body.Append("<table>\n<tr><th>Brano</th><th class=\"n\">Righe</th><th class=\"n\">Righe accordi</th>");
        body.Append("<th class=\"n\">Righe testo</th><th class=\"n\">Avvisi</th><th class=\"n\">Righe dubbie</th><th>Notazione</th></tr>\n");
        foreach (var song in songs.OrderByDescending(s => s.DoubtfulLines).ThenByDescending(s => s.Warnings).ThenBy(s => s.Name, StringComparer.Ordinal))
            body.Append(Row(song));
        body.Append("</table>\n");
        return Html.Page("Corpus: elenco dei brani", body.ToString());
    }

    private static string Row(SongSummary song) =>
        $"<tr><td><a href=\"{Html.Link(song.PageName)}\">{Html.Escape(song.Name)}</a></td>" +
        $"<td class=\"n\">{Html.Number(song.Lines)}</td><td class=\"n\">{Html.Number(song.Result.ChordLineCount)}</td><td class=\"n\">{Html.Number(song.LyricLines)}</td>" +
        $"<td class=\"n\">{Html.Number(song.Warnings)}</td><td class=\"n\">{Html.Number(song.DoubtfulLines)}</td><td>{Html.Escape(Html.NotationLabel(song.Result))}</td></tr>\n";
}
