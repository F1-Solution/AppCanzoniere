using AppCanzoniere.MAUI.ViewModels;

namespace AppCanzoniere.MAUI.Pages;

public partial class AlbumDetailPage : ContentPage
{
    private readonly AlbumDetailViewModel _viewModel;

    public AlbumDetailPage(AlbumDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAlbumSongsCommand.ExecuteAsync(null);
    }
}
