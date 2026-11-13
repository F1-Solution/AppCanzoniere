using System.Globalization;

namespace AppCanzoniere.Application.Layout;

/// <summary>Wraps the caller's measurer and rejects metrics that are not finite, or that have a negative width or height.</summary>
internal sealed class ValidatingMeasurer : ITextMeasurer
{
    private readonly ITextMeasurer _inner;

    public ValidatingMeasurer(ITextMeasurer inner)
    {
        _inner = inner;
    }

    public TextMetrics Measure(string text, TextRole role)
    {
        var metrics = _inner.Measure(text, role);
        Check(metrics.Width, "Width", role, nonNegative: true);
        Check(metrics.LineHeight, "LineHeight", role, nonNegative: true);
        Check(metrics.Ascent, "Ascent", role, nonNegative: false);
        return metrics;
    }

    private static void Check(double value, string field, TextRole role, bool nonNegative)
    {
        if (double.IsFinite(value) && (!nonNegative || value >= 0))
            return;

        var shown = value.ToString(CultureInfo.InvariantCulture);
        throw new InvalidOperationException($"The text measurer returned an invalid {field} ({shown}) for role {role}.");
    }
}
