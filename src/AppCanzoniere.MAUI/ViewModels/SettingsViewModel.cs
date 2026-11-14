using System.Diagnostics;
using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;
using AppCanzoniere.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppCanzoniere.MAUI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IUserSettingsRepository _settingsRepository;

    [ObservableProperty]
    private UserSettings? _settings;

    [ObservableProperty]
    private ObservableCollection<string> _instruments = new()
    {
        "Guitar",
        "Piano",
        "Both"
    };

    [ObservableProperty]
    private ObservableCollection<string> _languages = new()
    {
        "en",
        "it"
    };

    [ObservableProperty]
    private ObservableCollection<string> _cloudProviders = new()
    {
        "Local",
        "OneDrive",
        "Google Drive",
        "Dropbox"
    };

    [ObservableProperty]
    private ObservableCollection<string> _themes = new()
    {
        "System",
        "Light",
        "Dark"
    };

    private readonly IChordDisplaySettingsStore _displayStore;
    private int _notationIndex;
    private int _minorIndex;
    private int _accidentalsIndex;
    private bool _loadingDisplay;

    public IReadOnlyList<string> NotationLabels { get; } = new[] { "Automatic (from language)", "Do Re Mi", "C D E" };

    public IReadOnlyList<string> MinorLabels { get; } = new[] { "Lam / Am", "La- / A-" };

    public IReadOnlyList<string> AccidentalLabels { get; } = new[] { "Automatic", "Sharps", "Flats" };

    public int NotationIndex
    {
        get => _notationIndex;
        set { if (SetProperty(ref _notationIndex, value)) SaveDisplaySettings(); }
    }

    public int MinorIndex
    {
        get => _minorIndex;
        set { if (SetProperty(ref _minorIndex, value)) SaveDisplaySettings(); }
    }

    public int AccidentalsIndex
    {
        get => _accidentalsIndex;
        set { if (SetProperty(ref _accidentalsIndex, value)) SaveDisplaySettings(); }
    }

    public SettingsViewModel(IUserSettingsRepository settingsRepository, IChordDisplaySettingsStore displayStore)
    {
        _settingsRepository = settingsRepository;
        _displayStore = displayStore;
        LoadDisplaySettings();
    }

    private void LoadDisplaySettings()
    {
        _loadingDisplay = true;
        try
        {
            var (notation, minor, accidentals) = ChordDisplayPickers.ToIndexes(_displayStore.Load());
            NotationIndex = Math.Max(0, notation);
            MinorIndex = Math.Max(0, minor);
            AccidentalsIndex = Math.Max(0, accidentals);
        }
        finally
        {
            _loadingDisplay = false;
        }
    }

    // A picker reports -1 while it is being reset; that is not a choice.
    private void SaveDisplaySettings()
    {
        if (_loadingDisplay || !ChordDisplayPickers.TryToSettings(NotationIndex, MinorIndex, AccidentalsIndex, out var settings)) return;

        try
        {
            _displayStore.Save(settings);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Saving chord display settings failed: {ex}");
            _ = Shell.Current.DisplayAlertAsync("Error", "The chord display choice could not be saved on this device.", "OK");
        }
    }

    private void ResetDisplaySettings()
    {
        try
        {
            _displayStore.Save(ChordDisplaySettings.Default);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Resetting chord display settings failed: {ex}");
        }

        LoadDisplaySettings();
    }

    [RelayCommand]
    private async Task LoadSettingsAsync()
    {
        Settings = await _settingsRepository.GetOrCreateAsync();
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        if (Settings == null) return;

        try
        {
            await _settingsRepository.UpdateAsync(Settings);
            await Shell.Current.DisplayAlert("Success", "Settings saved successfully", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to save settings: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ResetSettingsAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Reset Settings",
            "Are you sure you want to reset all settings to default?",
            "Yes",
            "No");

        if (!confirm) return;

        Settings = new UserSettings();
        ResetDisplaySettings();
        await SaveSettingsAsync();
    }

    [RelayCommand]
    private async Task NavigateToCloudSync()
    {
        await Shell.Current.GoToAsync("cloudsync");
    }
}
