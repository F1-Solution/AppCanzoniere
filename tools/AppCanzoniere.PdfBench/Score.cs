namespace AppCanzoniere.PdfBench;

/// <summary>Chord-line counts: labelled chord lines, lines the converter marked as chords, and the overlap.</summary>
internal sealed record Score(int Labelled, int Predicted, int Found)
{
    public static readonly Score None = new(0, 0, 0);

    public double? Recall => Labelled == 0 ? null : (double)Found / Labelled;

    public double? Precision => Predicted == 0 ? null : (double)Found / Predicted;

    public static Score operator +(Score a, Score b) => new(a.Labelled + b.Labelled, a.Predicted + b.Predicted, a.Found + b.Found);

    public static Score Of(IReadOnlyCollection<bool> truth, IReadOnlyCollection<bool> predicted) =>
        new(truth.Count(t => t), predicted.Count(p => p), truth.Zip(predicted).Count(p => p.First && p.Second));
}
