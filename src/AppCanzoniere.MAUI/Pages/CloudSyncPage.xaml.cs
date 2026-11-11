using AppCanzoniere.MAUI.ViewModels;

namespace AppCanzoniere.MAUI.Pages;

public partial class CloudSyncPage : ContentPage
{
    private readonly CloudSyncViewModel _viewModel;

    public CloudSyncPage(CloudSyncViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }

    private async void OnProviderChanged(object? sender, EventArgs e)
    {
        await _viewModel.ChangeProviderCommand.ExecuteAsync(null);
    }
}
