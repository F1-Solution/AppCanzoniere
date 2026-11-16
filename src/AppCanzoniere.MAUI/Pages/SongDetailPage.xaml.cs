using AppCanzoniere.MAUI.ViewModels;

namespace AppCanzoniere.MAUI.Pages;

public partial class SongDetailPage : ContentPage
{
    private readonly SongDetailViewModel _viewModel;

    public SongDetailPage(SongDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        SongView.LayoutFailed += (_, _) => _viewModel.ShowAsStored();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.InitializeNewSongCommand.Execute(null);

        // Settings may have changed while another page was open; the transposition stays.
        await _viewModel.RefreshDisplayAsync();
    }
}
