using AppCanzoniere.MAUI.ViewModels;

namespace AppCanzoniere.MAUI.Pages;

public partial class AlbumListPage : ContentPage
{
    private readonly AlbumListViewModel _viewModel;

    public AlbumListPage(AlbumListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAlbumsCommand.ExecuteAsync(null);
    }
}
