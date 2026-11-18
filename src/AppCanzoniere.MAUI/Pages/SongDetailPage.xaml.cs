using AppCanzoniere.Application.PageTurning;
using AppCanzoniere.MAUI.ViewModels;

namespace AppCanzoniere.MAUI.Pages;

public partial class SongDetailPage : ContentPage
{
    private readonly SongDetailViewModel _viewModel;
    private readonly IHardwareKeySource _keys;

    public SongDetailPage(SongDetailViewModel viewModel, IHardwareKeySource keys)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _keys = keys;
        BindingContext = _viewModel;
        SongView.LayoutFailed += (_, _) => _viewModel.ShowAsStored();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _keys.PageTurnRequested -= OnPageTurnRequested;
        _keys.PageTurnRequested += OnPageTurnRequested;
        DeviceDisplay.Current.KeepScreenOn = true;
        _viewModel.InitializeNewSongCommand.Execute(null);

        // Settings may have changed while another page was open; the transposition stays.
        await _viewModel.RefreshDisplayAsync();
    }

    protected override void OnDisappearing()
    {
        _keys.PageTurnRequested -= OnPageTurnRequested;
        DeviceDisplay.Current.KeepScreenOn = false;
        base.OnDisappearing();
    }

    private void OnPageTurnRequested(object? sender, PageTurnDirection direction)
    {
        var target = PageTurnScroll.NextOffset(SongScroll.Height, SongScroll.ContentSize.Height, SongScroll.ScrollY, SongView.RowHeight, direction);
        Dispatcher.Dispatch(async () =>
        {
            try
            {
                await SongScroll.ScrollToAsync(0, target, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Page turn failed: {ex}");
            }
        });
    }
}
