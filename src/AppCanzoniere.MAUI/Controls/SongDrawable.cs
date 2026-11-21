using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.MAUI.Controls;

/// <summary>Draws a laid-out song; the layout is computed elsewhere so a frame only paints.</summary>
internal sealed class SongDrawable : IDrawable
{
    private readonly LeggioStyle _style;
    private readonly Action<Exception> _failed;

    public SongDrawable(LeggioStyle style, ITextMeasurer measurer, Action<Exception> failed)
    {
        _style = style;
        Measurer = measurer;
        _failed = failed;
    }

    public ITextMeasurer Measurer { get; }

    public SongDrawing? Drawing { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        try
        {
            canvas.FillColor = _style.Background;
            canvas.FillRectangle(dirtyRect);
            if (Drawing is { } drawing)
                DrawRuns(canvas, drawing, dirtyRect);
        }
        catch (Exception ex)
        {
            _failed(ex);
        }
    }

    private void DrawRuns(ICanvas canvas, SongDrawing drawing, RectF dirtyRect)
    {
        foreach (var run in drawing.Runs)
        {
            var metrics = Measurer.Measure(run.Text, run.Role);
            var top = run.BaselineY - metrics.Ascent;
            if (top > dirtyRect.Bottom || top + metrics.LineHeight * 1.5 < dirtyRect.Top)
                continue;

            var role = _style.For(run.Role);
            canvas.Font = new Microsoft.Maui.Graphics.Font(role.Font);
            canvas.FontSize = role.Size;
            canvas.FontColor = role.Color;
            // Drawn at the baseline with no box, so descenders and tall glyphs are never clipped by an estimated line height.
            canvas.DrawString(run.Text, (float)run.X, (float)run.BaselineY, HorizontalAlignment.Left);
        }
    }
}
