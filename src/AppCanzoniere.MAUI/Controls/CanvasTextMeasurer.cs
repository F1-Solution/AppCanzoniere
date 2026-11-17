using AppCanzoniere.Application.Layout;
using Microsoft.Maui.Graphics.Platform;

namespace AppCanzoniere.MAUI.Controls;

/// <summary>Measures text with the real font metrics of the canvas, one font and size per role.</summary>
internal sealed class CanvasTextMeasurer : ITextMeasurer
{
    private const string Probe = "|";
    private const string LineSample = "Hg";
    private const float AscentShare = 0.8f;

    private readonly LeggioStyle _style;
    private readonly PlatformStringSizeService _sizes = new();
    private readonly Dictionary<string, IFont> _fonts = new();

    public CanvasTextMeasurer(LeggioStyle style)
    {
        _style = style;
    }

    public TextMetrics Measure(string text, TextRole role)
    {
        var style = _style.For(role);
        var height = Size(LineSample, style).Height;
        return new TextMetrics(WidthOf(text, style), height, height * AscentShare);
    }

    // Some platforms drop trailing blanks from the size, so a probe character is measured with them and taken off.
    private double WidthOf(string text, RoleStyle style)
    {
        if (text.Length == 0)
            return 0;

        if (!char.IsWhiteSpace(text[^1]))
            return Size(text, style).Width;

        var probe = Size(Probe, style).Width;
        return Size(text + Probe, style).Width - probe;
    }

    private SizeF Size(string text, RoleStyle style)
    {
        if (!_fonts.TryGetValue(style.Font, out var font))
            _fonts[style.Font] = font = new Microsoft.Maui.Graphics.Font(style.Font);

        return _sizes.GetStringSize(text, font, style.Size);
    }
}
