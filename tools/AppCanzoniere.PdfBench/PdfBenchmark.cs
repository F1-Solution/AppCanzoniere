using System.Diagnostics;
using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.PdfBench;

/// <summary>Scores of one labelled text: the pdftotext baseline and the in-app extraction of the same pages.</summary>
internal sealed record ExtractScore(string Name, IReadOnlyList<int> Pages, Score Baseline, Score Extracted, int Lines, int Unmatched, int Fragments)
{
    /// <summary>Non-blank pdftotext lines of the labelled pages.</summary>
    public int SourceLines { get; init; }

    /// <summary>pdftotext lines whose words went to both columns in the app.</summary>
    public int MixedSourceLines { get; init; }

    /// <summary>Scored lines where the converter and the label disagree.</summary>
    public IReadOnlyList<Disagreement> Disagreements { get; init; } = Array.Empty<Disagreement>();

    /// <summary>Extracted lines no pdftotext line of the page contains.</summary>
    public IReadOnlyList<string> UnmatchedLines { get; init; } = Array.Empty<string>();
}

/// <summary>An extracted line the converter classified differently from its label.</summary>
internal sealed record Disagreement(int Page, string Line, bool Labelled, TruthSource Source);

/// <summary>One scored extracted line; Column is -1 for full-width blocks.</summary>
internal sealed record ScoredLine(int Page, int Column, string Text, Truth Truth, bool Predicted);

/// <summary>Everything measured for one PDF.</summary>
internal sealed record PdfBenchResult(string File, PdfTextResult Text, TimeSpan Elapsed, IReadOnlyList<ExtractScore> Extracts);

/// <summary>Runs one PDF through extraction, layout and the chord sheet converter, and scores it against the labels.</summary>
internal sealed class PdfBenchmark
{
    private readonly PdfTextImporter _importer;

    public PdfBenchmark(PdfTextImporter importer) => _importer = importer;

    public async Task<PdfBenchResult> RunAsync(string pdfPath, IReadOnlyList<LabelledText> labels, CancellationToken cancellationToken)
    {
        var clock = Stopwatch.StartNew();
        PdfTextResult text;
        await using (var stream = File.OpenRead(pdfPath))
            text = await _importer.ImportAsync(stream, cancellationToken);
        clock.Stop();

        var extracts = labels.Select(l => ScoreExtract(l, text)).ToList();
        return new PdfBenchResult(Path.GetFileName(pdfPath), text, clock.Elapsed, extracts);
    }

    private static ExtractScore ScoreExtract(LabelledText labelled, PdfTextResult text)
    {
        var baseline = BaselineScore(labelled);
        var scored = labelled.Pages
            .Select(p => (Labels: p, Page: text.Pages.FirstOrDefault(t => t.PageNumber == p.PageNumber)))
            .Where(p => p.Page is not null)
            .Select(p => ScorePage(p.Labels, p.Page!))
            .ToList();
        var all = scored.SelectMany(s => s).ToList();
        var truth = all.Where(s => s.Truth.Source != TruthSource.Unmatched).ToList();
        var extracted = Score.Of(truth.Select(t => t.Truth.IsChordLine).ToList(), truth.Select(t => t.Predicted).ToList());
        var mixed = truth.Where(t => t.Column >= 0).GroupBy(t => (t.Page, t.Truth.SourceLine)).Count(g => g.Select(t => t.Column).Distinct().Count() > 1);
        return new ExtractScore(
            labelled.Name,
            labelled.Pages.Select(p => p.PageNumber).ToList(),
            baseline,
            extracted,
            all.Count,
            all.Count(s => s.Truth.Source == TruthSource.Unmatched),
            all.Count(s => s.Truth.Source == TruthSource.Fragment))
        {
            SourceLines = labelled.Pages.Sum(p => p.Lines.Count(l => l.Text.Trim().Length > 0)),
            MixedSourceLines = mixed,
            UnmatchedLines = all.Where(s => s.Truth.Source == TruthSource.Unmatched).Select(s => $"p{s.Page}: {s.Text.Trim()}").ToList(),
            Disagreements = truth.Where(t => t.Truth.IsChordLine != t.Predicted).Select(t => new Disagreement(t.Page, t.Text, t.Truth.IsChordLine, t.Truth.Source)).ToList(),
        };
    }

    // The converter on the labelled pdftotext text, scored by physical line number as the corpus tests do.
    private static Score BaselineScore(LabelledText labelled)
    {
        var result = ChordSheetConverter.Convert(labelled.Text);
        var physical = PhysicalLines.Map(labelled.Text);
        var predicted = result.Lines.Where(l => l.CarriesChords).Select(l => physical[l.LineNumber - 1]).ToHashSet();
        return new Score(labelled.Labels.Count, predicted.Count, labelled.Labels.Count(predicted.Contains));
    }

    private static List<ScoredLine> ScorePage(LabelledPage labels, PageText page)
    {
        var lines = page.Blocks
            .SelectMany((b, i) => (i == 0 ? b.Lines : b.Lines.Prepend(new AlignedLine(string.Empty, null))).Select(l => (Line: l, Column: b.ColumnCount == 2 ? b.ColumnIndex : -1)))
            .ToList();
        var text = page.Text;
        var result = ChordSheetConverter.Convert(text);
        var physical = PhysicalLines.Map(text);
        var predicted = result.Lines.Where(l => l.CarriesChords).Select(l => physical[l.LineNumber - 1]).ToHashSet();
        var rows = lines.Where(l => l.Line.Row is not null).Select(l => l.Line.Row!).ToList();
        var top = rows.Count == 0 ? 0 : rows.Max(r => r.Baseline);
        var span = rows.Count == 0 ? 1 : Math.Max(1, top - rows.Min(r => r.Baseline));
        return lines
            .Select((l, i) => (l.Line, l.Column, Number: i + 1))
            .Where(l => l.Line.Row is not null)
            .Select(l => new ScoredLine(page.PageNumber, l.Column, l.Line.Text, LineTruth.Of(l.Line.Text, (top - l.Line.Row!.Baseline) / span, labels), predicted.Contains(l.Number)))
            .ToList();
    }
}
