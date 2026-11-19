using Microsoft.UI.Windowing;

namespace AppCanzoniere.MAUI.Platforms.Windows;

/// <summary>Reports when the window is minimized or restored, which the cross-platform window events do not on Windows.</summary>
internal static class MinimizeWatcher
{
    private static bool _minimized;

    /// <summary>Raised with true when the window is minimized and false when it is restored.</summary>
    public static event Action<bool>? Changed;

    public static void Attach(Microsoft.UI.Xaml.Window window) => window.AppWindow.Changed += OnChanged;

    private static void OnChanged(AppWindow sender, AppWindowChangedEventArgs args)
    {

        var minimized = sender.Presenter is OverlappedPresenter { State: OverlappedPresenterState.Minimized };
        if (minimized == _minimized)
            return;

        _minimized = minimized;
        Changed?.Invoke(minimized);
    }
}
