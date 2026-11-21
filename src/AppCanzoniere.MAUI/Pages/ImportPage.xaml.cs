using System.ComponentModel;
using AppCanzoniere.MAUI.ViewModels;

namespace AppCanzoniere.MAUI.Pages;

public partial class ImportPage : ContentPage
{
    private readonly ImportViewModel _viewModel;
    private IDispatcherTimer? _tick;
    private Window? _window;
    private bool _sheetShown;

    public ImportPage(ImportViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        SizeChanged += (_, _) => OnPageSizeChanged();
        viewModel.PropertyChanged += OnViewModelChanged;
#if WINDOWS
        // Tab cycles inside the sheet and never reaches the navigation bar or the page behind.
        DeclarationSheet.HandlerChanged += (_, _) =>
        {
            if (DeclarationSheet.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement native)
                native.TabFocusNavigation = Microsoft.UI.Xaml.Input.KeyboardNavigationMode.Cycle;
        };
#endif
        PreviewView.LayoutFailed += (_, ex) => System.Diagnostics.Debug.WriteLine($"Preview drawing failed: {ex}");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            ImportKeys.Handler = _viewModel.HandleKey;
            _viewModel.OnPageShown();
            StartTick();
            HookWindow();
            if (Shell.Current is { } shell)
                shell.Navigating += OnShellNavigating;

            ApplyLayout();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Import page appearing failed: {ex}");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        try
        {
            if (Equals(ImportKeys.Handler, (Func<string, bool, bool, bool>)_viewModel.HandleKey))
                ImportKeys.Handler = null;

            _viewModel.OnPageHidden();
            _tick?.Stop();
            UnhookWindow();
            if (Shell.Current is { } shell)
                shell.Navigating -= OnShellNavigating;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Import page disappearing failed: {ex}");
        }
    }

    protected override bool OnBackButtonPressed() => _viewModel.HandleBack() || base.OnBackButtonPressed();

    // Leaving for another tab with unsaved edits asks first; opening a song does not.
    private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        // Nothing leaves the page while the declaration is open or songs are being saved.
        if (_viewModel.Session.IsLocked)
        {
            e.Cancel();
            return;
        }

        if (_viewModel.Session.ModifiedUnsavedCount == 0 || e.Target?.Location.ToString().Contains("songdetail") == true)
            return;

        var deferral = e.GetDeferral();
        try
        {
            if (!await _viewModel.ConfirmLeaveAsync())
                e.Cancel();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Leave confirmation failed: {ex}");
        }
        finally
        {
            deferral.Complete();
        }
    }

    private void StartTick()
    {
        _tick ??= Dispatcher.CreateTimer();
        _tick.Interval = TimeSpan.FromSeconds(1);
        _tick.Tick -= OnTick;
        _tick.Tick += OnTick;
        _tick.Start();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (_viewModel.ShowTimer)
            _viewModel.NotifyTimer();
    }

    private void HookWindow()
    {
        _window = Window;
        if (_window is null)
            return;

        _window.Stopped += OnWindowStopped;
        _window.Resumed += OnWindowResumed;
    }

    private void UnhookWindow()
    {
        if (_window is null)
            return;

        _window.Stopped -= OnWindowStopped;
        _window.Resumed -= OnWindowResumed;
        _window = null;
    }

    private void OnWindowStopped(object? sender, EventArgs e) => _viewModel.OnPageHidden();

    private void OnWindowResumed(object? sender, EventArgs e) => _viewModel.OnPageShown();

    private void OnPageSizeChanged()
    {
        if (Width > 0)
            _viewModel.SetWidth(Width);
    }

    private void OnViewModelChanged(object? sender, PropertyChangedEventArgs e) => Dispatcher.Dispatch(ApplyLayout);

    /// <summary>Places the panels and the list for the width class and shows the chosen panel.</summary>
    private void ApplyLayout()
    {
        try
        {
            PlaceResultColumns();
            PlacePanels();
            StyleSegments();
            DeclarationSheet.VerticalOptions = _viewModel.IsNarrow ? LayoutOptions.End : LayoutOptions.Center;
            FocusSheetWhenOpened();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Import layout failed: {ex}");
        }
    }

    // The page behind is disabled; the focus moves into the sheet when it opens.
    private void FocusSheetWhenOpened()
    {
        var open = _viewModel.DeclarationOpen;
        if (open && !_sheetShown)
            Dispatcher.Dispatch(() => DeclarationCheck.Focus());

        _sheetShown = open;
    }

    private void PlaceResultColumns()
    {
        if (_viewModel.IsWide)
        {
            ResultGrid.ColumnDefinitions[0].Width = new GridLength(360);
            ResultGrid.ColumnDefinitions[1].Width = GridLength.Star;
            Grid.SetColumn(ListPane, 0);
            Grid.SetColumnSpan(ListPane, 1);
            Grid.SetColumn(ReviewPane, 1);
            Grid.SetColumnSpan(ReviewPane, 1);
            return;
        }

        ResultGrid.ColumnDefinitions[0].Width = GridLength.Star;
        ResultGrid.ColumnDefinitions[1].Width = new GridLength(0);
        Grid.SetColumn(ListPane, 0);
        Grid.SetColumnSpan(ListPane, 2);
        Grid.SetColumn(ReviewPane, 0);
        Grid.SetColumnSpan(ReviewPane, 2);
    }

    private void PlacePanels()
    {
        var columns = Panels.ColumnDefinitions;
        if (_viewModel.IsWide)
        {
            SetColumns(columns, 3);
            Place(OriginalPanel, 0, true);
            Place(ChordProPanel, 1, true);
            Place(PreviewPanel, 2, true);
            SegmentBar.IsVisible = false;
        }
        else if (_viewModel.IsMedium)
        {
            SetColumns(columns, 2);
            var previewOnLeft = _viewModel.MediumLeftPanel == 1;
            Place(OriginalPanel, 0, !previewOnLeft);
            Place(PreviewPanel, 0, previewOnLeft);
            Place(ChordProPanel, 1, true);
            SegmentBar.IsVisible = _viewModel.ShowReviewContent;
            SegChordPro.IsVisible = false;
            Grid.SetColumn(SegPreview, 1);
        }
        else
        {
            SetColumns(columns, 1);
            Place(OriginalPanel, 0, _viewModel.NarrowPanel == 0);
            Place(ChordProPanel, 0, _viewModel.NarrowPanel == 1);
            Place(PreviewPanel, 0, _viewModel.NarrowPanel == 2);
            SegmentBar.IsVisible = _viewModel.ShowReviewContent;
            SegChordPro.IsVisible = true;
            Grid.SetColumn(SegPreview, 2);
        }
    }

    private static void SetColumns(ColumnDefinitionCollection columns, int count)
    {
        for (var i = 0; i < columns.Count; i++)
            columns[i].Width = i < count ? GridLength.Star : new GridLength(0);
    }

    private static void Place(View panel, int column, bool visible)
    {
        Grid.SetColumn(panel, column);
        panel.IsVisible = visible;
    }

    // The active segment gets the soft accent and a heavier look; the state is also read by screen readers.
    private void StyleSegments()
    {
        var active = _viewModel.IsMedium ? (_viewModel.MediumLeftPanel == 1 ? 2 : 0) : _viewModel.NarrowPanel;
        var segments = new[] { SegOriginal, SegChordPro, SegPreview };
        for (var i = 0; i < segments.Length; i++)
        {
            var on = i == active;
            segments[i].BackgroundColor = on ? Res("Token.accentSoft") : Res("Token.surface");
            segments[i].TextColor = on ? Res("Token.onAccentSoft") : Res("Token.text");
            SemanticProperties.SetHint(segments[i], on ? "Selezionato" : string.Empty);
        }
    }

    private static Color Res(string key) =>
        Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color
            ? color
            : Colors.Transparent;
}
