using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Layout;

namespace AppCanzoniere.MAUI.Controls;

/// <summary>Draws a prepared song wrapped at the real width of the view and grows to the height of the content.</summary>
public sealed class SongDrawingView : GraphicsView
{
    private const int MaxCachedWidths = 8;

    public static readonly BindableProperty SongProperty = BindableProperty.Create(
        nameof(Song), typeof(PreparedSong), typeof(SongDrawingView),
        propertyChanged: (view, _, _) => ((SongDrawingView)view).Rebuild());

    private readonly Dictionary<int, SongDrawing> _byWidth = new();
    private SongDrawable? _drawable;
    private double _width;
    private bool _failed;

    public PreparedSong? Song
    {
        get => (PreparedSong?)GetValue(SongProperty);
        set => SetValue(SongProperty, value);
    }

    /// <summary>Height of the tallest row of the current layout, or a default before the first layout.</summary>
    public double RowHeight => _drawable?.Drawing is { Rows.Count: > 0 } d ? d.Rows.Max(r => r.Height) : 24;

    /// <summary>Raised once when measuring, laying out or drawing throws; the owner then shows the song as stored.</summary>
    public event EventHandler<Exception>? LayoutFailed;

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (width > 0 && Math.Abs(width - _width) > 0.5)
        {
            _width = width;
            Relayout();
        }
    }

    // A new song or a style change drops every cached layout.
    private void Rebuild()
    {
        try
        {
            _byWidth.Clear();
            _failed = false;
            var style = LeggioStyle.Load();
            var measurer = new CachingTextMeasurer(new CanvasTextMeasurer(style));
            _drawable = new SongDrawable(style, measurer, Fail);
            Drawable = _drawable;
            Relayout();
        }
        catch (Exception ex)
        {
            Fail(ex);
        }
    }

    private void Relayout()
    {
        if (_failed || _drawable is null || Song is not { } song || _width <= 0)
            return;

        try
        {
            var key = (int)Math.Round(_width);
            if (!_byWidth.TryGetValue(key, out var drawing))
            {
                drawing = song.LayoutAt(_width, _drawable.Measurer);
                if (_byWidth.Count >= MaxCachedWidths)
                    _byWidth.Clear();

                _byWidth[key] = drawing;
            }

            _drawable.Drawing = drawing;
            HeightRequest = drawing.Height;
            Invalidate();
        }
        catch (Exception ex)
        {
            Fail(ex);
        }
    }

    private void Fail(Exception ex)
    {
        if (_failed)
            return;

        _failed = true;
        System.Diagnostics.Debug.WriteLine($"Song drawing failed: {ex}");
        Dispatcher.Dispatch(() => LayoutFailed?.Invoke(this, ex));
    }
}
