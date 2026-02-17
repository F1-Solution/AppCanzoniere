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
    private ObservableCollection<string> _chordNotations = new()
    {
        "Anglo",
        "Italian"
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

    public SettingsViewModel(IUserSettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
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
        await SaveSettingsAsync();
    }

    [RelayCommand]
    private async Task NavigateToCloudSync()
    {
        await Shell.Current.GoToAsync("cloudsync");
    }
}
