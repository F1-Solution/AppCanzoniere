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

    public SongDetailPage(SongDetailViewModel viewModel, IHardwareKeySource keys, ScreenWakeLock wake)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _keys = keys;
        _wake = wake;
        BindingContext = _viewModel;
        SongView.LayoutFailed += (_, _) => _viewModel.ShowAsStored();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _keys.PageTurnRequested -= OnPageTurnRequested;
        _keys.PageTurnRequested += OnPageTurnRequested;
        HoldScreenOn();
        _window = Window;
        if (_window is not null)
        {
            _window.Stopped += OnWindowStopped;
            _window.Resumed += OnWindowResumed;
        }
        _viewModel.InitializeNewSongCommand.Execute(null);

        // Settings may have changed while another page was open; the transposition stays.
        await _viewModel.RefreshDisplayAsync();
    }

    protected override void OnDisappearing()
    {
        _keys.PageTurnRequested -= OnPageTurnRequested;
        if (_window is not null)
        {
            _window.Stopped -= OnWindowStopped;
            _window.Resumed -= OnWindowResumed;
            _window = null;
        }

        ReleaseScreen();
        base.OnDisappearing();
    }

    private void HoldScreenOn() => _wakeLease ??= _wake.Acquire();

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
