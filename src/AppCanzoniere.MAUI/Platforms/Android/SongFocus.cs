using Android.OS;
using Android.Views;
using AppCanzoniere.MAUI.Controls;

namespace AppCanzoniere.MAUI;

/// <summary>Keeps keyboard focus on the song so the first pedal key after a touch is not lost to the touch-mode switch.</summary>
internal sealed class SongFocus : IDisposable
{
    private readonly SongDrawingView _drawing;
    private readonly Microsoft.Maui.Controls.ScrollView _scroll;
    private ViewTreeObserver? _observer;
    private string _lastRequest = "none";

    public SongFocus(SongDrawingView drawing, Microsoft.Maui.Controls.ScrollView scroll)
    {
        _drawing = drawing;
        _scroll = scroll;
    }

    /// <summary>Makes the song focusable, takes focus and follows touch-mode changes until disposed.</summary>
    public void Attach()
    {
        Prepare();
        Detach();
        _observer = Target()?.ViewTreeObserver;
        if (_observer is { IsAlive: true })
            _observer.TouchModeChange += OnTouchModeChange;

        Request();
    }

    /// <summary>Asks for focus unless a text field has it.</summary>
    public void Request()
    {
        try
        {
            if (Target() is not { } view || view.RootView?.FindFocus() is Android.Widget.EditText)
                return;

            Prepare();
            var granted = view.RequestFocus();
            _lastRequest = granted ? "granted" : "refused";
            if (!granted)
            {
                // The view may not be shown or laid out yet, so it asks again shortly.
                view.PostDelayed(() =>
                {
                    if (view.RootView?.FindFocus() is not Android.Widget.EditText)
                        view.RequestFocus();
                }, 200);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Song focus failed: {ex}");
        }
    }

    /// <summary>The view class that has focus and the touch mode, for the emulator tests.</summary>
    public string Describe()
    {
        var root = Target()?.RootView;
        var target = Target();
        var blockers = new List<string>();
        for (var parent = target?.Parent; parent is not null; parent = parent.Parent)
        {
            if (parent is ViewGroup { DescendantFocusability: DescendantFocusability.BlockDescendants } blocking)
                blockers.Add(blocking.Class?.SimpleName ?? "?");
        }

        return $"{root?.FindFocus()?.Class?.SimpleName ?? "none"} target={target?.Class?.SimpleName ?? "none"} focusable={target?.Focusable} shown={target?.IsShown} enabled={target?.Enabled} fit={target?.FocusableInTouchMode} size={target?.Width}x{target?.Height} attached={target?.IsAttachedToWindow} windowFocus={target?.HasWindowFocus} vis={target?.Visibility} request={_lastRequest} blockers=[{string.Join(",", blockers)}] touch={(root?.IsInTouchMode ?? false).ToString().ToLowerInvariant()}";
    }

    public void Dispose() => Detach();

    private void Detach()
    {
        if (_observer is { IsAlive: true })
            _observer.TouchModeChange -= OnTouchModeChange;

        _observer = null;
    }

    // The drawing when it is shown, otherwise the scroll view that holds the plain text.
    private Android.Views.View? Target() => _drawing.IsVisible ? _drawing.Handler?.PlatformView as Android.Views.View : _scroll.Handler?.PlatformView as Android.Views.View;

    private void Prepare()
    {
        foreach (var view in new[] { _drawing.Handler?.PlatformView as Android.Views.View, _scroll.Handler?.PlatformView as Android.Views.View })
        {
            if (view is null)
                continue;

            view.Focusable = true;
            view.FocusableInTouchMode = true;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                view.DefaultFocusHighlightEnabled = false;
        }

        if (_scroll.Handler?.PlatformView is ViewGroup group)
            group.DescendantFocusability = DescendantFocusability.BeforeDescendants;
    }

    private void OnTouchModeChange(object? sender, ViewTreeObserver.TouchModeChangeEventArgs e)
    {
        if (e.IsInTouchMode)
            Request();
    }
}
