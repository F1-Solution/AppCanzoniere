using AppCanzoniere.MAUI.ViewModels;

namespace AppCanzoniere.MAUI.Pages;

public partial class PublicSongsPage : ContentPage
{
    private readonly PublicSongsViewModel _viewModel;

    public PublicSongsPage(PublicSongsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CheckAuthenticationAsync();
        await _viewModel.LoadSongsCommand.ExecuteAsync(null);
    }
}
