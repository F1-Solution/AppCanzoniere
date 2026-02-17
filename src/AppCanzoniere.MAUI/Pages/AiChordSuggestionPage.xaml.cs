using AppCanzoniere.MAUI.ViewModels;

namespace AppCanzoniere.MAUI.Pages;

public partial class AiChordSuggestionPage : ContentPage
{
    private readonly AiChordSuggestionViewModel _viewModel;

    public AiChordSuggestionPage(AiChordSuggestionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CheckServiceAvailabilityCommand.ExecuteAsync(null);
        if (_viewModel.SongId != Guid.Empty)
        {
            await _viewModel.LoadSongCommand.ExecuteAsync(null);
        }
    }
}
