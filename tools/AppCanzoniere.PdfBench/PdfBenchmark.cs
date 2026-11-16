using System.Diagnostics;
using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.PdfBench;

/// <summary>Scores of one labelled text: the pdftotext baseline and the in-app extraction of the same pages.</summary>
internal sealed record ExtractScore(string Name, IReadOnlyList<int> Pages, Score Baseline, Score Extracted, int Lines, int Unmatched, int Fragments)
{
    /// <summary>In-app score over lines whose label is a hand label.</summary>
    public Score Human { get; init; } = Score.None;

    /// <summary>In-app score over fragment and unmatched lines, whose label comes from the strict chord pattern.</summary>
    public Score Pattern { get; init; } = Score.None;

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
internal sealed record PdfBenchResult(string File, PdfTextResult Text, TimeSpan Elapsed, IReadOnlyList<ExtractScore> Extracts)
{
    public SplitResult Split { get; init; } = SplitResult.None;

    public AlignmentScore Alignment { get; init; } = AlignmentScore.None;

    public SongsFound Songs { get; init; } = SongsFound.None;
}

/// <summary>Runs one PDF through extraction, layout and the chord sheet converter, and scores it against the labels.</summary>
internal sealed class PdfBenchmark
{
    private readonly PdfTextImporter _importer;

    public PdfBenchmark(PdfTextImporter importer) => _importer = importer;

    public async Task<PdfBenchResult> RunAsync(string pdfPath, IReadOnlyList<LabelledText> labels, IReadOnlyList<LabelledPage> reference, CancellationToken cancellationToken)
    {
        var clock = Stopwatch.StartNew();
        PdfTextResult text;
        await using (var stream = File.OpenRead(pdfPath))
            text = await _importer.ImportAsync(stream, cancellationToken);
        clock.Stop();

        var extracts = labels.Select(l => ScoreExtract(l, text)).ToList();
        return new PdfBenchResult(Path.GetFileName(pdfPath), text, clock.Elapsed, extracts)
        {
            Split = SplitCheck.Measure(text, reference, TwoColumnPages(pdfPath, labels)),
            Alignment = ChordAlignment.Measure(pdfPath, text),
            Songs = SongsFound.Measure(text),
        };
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
        var human = all.Where(s => s.Truth.Source == TruthSource.Label).ToList();
        var pattern = all.Where(s => s.Truth.Source != TruthSource.Label).ToList();
        return new ExtractScore(
            labelled.Name,
            labelled.Pages.Select(p => p.PageNumber).ToList(),
            baseline,
            Of(all),
            all.Count,
            all.Count(s => s.Truth.Source == TruthSource.Unmatched),
            all.Count(s => s.Truth.Source == TruthSource.Fragment))
        {
            Human = Of(human),
            Pattern = Of(pattern),
            UnmatchedLines = all.Where(s => s.Truth.Source == TruthSource.Unmatched).Select(s => $"p{s.Page}: {s.Text.Trim()}").ToList(),
            Disagreements = all.Where(t => t.Truth.IsChordLine != t.Predicted).Select(t => new Disagreement(t.Page, t.Text, t.Truth.IsChordLine, t.Truth.Source)).ToList(),
        };
    }

    // Labelled pages of the PDFs the corpus description lists as two-column.
    private static HashSet<int> TwoColumnPages(string pdfPath, IReadOnlyList<LabelledText> labels) =>
        CorpusFolder.IsTwoColumn(pdfPath)
            ? labels.SelectMany(l => l.Pages).Select(p => p.PageNumber).ToHashSet()
            : new HashSet<int>();

    private static Score Of(IReadOnlyList<ScoredLine> lines) =>
        Score.Of(lines.Select(l => l.Truth.IsChordLine).ToList(), lines.Select(l => l.Predicted).ToList());

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
