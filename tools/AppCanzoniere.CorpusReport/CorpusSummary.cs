using System.Globalization;
using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.CorpusReport;

/// <summary>Corpus-wide counts printed after the report: files kept as ChordPro, chord lines, unbalanced chords, control characters.</summary>
internal sealed class CorpusSummary
{
    private int _files;
    private int _alreadyChordPro;
    private int _chordLines;
    private int _unbalancedChords;
    private int _controlLines;

    public void Add(string name, ChordSheetResult result)
    {
        _files++;
        _alreadyChordPro += result.WasChordPro ? 1 : 0;
        _chordLines += result.ChordLineCount;
        var parsed = new ChordProParser().Parse(result.ChordPro);
        var chords = parsed.Lines.Where(l => l.Type != ChordProLineType.Tab).SelectMany(l => l.Chords ?? new List<ChordPosition>());
        _unbalancedChords += chords.Count(c => c.Chord.Count(x => x == '(') != c.Chord.Count(x => x == ')'));
        _controlLines += result.ChordPro.Split('\n').Count(l => l.Any(c => char.IsControl(c)));
        if (name.StartsWith("17-", StringComparison.Ordinal))
            Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"Chord lines in {name}: {result.ChordLineCount}"));
    }

    public void Print()
    {
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"Files {_files}; already ChordPro {_alreadyChordPro}; chord lines {_chordLines}; chords with unbalanced parentheses {_unbalancedChords}; output lines with a control character {_controlLines}."));
    }
}
