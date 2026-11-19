using AppCanzoniere.Application.PageTurning;
using AppCanzoniere.MAUI.ViewModels;

namespace AppCanzoniere.MAUI.Pages;

public partial class SongDetailPage : ContentPage
{
    private readonly SongDetailViewModel _viewModel;
    private readonly IHardwareKeySource _keys;
    private readonly ScreenWakeLock _wake;
    private IDisposable? _wakeLease;
    private Window? _window;
#if ANDROID
    private SongFocus? _focus;
#endif

    public SongDetailPage(SongDetailViewModel viewModel, IHardwareKeySource keys, ScreenWakeLock wake)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _keys = keys;
        _wake = wake;
        BindingContext = _viewModel;
#if ANDROID
        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SongDetailViewModel.IsEditMode) && !_viewModel.IsEditMode)
                Dispatcher.Dispatch(() => _focus?.Request());
        };
#endif
        SongView.LayoutFailed += (_, _) => _viewModel.ShowAsStored();
#if DEBUG && ANDROID
        // Debug-only: lets the emulator tests read the scroll position from logcat.
        SongScroll.Scrolled += (_, e) => Android.Util.Log.Info("AppCanzoniereSelfTest", FormattableString.Invariant(
            $"SCROLL y={e.ScrollY:F1} viewport={SongScroll.Height:F1} content={SongScroll.ContentSize.Height + SongScroll.Padding.VerticalThickness:F1} row={SongView.RowHeight:F1}"));
#endif
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _keys.PageTurnRequested -= OnPageTurnRequested;
        _keys.PageTurnRequested += OnPageTurnRequested;
        HoldScreenOn();
        UnsubscribeWindow();
        _window = Window;
        if (_window is not null)
        {
            _window.Stopped += OnWindowStopped;
            _window.Resumed += OnWindowResumed;
        }
#if WINDOWS
        Platforms.Windows.MinimizeWatcher.Changed -= OnMinimizedChanged;
        Platforms.Windows.MinimizeWatcher.Changed += OnMinimizedChanged;
        _minimized = Platforms.Windows.MinimizeWatcher.IsMinimized;
        if (_minimized)
            ReleaseScreen();
#endif
#if ANDROID
        _focus ??= new SongFocus(SongView, SongScroll);
        _focus.Attach();
#endif
        _viewModel.InitializeNewSongCommand.Execute(null);

        // Settings may have changed while another page was open; the transposition stays.
        await _viewModel.RefreshDisplayAsync();
#if ANDROID
        // The drawing may have appeared only now, so focus is requested again and reported for the emulator tests.
        _focus.Attach();
#if DEBUG
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(300), () =>
            Android.Util.Log.Info("AppCanzoniereSelfTest", $"FOCUS {_focus?.Describe()}"));
#endif
#endif
    }

    protected override void OnDisappearing()
    {
        _keys.PageTurnRequested -= OnPageTurnRequested;
        UnsubscribeWindow();
#if ANDROID
        _focus?.Dispose();
#endif
#if WINDOWS
        Platforms.Windows.MinimizeWatcher.Changed -= OnMinimizedChanged;
        _minimized = false;
#endif
        ReleaseScreen();
        base.OnDisappearing();
    }

    private void UnsubscribeWindow()
    {
        if (_window is null)
            return;

        _window.Stopped -= OnWindowStopped;
        _window.Resumed -= OnWindowResumed;
        _window = null;
    }

#if WINDOWS
    // Windows does not raise Stopped for a minimized window.
    private void OnMinimizedChanged(bool minimized) => Dispatcher.Dispatch(() =>
    {
        _minimized = minimized;
        if (minimized)
            ReleaseScreen();
        else
            HoldScreenOn();
    });
#endif

#if WINDOWS
    private bool _minimized;
#else
    private const bool _minimized = false;
#endif

    // A minimized window must not take the display back when another event resumes it.
    private void HoldScreenOn()
    {
        if (!_minimized)
            _wakeLease ??= _wake.Acquire();
    }

    private void ReleaseScreen()
    {
        _wakeLease?.Dispose();
        _wakeLease = null;
    }

    // A minimised window must not keep the display on.
    private void OnWindowStopped(object? sender, EventArgs e) => ReleaseScreen();

    private void OnWindowResumed(object? sender, EventArgs e) => HoldScreenOn();

    private void OnPageTurnRequested(object? sender, PageTurnDirection direction)
    {
        Dispatcher.Dispatch(async () =>
        {
            try
            {
                var contentHeight = SongScroll.ContentSize.Height + SongScroll.Padding.VerticalThickness;
                var target = PageTurnScroll.NextOffset(SongScroll.Height, contentHeight, SongScroll.ScrollY, SongView.RowHeight, direction);
                await SongScroll.ScrollToAsync(0, target, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Page turn failed: {ex}");
            }
        });
    }
}
