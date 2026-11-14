using System.Text;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.CorpusReport;

/// <summary>Draws ChordPro back as plain "chords above lyrics" text so it can be compared with the original by eye.</summary>
internal static class ChordsAboveText
{
    public static string Render(string chordPro)
    {
        var document = new ChordProParser().Parse(chordPro);
        var output = new List<string>();
        foreach (var line in document.Lines)
            output.AddRange(RenderLine(line));
        return string.Join("\n", output);
    }

    private static IEnumerable<string> RenderLine(ChordProLine line)
    {
        switch (line.Type)
        {
            case ChordProLineType.Empty:
                return new[] { string.Empty };
            case ChordProLineType.Title:
                return new[] { line.Text };
            case ChordProLineType.Comment:
                return new[] { $"({line.Text})" };
            case ChordProLineType.Tab:
                return new[] { line.Text };
            default:
                return line.Chords.Count == 0
                    ? new[] { line.Text }
                    : TrimmedPair(ChordRow(line.Chords), line.Text);
        }
    }

    private static IEnumerable<string> TrimmedPair(string chordRow, string lyric) =>
        lyric.Trim().Length == 0 ? new[] { chordRow } : new[] { chordRow, lyric };

    private static string ChordRow(IEnumerable<ChordPosition> chords)
    {
        var row = new StringBuilder();
        foreach (var chord in chords.OrderBy(c => c.Position))
        {
            var label = chord.Chord.TrimStart('*');
            var column = Math.Max(chord.Position, row.Length == 0 ? 0 : row.Length + 1);
            row.Append(' ', column - row.Length).Append(label);
        }

        return row.ToString();
    }
}
