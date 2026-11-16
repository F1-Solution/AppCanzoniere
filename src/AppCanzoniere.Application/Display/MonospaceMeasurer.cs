using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.Application.Display;

/// <summary>Measures every role as one unit per UTF-16 character and one unit per line, so positions are text columns.</summary>
internal sealed class MonospaceMeasurer : ITextMeasurer
{
    public TextMetrics Measure(string text, TextRole role) => new(text.Length, 1, 1);
}
