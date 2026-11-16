using AppCanzoniere.Application.Import;

namespace AppCanzoniere.Application.PdfImport;

/// <summary>A split song converted to ChordPro, with what a reviewer may want to check.</summary>
public sealed record ConvertedSong(
    string Title,
    int FirstPage,
    int LastPage,
    string ChordPro,
    int ChordLineCount,
    IReadOnlyList<SheetWarning> Warnings,
    int DoubtfulLineCount);

/// <summary>Runs the chords-above-lyrics converter on one split song and sets its title.</summary>
public static class PdfSongConverter
{
    public static ConvertedSong Convert(SplitSong song)
    {
        ArgumentNullException.ThrowIfNull(song);
        var text = "Titolo: " + song.Title + "\n" + string.Join('\n', song.Lines);
        var result = ChordSheetConverter.Convert(text);
        var doubtful = result.DoubtfulLines.ToList();
        var warnings = doubtful.SelectMany(l => l.Warnings).Where(w => w != SheetWarning.ChordInsideWord).Distinct().ToList();
        return new ConvertedSong(song.Title, song.FirstPage, song.LastPage, WithSubtitles(result.ChordPro, song.Subtitles), result.ChordLineCount, warnings, doubtful.Count);
    }

    // The title directive is the first header line; subtitles follow it.
    private static string WithSubtitles(string chordPro, IReadOnlyList<string> subtitles)
    {
        if (subtitles.Count == 0)
            return chordPro;

        var lines = chordPro.Split('\n').ToList();
        var at = lines.FindIndex(l => l.StartsWith("{title:", StringComparison.Ordinal)) + 1;
        lines.InsertRange(at, subtitles.Select(s => "{subtitle:" + TextSafety.Replace(s) + "}"));
        return string.Join('\n', lines);
    }
}
