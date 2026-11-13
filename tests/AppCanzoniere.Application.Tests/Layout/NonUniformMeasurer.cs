using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.Application.Tests.Layout;

/// <summary>Deterministic measurer whose character widths vary with the character, like a proportional font.</summary>
internal sealed class NonUniformMeasurer : ITextMeasurer
{
    private static readonly Dictionary<TextRole, (double Base, double LineHeight)> Table = new()
    {
        [TextRole.Lyric] = (9.3, 23),
        [TextRole.ChorusLyric] = (10.1, 25),
        [TextRole.Chord] = (7.7, 19),
        [TextRole.Annotation] = (6.1, 17),
        [TextRole.SectionLabel] = (8.9, 21),
        [TextRole.Comment] = (8.3, 22),
        [TextRole.Tab] = (6.0, 16),
        [TextRole.LineNumber] = (5.5, 18),
    };

    public TextMetrics Measure(string text, TextRole role)
    {
        var (unit, height) = Table[role];
        var width = 0.0;
        foreach (var c in text)
            width += unit * (0.55 + 0.05 * (c % 11));

        return new TextMetrics(width, height, height * 0.8);
    }
}
