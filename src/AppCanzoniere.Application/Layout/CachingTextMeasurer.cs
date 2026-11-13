namespace AppCanzoniere.Application.Layout;

/// <summary>Memoises another measurer by text and role; not thread-safe, one per measurer instance.</summary>
public sealed class CachingTextMeasurer : ITextMeasurer
{
    private readonly ITextMeasurer _inner;
    private readonly Dictionary<(string Text, TextRole Role), TextMetrics> _cache = new();

    public CachingTextMeasurer(ITextMeasurer inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public TextMetrics Measure(string text, TextRole role)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (_cache.TryGetValue((text, role), out var cached))
            return cached;

        var metrics = _inner.Measure(text, role);
        _cache[(text, role)] = metrics;
        return metrics;
    }
}
