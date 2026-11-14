using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.Application.Tests.Layout;

/// <summary>Deterministic measurer: width is UTF-16 length times a per-role advance; counts calls per text and role.</summary>
internal sealed class FixedAdvanceMeasurer : ITextMeasurer
{
    private static readonly Dictionary<TextRole, (double Advance, double LineHeight, double Ascent)> Table = new()
    {
        [TextRole.Lyric] = (10, 24, 19),
        [TextRole.ChorusLyric] = (11, 24, 19),
        [TextRole.Chord] = (8, 20, 16),
        [TextRole.Annotation] = (7, 20, 16),
        [TextRole.SectionLabel] = (8, 18, 14),
        [TextRole.Comment] = (9, 22, 17),
        [TextRole.Tab] = (6, 16, 12),
        [TextRole.LineNumber] = (6, 24, 19),
    };

    private readonly Dictionary<(string Text, TextRole Role), int> _calls = new();

    public int TotalCalls { get; private set; }

    public long TotalCharacters { get; private set; }

    public IReadOnlyDictionary<(string Text, TextRole Role), int> Calls => _calls;

    public TextMetrics Measure(string text, TextRole role)
    {
        var (advance, lineHeight, ascent) = Table[role];
        TotalCalls++;
        TotalCharacters += text.Length;
        _calls[(text, role)] = _calls.GetValueOrDefault((text, role)) + 1;
        return new TextMetrics(text.Length * advance, lineHeight, ascent);
    }
}
